using System.Text.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    internal class EsppLot
        : StockLotBase
    {
       [JsonPropertyOrder(61)]
        public DateOnly PeriodEnd { get; init; }

        [JsonPropertyOrder(60)]
        public DateOnly PeriodStart { get; init; }

        [JsonPropertyOrder(62)]
        public decimal ESPPAmount { get; init; }

       [JsonPropertyOrder ( 65)]
        public decimal ESPPContribution { get; init; }

       [JsonPropertyOrder (63)]
        [DefaultValue("AUD")]
        public string ESPPCurrency { get; init; }

        [JsonPropertyOrder ( 65)]
       public decimal Excess { get; init; }

        public decimal BroughtForward { get; set; }

       [JsonPropertyOrder( 64)]
        public decimal Refunded { get; init; }

       [JsonPropertyOrder( 64)]
        public decimal Residual { get; init; }

        //public override List<string> Validate()
        //{
        //    List<string> exceptions = base.Validate();

        //    if (this.PeriodStart == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid ESPP period start date.");
        //    if (this.PeriodEnd == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid ESPP period end date.");
        //    if (!IsValidPeriod(this.PeriodStart, this.PeriodEnd)) exceptions.Add($"{Program.GetValidationHeader(this)} invalid period start or end date. start:{this.PeriodStart:dd-MMM-yyyy} end:{this.PeriodEnd:dd-MMM-yyyy}");
        //    if (this.EsppAmount <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid espp deduction amount. {this.EsppAmount}");
        //    if (this.EsppContribution <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid espp contribution amount. {this.EsppContribution}");
        //    if (!StockQuotes[this.Symbol].ContainsKey(this.TransactionDate) || StockQuotes[this.Symbol][this.TransactionDate] == null) exceptions.Add($"{Program.GetValidationHeader(this)} Stock Price missing: Symbol: {this.Symbol} Date:{this.TransactionDate}");
        //    decimal adjustedFMV = StockSplitManager.GetAdjustedPrice(this.Symbol, this.UnitFMV, this.TransactionDate, DateTime.Now.Date);
        //    if (StockQuotes[this.Symbol].ContainsKey(this.TransactionDate) && Math.Abs(StockQuotes[this.Symbol][this.TransactionDate].Close - adjustedFMV) > 0.005m) exceptions.Add($"{Program.GetValidationHeader(this)} FMV valid provided doesn't match historical trading data: FMV:{this.UnitFMV}({adjustedFMV}) Close:{StockQuotes[this.Symbol][this.TransactionDate].Close} Date:{this.TransactionDate:dd/MM/yyyy}");
        //    if (this.Excess < 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid excess amount.");
        //    if (this.Refunded < 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid refund amount.");
        //    if (this.Excess > 0 | this.Refunded > 0)
        //    {
        //        if (this.Refunded <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} missing refund amount.");
        //        if (this.Excess <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} missing excess amount.");
        //    }
        //    if (this.Residual < 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid residual amount.");
        //    if (this.BroughtForward < 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid brought forward amount.");
        //    if (this.BroughtForward + this.EsppContribution - this.CheckCost != this.Residual + this.Excess)
        //    {
        //        if (!this.Overrides.Contains(OverrideTypes.DataEntryBalancesInvalid))
        //        {
        //            exceptions.Add($"{Program.GetValidationHeader(this)} Non-Balancing Transaction. Opening:{this.BroughtForward:#,##0.00} + Contribution:{this.EsppContribution:#,##0.00} - CheckCost:{this.CheckCost:#,##0.00} != Residual:{this.Residual:#,##0.00} + Excess:{this.Excess:#,##0.00}. Delta: {this.BroughtForward + this.EsppContribution - this.CheckCost - this.Residual - this.Excess:#,##0.00##}");
        //        }
        //        else
        //        {
        //            Program.PrintOverride(this, $"{Enum.GetName<OverrideTypes>(OverrideTypes.DataEntryBalancesInvalid)}: (Balance:{this.BroughtForward} + Espp:{this.EsppAmountUs} - Cost:{this.CheckCost} - Residual:{this.Residual} - Excess:{this.Excess:#,##0.00} != 0): Result: {this.BroughtForward + this.EsppContribution - this.CheckCost - this.Residual - this.Excess:#,##0.00##}");
        //        }
        //    }

        //    /*
        //                    if (this.BroughtForward + this.EsppAmountUs - this.BuyCost != this.Residual + this.Excess)
        //                    {
        //                        if (!this.Overrides.Contains(OverrideTypes.TransactionBalancesInvalid))
        //                        {
        //                            exceptions.Add($"{Program.GetValidationHeader(this)} Non-Balancing Transaction. Opening:{this.BroughtForward:#,##0.00} + EsppUsd:{this.EsppAmountUs:#,##0.00} - BuyCost:{this.BuyCost:#,##0.00} != Residual:{this.Residual:#,##0.00} + Excess:{this.Excess:#,##0.00}. Delta: {this.BroughtForward + this.EsppAmountUs - this.BuyCost - this.Residual - this.Excess:#,##0.00##}");
        //                        }
        //                        else
        //                        {
        //                            Program.PrintOverride(this, $"{Enum.GetName<OverrideTypes>(OverrideTypes.TransactionBalancesInvalid)}: (Balance:{this.BroughtForward} + EsppUsd:{this.EsppAmountUs} - BuyCost:{this.BuyCost} - Residual:{this.Residual} - Excess:{this.Excess:#,##0.00} != 0): Result: {this.BroughtForward + this.EsppAmountUs - this.BuyCost - this.Residual - this.Excess:#,##0.00##}");
        //                        }
        //                    }
        //    */
        //    var checkEsppPrice = EsppPriceHistory.First(e => e.PeriodStart == this.PeriodStart).PeriodPurchasePrice;
        //    if (checkEsppPrice != this.UnitCost) exceptions.Add($"{Program.GetValidationHeader(this)} Espp Buy Price Mismatch. Price given '{checkEsppPrice:0.00##}'");

        //    if (this.IsEssApplicable && this.BuyFMVAu - this.BuyCostAu != this.AcquisitionGainAu) exceptions.Add($"{Program.GetValidationHeader(this)} ESS calculation error.");
        //    if (this.EsppAmount != this.BuyFMVAu - this.ForexGain - this.AcquisitionGainAu + this.ExcessAu) exceptions.Add($"{Program.GetValidationHeader(this)} transaction amounts do not equal total. {this.EsppAmount:#,##0.00}, {this.BuyFMVAu - this.ForexGain - this.AcquisitionGainAu + this.ExcessAu:#,##0.00}");

        //    //var esppAmountCalc = Math.Round((this.Residual - this.BroughtForward +this.BuyCost) / GetFxRates(this.TransactionDate).ReserveBank.Value, 2, MidpointRounding.AwayFromZero);

        //    //if (StockPrices[symbol][transactionDate].Close!=unitFMV) throw new ArgumentException($"{symbol} Close does not match FMV for award: {transactionDate:dd-MMM-yyyy}={StockPrices[symbol][transactionDate].Close}", nameof(unitFMV));
        //    if (this.EsppFx >= 0 & this.Fx >= 0)
        //    {
        //        decimal gap = Math.Abs(this.EsppFx - this.Fx);
        //        if (!this.Overrides.Contains(OverrideTypes.FxRateThreshold))
        //        {
        //            if (gap > Program.FxTolerance) exceptions.Add($"{Program.GetValidationHeader(this)} {Enum.GetName<OverrideTypes>(OverrideTypes.FxRateThreshold)}: ESPP Purchase Fx Tolerance Exceeded - check forex rates. Buy fx:{this.EsppFx:0.0000} Forex:{this.Fx:0.0000} ({gap:0.0000##})");
        //        }
        //        else
        //        {
        //            Program.PrintOverride(this, $"{Enum.GetName<OverrideTypes>(OverrideTypes.FxRateThreshold)}: ESPP Purchase Fx Tolerance ({Program.FxTolerance:0.00####}) Espp:{this.EsppFx:0.0000} Forex:{this.Fx:0.0000} ({gap:0.0000##})");
        //        }
        //    }
        //    else
        //    {
        //        exceptions.Add($"{Program.GetValidationHeader(this)} Fx rates are invalid: {this.EsppFx:0.0000} Forex: {this.Fx:0.0000} ");
        //    }
        //    return exceptions;
        //}

        //private static bool IsValidPeriod(DateTime start, DateTime end)
        //{
        //    if (end <= start) return false;
        //    if ((start.Month + 2) % 3 != 0) return false;
        //    if (end.Month % 3 != 0) return false;
        //    return true;
        //}
    }
}
