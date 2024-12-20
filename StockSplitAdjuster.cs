using SharePortfolio.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio
{
    public class StockSplitAdjuster
    {
        private Dictionary<DateOnly, decimal> adjustmentFactors;
        private Dictionary<(DateOnly splitDate, DateOnly effectiveDate), decimal> adjustmentMatrix;

	public StockSplitAdjuster(IEnumerable<StockSplit> splits)
	{
	    adjustmentFactors = splits.OrderBy(s => s.SplitDate)
	                              .ToDictionary(s => s.SplitDate, s => s.Ratio);
	    adjustmentMatrix = new Dictionary<(DateOnly, DateOnly), decimal>();
	
	    decimal cumulativeRatio = 1;
	    foreach (var split in splits.OrderBy(s => s.SplitDate))
	    {
	        cumulativeRatio *= split.Ratio;
	        adjustmentFactors[split.SplitDate] = cumulativeRatio;
	        foreach (var entry in adjustmentFactors.Keys)
	        {
	            adjustmentMatrix[(split.SplitDate, entry)] = adjustmentFactors[entry] / cumulativeRatio;
	            adjustmentMatrix[(entry, split.SplitDate)] = cumulativeRatio / adjustmentFactors[entry];
	        }
	    }
	}

        public decimal GetAdjustmentFactor(DateOnly transactionDate)
            => this.GetAdjustmentFactor(transactionDate, DateOnly.FromDateTime(DateTime.Today));

	public decimal GetAdjustmentFactor(DateOnly transactionDate, DateOnly referenceDate)
        {
            // Find the minimum split date less than or equal to the transaction date
            var minSplitDate = adjustmentFactors.Keys.LastOrDefault(d => d <= transactionDate);

            // Find the maximum split date less than or equal to the reference date
            var maxSplitDate = adjustmentFactors.Keys.LastOrDefault(d => d <= referenceDate);

            // If no split date found for either transaction or reference date, no adjustment needed
            if (minSplitDate == default(DateOnly) || maxSplitDate == default(DateOnly))
                return 1;

            // Retrieve adjustment factor from the matrix based on the calculated split dates
            decimal adjustmentFactor = adjustmentMatrix[(minSplitDate, maxSplitDate)];

            // Adjust value and return
            return adjustmentFactor;
        }
    }
}
