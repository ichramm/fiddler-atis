using Fiddler;
using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FiddlerATIS
{
    public class XmlStore
    {
        public static readonly XmlStore Instance = new XmlStore();

        private readonly string cacheDirectory_;
        private readonly Dictionary<string, XmlDocument> storage_;

        private XmlStore()
        {
            storage_ = new Dictionary<string, XmlDocument>();
            cacheDirectory_ = CONFIG.GetPath("Root") + "ATISCache" + Path.DirectorySeparatorChar;
            LoadCache();
        }

        private void LoadCache()
        {
            var regex = new Regex(@"\\([^.\\]+).xml$");

            Directory.CreateDirectory(cacheDirectory_);

            foreach (string fileName in Directory.GetFiles(cacheDirectory_, "*.xml"))
            {
                var doc = new XmlDocument();
                doc.Load(fileName);

                var operationCode = regex.Match(fileName).Groups[1].Captures[0].Value;
                storage_.Add(operationCode, doc);
            }
        }

        private static XmlDocument DownloadXml(Session session, string operationCode)
        {
            string url = String.Format("http://{0}/ws/Mensajes/{1}.xml", session.host, operationCode);

            using (var webClient = new WebClient())
            {
                webClient.Headers["X-Fiddler-Ignore"] = "true";
                var xml = webClient.DownloadString(url);
                var doc = new XmlDocument();
                doc.LoadXml(xml);
                return doc;
            }
        }

        public bool TryGetXml(Session session, out XmlDocument doc)
        {
            return TryGetXml(session.RequestHeaders["na_fis_code"], out doc);
        }

        public bool TryGetXml(string operationCode, out XmlDocument doc)
        {
            return storage_.TryGetValue(operationCode, out doc);
        }

        public XmlDocument GetXml(Session session)
        {
            return GetXml(session, session.RequestHeaders["na_fis_code"]);
        }

        public XmlDocument GetXml(Session session, string operationCode)
        {
            XmlDocument doc;

            if (!TryGetXml(operationCode, out doc))
            {
                doc = DownloadXml(session, operationCode);
                storage_[operationCode] = doc;
                doc.Save(Path.Combine(cacheDirectory_, String.Format("{0}.xml", operationCode)));
            }

            return doc;
        }
    }
}
