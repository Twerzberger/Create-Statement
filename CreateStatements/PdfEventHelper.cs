using System;
using System.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CreateStatements
{
    public class pdfPage : iTextSharp.text.pdf.PdfPageEventHelper
    {
        private string TempPAth = AppDomain.CurrentDomain.BaseDirectory + "Temp.XML";
        //I create a font object to use within my footer
        //protected Font footer
        //{
        //get
        //{
        //// create a basecolor to use for the footer font, if needed.
        //BaseColor grey = new BaseColor(128, 128, 128);
        //Font font = FontFactory.GetFont("Arial", 7, Font.ITALIC,BaseColor.BLACK);
        //return font;
        //}
        //}
        //override the OnStartPage event handler to add our header
        //public override void OnStartPage(PdfWriter writer, Document doc)
        //{
        ////I use a PdfPtable with 1 column to position my header where I want it
        //PdfPTable headerTbl = new PdfPTable(1);

        ////set the width of the table to be the same as the document
        //headerTbl.TotalWidth = doc.PageSize.Width;

        ////I use an image logo in the header so I need to get an instance of the image to be able to insert it. I believe this is something you couldn't do with older versions of iTextSharp
        //Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("/images/logo.jpg"));

        ////I used a large version of the logo to maintain the quality when the size was reduced. I guess you could reduce the size manually and use a smaller version, but I used iTextSharp to reduce the scale. As you can see, I reduced it down to 7% of original size.
        //logo.ScalePercent(7);

        ////create instance of a table cell to contain the logo
        //PdfPCell cell = new PdfPCell(logo);

        ////align the logo to the right of the cell
        //cell.HorizontalAlignment = Element.ALIGN_RIGHT;

        ////add a bit of padding to bring it away from the right edge
        //cell.PaddingRight = 20;

        ////remove the border
        //cell.border = 0;

        ////Add the cell to the table
        //headerTbl.AddCell(cell);

        ////write the rows out to the PDF output stream. I use the height of the document to position the table. Positioning seems quite strange in iTextSharp and caused me the biggest headache.. It almost seems like it starts from the bottom of the page and works up to the top, so you may ned to play around with this.
        //headerTbl.WriteSelectedRows(0,-1, 0, (doc.PageSize.Height-10), writer.DirectContent);
        //}

        //override the OnPageEnd event handler to add our footer
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            Font Verdana = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL);
            Font Verdanasmal = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL);

            DataTable dtFtrVal = new DataTable();
            PdfPTable FootTable = new PdfPTable(7);

            FootTable = new PdfPTable(6);
            FootTable.TotalWidth = 550f;

            PdfPCell FootrHd;
            Phrase nwPhrase;
            // FootTable.DefaultCell.Border = Rectangle.NO_BORDER;

            FootTable.HorizontalAlignment = Element.ALIGN_CENTER;
            FootTable.HorizontalAlignment = Element.ALIGN_MIDDLE;
            //  FootTable.DefaultCell.MinimumHeight = 20f;
            //FootTable.AddCell(new Phrase(" "));

            nwPhrase = new Phrase("Current", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("1-30 DAYS PAST DUE", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("31-60 DAYS PAST DUE", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("61-90 DAYS PAST DUE", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("OVER 90 DAYS PAST DUE", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("Amount Due", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(AgingDetail.Current, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;

            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(AgingDetail.tilThirty, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(AgingDetail.tillSixty, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(AgingDetail.tillNinty, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(AgingDetail.aboveNinty, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("$" + AgingDetail.TotalBal, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            if (AgingDetail.PageNumber)
            {
                nwPhrase = new Phrase("Page " + doc.PageNumber, Verdanasmal);
                FootrHd = new PdfPCell(nwPhrase);
                FootrHd.MinimumHeight = 30f;
                FootrHd.Colspan = 6;
                FootrHd.Border = Rectangle.NO_BORDER;
                FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                FootTable.AddCell(FootrHd);
            }
            // FootTable.DefaultCell.PaddingLeft = 40f;
            FootTable.WriteSelectedRows(0, -1, doc.LeftMargin - 13, (doc.BottomMargin + 44), writer.DirectContent);

        }

    }

    public class pdfPageInv : iTextSharp.text.pdf.PdfPageEventHelper
    {
        private string TempPAth = AppDomain.CurrentDomain.BaseDirectory + "Temp.XML";
        //I create a font object to use within my footer
        //protected Font footer
        //{
        //get
        //{
        //// create a basecolor to use for the footer font, if needed.
        //BaseColor grey = new BaseColor(128, 128, 128);
        //Font font = FontFactory.GetFont("Arial", 7, Font.ITALIC,BaseColor.BLACK);
        //return font;
        //}
        //}
        //override the OnStartPage event handler to add our header
        //public override void OnStartPage(PdfWriter writer, Document doc)
        //{
        ////I use a PdfPtable with 1 column to position my header where I want it
        //PdfPTable headerTbl = new PdfPTable(1);

        ////set the width of the table to be the same as the document
        //headerTbl.TotalWidth = doc.PageSize.Width;

        ////I use an image logo in the header so I need to get an instance of the image to be able to insert it. I believe this is something you couldn't do with older versions of iTextSharp
        //Image logo = Image.GetInstance(HttpContext.Current.Server.MapPath("/images/logo.jpg"));

        ////I used a large version of the logo to maintain the quality when the size was reduced. I guess you could reduce the size manually and use a smaller version, but I used iTextSharp to reduce the scale. As you can see, I reduced it down to 7% of original size.
        //logo.ScalePercent(7);

        ////create instance of a table cell to contain the logo
        //PdfPCell cell = new PdfPCell(logo);

        ////align the logo to the right of the cell
        //cell.HorizontalAlignment = Element.ALIGN_RIGHT;

        ////add a bit of padding to bring it away from the right edge
        //cell.PaddingRight = 20;

        ////remove the border
        //cell.border = 0;

        ////Add the cell to the table
        //headerTbl.AddCell(cell);

        ////write the rows out to the PDF output stream. I use the height of the document to position the table. Positioning seems quite strange in iTextSharp and caused me the biggest headache.. It almost seems like it starts from the bottom of the page and works up to the top, so you may ned to play around with this.
        //headerTbl.WriteSelectedRows(0,-1, 0, (doc.PageSize.Height-10), writer.DirectContent);
        //}

        //override the OnPageEnd event handler to add our footer
        public override void OnEndPage(PdfWriter writer, Document doc)
        {
            Font Verdana = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL);
            Font Verdanasmal = FontFactory.GetFont("Verdana", 9, iTextSharp.text.Font.NORMAL);

            DataTable dtFtrVal = new DataTable();
            PdfPTable FootTable = new PdfPTable(7);

            FootTable = new PdfPTable(6);
            FootTable.TotalWidth = 550f;

            PdfPCell FootrHd;
            Phrase nwPhrase;
            // FootTable.DefaultCell.Border = Rectangle.NO_BORDER;

            FootTable.HorizontalAlignment = Element.ALIGN_CENTER;
            FootTable.HorizontalAlignment = Element.ALIGN_MIDDLE;
            //  FootTable.DefaultCell.MinimumHeight = 20f;
            //FootTable.AddCell(new Phrase(" "));

            //nwPhrase = new Phrase(" ", Verdana);
            //FootrHd = new PdfPCell(nwPhrase);
            //FootrHd.Border = Rectangle.NO_BORDER;
            //FootrHd.MinimumHeight = 20f;
            //FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            //FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            //FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(" ", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(" ", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(" ", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase(" ", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("Total", Verdana);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("" , Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("", Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("", Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("", Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("", Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.Border = Rectangle.NO_BORDER;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            nwPhrase = new Phrase("$" + AgingDetail.TotalBal, Verdanasmal);
            FootrHd = new PdfPCell(nwPhrase);
            FootrHd.MinimumHeight = 20f;
            FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
            FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
            FootTable.AddCell(FootrHd);

            if (AgingDetail.PageNumber)
            {
                nwPhrase = new Phrase("Page " + doc.PageNumber, Verdanasmal);
                FootrHd = new PdfPCell(nwPhrase);
                FootrHd.MinimumHeight = 30f;
                FootrHd.Colspan = 6;
                FootrHd.Border = Rectangle.NO_BORDER;
                FootrHd.HorizontalAlignment = Rectangle.ALIGN_CENTER;
                FootrHd.VerticalAlignment = Rectangle.ALIGN_MIDDLE;
                FootTable.AddCell(FootrHd);
            }
            // FootTable.DefaultCell.PaddingLeft = 40f;
            FootTable.WriteSelectedRows(0, -1, doc.LeftMargin - 13, (doc.BottomMargin + 44), writer.DirectContent);

        }

    }
}