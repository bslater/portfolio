using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public class StockQuote
        : IComparable<StockQuote>
        , IComparable
        , IHasDateProperty
    {
        public decimal Close { get; init; }
        public DateOnly Date { get; init; }
        public decimal High { get; init; }
        public decimal Low { get; init; }
        public decimal Open { get; init; }

        public override string ToString()
            => $"{this.Date:dd/MM/yyyy} Open:{this.Open:#,##0.00##} Close:{this.Close:#,##0.00##}";
        int IComparable.CompareTo(object? obj)
       => (obj is StockQuote other) ? (this as IComparable<StockQuote>).CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(StockQuote)}", nameof(obj));

        int IComparable<StockQuote>.CompareTo(StockQuote? other)
            => (other == null) ? 1 : this.Date.CompareTo(other.Date);
    }
}