using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio.Model
{
    public class InterestTransaction 
        : FinancialTransaction
    {
        //public override List<string> Validate()
        //{
        //    List<string> exceptions = base.Validate();

        //    if (this.Tax > 0)
        //    {
        //        decimal expected = this.TransactionDate>=Program.RepatriationDate ? 0.15m : 0.30m;
        //        //if (this.Tax != Math.Round(this.Amount * expected, 2, MidpointRounding.ToZero)) exceptions.Add($"{Program.GetValidationHeader(this)} invalid tax amount. {this.Tax}");
        //    }

        //    return exceptions;
        //}
    }
}
