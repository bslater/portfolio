using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SharePortfolio.Program;

namespace SharePortfolio.Model
{
    public interface IEntity
    {
        public bool Entered { get; set; }
        public string Owner { get; init; }
        public string Reference { get; init; }
        public string Symbol { get; init; }
        public DateOnly TransactionDate { get; init; }
    }

}