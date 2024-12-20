using System;
using System.Collections.Generic;
using System.Text;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace SharePortfolio
{
    public interface ITransaction
    {

       [JsonPropertyOrder(2)]
        public string Ref { get; }

       [JsonPropertyOrder(1)]
        public bool Entered { get; set; }

       [JsonPropertyOrder(3)]
        public string Owner { get; }

       [JsonPropertyOrder(2)]
        public string Symbol { get; }

       [JsonPropertyOrder(10)]
        public DateOnly TransactionDate { get; }

       [JsonPropertyOrder(100)]
        public decimal TransactionAmount { get; }
    }
}