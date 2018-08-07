using System;
using System.IO;

using Newtonsoft.Json;

namespace JABot.Models
{
    namespace Config
    {
        public sealed class Config
        {
            public class ConfigData
            {
                public string token { get; set; }
                public string prefix { get; set; }
            }
            private static Config instance = null;
            private static readonly object padlock = new object();
            private ConfigData configData;
            private bool parsed = false;

            Config()
            {
            }
            public bool reload()
            {
                parsed = false;
                return parse();
            }

            private bool parse()
            {
                var path = Path.Combine(Environment.CurrentDirectory + @"../config.json");
                if (!File.Exists(path))
                    return false;
                configData = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(path));
                parsed = true;
                return true;
            }
            public ConfigData data()
            {
                if (!parsed)
                    parse();

                return configData;
            }

            public static Config Instance
            {
                get
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Config();
                        }
                        return instance;
                    }
                }
            }
        }
    }
}
