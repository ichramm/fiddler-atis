using Fiddler;
using System;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Windows.Forms;

namespace FiddlerATIS
{
    public abstract class ATISInspectorBase: Inspector2, IBaseInspector2
    {
        /* Filled with the name of the child class */
        protected readonly string instanceName_;
        /* The GUI */
        protected ATISView view_;
        /* Indicates whether the current session is a valid atis session or not*/
        protected bool isAtisSession_;
        /* The current session, valid only when _isAtisSession is true */
        protected Session session_;
        //protected Encoding _encoding = CONFIG.oHeaderEncoding;

        protected ATISInspectorBase(string name)
        {
            instanceName_ = name;
        }

        // virtual in order to get the correct headers
        protected abstract HTTPHeaders Headers { get; }

        // virtual in order to get the correct body
        protected abstract byte[] Body { get; }

        // allow child classes to clear their data
        protected abstract void OnClear();

        // serialize to string
        protected abstract string Serialize(bool showHeader);

        // might be override, might change my mind in the future
        protected virtual void UpdateDisplay()
        {
            string text = isAtisSession_ ? this.Serialize(this.view_.miContextShowHeader.Checked) : "Not an ATIS request.";
            this.view_.Reset();
            this.view_.txtRaw.Text = text.Replace("\0", " ");
            this.view_.btnSpawnTextEditor.Enabled = isAtisSession_;
        }

        // open request outside fiddler
        public void SpawnTextEditor()
        {
            string path = String.Format("{0}{1}.txt", CONFIG.GetPath("SafeTemp"), instanceName_);
            InspectorUtils.OpenTextEditor(path, this.Serialize(this.view_.miContextShowHeader.Checked));
        }

        public void OpenXml()
        {
            string path = String.Format("{0}{1}.xml", CONFIG.GetPath("SafeTemp"), instanceName_);

            var doc = XmlStore.Instance.GetXml(session_);
            using (var stringWriter = new StringWriter())
            {
                var settings = new XmlWriterSettings();
                settings.Indent = true;
                using (var xmlTextWriter = XmlWriter.Create(stringWriter, settings))
                {
                    doc.WriteTo(xmlTextWriter);
                    xmlTextWriter.Flush();
                    InspectorUtils.OpenTextEditor(path, stringWriter.GetStringBuilder().ToString());
                }
            }
        }

        #region implemented abstract members of Inspector2

        public override void AddToTab(System.Windows.Forms.TabPage o)
        {
            this.view_ = new ATISView(this);
            this.view_.txtRaw.ReadOnly = true;
            this.view_.txtRaw.BackColor = CONFIG.colorDisabledEdit;

            o.Text = "ATIS";
            o.Controls.Add(this.view_);
            o.Controls[0].Dock = DockStyle.Fill;

            this.view_.miContextShowHeader.CheckedChanged += new EventHandler(this.ContextShowHeaders_CheckedChanged);
        }

        public override int GetOrder()
        {
            return 255; // put this extension at the very end
        }

        #endregion

        #region overrides of Inspector2

        public override void AssignSession(Session oS)
        {
            this.Clear();

            isAtisSession_ = Protocol.IsAtisSession(oS); 
            if (isAtisSession_)
            {
                session_ = oS;
            }

            this.UpdateDisplay();
        }

        public override void SetFontSize(float flSizeInPoints)
        {
            if (this.view_ != null)
            {
                this.view_.txtRaw.Font = new Font(this.view_.txtRaw.Font.FontFamily, flSizeInPoints);
            }
        }

        #endregion

        #region IBaseInspector2 implementation

        public void Clear()
        {
            this.OnClear();
            this.session_ = null;
            this.view_.txtRaw.Clear();
            this.view_.btnSpawnTextEditor.Enabled = false;
        }

        public byte[] body
        {
            get
            {
                return null;
            }
            set
            {
                value = null;
                this.UpdateDisplay();
            }
        }

        public bool bDirty
        {
            get
            {
                return false;
            }
        }

        public bool bReadOnly
        {
            get
            {
                return true;
            }
            set
            {
                value = true;
            }
        }

        #endregion

        private void ContextShowHeaders_CheckedChanged(object sender, EventArgs e)
        {
            this.UpdateDisplay();
        }
    }
}

