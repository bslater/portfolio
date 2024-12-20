using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Engine
{
    public class InterestEngine
    {
        public static DTO.InterestTransaction ConvertTransaction(Model.InterestTransaction transaction, IEnumerable<Model.FxRate> fxRates)
        {
            // retrieve the fx rate for the transaction
            var fxRate = fxRates.GetByDate(transaction.TransactionDate, true);

            return new DTO.InterestTransaction
            {
                Reference = transaction.Reference,
                Beneficiary = transaction.Owner,

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