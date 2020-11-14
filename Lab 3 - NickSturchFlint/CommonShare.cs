using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3___NickSturchFlint
{
    class CommonShare: Share
    {
        //Data Members
        const double commonShareRate = 42.0;

        //Constructor
        public CommonShare(string name, string date, double shares) : base (name, date, shares)
        {
            this.buyerName = base.buyerName;
            this.buyDate = base.buyDate;
            this.shares = base.shares;
        }
        //Overridden Method
        public override double ShareValue()
        {
            double value = this.shares * commonShareRate;
            return value;
        }
    }
}
