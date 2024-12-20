using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public class StockSale
        : BaseEntity
        , IHasDateProperty
		, ITransactionType
    {

        private List<Lot> lots = new List<Lot>();

        [JsonPropertyOrder(63)]
        public decimal Amount { get; init; }

       [JsonPropertyOrder(62)]
        public Currency Currency { get; init; }

       [JsonPropertyOrder(64)]
        public decimal Fees { get; init; }

       [JsonPropertyOrder(65)]
        public decimal Tax { get; init; }

       [JsonPropertyOrder(61)]
        public decimal Quantity { get; init; }

       [JsonPropertyOrder(1000)]
        public List<Lot> Lots { get; init; } = new List<Lot>();

       [JsonPropertyOrder(10)]
        public DateOnly SettlementDate { get; init; }

       [JsonPropertyOrder(60)]
        public decimal StrikePrice { get; init; }

        public decimal NetAmount => this.Amount - this.Fees;

		DateOnly IHasDateProperty.Date=>this.TransactionDate;

		//public override List<string> Validate()
		//{
		//    List<string> exceptions = base.Validate();

		//    // map the lots to transaction lots
		//    Lot[] lots = this.lots.ToArray();
		//    this.lots = new List<Lot>();
		//    for (int i = 0; i < lots.Length; i++)
		//    {
		//        //Debug.Assert(lots[i].Reference != "18058-1447890");
		//        var stockLot = ShareLots[lots[i].Reference] as StockLot;
		//        if (stockLot is StockLot)
		//        {
		//            stockLot.SoldShares(lots[i].Quantity);
		//            this.lots.Add(new Lot(lots[i].Reference, lots[i].Quantity, stockLot, this));
		//            if (stockLot.Quantity - stockLot.Sold < 0) exceptions.Add($"{Program.GetValidationHeader(this)} insufficient stocks in lot {stockLot.Reference}. available:{stockLot.Sold + lots[i].Quantity} sold:{lots[i].Quantity}");
		//        }
		//        else
		//        {
		//            exceptions.Add($"{Program.GetValidationHeader(this)} Unknown stock lot transaction: #{lots[i].Reference}");
		//        }
		//    }

		//    if (this.Fx == decimal.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} missing fx rate for {this.TransactionDate:dd/MM/yyyy}");
		//    if (this.SettlementDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid settlement date.");
		//    if (this.StrikePrice == 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid strike price. {this.StrikePrice}");
		//    if (this.Quantity <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid sale quantity. {this.Quantity}");
		//    if (this.Amount <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid sale proceed amount. {this.Amount}");
		//    if (this.Fees < 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid sale fees amount. {this.Fees}");
		//    if (this.lots.Count == 0) exceptions.Add($"{Program.GetValidationHeader(this)} no stock lots specified.");
		//    if (Math.Round(this.Quantity * this.StrikePrice, 2, MidpointRounding.AwayFromZero) != this.Amount) exceptions.Add($"{Program.GetValidationHeader(this)} Amount does not equal quantity x strike price: {Math.Round(this.Quantity * this.StrikePrice, 2, MidpointRounding.AwayFromZero)}");
		//    if (this.lots.Sum(l => l.Quantity) != this.Quantity) exceptions.Add($"{Program.GetValidationHeader(this)} Sum of lots does not equal quantity: {this.lots.Sum(l => l.Quantity)}");

		//    var proceeds = this.Lots.Sum(l => l.CostBase + l.DisposalGain);
		//    if (proceeds != this.Amount) exceptions.Add($"{Program.GetValidationHeader(this)} Lot USD proceeds {this.Amount} is not the same as sale amount: {proceeds}");

		//    proceeds = this.lots.Sum(l => l.CostBaseTotalAu + l.DisposalGainAu);
		//    if (Math.Abs(proceeds - this.AmountAu) > 0.03M) exceptions.Add($"{Program.GetValidationHeader(this)} Lot AUD proceeds {this.AmountAu} is not the same as sale amount: {proceeds}");

		//    exceptions.AddRange(this.lots.Where(l => l.Owner != this.Owner).Select(l => $"{Program.GetValidationHeader(this)} Owner of Lot {l.Reference} is different to sale owner {this.Owner}"));
		//    return exceptions;
		//}

		public struct Lot
        {

           [JsonPropertyOrder(2)]
            public decimal Quantity { get; init; }
           [JsonPropertyOrder(1)]
            public string Reference { get; init; }
        }
    }
}
