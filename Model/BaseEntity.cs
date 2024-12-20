using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharePortfolio.Program;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public abstract class BaseEntity
        : IEntity
        , IComparable<BaseEntity>
        , IComparable
    {

       [JsonPropertyOrder(1)]
        public bool Entered { get; set; }

        [JsonIgnore]
        public string FiscalPeriod => $"P{this.TransactionDate.Month + (this.TransactionDate.Month > 6 ? -6 : 6):00}";

        [JsonIgnore]
        public string FiscalYear => $"FY{this.TransactionDate.Year + (this.TransactionDate.Month > 6 ? 1 : 0):0000}";

        [JsonIgnore]
        public string JsonFilename { get; set; }

       [JsonPropertyOrder(4)]
        public string Owner { get; init; }

       [JsonPropertyOrder(3)]
        public string Reference { get; init; }

       [JsonPropertyOrder(2)]
        public string Symbol { get; init; }

       [JsonPropertyOrder(10)]
        public DateOnly TransactionDate { get; init; }

       [JsonPropertyOrder ( 9999)]
        public List<OverrideTypes> Overrides { get; init; } = new List<OverrideTypes>();

        int IComparable.CompareTo(object? obj)
            => (obj is BaseEntity other) ? (this as IComparable<BaseEntity>).CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(BaseEntity)}", nameof(obj));

        int IComparable<BaseEntity>.CompareTo(BaseEntity? other)
            => (other == null) ? 1 : this.Reference.CompareTo(other.Reference);

        //public virtual List<string> Validate()
        //{
        //    List<string> exceptions = new List<string>();

        //    if (string.IsNullOrEmpty(this.Reference)) exceptions.Add($"{Program.GetValidationHeader(this)} missing reference.");
        //    if (string.IsNullOrEmpty(this.Symbol) || (this.Symbol != "n/a" & !StockSymbols.ContainsKey(this.Symbol))) exceptions.Add($"{Program.GetValidationHeader(this)} invalid symbol. {this.Symbol ?? "(null)"}");
        //    if (string.IsNullOrEmpty(this.Owner)) exceptions.Add($"{Program.GetValidationHeader(this)} missing owner.");
        //    if (this.TransactionDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid transaction date.");

        //    return exceptions;
        //}
    }
}
