using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public class FxRate
        : IComparable<FxRate>
        , IComparable
        , IHasDateProperty
    {
        public static DateOnly ReserveBankCutover = new DateOnly(2020, 1, 1);

        public decimal? ATO { get; init; }

        public DateOnly Date { get; init; }

        public decimal OANDA { get; init; }

        public decimal? OFX { get; init; }

        public decimal? ReserveBank { get; init; }

        public decimal Rate => this.Date < ReserveBankCutover
            ? this.ATO ?? this.OANDA
            : this.ReserveBank ?? this.OANDA;

		public string Provider  => this.Date<ReserveBankCutover
			? "ATO" ?? "Oanda"
            : "RBA" ?? "Oanda";

		int IComparable.CompareTo(object? obj)
        => (obj is FxRate other) ? (this as IComparable<FxRate>).CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(FxRate)}", nameof(obj));

        int IComparable<FxRate>.CompareTo(FxRate? other)
            => (other == null) ? 1 : this.Date.CompareTo(other.Date);
    }

}