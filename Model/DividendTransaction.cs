using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public class DividendTransaction 
        : FinancialTransaction
    {
       [JsonPropertyOrder(60)]
        public decimal Dividend { get; init; }

       [JsonPropertyOrder(10)]
        public DateOnly RecordDate { get; init; }

       [JsonPropertyOrder(11)]
        public DateOnly SettlementDate { get; init; }

       [JsonPropertyOrder(61)]
        public decimal Shares { get; init; }

        //public override List<string> Validate()
        //{
        //    List<string> exceptions = base.Validate();

        //    if (this.RecordDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid record date.");
        //    if (this.SettlementDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid settlement date.");
        //    if (this.RecordDate >= this.SettlementDate) exceptions.Add($"{Program.GetValidationHeader(this)} record date greater than or equal to settlement date.");
        //    if (this.Dividend <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid dividend. {this.Dividend}");
        //    if (this.Shares <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid shares. {this.Shares}");
        //    if (this.Tax <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid tax. {this.Tax}");
        //    if (Math.Round(this.Shares * this.Dividend, 2, MidpointRounding.AwayFromZero) != this.Amount) exceptions.Add($"{Program.GetValidationHeader(this)} shares @ dividend does not equal total. {Math.Round(this.Shares * this.Dividend, 2, MidpointRounding.AwayFromZero)}");

        //    Dividend[] dividends = StockDividends[this.Symbol].Where(d => d.Record == this.RecordDate).ToArray();
        //    if (dividends == null)
        //    {
        //        exceptions.Add($"{Program.GetValidationHeader(this)} Dividend details do not match published company dividends");
        //    }
        //    else
        //    {
        //        if (dividends.All(d => this.Dividend != d.Amount)) exceptions.Add($"{Program.GetValidationHeader(this)} Dividend does not match published company dividend. Entered:{this.Dividend}, Published:{string.Join(',', dividends.Select(d => d.Amount.ToString("0.00")))}");
        //        if (dividends.All(d => this.TransactionDate != d.Payment)) exceptions.Add($"{Program.GetValidationHeader(this)} Dividend transaction date does not match published payment date. Entered:{this.TransactionDate}, Published:{string.Join(',', dividends.Select(d => d.Payment.ToString("dd-MMM-yyyy")))}");
        //    }

        //    return exceptions;
        //}
    }
}
