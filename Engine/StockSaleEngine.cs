using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Engine
{
    public class StockSaleEngine
    {
        public static DTO.StockSaleTransaction ConvertTransaction(Model.StockSale transaction, IEnumerable<Model.StockLotBase> stockLots, IEnumerable<Model.FxRate> fxRates)
        {
            // retrieve the fx rate for the transaction
            var fxRate = fxRates.GetByDate(transaction.TransactionDate, true);

            return new DTO.StockSaleTransaction
            {
                Reference = transaction.Reference,
                Beneficiary = transaction.Owner,
                Quantity = transaction.Quantity,
                StrikePrice = transaction.StrikePrice,
                Symbol = transaction.Symbol + ".NASDAQ",
                Company = "Microsoft Corporation",

                TransactionDate = transaction.TransactionDate,
                SettlementDate = transaction.SettlementDate,
                GrossSaleProceeds = transaction.Amount,
                Fee = transaction.Fees,

                Lots = transaction.Lots.Select(l => StockLotEngine.ConvertTransaction(stockLots.Single(e => e.Reference == l.Reference), fxRates)).ToList(),

                FxRate = new DTO.FxRate
                {
                    Rate = fxRate.Rate,
                    Date = fxRate.Date,
                    Provider = fxRate.Provider
                }
            };
        }
    }
}