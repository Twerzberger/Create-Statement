using System;
using System.Collections.Generic;

namespace CreateStatements
{
    public class InvDetails
    {
        public List<OrderLineItem> oLstOrderLineItem { get; set; }

        public List<CustAddress> oCustAddress { get; set; }

        public string poNum { get; set; }

        public double taxAmount { get; set; }

        public string InvNum { get; set; }

        public string CustomerName { get; set; }

        public DateTime InvDate { get; set; }

        public double InvAmount { get; set; }

        public string Tax { get; set; }

        public double TotalTax { get; set; }

        public string Terms { get; set; }

        public string GUID { get; set; }
        public string SONum { get; set; }
        public DateTime DueDate{get;set;}
        public string Rep { get; set; }
        public string ShippingMethod { get; set; }
        public string FOB { get; set; }
        public string Other { get; set; }
        public DateTime ShipDate { get; set; }
     
        public bool HasTax
        {
            get
            {
                if (this.taxAmount > 0)
                    return true;
                else
                    return false;
            }
        }
    }
}