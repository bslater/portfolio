using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Engine
{
	public class StockLotEngine
	{
		public static DTO.StockSaleLot ConvertTransaction(Model.StockLotBase transaction, IEnumerable<Model.FxRate> fxRates)
		{
			// retrieve the fx rate for the transaction
			var fxRate = fxRates.GetByDate(transaction.TransactionDate, true);

			return new DTO.StockSaleLot
			{
				Reference = transaction.Reference,
				Quantity = transaction.Quantity,
				EssAmount = transaction.ReportedEssAmount,
				FxRate = new DTO.FxRate
				{
					Rate = fxRate.Rate,
					Date = fxRate.Date,
					Provider = fxRate.Provider
				},
				StrikePrice = transaction.UnitCost,
				MarketPrice = transaction.UnitFMV,
				TransactionDate = transaction.TransactionDate,
				TotalCostBase = transaction.ReportedEssAmount +
					((transaction is Model.EsppLot esppLot) ? esppLot.ESPPAmount - esppLot.Refunded : 0),
			};
		}
	}
}