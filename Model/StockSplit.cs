using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static SharePortfolio.Program;

namespace SharePortfolio.Model
{
    public class StockSplit
         : IComparable<StockSplit>
        , IComparable
        , IHasDateProperty
    {
        public decimal CloseAfter { get; init; }

        public decimal CloseBefore { get; init; }

        public DateOnly PayableDate { get; init; }

        public DateOnly AnnounceDate { get; init; }

        [JsonConverter(typeof(RatioStringJsonConverter))]
        public decimal Ratio { get; init; }

        public DateOnly RecordDate { get; init; }

        public DateOnly SplitDate { get; init; }

        DateOnly IHasDateProperty.Date => this.SplitDate;

        int IComparable.CompareTo(object? obj)
           => (obj is StockSplit other) ? (this as IComparable<StockSplit>).CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(StockSplit)}", nameof(obj));

        int IComparable<StockSplit>.CompareTo(StockSplit? other)
            => (other == null) ? 1 : this.SplitDate.CompareTo(other.SplitDate);
    }
}
