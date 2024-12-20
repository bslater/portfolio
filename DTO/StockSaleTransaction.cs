using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.DTO
{
	public class FxRate
	{
        public string Provider { get; init; }
        public DateOnly Date { get; init; }
        public decimal Rate { get; init; }
    }

    public class StockSaleLot
	{
		public string Reference { get; init; }
		public DateOnly TransactionDate { get; init; }
		public decimal Quantity { get; init; }

        public FxRate FxRate { get; init; }


        // foreign currency properties
        public decimal StrikePrice { get; init; }
        public decimal MarketPrice { get; init; }
        public decimal ImmediateGain => this.MarketPrice - this.StrikePrice;
        public decimal TotalImmediateGain => Math.Round(this.ImmediateGain * this.Quantity, 2);

        // local currency properties
        public decimal CostBase => Math.Round(this.TotalCostBase / this.Quantity, 4);
        public decimal TotalCostBase { get; init; }
        public decimal EssAmount { get; init; }

    }

    public class StockSaleTransaction
	{

		public string Reference { get; init; }
		public string Beneficiary { get; init; }
		public DateOnly TransactionDate { get; init; }
		public DateOnly SettlementDate { get; init; }
		public string Company { get; init; }
		public string Symbol { get; init; }
		public decimal Quantity { get; init; }
		public decimal StrikePrice { get; init; }

		public List<StockSaleLot> Lots { get; init; }= new List<StockSaleLot>();

        public decimal GrossSaleProceeds { get; init; }
        public decimal GrossSaleProceedsAUD => Math.Round(this.GrossSaleProceeds / this.FxRate.Rate, 2);

        public decimal Fee { get; init; }
        public decimal FeeAUD => Math.Round(this.Fee / this.FxRate.Rate, 2);

        public decimal CostBase => this.Lots.Sum(l => l.TotalCostBase) + this.FeeAUD;
        public decimal NetSaleProceeds => this.GrossSaleProceeds - this.Fee;
        public decimal NetSaleProceedsAUD => Math.Round(this.NetSaleProceeds / this.FxRate.Rate, 2);

        public decimal NetSaleGain => this.NetSaleProceedsAUD - this.CostBase + this.FeeAUD;

        // difference between the net amount and the sum of the amount and tax
        public decimal Rounding => 0;


		public DTO.FxRate FxRate { get; init; }
    }
}