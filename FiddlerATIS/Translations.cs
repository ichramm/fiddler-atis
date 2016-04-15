using Fiddler;
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FiddlerATIS
{
    public class Translations
    {
        public static readonly Translations Instance = new Translations();

        private readonly Dictionary<string, string> translations_ = new Dictionary<string, string>();

        private Translations()
        {
            Init();
        }

        void Init()
        {
            var fileName = CONFIG.GetPath("Root") + "ATISTranslations.json";

            if (File.Exists(fileName))
            {
                try
                {
                    using (var file = File.OpenText(fileName))
                    {
                        using (var reader = new JsonTextReader(file))
                        {
                            var obj = (JObject)JToken.ReadFrom(reader);
                            foreach (var node in obj)
                            {
                                translations_.Add(node.Key, node.Value.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Failed to load JSON file:\r\n{0}\r\n\t{1}", fileName, ex.Message));
                }
            }
        }

        public string this [string key]
        {
            get
            {
                string result;
                return translations_.TryGetValue(key, out result) ? result : null;
            }
        }
    }
}
