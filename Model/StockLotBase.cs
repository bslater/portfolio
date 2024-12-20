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
    public abstract class StockLotBase 
        : BaseEntity
		, IHasDateProperty
	{

        [JsonPropertyOrder(51)]
        public decimal UnitCost { get; init; }

       [JsonPropertyOrder(52)]
        public decimal UnitFMV { get; init; }

       [JsonPropertyOrder ( 60)]
        [DefaultValue(-1)]
        public decimal ReportedEssAmount { get; init; }

       [JsonPropertyOrder(50)]
        public decimal Quantity { get; init; }

       [JsonPropertyOrder(10)]
        public DateOnly SettlementDate { get; init; }

        [JsonPropertyOrder(90)]
        public decimal CheckCost { get; init; }

        [JsonPropertyOrder(91)]
        public decimal CheckFMV { get; init; }
        DateOnly IHasDateProperty.Date=> this.TransactionDate;

		//public override List<string> Validate()
		//{
		//    List<string> exceptions = base.Validate();

		//    if (this.Fx == decimal.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} missing fx rate for {this.TransactionDate:dd/MM/yyyy}.");
		//    if (this.IsEssApplicable)
		//    {
		//        if (this.ReportedEssAmount == -1) exceptions.Add($"{Program.GetValidationHeader(this)} missing ESS Amount. Calculated:{this.EssCalc}");
		//        else if (Math.Abs(this.ReportedEssAmount - this.EssCalc) > 1.0m)
		//        {
		//            if (!this.Overrides.Contains(OverrideTypes.EssAmountThreshold))
		//            {
		//                exceptions.Add($"{Program.GetValidationHeader(this)} {Enum.GetName<OverrideTypes>(OverrideTypes.EssAmountThreshold)}: ESS Amount exceeds tollerance. Ess:{this.ReportedEssAmount}, Calculated:{this.EssCalc}, Delta:{this.ReportedEssAmount - this.EssCalc}");
		//            }
		//            else
		//            {
		//                Program.PrintOverride(this, $"{Enum.GetName<OverrideTypes>(OverrideTypes.EssAmountThreshold)}: issues with ESS Amount exeeding tollerance:{this.ReportedEssAmount}, Calculated:{this.EssCalc}, Delta:{this.ReportedEssAmount - this.EssCalc}");
		//            }
		//        }
		//    }
		//    if (this.EssFx == decimal.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} missing ESS fx rate for {this.TransactionDate:dd/MM/yyyy}");
		//    if (this.SettlementDate == DateTime.MinValue || this.SettlementDate < this.TransactionDate) exceptions.Add($"{Program.GetValidationHeader(this)} invalid settlement date.");
		//    if ((this.SettlementDate - this.TransactionDate).Days > 21)
		//    {
		//        if (!this.Overrides.Contains(OverrideTypes.SettlementDateThreshold))
		//        {
		//            exceptions.Add($"{Program.GetValidationHeader(this)} {Enum.GetName<OverrideTypes>(OverrideTypes.SettlementDateThreshold)}: settlement date exceeds threshold.");
		//        }
		//        else
		//        {
		//            Program.PrintOverride(this, $"{Enum.GetName<OverrideTypes>(OverrideTypes.SettlementDateThreshold)}: issues with transaction ({this.TransactionDate:dd/MM/yyyy}) and settlement ({this.SettlementDate:dd/MM/yyyy}) dates ({(this.SettlementDate - this.TransactionDate).TotalDays} days)");
		//        }
		//    }
		//    if (this.Quantity <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid quantity.");
		//    if (this.UnitFMV == 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid FMV value.");
		//    if (Math.Round(this.UnitCost * this.Quantity, 2, MidpointRounding.AwayFromZero) != this.CheckCost) exceptions.Add($"{Program.GetValidationHeader(this)} cost x quantity does not equal total cost. {Math.Round(this.UnitCost * this.Quantity, 2, MidpointRounding.AwayFromZero)}");
		//    if (Math.Round(this.UnitFMV * this.Quantity, 2, MidpointRounding.AwayFromZero) != this.CheckFMV) exceptions.Add($"{Program.GetValidationHeader(this)} FMV x quantity does not equal total FMV. {Math.Round(this.UnitFMV * this.Quantity, 2, MidpointRounding.AwayFromZero)}");

		//    return exceptions;
		//}
	}
}
