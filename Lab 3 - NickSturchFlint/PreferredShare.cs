using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3___NickSturchFlint
{
    class PreferredShare: Share
    {
        //Data Members
        const double preferredShareRate = 100.0;

        //Constructor
        public PreferredShare(string name, string date, double shares) : base(name, date, shares)
        {
            this.buyerName = base.buyerName;
            this.buyDate = base.buyDate;
            this.shares = base.shares;
        }
        //Overridden Method
        public override double ShareValue()
        {
            double value = this.shares * preferredShareRate;
            return value;
        }

        public override string ToString()
        {
            string output;
            output = "Buyer Name    : " + buyerName + "\n";
            output += "Bought Date   : " + buyDate + "\n";
            output += "Share's Bought: " + shares + "\n";
            output += "Total Value   : " + ShareValue();
            return output;
        }
    }
}
