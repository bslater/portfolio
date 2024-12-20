using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Engine
{
    public class DividendEngine
    {
        public static DTO.DividendTransaction ConvertTransaction(Model.DividendTransaction transaction, IEnumerable<Model.FxRate> fxRates)
        {
            // retrieve the fx rate for the transaction
            var fxRate = fxRates.GetByDate(transaction.TransactionDate, true);

            return new DTO.DividendTransaction
            {
                Reference = transaction.Reference,
                Beneficiary = transaction.Owner,
                Shares = transaction.Shares,
                Dividend = transaction.Dividend,
                Symbol = transaction.Symbol + ".NASDAQ",
                Company = "Microsoft Corporation",

                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                Tax = transaction.Tax,

                FxRate = fxRate.Rate,
                FxDate = fxRate.Date,
                FxProvider = fxRate.Provider
            };
        }
    }
}