using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace nova_study_api
{
    internal class Config
    {
        static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        internal Dictionary<string, string> AppValues { get; set; }

        internal Config()
        {
            string configStr = Encoding.UTF8.GetString(
                    File.ReadAllBytes(
                        Path.Combine(AssemblyDirectory, "..", "config.json")
                    )
                );

            
            this.AppValues = JsonConvert.DeserializeObject<Dictionary<string, string>>(configStr);
        }
    }

    

}