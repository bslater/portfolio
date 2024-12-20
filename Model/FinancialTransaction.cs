
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public abstract class FinancialTransaction
        : BaseEntity
        , ITransactionType
    {

        [JsonPropertyOrder(10)]
        public DateOnly SettlementDate { get; init; }

        [JsonPropertyOrder(60)]
        public decimal Amount { get; init; }

        [JsonPropertyOrder( 62)]
        public Currency Currency { get; init; }

        [JsonIgnore]
        public decimal NetAmount => this.Amount - this.Tax;

       [JsonPropertyOrder(61)]
        public decimal Tax { get; init; }

        //public override List<string> Validate()
        //{
        //    List<string> exceptions = base.Validate();

        //    if (this.Fx == decimal.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} missing fx rate for {this.TransactionDate:dd/MM/yyyy}");
        //    if (this.Amount <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid amount. {this.Amount}");

        //    return exceptions;
        //}
    }
}
