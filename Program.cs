using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using PuppeteerSharp.Media;
using PuppeteerSharp;
using RazorLight;
using SharePortfolio.Model;
using static SharePortfolio.Model.StockSale;
using Microsoft.CodeAnalysis;
using System.Reflection;
using System.Net.Http.Json;
using static SharePortfolio.Model.Transfer;
using System.Collections;

namespace SharePortfolio
{
	public enum OverrideTypes
	{
		FxRateThreshold, // FxRateThreshold: ignore fx rates that exceed defined threshold
		EssAmountThreshold, // EssAmountThreshold: ignore differences exceeding threshold for calculated and given ess amounts
		SettlementDateThreshold, // SettlementDateThreshold: ignore threshold between transaction and settlement date
		DataEntryBalancesInvalid, // DataEntryBalancesInvalid: ignore gaps when the balances for the data entry values are incorrect
		TransactionBalancesInvalid, // TransactionBalancesInvalid: ignore gaps when the balances for the transaction are incorrect
	}

	public partial class Program
	{

		public static readonly string StockFileRootPath = @"C:\Users\bslater\OneDrive\Code\SharePortfolio\Portfolio Files";

		private static readonly DateOnly EssTaxingDate = DateOnly.FromDateTime(new DateTime(2009, 7, 1));
		private static readonly decimal FxTolerance = 0.0094m;

		private static int UnitDecimalPlaces = 4;

		private static IBrowser browser;


		private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
		{
			AllowTrailingCommas = true,
			WriteIndented = true,
			UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
			Converters = {
				new DateOnlyConverter(),
				new RatioStringJsonConverter(),
				new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false) // case-insensitive for enums
            },
		};

