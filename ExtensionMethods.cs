using SharePortfolio.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SharePortfolio { 
    public static class ExtensionMethods
    {



        private static ConcurrentDictionary<int, StockSplitAdjuster> adjusters = new ConcurrentDictionary<int, StockSplitAdjuster>();

        public static decimal GetAdjustmentFactor(this IEnumerable<StockSplit> splits, DateOnly transactionDate)
            => splits.GetAdjustmentFactor(transactionDate, null);

        public static decimal GetAdjustmentFactor(this IEnumerable<StockSplit> splits, DateOnly transactionDate, DateOnly? referenceDate = null)
        {
            referenceDate ??= DateOnly.MaxValue;
            // Key for identifying adjustment factors for a specific security
            int key = splits.GetHashCode();

            // Retrieve or create adjuster for this security
            if (!adjusters.TryGetValue(key, out StockSplitAdjuster adjuster))
            {
                adjuster = new StockSplitAdjuster(splits);
                adjusters[key] = adjuster;
            }

            // Perform adjustment using the adjuster
            return adjuster.GetAdjustmentFactor(transactionDate, referenceDate.Value);
        }

        public static bool Equals(this decimal value, decimal? obj, decimal tolerance)
            => obj is decimal other && Math.Abs(value - other) <= tolerance;

        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts the DateOnly value to Unix time in milliseconds.
        /// </summary>
        public static long ToUnixTimeMilliseconds(this DateOnly date)
        {
            return (long)(date.ToDateTime(TimeOnly.MinValue) - epoch).TotalMilliseconds;
        }

        /// <summary>
        /// Converts the DateOnly value to Unix time in seconds.
        /// </summary>
        public static long ToUnixTimeSeconds(this DateOnly date)
        {
            return (long)(date.ToDateTime(TimeOnly.MinValue) - epoch).TotalSeconds;
        }

        /// <summary>
        /// Converts Unix time in milliseconds to DateTime.
        /// </summary>
        public static DateTime ToDateTimeFromUnixTimeMilliseconds(long unixTime)
        {
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Converts Unix time in seconds to DateTime.
        /// </summary>
        public static DateTime ToDateTimeFromUnixTimeSeconds(long unixTime)
        {
            return epoch.AddSeconds(unixTime);
        }

        public static FxRateDetail ToFxRateDetail(this FxRate fx)
            => new FxRateDetail { Rate = fx.Rate, Date = fx.Date, Provider = fx.Provider };

        public static T? GetByDate<T>(this IEnumerable<T> collection, DateOnly date, bool findClosest = false, SearchDirection direction = SearchDirection.NearestDate)
           where T : class, IHasDateProperty
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            T? result = collection.FirstOrDefault(item => item is not null && item.Date == date);
            if (result is not null || !findClosest)
            {
                // If an exact match is found or findClosest is false, return the result
                return result;
            }

            switch (direction)
            {
                case SearchDirection.NearestDate:
                    return collection.OrderBy(item => Math.Abs(item.Date.DayNumber - date.DayNumber)).FirstOrDefault();

                case SearchDirection.PastDate:
                    return collection.Where(item => item.Date <= date).OrderByDescending(item => item.Date).FirstOrDefault();

                case SearchDirection.FutureDate:
                    return collection.Where(item => item.Date >= date).OrderBy(item => item.Date).FirstOrDefault();

                default:
                    throw new ArgumentException("Invalid search direction", nameof(direction));
            }
        }

        public static T? GetByDate<T>(this IDictionary<DateOnly, T> dictionary, DateOnly date, bool findClosest = false, SearchDirection direction = SearchDirection.NearestDate)
        {
            if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

            if (dictionary.TryGetValue(date, out T value) || !findClosest)
            {
                // If an exact match is found or findClosest is false, return the result
                return value;
            }

            switch (direction)
            {
                case SearchDirection.NearestDate:
                    return dictionary[dictionary.Keys.OrderBy(key => Math.Abs(key.DayNumber - date.DayNumber)).First()];

                case SearchDirection.PastDate:
                    return dictionary[dictionary.Keys.Where(key => key <= date).Max()];

                case SearchDirection.FutureDate:
                    return dictionary[dictionary.Keys.Where(key => key <= date).Min()];

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), "Invalid search direction");
            }
        }
    }
}