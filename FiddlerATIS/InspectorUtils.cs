using Fiddler;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Text;

namespace FiddlerATIS
{
    internal static class InspectorUtils
    {
        internal static void OpenTextEditor(string sFilename, string text)
        {
            try
            {
                string path = CONFIG.GetPath("TextEditor");
                InspectorUtils.WriteFile(sFilename, text);
                using (Process.Start(path, '"' + sFilename + '"'))
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + sFilename, "TextEditor Failed");
            }
        }

        internal static void OpenWith(string sFilename, string text)
        {
            try
            {
                InspectorUtils.WriteFile(sFilename, text);
                Utilities.DoOpenFileWith(sFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + sFilename, "Editor Failed");
            }
        }

        private static void WriteFile(string sFilename, string text)
        {
            FileStream fileStream = new FileStream(sFilename, FileMode.Create, FileAccess.Write);
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }
    }
}
