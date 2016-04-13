using Fiddler;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FiddlerATIS
{
    internal class CommonMethods
    {
        //
        // Static Fields
        //
        private const int WM_VSCROLL = 277;

        private const int EM_GETLINECOUNT = 186;

        //
        // Static Methods
        //
        internal static void DoRichTextSearch(RichTextBox rtbSearch, string sSearchFor, bool bHighlightAll)
        {
            if (rtbSearch.TextLength < 1)
            {
                return;
            }
            if (sSearchFor.OICStartsWith("regex:"))
            {
                FiddlerApplication.DoNotifyUser("This Inspector cannot perform regular expression searches.", "RegEx Unsupported");
            }
            if (bHighlightAll)
            {
                try
                {
                    CommonMethods.LockWindowUpdate(rtbSearch.Handle);
                    rtbSearch.SelectionStart = 0;
                    rtbSearch.SelectionLength = rtbSearch.TextLength;
                    rtbSearch.SelectionBackColor = rtbSearch.BackColor;
                    rtbSearch.SelectionLength = 0;
                    if (!string.IsNullOrEmpty(sSearchFor))
                    {
                        int i = rtbSearch.Find(sSearchFor, 0, RichTextBoxFinds.None);
                        int num = i;
                        while (i > -1)
                        {
                            rtbSearch.SelectionBackColor = Color.Yellow;
                            i++;
                            if (i >= rtbSearch.TextLength)
                            {
                                break;
                            }
                            i = rtbSearch.Find(sSearchFor, i, RichTextBoxFinds.None);
                        }
                        if (num > -1)
                        {
                            rtbSearch.SelectionLength = 0;
                            rtbSearch.SelectionStart = num;
                        }
                    }
                    return;
                }
                finally
                {
                    CommonMethods.LockWindowUpdate(IntPtr.Zero);
                }
            }
            int start = Math.Min(rtbSearch.SelectionStart + 1, rtbSearch.TextLength);
            rtbSearch.Find(sSearchFor, start, RichTextBoxFinds.None);
        }

        internal static int GetLineCountForTextBox(TextBoxBase txtBox)
        {
            return (int)CommonMethods.SendMessage(txtBox.Handle, 186, 0, 0);
        }

        internal static string GetTreeNodeFullText(TreeNode oTN)
        {
            if (oTN.Tag == null)
            {
                return oTN.Text;
            }
            return oTN.Tag as string;
        }

        [DllImport("user32.dll")]
        private static extern bool LockWindowUpdate(IntPtr hWndLock);

        internal static void ScrollDown(RichTextBox rtb)
        {
            CommonMethods.SendMessage(rtb.Handle, 277, 1, 0);
        }

        internal static void ScrollUp(RichTextBox rtb)
        {
            CommonMethods.SendMessage(rtb.Handle, 277, 0, 0);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        internal static void SetTreeNodeLimitedText(TreeNode oTN, string sText)
        {
            string text = sText;
            if (text.Length > 259)
            {
                text = Utilities.TrimTo(text, 258) + '…';
            }
            oTN.Text = text;
            if (text != sText)
            {
                oTN.Tag = sText;
            }
        }
    }
}
