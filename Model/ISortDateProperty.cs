using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public interface ISortDateProperty
    {
        public DateOnly Date{ get; }
    }
}
