using Fiddler;
using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FiddlerATIS
{
    internal class RichTextBox5 : RichTextBox
    {
        //
        // Properties
        //
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                if (RichTextBox5.LoadLibrary("msftedit.dll") != IntPtr.Zero)
                {
                    createParams.ClassName = "RICHEDIT50W";
                }
                return createParams;
            }
        }

        //
        // Static Methods
        //
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);
    }

    public class ATISView : UserControl
    {
        //
        // Fields
        //
        internal RichTextBox5 txtRaw;

        private ToolStripSeparator toolStripMenuItem1;

        private ToolStripMenuItem miContextWordWrap;

        public ToolStripMenuItem miContextShowHeader;

        private IContainer components;

        private ToolStripMenuItem miTextWizard;

        private ToolStripMenuItem miOpenXml;

        private string sPrefKey;

        private ToolStripMenuItem miContextPaste;

        private ATISInspectorBase m_owner;

        internal Button btnSpawnTextEditor;

        private TextBox txtSearchFor;

        private Panel pnlActions;

        private ContextMenuStrip mnuContextStrip;

        private ToolStripMenuItem miContextCut;

        private ToolStripMenuItem miContextCopy;

        //
        // Constructors
        //
        public ATISView(ATISInspectorBase oOwner)
        {
            this.InitializeComponent();
            this.txtRaw.Font = new Font("Lucida Console", CONFIG.flFontSize);
            this.m_owner = oOwner;
            this.sPrefKey = ((this.m_owner is IRequestInspector2) ? "fiddler.inspectors.request.atis." : "fiddler.inspectors.response.atis.");
            this.SetWordWrapState(FiddlerApplication.Prefs.GetBoolPref(this.sPrefKey + "wordwrap", false));
            this.txtRaw.BackColor = CONFIG.colorDisabledEdit;
            Utilities.SetCueText(this.txtSearchFor, " Find... (press Ctrl+Enter to highlight all)");
        }

        //
        // Methods
        //
        private void btnSpawnTextEditor_Click(object sender, EventArgs e)
        {
            this.m_owner.SpawnTextEditor();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void DoPlainTextPaste()
        {
            if (this.txtRaw.ReadOnly)
            {
                return;
            }
            this.txtRaw.Paste(DataFormats.GetFormat(DataFormats.Text));
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            this.txtRaw = new RichTextBox5();
            this.mnuContextStrip = new ContextMenuStrip(this.components);
            this.miContextCopy = new ToolStripMenuItem();
            this.miContextCut = new ToolStripMenuItem();
            this.miContextPaste = new ToolStripMenuItem();
            this.miTextWizard = new ToolStripMenuItem();
            this.miOpenXml = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripSeparator();
            this.miContextShowHeader = new ToolStripMenuItem();
            this.miContextWordWrap = new ToolStripMenuItem();
            this.pnlActions = new Panel();
            this.txtSearchFor = new TextBox();
            this.btnSpawnTextEditor = new Button();
            this.mnuContextStrip.SuspendLayout();
            this.pnlActions.SuspendLayout();
            base.SuspendLayout();
            this.txtRaw.AcceptsTab = true;
            this.txtRaw.AllowDrop = true;
            this.txtRaw.BackColor = Color.LightGray;
            this.txtRaw.ContextMenuStrip = this.mnuContextStrip;
            this.txtRaw.Dock = DockStyle.Fill;
            this.txtRaw.Font = new Font("Lucida Console", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtRaw.HideSelection = false;
            this.txtRaw.Location = new Point(0, 0);
            this.txtRaw.MaxLength = 100000000;
            this.txtRaw.Name = "txtRaw";
            this.txtRaw.ReadOnly = true;
            this.txtRaw.Size = new Size(380, 206);
            this.txtRaw.TabIndex = 0;
            this.txtRaw.Text = "";
            this.txtRaw.WordWrap = false;
            this.txtRaw.DragDrop += new DragEventHandler(this.txtRaw_DragDrop);
            this.txtRaw.DragEnter += new DragEventHandler(this.txtRaw_DragEnter);
            this.txtRaw.DragLeave += new EventHandler(this.txtRaw_DragLeave);
            this.txtRaw.DragOver += new DragEventHandler(this.txtRaw_DragOver);
            this.txtRaw.LinkClicked += new LinkClickedEventHandler(this.txtRaw_LinkClicked);
            this.txtRaw.ReadOnlyChanged += new EventHandler(this.txtRaw_ReadOnlyChanged);
            this.txtRaw.KeyDown += new KeyEventHandler(this.txtRaw_KeyDown);
            this.mnuContextStrip.Items.AddRange(new ToolStripItem[]
                {
                    this.miContextCopy,
                    this.miContextCut,
                    this.miContextPaste,
                    this.miTextWizard,
                    this.miOpenXml,
                    this.toolStripMenuItem1,
                    this.miContextShowHeader,
                    this.miContextWordWrap
                });
            this.mnuContextStrip.Name = "mnuContextStrip";
            this.mnuContextStrip.ShowCheckMargin = true;
            this.mnuContextStrip.ShowImageMargin = false;
            this.mnuContextStrip.Size = new Size(185, 142);
            this.mnuContextStrip.Opening += new CancelEventHandler(this.mnuContextStrip_Opening);
            this.miContextCopy.Name = "miContextCopy";
            this.miContextCopy.Size = new Size(184, 22);
            this.miContextCopy.Text = "&Copy";
            this.miContextCopy.Click += new EventHandler(this.miContextCopy_Click);
            this.miContextCut.Name = "miContextCut";
            this.miContextCut.Size = new Size(184, 22);
            this.miContextCut.Text = "Cu&t";
            this.miContextCut.Click += new EventHandler(this.miContextCut_Click);
            this.miContextPaste.Name = "miContextPaste";
            this.miContextPaste.Size = new Size(184, 22);
            this.miContextPaste.Text = "&Paste";
            this.miContextPaste.Click += new EventHandler(this.miContextPaste_Click);
            this.miTextWizard.Name = "miTextWizard";
            this.miTextWizard.Size = new Size(184, 22);
            this.miTextWizard.Text = "Send to T&extWizard...";
            this.miTextWizard.Click += new EventHandler(this.miTextWizard_Click);
            this.miOpenXml.Name = "miOpenXml";
            this.miOpenXml.Size = new Size(184, 22);
            this.miOpenXml.Text = "Open Xml...";
            this.miOpenXml.Click += new EventHandler(this.miOpenXml_Click);
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new Size(181, 6);
            this.miContextShowHeader.Checked = true;
            this.miContextShowHeader.CheckOnClick = true;
            this.miContextShowHeader.CheckState = CheckState.Checked;
            this.miContextShowHeader.Name = "miContextShowHeader";
            this.miContextShowHeader.Size = new Size(184, 22);
            this.miContextShowHeader.Text = "&Show Header";
            this.miContextWordWrap.Name = "miContextWordWrap";
            this.miContextWordWrap.Size = new Size(184, 22);
            this.miContextWordWrap.Text = "&Word Wrap";
            this.miContextWordWrap.Click += new EventHandler(this.miContextWordWrap_Click);
            this.pnlActions.BackColor = SystemColors.AppWorkspace;
            this.pnlActions.Controls.Add(this.txtSearchFor);
            this.pnlActions.Controls.Add(this.btnSpawnTextEditor);
            this.pnlActions.Dock = DockStyle.Bottom;
            this.pnlActions.Location = new Point(0, 206);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new Size(380, 22);
            this.pnlActions.TabIndex = 4;
            this.txtSearchFor.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            this.txtSearchFor.BackColor = SystemColors.Window;
            this.txtSearchFor.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearchFor.Font = new Font("Tahoma", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.txtSearchFor.ForeColor = SystemColors.ControlText;
            this.txtSearchFor.Location = new Point(3, 0);
            this.txtSearchFor.Name = "txtSearchFor";
            this.txtSearchFor.Size = new Size(275, 21);
            this.txtSearchFor.TabIndex = 10;
            this.txtSearchFor.TextChanged += new EventHandler(this.txtSearchFor_TextChanged);
            this.txtSearchFor.Enter += new EventHandler(this.txtSearchFor_Enter);
            this.txtSearchFor.KeyDown += new KeyEventHandler(this.txtSearchFor_KeyDown);
            this.btnSpawnTextEditor.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnSpawnTextEditor.BackColor = SystemColors.Control;
            this.btnSpawnTextEditor.Enabled = false;
            this.btnSpawnTextEditor.FlatStyle = FlatStyle.Flat;
            this.btnSpawnTextEditor.Font = new Font("Tahoma", 8);
            this.btnSpawnTextEditor.Location = new Point(280, 0);
            this.btnSpawnTextEditor.Name = "btnSpawnTextEditor";
            this.btnSpawnTextEditor.Size = new Size(98, 20);
            this.btnSpawnTextEditor.TabIndex = 11;
            this.btnSpawnTextEditor.Text = "View in Notepad";
            this.btnSpawnTextEditor.UseVisualStyleBackColor = false;
            this.btnSpawnTextEditor.Click += new EventHandler(this.btnSpawnTextEditor_Click);
            base.Controls.Add(this.txtRaw);
            base.Controls.Add(this.pnlActions);
            this.Font = new Font("Tahoma", 8);
            base.Name = "ATISView";
            base.Size = new Size(380, 228);
            this.mnuContextStrip.ResumeLayout(false);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            base.ResumeLayout(false);
        }

        private void miContextCopy_Click(object sender, EventArgs e)
        {
            this.txtRaw.Copy();
        }

        private void miContextCut_Click(object sender, EventArgs e)
        {
            this.txtRaw.Cut();
        }

        private void miContextPaste_Click(object sender, EventArgs e)
        {
            this.DoPlainTextPaste();
        }

        private void miContextWordWrap_Click(object sender, EventArgs e)
        {
            this.miContextWordWrap.Checked = !this.miContextWordWrap.Checked;
            this.SetWordWrapState(this.miContextWordWrap.Checked);
            FiddlerApplication.Prefs.SetBoolPref(this.sPrefKey + "wordwrap", this.txtRaw.WordWrap);
        }

        private void miTextWizard_Click(object sender, EventArgs e)
        {
            FiddlerApplication.UI.actShowTextWizard(this.txtRaw.SelectedText);
        }

        private void miOpenXml_Click(object sender, EventArgs e)
        {
            this.m_owner.OpenXml();
        }

        private void mnuContextStrip_Opening(object sender, CancelEventArgs e)
        {
            this.miTextWizard.Enabled = (this.miContextCopy.Enabled = (this.txtRaw.SelectedText.Length > 0));
            this.miContextCut.Enabled = (!this.txtRaw.ReadOnly && this.txtRaw.SelectedText.Length > 0);
            this.miContextPaste.Enabled = (!this.txtRaw.ReadOnly && Clipboard.GetDataObject() != null && Clipboard.GetDataObject().GetDataPresent("Text", true));
            this.miContextWordWrap.Checked = this.txtRaw.WordWrap;
        }

        internal void Reset()
        {
            this.txtRaw.Clear();
            this.txtSearchFor.BackColor = Color.FromKnownColor(KnownColor.Window);
        }

        private void SetWordWrapState(bool bWordWrap)
        {
            bool modified = this.txtRaw.Modified;
            this.txtRaw.WordWrap = bWordWrap;
            if (this.txtRaw.Modified != modified)
            {
                this.txtRaw.Modified = modified;
            }
        }

        private void txtRaw_DragDrop(object sender, DragEventArgs e)
        {
            if (this.txtRaw.ReadOnly)
            {
                return;
            }
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] array = (string[])e.Data.GetData("FileDrop", false);
                if (array.Length > 0)
                {
                    return;
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                this.txtRaw.SelectedText = (string)e.Data.GetData(DataFormats.Text);
            }
        }

        private void txtRaw_DragEnter(object sender, DragEventArgs e)
        {
            if (!this.txtRaw.ReadOnly && e.Data.GetDataPresent("Fiddler.Session"))
            {
                e.Effect = DragDropEffects.Copy;
                this.txtRaw.BackColor = Color.Lime;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void txtRaw_DragLeave(object sender, EventArgs e)
        {
            if (!this.txtRaw.ReadOnly)
            {
                this.txtRaw.BackColor = Color.FromKnownColor(KnownColor.Window);
            }
        }

        private void txtRaw_DragOver(object sender, DragEventArgs e)
        {
            if (!this.txtRaw.ReadOnly && (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.FileDrop)))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }
            e.Effect = DragDropEffects.None;
        }

        private void txtRaw_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys)131158)
            {
                this.DoPlainTextPaste();
                e.Handled = (e.SuppressKeyPress = true);
                return;
            }
            if (e.KeyCode == Keys.F3)
            {
                this.txtSearchFor.Focus();
                e.SuppressKeyPress = (e.Handled = true);
            }
        }

        private void txtRaw_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Utilities.LaunchHyperlink(e.LinkText);
        }

        private void txtRaw_ReadOnlyChanged(object sender, EventArgs e)
        {
            this.txtRaw.BackColor = (this.txtRaw.ReadOnly ? CONFIG.colorDisabledEdit : Color.FromKnownColor(KnownColor.Window));
        }

        private void txtSearchFor_Enter(object sender, EventArgs e)
        {
            this.txtSearchFor.SelectAll();
        }

        private void txtSearchFor_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (keyCode <= Keys.Escape)
            {
                if (keyCode != Keys.Return)
                {
                    if (keyCode != Keys.Escape)
                    {
                        return;
                    }
                    this.txtSearchFor.Clear();
                    e.Handled = (e.SuppressKeyPress = true);
                    return;
                }
            }
            else
            {
                switch (keyCode)
                {
                    case Keys.Up:
                        e.Handled = (e.SuppressKeyPress = true);
                        CommonMethods.ScrollUp(this.txtRaw);
                        return;
                    case Keys.Right:
                        return;
                    case Keys.Down:
                        e.Handled = (e.SuppressKeyPress = true);
                        CommonMethods.ScrollDown(this.txtRaw);
                        return;
                    default:
                        if (keyCode != Keys.F3)
                        {
                            return;
                        }
                        break;
                }
            }
            e.Handled = (e.SuppressKeyPress = true);
            CommonMethods.DoRichTextSearch(this.txtRaw, this.txtSearchFor.Text, Control.ModifierKeys == Keys.Control);
        }

        private void txtSearchFor_TextChanged(object sender, EventArgs e)
        {
            if (this.txtSearchFor.Text.Length > 0)
            {
                this.txtSearchFor.BackColor = ((this.txtRaw.Find(this.txtSearchFor.Text, 0, RichTextBoxFinds.None) > -1) ? Color.LightGreen : Color.OrangeRed);
                return;
            }
            this.txtSearchFor.BackColor = Color.FromKnownColor(KnownColor.Window);
            this.txtRaw.Select(0, 0);
        }
    }
}
