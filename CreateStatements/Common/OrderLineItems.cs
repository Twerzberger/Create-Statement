using System;
namespace CreateStatements
{
    public class OrderLineItem
    {
        public string InvNumbr { get; set; }

        public string ItemSKU { get; set; }

        public string ItemDesc { get; set; }

        public string Quanity { get; set; }

        public string Price { get; set; }

        public string TaxItem { get; set; }

        public string UOM { get; set; }

        public double TaxAmount { get; set; }
        public DateTime ServiceDate { get; set; }
        public string Box { get; set; }
        public string Other1 { get; set; }
        public string Other2 { get; set; }
    }
}