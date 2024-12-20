
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharePortfolio.Model
{
    public class Transfer
           : IComparable<Transfer>
        , IComparable
 {

        [JsonPropertyOrder( 11)]
        public Entry Credit { get; init; }

        [JsonPropertyOrder( 10)]
        public Entry Debit { get; init; }

        [JsonPropertyOrder( 1)]
        public bool Entered { get; set; }


        [JsonPropertyOrder( 4)]
        public string Owner { get; init; }

        [JsonPropertyOrder(1000)]
        public List<Transaction> Transactions { get; init; } = new List<Transaction>();

        [JsonPropertyOrder(9999)]
        public List<OverrideTypes> Overrides { get; init; } = new List<OverrideTypes>();

        int IComparable.CompareTo(object? obj)
    => (obj is Transfer other) ? (this as IComparable<Transfer>).CompareTo(other) : throw new ArgumentException($"Object is not a {nameof(Transfer)}", nameof(obj));

        int IComparable<Transfer>.CompareTo(Transfer? other)
            => (other == null) ? 1 : this.Credit.Reference.CompareTo(other.Credit.Reference) == 0
                ? this.Debit.Reference.CompareTo(other.Debit.Reference)
                : this.Credit.Reference.CompareTo(other.Credit.Reference);

        //public List<string> Validate()
        //{
        //    //System.Diagnostics.//Debug.Assert(this.Credit.Reference != "C014208133772");
        //    //System.Diagnostics.//Debug.Assert(this.Debit.Reference != "C014208133772");
        //    //System.Diagnostics.//Debug.Assert(!(this.Debit.TransactionDate.Year == 2019 & this.Owner == "Ben"));
        //    List<string> exceptions = new List<string>();

        //    // map the transaction references to their corresponding transaction entities
        //    Transaction[] transactions = this.transactions.ToArray();
        //    this.transactions = new List<Transaction>();
        //    for (int i = 0; i < transactions.Length; i++)
        //    {
        //        var transaction = TransactionProceeds.ContainsKey(transactions[i].Reference)
        //            ? TransactionProceeds[transactions[i].Reference] as ITransactionType
        //            : null;
        //        if (transaction != null)
        //        {
        //            if (transactions[i].Amount == 0) exceptions.Add($"[{this.GetType().Name} #{this.Debit.Reference ?? "Missing"}/Transaction #{transactions[i].Reference ?? "Missing"}] invalid amount.");
        //            this.transactions.Add(new Transaction(transactions[i].Reference, transactions[i].Amount, transaction, this));
        //        }
        //        else
        //        {
        //            exceptions.Add($"{Program.GetValidationHeader(this.Debit)} Unknown transfer transaction: #{transactions[i].Reference}");
        //        }
        //    }
        //    if (this.Credit.Currency != this.Debit.Currency)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"{this.Credit.Reference}|{this.transactions.Count}");
        //        foreach (var transaction in this.transactions.OrderBy(t => t.TransactionDate))
        //        {
        //            var trans = TransactionProceeds[transaction.Reference] as ITransactionType;
        //            trans.Transfer(transaction.Amount);
        //            System.Diagnostics.Debug.WriteLine($"{trans.Reference}|{trans.NetAmount:#,###0.00##}|{trans.NetAmountTransfered:#,###0.00##}");
        //            if (trans.NetAmount - trans.NetAmountTransfered < 0) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} insufficient funds in transaction {transaction.Reference}. available:{trans.NetAmountTransfered + transaction.Amount} amount:{transaction.Amount}");
        //            if (trans.NetAmount - trans.NetAmountTransfered > 0 && trans.NetAmount - trans.NetAmountTransfered < 1) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} remaining funds in transaction {transaction.Reference}. available:{trans.NetAmount - trans.NetAmountTransfered}");
        //            //ITransactionType pTrans;
        //            //if ((pTrans = TransactionProceeds.Values.FirstOrDefault(t => t.Owner == transaction.Owner && t.TransactionDate < transaction.TransactionDate && t.NetAmount - t.NetAmountTransfered > 0)) != null)
        //            //{
        //            //    exceptions.Add($"[{this.GetType().Name} #{this.Debit.Reference ?? "Missing"}] transfer {transaction.Reference} should draw on earlier transaction amounts ({pTrans.Reference} has {pTrans.NetAmount - pTrans.NetAmountTransfered:#,###0.00##} outstanding)");
        //            //}
        //        }
        //    }

        //    if (this.Debit == null) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} invalid debit details.");
        //    if (this.Debit != null) this.Debit.Validate(exceptions, "Debit");
        //    if (this.Credit == null) exceptions.Add($"{Program.GetValidationHeader(this.Credit)} invalid credit details.");
        //    if (this.Credit != null) this.Debit.Validate(exceptions, "Credit");

        //    if (this.Credit.TransactionDate != this.Debit.TransactionDate && this.Credit.TransactionDate < this.Debit.TransactionDate) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} Credit date is before Debit date.");
        //    if (!this.Overrides.Contains(OverrideTypes.SettlementDateThreshold))
        //    {
        //        if (this.Credit.TransactionDate != this.Debit.TransactionDate && this.Credit.TransactionDate - this.Debit.TransactionDate > TimeSpan.FromDays(5)) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} {Enum.GetName<OverrideTypes>(OverrideTypes.SettlementDateThreshold)}: Credit date is misaligned with Debit date.");
        //    }
        //    else
        //    {
        //        Program.PrintOverride(this.Debit, $"{Enum.GetName<OverrideTypes>(OverrideTypes.SettlementDateThreshold)}: issues with transfer credit ({this.Credit.TransactionDate:dd/MM/yyyy}) and debit ({this.Debit.TransactionDate:dd/MM/yyyy}) dates ({(this.Credit.TransactionDate - this.Debit.TransactionDate).TotalDays} days)");
        //    }

        //    if (this.Credit.Symbol != this.Debit.Symbol)
        //    {
        //        if (string.IsNullOrEmpty(this.Debit.Reference)) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} missing reference.");
        //        if (string.IsNullOrEmpty(this.Credit.Reference)) exceptions.Add($"{Program.GetValidationHeader(this.Credit)} missing reference.");
        //    }
        //    if (this.transactions.Count == 0) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} no transactions specified.");

        //    var proceeds = this.transactions.Sum(l => l.Amount);
        //    if (proceeds - this.Debit.Fees != this.Debit.Amount) exceptions.Add($"{Program.GetValidationHeader(this.Debit)} Debit {this.Debit.Currency} amount does not equal transaction proceeds less fees: Proceeds={proceeds} | Delta:{this.Debit.Amount - proceeds - this.Debit.Fees}");
        //    if (this.Debit.Currency == this.Credit.Currency)
        //    {
        //        if (proceeds - this.Credit.Fees != this.Credit.Amount) exceptions.Add($"{Program.GetValidationHeader(this.Credit)} Credit {this.Credit.Currency} amount does not equal transaction proceeds less fees: Proceeds={proceeds} | Delta:{this.Credit.Amount - proceeds - this.Credit.Fees}");
        //    }
        //    else
        //    {
        //        if (Math.Round(this.Debit.Amount * this.Credit.Fx, 2, MidpointRounding.AwayFromZero) != this.Credit.Amount) exceptions.Add($"{Program.GetValidationHeader(this.Credit)} Credit {this.Credit.Currency} amount does not match Debit {this.Debit.Currency} using Fx {this.Credit.Fx:0.0000}: Calculated Amount={Math.Round(this.Debit.Amount * this.Credit.Fx, 2, MidpointRounding.AwayFromZero)}");
        //    }

        //    exceptions.AddRange(this.transactions.Where(t => t.Owner != this.Owner).Select(t => $"{Program.GetValidationHeader(this.Debit)} Owner of Transaction {t.Reference} is different to sale owner {this.Owner}"));

        //    return exceptions;
        //}

        public class Transaction
        {
           [JsonPropertyOrder(2)]
            public decimal Amount { get; init; }

           [JsonPropertyOrder(1)]
            public string Reference { get; init; }
        }
        public class Entry 
            : BaseEntity
        {

           [JsonPropertyOrder(201)]
            public decimal Amount { get; init; }

           [JsonPropertyOrder(200)]
            public Currency Currency { get; init; }

            [JsonPropertyOrder(202)]
            public decimal Fees { get; init; }

            [JsonPropertyOrder(203)]
            public decimal Fx { get; init; }

            //public void Validate(List<string> exceptions, string type)
            //{
            //    if (string.IsNullOrEmpty(this.Symbol)) exceptions.Add($"{Program.GetValidationHeader(this)} invalid symbol.");
            //    if (this.TransactionDate == DateTime.MinValue) exceptions.Add($"{Program.GetValidationHeader(this)} invalid transaction date.");
            //    if (this.Amount <= 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid amount.");
            //    if (this.Fx == 0) exceptions.Add($"{Program.GetValidationHeader(this)} invalid Fx. {this.Fx}");
            //}
        }
    }
}
