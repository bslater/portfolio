using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public interface ITransactionType
    {
        Currency Currency { get; init; }
        decimal NetAmount { get; }
        string Owner { get; init; }
        string Reference { get; init; }
        DateOnly TransactionDate { get; init; }
    }
}
