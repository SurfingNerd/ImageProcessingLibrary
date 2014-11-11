using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessingLibrary.Metadata
{
    public class AppConfigFascade
    {
        private static AppConfigFascade s_instance;

        public static AppConfigFascade Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new AppConfigFascade();
                    s_instance.ReadFromAppConfig();
                }
                return s_instance;
            }
        }

        private AppConfigFascade()
        {
 
        }

        static readonly string DLLDIRECTORIESID = "DllDirectories";
        static readonly string DLLSID = "Dlls";

        public List<string> DllDirectories { get; private set; }
        public List<string> Dlls { get; private set; }

        public void ReadFromAppConfig()
        {
            DllDirectories = ReadFromConfig(DLLDIRECTORIESID);
            Dlls = ReadFromConfig(DLLSID);
        }

        private List<string> ReadFromConfig(string appConfigID)
        {
            List<string> result = new List<string>();
            if (ConfigurationManager.AppSettings.AllKeys.Contains(appConfigID))
            {
                string allValues = ConfigurationManager.AppSettings[appConfigID];
                result.AddRange(allValues.Split(';'));
            }
            return result;
        }

        public void WriteToAppConfig()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
            foreach(string key in ConfigurationManager.AppSettings.AllKeys)
            {
                configuration.AppSettings.Settings[key].Value = ConfigurationManager.AppSettings[key];
            }

            configuration.AppSettings.Settings[DLLDIRECTORIESID].Value = string.Join(";", DllDirectories);
            configuration.AppSettings.Settings[DLLSID].Value = string.Join(";", Dlls);

            configuration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
