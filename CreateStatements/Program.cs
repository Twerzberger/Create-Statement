using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Collections;
using System.IO;

namespace CreateStatements
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
        

            Settings objSettings = new Settings();
            if (!File.Exists(Application.StartupPath + "\\Settings.xml"))
            {
                Hashtable htSettings = new Hashtable();
                htSettings.Add("QBPath", "");
                htSettings.Add("QBCountry", "US");
                htSettings.Add("QBMajorVer", "12");
                htSettings.Add("QBMinorVer", "0");
                htSettings.Add("CheckStatus", "0");
                objSettings.CreateXML(htSettings, "Root", "Settings", "Settings.xml");
                objSettings.ReadSettings();
            }
            else
                objSettings.ReadSettings();
            Application.Run(new CreateStatements());
        }
    }
}
