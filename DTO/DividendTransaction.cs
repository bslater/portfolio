using SharePortfolio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.DTO
{
    public class DividendTransaction
	{
		public string Reference { get; init; }
		public string Beneficiary { get; init; }
		public DateOnly TransactionDate { get; init; }
		public string Company { get; init; }
		public string Symbol { get; init; }
		public decimal Shares { get; init; }
		public decimal Dividend { get; init; }

		public decimal Amount { get; init; }
        public decimal AmountLoc => Math.Round(this.Amount / this.FxRate, 2);
        public string AmountAccountCode => XeroAccountCodes.UnfrankedDividendForeign;

        public decimal Tax { get; init; }
        public decimal TaxLoc => Math.Round(this.Tax / this.FxRate, 2);
        public string TaxAccountCode => XeroAccountCodes.TaxWithheldForeign;

        public decimal NetAmount => this.Amount - this.Tax;
        public decimal NetAmountLoc => Math.Round(this.NetAmount / this.FxRate, 2);

        // difference between the net amount and the sum of the amount and tax
        public decimal Rounding => this.NetAmountLoc - (this.AmountLoc - this.TaxLoc);

		public decimal FxRate { get; init; }
		public string FxProvider { get; init; }
		public DateOnly FxDate { get; init; }
    }
}