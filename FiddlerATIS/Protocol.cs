﻿using System;
using Fiddler;
using System.Net;
using System.Xml;
using System.Windows.Forms;
using System.Collections.Generic;
using Fiddler.WebFormats;
using System.Collections;
using System.Windows.Forms.VisualStyles;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FiddlerATIS
{
    public static class Protocol
    {
        static Dictionary<string, XmlDocument> xmlStorage = new Dictionary<string, XmlDocument>();

        private const string headerOpName = "C_ASSCC_V2";

        private static readonly string[] requiredHeaders = new string[]
        {
            "clienttype", 
            "operation", 
            "sequence", 
            "na_fis_code", 
            "fincode",
            "bufferlength", 
            "applicationname"
        };

        public static bool IsAtisSession(Session oS)
        {
            var requestHeaders = oS.RequestHeaders;

            foreach (string header in requiredHeaders)
            {
                if (!requestHeaders.Exists(header))
                {
                    return false;
                }
            }

            return true;
        }

        public static string SerializeRequest(Session session)
        {
            if (session.RequestBody == null || session.RequestBody.Length == 0)
            {
                return session.RequestHeaders.ToString(true, true, true);
            }

            string operationCode = session.RequestHeaders["na_fis_code"];
            return Serialize(operationCode, session, session.RequestBody);
        }

        public static string SerializeRespone(Session session)
        {
            if (session.ResponseBody == null || session.ResponseBody.Length == 0)
            {
                return session.ResponseHeaders.ToString(true, true);
            }
                
            string operationCode = session.RequestHeaders["na_fis_code"];
            return Serialize(operationCode, session, session.ResponseBody);
        }

        private static string DecodeNumber(string value)
        {
            return Regex.Replace(value, "[{A-I]$", delegate(Match match)
                {
                    switch (match.ToString())
                    {
                        case "{":
                            return "0";
                        case "A":
                            return "1";
                        case "B":
                            return "2";
                        case "C":
                            return "3";
                        case "D":
                            return "4";
                        case "E":
                            return "5";
                        case "F":
                            return "6";
                        case "G":
                            return "7";
                        case "H":
                            return "8";
                        case "I":
                            return "9";
                        default:
                            return match.ToString();
                    }
                });
        }

        private static bool TryExtractString(Array body, ref int offset, int length, out string result)
        {
            if (offset + length >= body.Length)
            {
                offset = body.Length;
                result = null;
                return false;
            }

            var val = new byte[length];
            Array.Copy(body, offset, val, 0, length);
            offset += length;

            result = Encoding.UTF8.GetString(val).Trim().Replace("\0", "");
            return true;
        }

        private static bool IsEmpty(IDictionary ht)
        {
            foreach (object value in ht.Values)
            {
                if ((value as Hashtable) != null)
                {
                    if ((value as Hashtable).Count > 0)
                    {
                        return false;
                    }
                }
                else if ((value as ArrayList) != null)
                {
                    if ((value as ArrayList).Count > 0)
                    {
                        return false;
                    }
                }
                else if (value is int)
                {
                    if ((int)value != 0)
                    {
                        return false;
                    }
                }
                else if (value is double)
                {
                    if ((double)value > 0.0)
                    {
                        return false;
                    }
                }
                else if (value.ToString() != "")
                {
                    return false;
                }
            }

            return true;
        }

        private static void ParseGroup(IDictionary target, XmlNode node, byte[] body, ref int offset)
        {
            foreach (XmlNode childNode in node.ChildNodes)
            {
                if (offset >= body.Length)
                {
                    break;
                }

                if (childNode.Name == "NA_CAMPO")
                {
                    object value = ParseValue(childNode, body, ref offset);
                    if (value != null)
                    {
                        target.Add(childNode.Attributes["Nombre"].Value, value);
                    }
                }
                else if (childNode.Name == "NA_GRUPO")
                {
                    if (childNode.Attributes["Occurs"] != null)
                    {
                        target.Add(childNode.Attributes["Nombre"].Value, ParseArray(childNode, body, ref offset));
                    }
                    else
                    {
                        target.Add(childNode.Attributes["Nombre"].Value, ParseGroup(childNode, body, ref offset));
                    }
                }
            }
        }

        private static Hashtable ParseGroup(XmlNode node, byte[] body, ref int offset)
        {
            var result = new Hashtable();
            ParseGroup(result, node, body, ref offset);
            return result;
        }

        private static ArrayList ParseArray(XmlNode node, byte[] body, ref int offset)
        {
            var result = new ArrayList();

            var occurs = Int32.Parse(node.Attributes["Occurs"].Value);

            for (int i = 0; i < occurs; ++i)
            {
                Hashtable res = ParseGroup(node, body, ref offset);
                if (res.Count > 0)
                {
                    result.Add(res);
                }
            }

            return result;
        }

        private static object ParseValue(XmlNode node, byte[] body, ref int offset)
        {
            if (node.Attributes["Redefine"] != null)
            {
                return null;
            }

            var length = Int32.Parse(node.Attributes["Longitud"].Value);

            string value;

            if (TryExtractString(body, ref offset, length, out value))
            {
                switch (node.Attributes["Tipo"].Value)
                {
                    case "4":
                        value = DecodeNumber(value);
                        try
                        {
                            return int.Parse(value);
                        }
                        catch (Exception ex)
                        {
                            return String.Format("'{0}' - {1}", value, ex.Message);
                        }
                    case "6":
                        try
                        {
                            double d = Double.Parse(DecodeNumber(value));
                            if (node.Attributes["Decimales"] != null)
                            {
                                var decimales = Int32.Parse(node.Attributes["Decimales"].Value);
                                if (decimales != 0)
                                {
                                    d = d / Math.Pow(10, decimales);
                                }
                            }
                            return d;
                        }
                        catch (Exception ex)
                        {
                            return String.Format("'{0}' - {1}", value, ex.Message);
                        }
                    case "PIC_9E":
                        try
                        {
                            double d = Double.Parse(value);
                            if (node.Attributes["Decimales"] != null)
                            {
                                var decimales = Int32.Parse(node.Attributes["Decimales"].Value);
                                if (decimales != 0)
                                {
                                    d = d / Math.Pow(10, decimales);
                                }
                            }
                            return d;
                        }
                        catch (Exception ex)
                        {
                            return String.Format("'{0}' - {1}", value, ex.Message);
                        }
                    default:
                        return value;
                }
            }

            return null;
        }

        private static void ParseOperation(IDictionary target, string operationCode, Session session, byte[] body, ref int offset)
        {
            XmlDocument doc;

            if (!xmlStorage.TryGetValue(operationCode, out doc))
            {
                string url = String.Format("http://{0}/ws/Mensajes/{1}.xml", session.host, operationCode);

                using (var webClient = new WebClient())
                {
                    webClient.Headers["X-Fiddler-Ignore"] = "true";
                    var xml = webClient.DownloadString(url);
                    doc = new XmlDocument();
                    doc.LoadXml(xml);
                    xmlStorage[operationCode] = doc;
                }
            }

            ParseGroup(target, doc.DocumentElement, body, ref offset);
        }

        private static Hashtable ParseOperation(string operationCode, Session session, byte[] body, ref int offset)
        {
            var result = new Hashtable();
            ParseOperation(result, operationCode, session, body, ref offset);
            return result;
        }

        private static string Serialize(string operationCode, Session session, byte[] body)
        {
            try
            {
                int offset = 0;
                Hashtable result = ParseOperation(headerOpName, session, body, ref offset);
                ParseOperation(result, operationCode, session, body, ref offset);

                return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}