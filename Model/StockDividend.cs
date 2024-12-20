using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public class StockDividend
    {
        public decimal Amount { get; init; }
        public DateOnly Announced { get; init; }
        public DateOnly ExDividend { get; init; }
        public DateOnly Payment { get; init; }
        public DateOnly Record { get; init; }
        public string Type { get; init; }
    }
}
