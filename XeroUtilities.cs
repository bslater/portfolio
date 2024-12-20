using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePortfolio
{
    public  static class XeroUtilities
    {
        public static void GenerateXeroFileHeader(StreamWriter writer)
        {
            writer.WriteLine("ContactName,InvoiceNumber,Reference,InvoiceDate,DueDate,Total,Description,Quantity,UnitAmount,LineAmount,AccountCode,TaxType,TrackingName1,TrackingOption1,Currency,Type,Sent,Status");
        }

    }
}
