using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharePortfolio.Program;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public class AwardLot 
        : StockLotBase
    {
       [JsonPropertyOrder(60)]
        public DateOnly AwardDate { get; init; }

       [JsonPropertyOrder(61)]
        public string AwardId { get; init; }

       [JsonPropertyOrder(62)]
        public string AwardType { get; init; }

        //public override List<string> Validate()
        //{
        //    List<string> exceptions = base.Validate();

        //    if (this.AwardDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid award date.");
        //    if (this.AwardId == 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid award identifier. {this.AwardId}");
        //    if (string.IsNullOrEmpty(this.AwardType)) exceptions.Add($"{Program.GetValidationHeader(this)} invalid award type description. {this.AwardType ?? "(null)"}");

        //    var buyFmv = StockSplitManager.GetAdjustedPrice(this.Symbol, this.BuyUnitFMV, this.TransactionDate, DateTime.Today);
        //    // Effective for transactions occurring May 17, 2010 and later, the cost basis for shares received at vest is the
        //    // previous market day closing fair market value (FMV).
        //    // For example, if your vest date is August 31, 2021, the closing FMV on August 30, 2021 will be used.
        //    var quote = (this.TransactionDate >= new DateTime(2010, 5, 17))
        //        ? GetPrecedingQuote(this.Symbol, this.TransactionDate)
        //        : StockQuotes[this.Symbol][this.TransactionDate];
        //    if (Math.Abs(buyFmv - quote.Close) > 0.001m) exceptions.Add($"{Program.GetValidationHeader(this)} FMV outside market close value. FMV:{buyFmv:0.00##} Quote:{quote} ({Math.Abs(buyFmv - quote.Close)})");

        //    if (this.IsEssApplicable && this.BuyFMVAu != this.AcquisitionGainAu) exceptions.Add($"{Program.GetValidationHeader(this)} ESS calculation error.");

        //    return exceptions;
        //}
    }
}
