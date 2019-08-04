using System;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Collections;

namespace CreateStatements
{
    public class Settings
    {
        //public static string APP_NAME = "BillSys;
        public static string QBPath { get; set; }
        public static string QBCountry { get; set; }
        public static short QBMajorVer { get; set; }
        public static short QBMinorVer { get; set; }
        
        public void CreateXML(Hashtable htAttributes, string root, string child, string file)
        {
            string path = Application.StartupPath + "\\" + file;
            int id = 1;
            XDocument xmlDoc = new XDocument();
            XElement rootElement, childElement, childAttribute;

            if (!File.Exists(path))
            {
                rootElement = new XElement(root);
                xmlDoc.Add(rootElement);
                xmlDoc.Save(path);

            }

            xmlDoc = XDocument.Load(path);
            var isIdElement = (from findID in xmlDoc.Descendants("Root")
                               select findID.Descendants("Id").Any()).Single();

            if (isIdElement)
            {
                var xmlMaxID = (from settings in xmlDoc.Descendants(child)
                                select
                                (
                                   int.Parse(settings.Element("Id").Value)
                                )).Max();

                id = Convert.ToInt32(xmlMaxID) + 1;
            }

            rootElement = new XElement(root);
            rootElement = xmlDoc.Element(root);
            childElement = new XElement(child);

            //Create Id Element
            childAttribute = new XElement("Id", id.ToString());
            childElement.Add(childAttribute);

            IDictionaryEnumerator enumerator = htAttributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                childAttribute = new XElement(enumerator.Key.ToString(), enumerator.Value.ToString());
                childElement.Add(childAttribute);
            }

            rootElement.Add(childElement);
            xmlDoc.Save(path);
        }

        public void ModifyXML(Hashtable htAttributes, string id, string child, string file)
        {
            string path = Application.StartupPath + "\\" + file;
            XDocument xmlDoc = new XDocument();
            xmlDoc = XDocument.Load(path);

            var varElements = xmlDoc.Root.Elements(child).Where(t => t.Element("Id").Value == Convert.ToString(id));
            foreach (var ele in varElements)
            {
                IDictionaryEnumerator enumerator = htAttributes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ele.Element(enumerator.Key.ToString()).Value = enumerator.Value.ToString();
                }
            }
            xmlDoc.Save(path);
        }

        public void DeleteXmlElement(string id, string child, string file)
        {
            string path = Application.StartupPath + "\\" + file;
            XDocument xmlDoc = new XDocument();
            xmlDoc = XDocument.Load(path);
            //Remove the element
            xmlDoc.Root.Elements(child).Where(t => t.Element("Id").Value == Convert.ToString(id)).Remove();
            xmlDoc.Save(path);
        }


        public DataTable GetXMLToDataTable(string file)
        {
            string path = Application.StartupPath + "\\" + file;
            DataTable dt = new DataTable();
            if (File.Exists(path))
            {

                DataSet ds = new DataSet();
                ds.ReadXml(path);
                if (ds.Tables.Count > 0)
                    dt = ds.Tables[0];
            }
            return dt;
        }


        public DataTable ReadSettings()
        {
            XDocument xmlDoc = new XDocument();
            DataTable dt = new DataTable();
            string path = Application.StartupPath + "\\Settings.xml";
            if (File.Exists(path))
            {

                xmlDoc = XDocument.Load(path);
                DataSet ds = new DataSet();
                ds.ReadXml(path);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    QBPath = dt.Rows[0]["QBPath"].ToString();
                    QBCountry = dt.Rows[0]["QBCountry"].ToString();
                    QBMajorVer = short.Parse(dt.Rows[0]["QBMajorVer"].ToString());
                    QBMinorVer = short.Parse(dt.Rows[0]["QBMinorVer"].ToString());
                }
            }
            else
            {
                MessageBox.Show("Please set the Settings","QB Settings Info");
            }
            return dt;
        }
     

        public Boolean CheckDuplication(string CompanyName, string Id)
        {
            XDocument xmlDoc = new XDocument();
            DataTable dt = new DataTable();
            string Country = string.Empty;
            string Rate = string.Empty;
            string path = Application.StartupPath + "\\QBSettings.xml";
            bool IsAvailable = false;
            if (File.Exists(path))
            {

                xmlDoc = XDocument.Load(path);
                DataSet ds = new DataSet();
                ds.ReadXml(path);
                //dt.ReadXml(path);
                if (ds.Tables.Count==0)
                    return IsAvailable;
                
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (CompanyName == dr["ConfigName"].ToString() & Id != dr["Id"].ToString())
                        IsAvailable = true;

                }
            }
            return IsAvailable;
            //else
            //{
            //    //ShowMessage("Please set the Settings", MsgBoxType.Error);
            //    MessageBox.Show("Please set the Settings", "QB Settings Info");
            //    return IsAvailable;
            //}

        }

    }
}
