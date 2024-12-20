using SharePortfolio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio
{
	public class DividendRecord
	{
		public DateOnly Announced { get; set; }
		public DateOnly ExDividend { get; set; }
		public DateOnly Record { get; set; }
		public DateOnly Payment { get; set; }
		public string Type { get; set; }
		public decimal Amount { get; set; }
	}

	public interface ICashLedgerEntry
	{
		decimal NetProceeds { get; }
		string Reference { get; }
	}

	public class FxRateDetail
	{
		public DateOnly Date { get; init; }
		public decimal Rate { get; init; }
		public string Provider { get; init; }
	}

	public abstract class LedgerEntry : IComparable<LedgerEntry>, IComparable
	{
		public DateOnly TransactionDate { get; init; }
		public DateOnly SettlementDate { get; init; }
		public string Reference { get; init; }
		public string Company { get; init; }
		public string Beneficiary { get; init; }
		public FxRateDetail FxRate { get; init; }
		protected decimal CalculateAUD(decimal amount) => Math.Round(amount / FxRate.Rate, 2);

		public int CompareTo(object? obj) => obj is LedgerEntry other ? CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(LedgerEntry)}", nameof(obj));
		public int CompareTo(LedgerEntry? other) => other == null ? 1 : TransactionDate.CompareTo(other.TransactionDate) == 0 ? Reference.CompareTo(other.Reference) : TransactionDate.CompareTo(other.TransactionDate);
	}

	public abstract class FinancialLedgerEntry : LedgerEntry
	{
		protected abstract decimal ProceedDeductions { get; }
		public virtual decimal Proceeds { get; init; }
		public decimal ProceedsAUD => CalculateAUD(this.Proceeds);
		public decimal NetProceeds => this.Proceeds - this.ProceedDeductions;
		public decimal NetProceedsAUD => CalculateAUD(this.NetProceeds);

		// Consolidated method for rounding and currency conversions
		public decimal Rounding => this.NetProceedsAUD - (this.ProceedsAUD - Math.Round(this.ProceedDeductions / this.FxRate.Rate, 2));
	}

	// Buy Ledger Entry
	public class AwardLedgerEntry : BuyLedgerEntry
	{
		public string AwardName { get; init; }
		public DateOnly AwardDate { get; init; }
		public string AwardId { get; init; }
	}

	public class EsppLedgerEntry : BuyLedgerEntry
	{
		public DateOnly EsppPeriodStart { get; init; }
		public DateOnly EsppPeriodEnd { get; init; }
		public decimal EsppEmployeeContribution { get; init; }
		public decimal EsppEmployeeContributionRefunded { get; init; }
		public decimal EsppFundContribution { get; init; }
		public decimal EsppFundExcess { get; init; }
		public decimal EsppFundResidual { get; init; }

		public override decimal CostAUD => this.NetEsppEmployeeContribution;
		public bool EsppFundEffectiveFxIsValid =>
			Math.Round(this.EsppFundContribution / this.EsppEmployeeContribution, 4) ==
			Math.Round(this.NetEsppFundContribution / this.NetEsppEmployeeContribution, 4);
		public decimal EsppFundExcessAUD => CalculateAUD(this.EsppFundExcess);
		public decimal NetEsppEmployeeContribution => this.EsppEmployeeContribution - this.EsppEmployeeContributionRefunded;
		public decimal NetEsppFundContribution => this.EsppFundContribution - this.EsppFundExcess;
		public decimal EsppFundContributionAUD => CalculateAUD(this.EsppFundContribution);
		public FxRateDetail EsppFundEffectiveFx => new FxRateDetail
		{
			Rate = Math.Round(this.EsppFundContribution / this.EsppEmployeeContribution, 4),
			Date = this.TransactionDate,
			Provider = ""
		};
	}


	public abstract class BuyLedgerEntry : LedgerEntry
	{
		public decimal Quantity { get; init; }
		public string Symbol { get; init; }
		public string Description { get; init; }
		public decimal StrikePrice { get; init; }
		public decimal MarketPrice { get; init; }
		public decimal Fees { get; init; }
		public decimal EssTaxPaid { get; init; }
		public string EssTaxType { get; init; }
		public decimal Cost => Math.Round(this.StrikePrice * this.Quantity, 2);
		public FxRateDetail CostFxRate => this.Cost == 0
			? this.FxRate
			: new FxRateDetail
			{
				Rate = Math.Round(this.Cost / this.CostAUD, 4),
				Date = this.TransactionDate,
				Provider = ""
			};
		public virtual decimal CostAUD => CalculateAUD(this.Cost);
		public decimal CostBaseAUD => this.CostAUD + this.EssTaxPaid;
		public decimal StrikePriceAUD => Math.Round(this.CostAUD / this.Quantity, 4);
		public decimal Value => Math.Round(this.MarketPrice * this.Quantity, 2);
		public decimal ValueAUD => CalculateAUD(this.Value);
		public decimal MarketPriceeAUD => Math.Round(this.ValueAUD / this.Quantity, 4);
		public decimal EssGain => this.MarketPrice - this.StrikePrice;
		public decimal EssTotalGain => this.EssGain * this.Quantity;
	}

	// Sell Ledger Entry
	public class SellLedgerEntry : FinancialLedgerEntry, ICashLedgerEntry
	{
		public struct CapitalGainSummary
		{
			public CapitalGainSummary() { }

			public bool IsLongTerm { get; init; }
			public List<LotLedgerEntry> LotsSold { get; init; } = new();
			public decimal Rounding { get; init; }
			public decimal CostBaseAUD => LotsSold.Sum(l => l.CostAUD + l.EssTaxPaid);
			public decimal Quantity => LotsSold.Sum(l => l.QuantitySold);
			public decimal Fees => LotsSold.Sum(l => l.Fees);
			public decimal FeesAUD => LotsSold.Sum(l => l.FeesAUD);
			public decimal Proceeds => LotsSold.Sum(l => l.SaleProceeds);
			public decimal ProceedsAUD => LotsSold.Sum(l => l.SaleProceedsAUD);
			public decimal NetProceedsAUD => ProceedsAUD - FeesAUD;
			public decimal NetCapitalGainAUD => NetProceedsAUD - CostBaseAUD;
		}

		protected override decimal ProceedDeductions => BrokerageFees;
		public string Symbol { get; init; }
		public decimal Quantity { get; init; }
		public decimal StrikePrice { get; init; }
		public decimal BrokerageFees { get; init; }
		public List<LotLedgerEntry> LotsSold { get; init; } = new();

		public CapitalGainSummary CapitalLossLotsSold => new CapitalGainSummary
		{
			IsLongTerm = true,
			LotsSold = LotsSold.Where(l => l.IsLoss).ToList(),
		};

		public CapitalGainSummary LongTermLotsSold => new CapitalGainSummary
		{
			IsLongTerm = true,
			LotsSold = LotsSold.Where(l => l.IsLongTerm && !l.IsLoss).ToList(),
		};

		public CapitalGainSummary ShortTermLotsSold => new CapitalGainSummary
		{
			IsLongTerm = false,
			LotsSold = LotsSold.Where(l => !l.IsLongTerm && !l.IsLoss).ToList(),
		};

		public decimal CapitalLossesAUD => this.CapitalLossLotsSold.NetProceedsAUD - this.CapitalLossLotsSold.CostBaseAUD;
		public decimal ShortTermCapitalGainAUD => this.ShortTermLotsSold.NetProceedsAUD - this.ShortTermLotsSold.CostBaseAUD;
		public decimal ShortTermCapitaCapitalLossesAUD => Math.Min(CapitalLossesAUD, ShortTermCapitalGainAUD);
		public decimal NetShortTermCapitalGainAUD => ShortTermCapitalGainAUD - ShortTermCapitaCapitalLossesAUD;
		public decimal LongTermCapitalGainAUD => this.LongTermLotsSold.NetProceedsAUD - this.LongTermLotsSold.CostBaseAUD;
		public decimal LongTermCapitalGainConessionAmount => Math.Round(this.NetLongTermCapitalGainAUD * 0.5m, 2, MidpointRounding.ToZero);
		public decimal LongTermCapitaCapitalLossesAUD => CapitalLossesAUD - ShortTermCapitaCapitalLossesAUD;
		public decimal NetLongTermCapitalGainAUD => this.LongTermCapitalGainAUD - this.LongTermCapitaCapitalLossesAUD;
		public decimal NetLongTermDiscountedCapitalGainAUD => this.NetLongTermCapitalGainAUD - this.LongTermCapitalGainConessionAmount;
		public decimal NetCapitalGainAUD => NetShortTermCapitalGainAUD + NetLongTermDiscountedCapitalGainAUD;
	}

	public class LotLedgerEntry
	{
		public DateOnly BuyDate { get; init; }
		public string Reference { get; init; }
		public decimal QuantitySold { get; init; }
		public decimal CostAUD { get; init; }
		public decimal EssTaxPaid { get; init; }
		public decimal Fees { get; init; }
		public decimal FeesAUD { get; init; }
		public decimal BuyStrikePrice { get; init; }
		public decimal BuyMarketPrice { get; init; }
		public decimal SaleProceeds { get; init; }
		public decimal SaleProceedsAUD { get; init; }
		public decimal ForexGain { get; init; }
		public FxRateDetail FxRate { get; init; }
		public bool IsLongTerm { get; init; }
		public bool IsLoss => this.SaleProceedsAUD - this.FeesAUD < this.CostBaseAUD;
		public decimal CostBaseAUD => this.CostAUD + this.EssTaxPaid;
		public decimal EssGain => BuyMarketPrice - BuyStrikePrice;
		public decimal EssTotalGain => EssGain * QuantitySold;
		public decimal CostUnitAUD => Math.Round(this.CostAUD / this.QuantitySold, 4);
	}

	// Dividend Ledger Entry
	public class DividendLedgerEntry : FinancialLedgerEntry, ICashLedgerEntry
	{
		protected override decimal ProceedDeductions => TaxWithheld;
		public string Symbol { get; init; }
		public decimal TaxWithheld { get; init; }
		public decimal SharesHeld { get; init; }
		public DateOnly ExDividendDate { get; init; }
		public decimal Dividend { get; init; }

		public decimal TaxWithheldAUD => CalculateAUD(this.TaxWithheld);
	}

	// Interest Ledger Entry
	public class InterestLedgerEntry : FinancialLedgerEntry, ICashLedgerEntry
	{
		protected override decimal ProceedDeductions => TaxWithheld;
		public decimal TaxWithheld { get; init; }
		public string Symbol { get; init; }

		public decimal TaxWithheldAUD => CalculateAUD(this.TaxWithheld);
	}

	// Fees
	public class TransferFee : FinancialLedgerEntry, ICashLedgerEntry
	{
		protected override decimal ProceedDeductions => 0;
	}

	// Forex Transfer Ledger Entry
	public class ForexTransferLedgerEntry : FinancialLedgerEntry, ICashLedgerEntry
	{
		public List<TransferLedgerEntry> LedgerEntries { get; init; } = new();
		public decimal TransferFee { get; init; }
		protected override decimal ProceedDeductions => this.TransferFee;
		public override decimal Proceeds => LedgerEntries.Sum(td => td.Amount);
	}

	// Forex Transfer Detail
	public class TransferLedgerEntry
	{
		public string Reference { get; init; }
		public string Type { get; init; }
		public decimal Amount { get; init; }
	}
}