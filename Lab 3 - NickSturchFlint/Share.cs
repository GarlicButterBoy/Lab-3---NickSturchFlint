using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3___NickSturchFlint
{
    class Share
    {
        //Data Members
        protected string buyerName;
        protected string buyDate;
        protected double shares;
        //Getter and Setter for Name
        public string Name
        {
            get { return this.buyerName; }
            set { this.buyerName = value; }
        }
        //Getter and Setter for Date
        public string Date
        {
            get { return this.buyDate; }
            set { this.buyDate = value; }
        }
        //Getter and Setter for Shares
        public double Shares
        {
            get { return this.shares; }
            set { this.shares = value; }
        }

        //Constructor
        public Share(string name, string date, double shares)
        {
            this.buyerName = name;
            this.buyDate = date;
            this.shares = shares;
        }
        //Virtual Method
        public virtual double ShareValue()
        {
            double value = this.shares * 42;
            return value;
        }
    }
}