		public static void InitializeBrowser()
		{
			var browserFetcher = new BrowserFetcher();
			browserFetcher.DownloadAsync().GetAwaiter().GetResult();

			// Launch the browser and generate the PDF synchronously
			browser = Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }).GetAwaiter().GetResult();
		}

		[STAThread]
		private static void Main(string[] args)
		{

			// ensures that the date formats are correct
			System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
			System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

			// load all the data
			List<DividendRecord> dividendRecords = JsonSerializer.Deserialize<List<DividendRecord>>(File.ReadAllText(@"C:\Users\bslater\OneDrive\Code\SharePortfolio\Portfolio Files\prices\MSFT\MSFT-dividendrecords.json"), jsonSerializerOptions);

			UniqueSortedSet<FxRate> fxRates = Load<FxRate>("*-fx-rates.json");
			UniqueSortedSet<StockQuote> stockQuotes = Load<StockQuote>("MSFT-*-prices.json");
			// load all the stock lots into a single set
			UniqueSortedSet<StockLotBase> stockLots = new UniqueSortedSet<StockLotBase>(
				Load<AwardLot>("*-awards.json").Cast<StockLotBase>().Union(Load<EsppLot>("*-espp.json"))
			);
			UniqueSortedSet<InterestTransaction> interestTrainsactions = Load<InterestTransaction>("*-*-interest.json");
			UniqueSortedSet<DividendTransaction> dividendTransactions = Load<DividendTransaction>("*-*-dividends.json");
			UniqueSortedSet<StockSale> stockSales = Load<StockSale>("*-sales.json");
			UniqueSortedSet<StockSplit> stockSplits = Load<StockSplit>("MSFT*-splits.json");
			UniqueSortedSet<Transfer> transfers = Load<Transfer>("*-transfers.json");

			UniqueSortedSet<LedgerEntry> ledger = new UniqueSortedSet<LedgerEntry>();
			StockSplitAdjuster stockSplitAdjuster = new StockSplitAdjuster(stockSplits);
			foreach (var lot in stockLots.OrderBy(l => l.TransactionDate))
			{
				var fx = fxRates.GetByDate(lot.TransactionDate, true);
				ledger.Add((lot is EsppLot espp)
					? new EsppLedgerEntry
					{
						Fees = 0,
						TransactionDate = espp.TransactionDate,
						SettlementDate = espp.SettlementDate,
						Beneficiary = espp.Owner,
						EssTaxPaid = espp.ReportedEssAmount,
						EssTaxType = "Employee Share Scheme: Taxed upfront scheme [Box E]",
						FxRate = fx.ToFxRateDetail(),
						Quantity = espp.Quantity,
						Reference = espp.Reference,
						StrikePrice = espp.UnitCost,
						MarketPrice = espp.UnitFMV,
						EsppEmployeeContribution = espp.ESPPAmount,
						EsppEmployeeContributionRefunded = espp.Refunded,
						EsppFundContribution = espp.ESPPContribution,
						EsppFundExcess = espp.Excess,
						EsppFundResidual = espp.Residual,
						EsppPeriodStart = espp.PeriodStart,
						EsppPeriodEnd = espp.PeriodEnd,
						Symbol = espp.Symbol,
						Company = "Microsoft Corporation",
					}
					: (lot is AwardLot award) // Use AwardLot properties in AwardLedgerEntry
					? new AwardLedgerEntry
					{
						Fees = 0,
						TransactionDate = award.TransactionDate,
						SettlementDate = award.SettlementDate,
						Beneficiary = award.Owner,
						EssTaxPaid = award.ReportedEssAmount,
						EssTaxType = award.AwardDate < EssTaxingDate ? $"Employee Share Scheme: ESS Interests acquired pre {EssTaxingDate:d MMM yyyy} [Box G]" : "Employee Share Scheme: Deferral scheme [Box F]",
						FxRate = fx.ToFxRateDetail(),
						Quantity = award.Quantity,
						Reference = award.Reference,
						StrikePrice = award.UnitCost,
						MarketPrice = award.UnitFMV,
						AwardDate = award.AwardDate,
						AwardId = award.AwardId,
						AwardName = award.AwardType,
						Symbol = award.Symbol,
						Company = "Microsoft Corporation",
					}
					: null);
			}// Handle other cases if needed
			foreach (var sale in stockSales.OrderBy(e => e.TransactionDate))
			{
				var fx = fxRates.GetByDate(sale.TransactionDate, true);
				var fees = Math.Round(sale.Fees / fx.Rate, 2);
				var remainingFees = fees;
				ledger.Add(new SellLedgerEntry
				{
					TransactionDate = sale.TransactionDate,
					SettlementDate = sale.SettlementDate,
					Beneficiary = sale.Owner,
					FxRate = fx.ToFxRateDetail(),
					Reference = sale.Reference,
					BrokerageFees = sale.Fees,
					Quantity = sale.Quantity,
					Symbol = sale.Symbol,
					Company = "Microsoft Corporation",
					StrikePrice = sale.StrikePrice,
					Proceeds = sale.Amount,
					LotsSold = sale.Lots.OrderBy(e => e.Reference).Select((l, i) =>
					{
						// Find the corresponding BuyLedgerEntry
						var buyLotLedgerEntry = ledger.OfType<BuyLedgerEntry>().First(buyLot => buyLot.Reference == l.Reference);

						// Retrieve past sales for the lot - where the lot is sold across multiple sale entries
						var pastSales = ledger.OfType<SellLedgerEntry>()
							.Where(s => s.LotsSold.Any(ls => ls.Reference == buyLotLedgerEntry.Reference)).ToList();

						// Aggregate past sale data
						var aggregateData = pastSales.SelectMany(s => s.LotsSold.Select(ls => new { ls, s.TransactionDate }))
							.Where(ls => ls.ls.Reference == buyLotLedgerEntry.Reference)
							.Aggregate(new { Quantity = 0m, CostAUD = 0m, ForexGain = 0m, EssPaid = 0m }, (acc, ls) =>
							{
								var adjustmentFactor = stockSplitAdjuster.GetAdjustmentFactor(buyLotLedgerEntry.TransactionDate, ls.TransactionDate);
								return new
								{
									Quantity = acc.Quantity + adjustmentFactor * ls.ls.QuantitySold,
									CostAUD = acc.CostAUD + ls.ls.CostAUD,
									ForexGain = acc.ForexGain + ls.ls.ForexGain,
									EssPaid = acc.EssPaid + ls.ls.EssTaxPaid
								};
							});

						// Calculate remaining quantities and values
						var qtyRemaining = stockSplitAdjuster.GetAdjustmentFactor(buyLotLedgerEntry.TransactionDate, sale.TransactionDate) * buyLotLedgerEntry.Quantity
							- aggregateData.Quantity;
						var last = qtyRemaining - l.Quantity == 0;

						// Proportional values if this is not the last lot sold
						var proportionalCost = last ? buyLotLedgerEntry.CostAUD - aggregateData.CostAUD : Math.Round(((buyLotLedgerEntry.CostAUD - aggregateData.CostAUD) / qtyRemaining) * l.Quantity, 2);
						var proportionalEssPaid = last ? buyLotLedgerEntry.EssTaxPaid - aggregateData.EssPaid : Math.Round(((buyLotLedgerEntry.EssTaxPaid - aggregateData.EssPaid) / qtyRemaining) * l.Quantity, 2);
						var proportionalFee = (i == sale.Lots.Count - 1) ? remainingFees : Math.Round(fees * (l.Quantity / sale.Quantity), 2);
						remainingFees -= proportionalFee;

						// Create the SellLotEntry
						return new LotLedgerEntry
						{
							BuyDate = buyLotLedgerEntry.TransactionDate,
							BuyMarketPrice = buyLotLedgerEntry.MarketPrice,
							Reference = buyLotLedgerEntry.Reference,
							QuantitySold = l.Quantity,
							BuyStrikePrice = buyLotLedgerEntry.StrikePrice,
							CostAUD = proportionalCost,
							SaleProceeds = Math.Round(l.Quantity * sale.StrikePrice, 2),
							SaleProceedsAUD = Math.Round((l.Quantity * sale.StrikePrice) / fx.Rate, 2),
							FxRate = buyLotLedgerEntry.FxRate,
							EssTaxPaid = proportionalEssPaid,
							Fees = proportionalFee,
							FeesAUD = Math.Round(proportionalFee / fx.Rate, 2),
							IsLongTerm = buyLotLedgerEntry.TransactionDate.AddYears(1) < sale.TransactionDate,
						};
					}).ToList(),
				});
			}

			foreach (var div in dividendTransactions.OrderBy(e => e.TransactionDate))
			{
				var fx = fxRates.GetByDate(div.TransactionDate, true);
				ledger.Add(new DividendLedgerEntry
				{
					TransactionDate = div.TransactionDate,
					SettlementDate = div.SettlementDate,
					FxRate = fx.ToFxRateDetail(),
					Reference = div.Reference,
					ExDividendDate = dividendRecords.First(e => e.Payment == div.TransactionDate).ExDividend,
					Beneficiary = div.Owner,
					Proceeds = div.Amount,
					Symbol = div.Symbol,
					Company = "Microsoft Corporation",
					SharesHeld = div.Shares,
					Dividend = div.Dividend,
					TaxWithheld = div.Tax,
				});
			}

			foreach (var interest in interestTrainsactions.OrderBy(e => e.TransactionDate))
			{
				var fx = fxRates.GetByDate(interest.TransactionDate, true);
				ledger.Add(new InterestLedgerEntry
				{
					TransactionDate = interest.TransactionDate,
					SettlementDate = interest.SettlementDate,
					Beneficiary = interest.Owner,
					FxRate = fx.ToFxRateDetail(),
					Reference = interest.Reference,
					Proceeds = interest.Amount,
					Symbol = interest.Symbol,
					Company = "Fidelity Cash Account",
					TaxWithheld = interest.Tax,

				});
			}

			foreach (var transfer in transfers.OrderBy(e => e.Debit.TransactionDate))
			{
				// add the debit entry first
				ledger.Add(new ForexTransferLedgerEntry
				{
					TransactionDate = transfer.Debit.TransactionDate,
					Beneficiary = transfer.Debit.Owner,
					Reference = transfer.Debit.Reference,
					TransferFee = transfer.Debit.Fees,
					FxRate = transfer.Debit.Currency != Currency.USD
						? transfer.Debit.Fx != 0
							? new FxRateDetail
							{
								Date = transfer.Debit.TransactionDate,
								Provider = transfer.Debit.Symbol,
								Rate = transfer.Debit.Fx,
							}
							: fxRates.GetByDate(transfer.Debit.TransactionDate, true).ToFxRateDetail()
						: null,

					// Generate TransferDetails for each transaction
					LedgerEntries = ledger.Where(entry => transfer.Transactions
						.Any(e => e.Reference == entry.Reference))
						.OrderBy(entry => entry.TransactionDate)
						.Select(entry =>
							new TransferLedgerEntry
							{
								Reference = entry.Reference,
								Type = entry.GetType().Name,
								Amount = transfer.Transactions.First(e => e.Reference == entry.Reference).Amount,
							}
						).ToList(),
				});

				ledger.Add(new ForexTransferLedgerEntry
				{
					TransactionDate = transfer.Credit.TransactionDate,
					Beneficiary = transfer.Credit.Owner,
					Reference = transfer.Credit.Reference,
					TransferFee = transfer.Credit.Fees,
					FxRate = transfer.Credit.Currency != Currency.USD
						? transfer.Credit.Fx!=0
							? new FxRateDetail
								{
									Date= transfer.Credit.TransactionDate,
									Provider = transfer.Credit.Symbol,
									Rate = transfer.Credit.Fx,
								}
							: fxRates.GetByDate(transfer.Credit.TransactionDate, true).ToFxRateDetail()
						: null,

					// create a single entry that links to the debit
					LedgerEntries = new List<TransferLedgerEntry> {
						new TransferLedgerEntry {
							Reference = transfer.Debit.Reference,
							Type = typeof(ForexTransferLedgerEntry).Name,
							Amount = transfer.Debit.Amount,
						}
					}
				});
			}

			InitializeBrowser();

			string templatePath = @"C:\Users\bslater\OneDrive\Code\SharePortfolio\v1\SharePortfolio\Template-Sale.cshtml";

			var saleEntry = ledger.OfType<SellLedgerEntry>().Last(l => l.Beneficiary == "Kelly");
			GeneratePdf(@"C:\Users\bslater\OneDrive\Code\SharePortfolio\v1\SharePortfolio\Template-Sale.cshtml", saleEntry);

			var soldReferences = saleEntry.LotsSold.Select(s => s.Reference).ToList();

			foreach (var buyEntry in ledger.Where(l => soldReferences.Contains(l.Reference)))
			{
				GeneratePdf(@"C:\Users\bslater\OneDrive\Code\SharePortfolio\v1\SharePortfolio\Template-Buy.cshtml", buyEntry);
			}

			browser.CloseAsync().GetAwaiter().GetResult();
		}

		public static void GeneratePdf<T>(string templatePath, T model)
			where T : LedgerEntry
		{
			var engine = new RazorLightEngineBuilder()
				.UseFileSystemProject(Directory.GetCurrentDirectory())
				.UseMemoryCachingProvider()
				.EnableDebugMode()
				.Build();

			string template = File.ReadAllText(templatePath);
			string outputPath = @$"C:\Users\bslater\OneDrive - The Slater Family\Desktop\Results\{model.Reference.Replace('/', '-')}.pdf";
			string html = engine.CompileRenderStringAsync("templateKey", template, model).Result;
			GeneratePdf(html, outputPath);
		}

		public static void GeneratePdf(string htmlContent, string outputPath)
		{
			var page = browser.NewPageAsync().GetAwaiter().GetResult();
			page.SetContentAsync(htmlContent).GetAwaiter().GetResult();

			// Wait for the content to be fully loaded before measuring
			page.WaitForSelectorAsync("body").GetAwaiter().GetResult();

			// Measure content size
			var contentDimensions = page.EvaluateFunctionAsync<dynamic>(
				@"() => {
                        return {
                            width: document.body.scrollWidth,
                            height: document.body.scrollHeight
                        };
                    }"
			).GetAwaiter().GetResult();

			var contentWidth = contentDimensions.GetProperty("width") + "px";
			var contentHeight = (contentDimensions.GetProperty("height").GetInt32() + 50).ToString() + "px";

			// Generate the PDF with custom dimensions
			var pdfOptions = new PdfOptions
			{
				//Width = contentWidth, // Set width to match the content
				//Height = contentHeight, // Set height to match the content
				Landscape = true,
				Format = PaperFormat.A4,
				PrintBackground = true,
				MarginOptions = new MarginOptions
				{
					Top = "0.8cm",
					Bottom = "0.8cm",
					Left = "0.7cm",
					Right = "0.7cm",
				}
			};

			page.PdfAsync(outputPath, pdfOptions).GetAwaiter().GetResult();
		}

		public static DateTime GetFirstDateOfFinancialQuarter(DateTime date)
			=> new DateTime(date.Year, (((date.Month - 1) / 3 + 1) - 1) * 3 + 1, 1);

		public static DateTime GetLastDateOfFinancialQuarter(DateTime date)
			=> new DateTime(date.Year, date.Month, 1).AddMonths(3 - ((date.Month - 1) % 3)).AddDays(-1);

		internal static string GetValidationHeader(BaseEntity entity)
			=> $"[{entity.GetType().Name} Owner:{entity.Owner ?? "UNKNOWN"} Ref:{entity.Reference ?? "UNKNOWN"} Date:{entity.TransactionDate:dd-MM-yyyy} File:{entity.JsonFilename}]";



		private static string HeldForText(int days)
		{
			int years = days / 365;
			days -= (years * 365);

			string result = (years > 0)
				? $"{years} years, "
				: "";
			result += $"{days} days ";
			result += (years > 0)
				? "(Long)"
				: "(Short)";

			return result;
		}

		private static UniqueSortedSet<T> Load<T>(string pattern)
			where T : IComparable<T>, IComparable, new()
		{
			Console.CursorVisible = false;
			Console.WriteLine($"Loading {typeof(T).Name}.");
			UniqueSortedSet<T> items = new UniqueSortedSet<T>();
			int files = 0;
			foreach (string file in Directory.GetFiles(Program.StockFileRootPath, pattern, SearchOption.AllDirectories))
			{
				Console.Write($"\r  File: {Path.GetFileName(file).PadRight(40)}");
				items.AddRange(JsonSerializer.Deserialize<List<T>>(File.ReadAllText(file), jsonSerializerOptions));
				files++;
			}
			Console.WriteLine($"\r{new string(' ', Console.WindowWidth)}");
			Console.CursorTop -= 2;
			var colour = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write($"\rLoaded {files} files.                   ");
			Console.ForegroundColor = colour;
			Console.CursorLeft = 19;
			Console.WriteLine($"{items.Count} {typeof(T).Name} items loaded.");
			Console.CursorVisible = true;

			return items;
		}

		//private static List<Transfer> LoadCashTransfers()
		//{
		//    List<Transfer> transfers = new List<Transfer>();
		//    foreach (string file in Directory.GetFiles(Program.StockFileRootPath, "*-transfers.json", SearchOption.AllDirectories))
		//    {
		//        foreach (var transfer in JsonConvert.DeserializeObject<List<Transfer>>(System.IO.File.ReadAllText(file), new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd", Error=new EventHandler<System.Text.Json.Serialization.ErrorEventArgs>(JsonError)}))
		//        {
		//            transfer.Debit.JsonFilename = transfer.Credit.JsonFilename = Path.GetFileName(file);
		//            transfers.Add(transfer);
		//        }
		//    }

		//    Console.WriteLine($"Loaded {transfers.Count} Money Transfer Transactions");
		//    return transfers;
		//}

		//private static Dictionary<string, List<StockDividend>> LoadStockDividend()
		//{
		//    Dictionary<string, List<StockDividend>> stockDividends = new Dictionary<string, List<StockDividend>>();
		//    foreach (string file in Directory.GetFiles(Program.StockFileRootPath, "*-dividendrecords.json", SearchOption.AllDirectories))
		//    {
		//        List<StockDividend> dividends = new List<StockDividend>();
		//        string symbol = Path.GetFileNameWithoutExtension(file);
		//        symbol = symbol.Substring(0, symbol.IndexOf('-')).ToUpperInvariant();
		//        foreach (var dividend in JsonConvert.DeserializeObject<List<StockDividend>>(System.IO.File.ReadAllText(file), new JsonSerializerSettings { DateFormatString = "yyyy-MM-dd", Error=new EventHandler<System.Text.Json.Serialization.ErrorEventArgs>(JsonError)}))
		//        {
		//            dividends.Add(dividend);
		//        }
		//        stockDividends[symbol] = dividends;
		//    }

		//    Console.WriteLine($"Loaded {stockDividends.Sum(s => s.Value.Count)} Stock Dividends");
		//    return stockDividends;
		//}


		//public static void JsonError(object sender, System.Text.Json.Serialization.ErrorEventArgs args)
		//{
		//    //Debug.WriteLine(args.CurrentObject.ToString());
		//    Debug.WriteLine(args.ErrorContext.Error.Source);
		//    Debug.WriteLine(args.ErrorContext.Path);
		//    Debug.WriteLine(args.ErrorContext.Error.Message);
		//}

		//private static Dictionary<string, Company> LoadStockSymbols()
		//{
		//    List<Company> companies = JsonConvert.DeserializeObject<List<Company>>(System.IO.File.ReadAllText(Path.Combine(Program.StockFileRootPath, "symbols.json")), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DateFormatString = "yyyy-MM-dd" });

		//    return new Dictionary<string, Company>(companies.Select<Company, KeyValuePair<string, Company>>(e => new KeyValuePair<string, Company>(e.Symbol, e)));
		//}


		private static List<string> ValidateData()
		{
			List<string> exceptions = new List<string>();
			//exceptions.AddRange(StockAwardLots.SelectMany(e => e.Value.Validate()));
			//exceptions.AddRange(StockEsppLots.SelectMany(e => e.Value.Validate()));
			//exceptions.AddRange(ForeignInterestEarnings.SelectMany(e => e.Value.Validate()));
			//exceptions.AddRange(StockDividendEarnings.SelectMany(e => e.Value.Validate()));

			//ShareLots = new Dictionary<string, StockLot>();
			//foreach (var item in StockAwardLots) ShareLots.Add(item.Key, item.Value);
			//foreach (var item in StockEsppLots) ShareLots.Add(item.Key, item.Value);
			//exceptions.AddRange(StockLotSales.SelectMany(e => e.Value.Validate()));

			//TransactionProceeds = new Dictionary<string, ITransactionType>();
			//foreach (var item in StockDividendEarnings) TransactionProceeds.Add(item.Key, item.Value);
			//foreach (var item in ForeignInterestEarnings) TransactionProceeds.Add(item.Key, item.Value);
			//foreach (var item in StockLotSales) TransactionProceeds.Add(item.Key, item.Value);
			////foreach (var item in CashTransfers.Where(t => t.Debit.Symbol == "FDRXX" && t.Debit.Fees > 0)) TransactionProceeds.Add(item.Debit.Reference, new FeeTransaction(item.Debit.Reference, item.Owner, item.Debit.TransactionDate, 0, item.Debit.Fx));
			////foreach (var item in CashTransfers.Where(t => t.Debit.Symbol == "FDRXX" && t.Credit.Fees > 0)) TransactionProceeds.Add(item.Credit.Reference, new FeeTransaction(item.Credit.Reference, item.Owner, item.Credit.TransactionDate, 0, item.Credit.Fx));
			//foreach (var item in CashTransfers.Where(t => t.Debit.Symbol == "FDRXX" && t.Debit.Fees > 0)) TransactionProceeds.Add(item.Debit.Reference, new TransferFee(item.Debit.Entered, item.Debit.Symbol, item.Debit.Reference, item.Owner, item.Debit.TransactionDate, item.Debit.Currency, item.Debit.Fees, item.Debit.Fx, item.Debit.Overrides.ToList()));
			//foreach (var item in CashTransfers.Where(t => t.Debit.Symbol == "FDRXX" && t.Credit.Fees > 0)) TransactionProceeds.Add(item.Credit.Reference, new TransferFee(item.Credit.Entered, item.Credit.Symbol, item.Credit.Reference, item.Owner, item.Credit.TransactionDate, item.Credit.Currency, item.Credit.Fees, item.Credit.Fx,  item.Credit.Overrides.ToList()));
			//exceptions.AddRange(CashTransfers.OrderBy(t => t.Debit.TransactionDate).SelectMany(e => e.Validate()));

			//decimal amountRemaining = 0;
			//foreach (var trans in TransactionProceeds.Values.OrderBy(t => t.TransactionDate))
			//{
			//    var amount = trans.NetAmount - trans.NetAmountTransfered;
			//    if (amountRemaining > 0 && amount == 0) exceptions.Add($"{Program.GetValidationHeader(trans as BaseEntity)} transaction amount ({trans.NetAmount:#,##0.0###}) does not equal net amount transferred ({trans.NetAmountTransfered:#,##0.0###}).");
			//}

			//// validate share quantity at record date
			//foreach (var item in StockDividendEarnings.Values)
			//{
			//    var aquired = ShareLots.Values.Where(l => l.Symbol == item.Symbol && l.Owner == item.Owner && l.TransactionDate <= item.RecordDate).Sum(l => StockSplitManager.GetAdjustedQuantity(item.Symbol, l.Quantity, l.TransactionDate, item.RecordDate));
			//    var sold = StockLotSales.Values.Where(l => l.Symbol == item.Symbol && l.Owner == item.Owner && l.TransactionDate <= item.RecordDate).Sum(l => StockSplitManager.GetAdjustedQuantity(item.Symbol, l.Quantity, l.TransactionDate, item.RecordDate));
			//    decimal shares = aquired - sold;

			//    if (item.Shares != shares) exceptions.Add($"{Program.GetValidationHeader(item)} given share quantity ({item.Shares:#,##0.0###}) does not equal accquired shares ({aquired:#,##0.0###}) less sold shares ({sold:#,##0.0###}): shares calculated={shares} .");
			//}

			//// validate all dividends have been entered
			//var lots = StockAwardLots.Values.Cast<StockLot>().Union(StockEsppLots.Values.Cast<StockLot>());
			//string[] owners = lots.GroupBy(l => l.Owner).Select(g => g.Key).ToArray();
			//foreach (string owner in owners)
			//{
			//    foreach (var dividend in StockDividends["MSFT"].Where(d => d.Payment < DateTime.Today).OrderBy(d => d.Payment))
			//    {
			//        if (lots.Where(l => l.Owner == owner && l.SettlementDate <= dividend.ExDividend).Sum(l => l.Quantity) > 0)
			//        {
			//            if (StockDividendEarnings.Values.Where(d => d.Owner == owner && d.RecordDate == dividend.Record).Count() == 0)
			//            {
			//                var item = new StockDividend(false, "MSFT", "UNKNOWN", owner, dividend.Record, dividend.Payment, dividend.Payment, 0, Currency.USD, 0, 0, 0, null);
			//                exceptions.Add($"{Program.GetValidationHeader(item)} Dividend is missing where owner has stock.");
			//            }
			//        }
			//    }
			//}

			return exceptions;
		}
	}
}