using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public struct EsppPeriod
    {
        public DateOnly PeriodEnd { get; init; }
        public decimal PeriodEndPrice { get; init; }
        public decimal PeriodPurchasePrice { get; init; }
        public DateOnly PeriodStart { get; init; }
        public decimal PeriodStartPrice { get; init; }
    }
}
