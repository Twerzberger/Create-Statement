using System;

namespace CreateStatements
{
    public class InvHeader
    {
        public string InvNum { get; set; }

        public string CustomerName { get; set; }

        public DateTime InvDate { get; set; }

        public double _invAmount;

        public double _tax;

        public double InvAmount
        {
            get
            {
                return this._invAmount + this._tax;
            }
        }

        public string GUID { get; set; }
    }
}