using Fiddler;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace FiddlerATIS
{
    public abstract class BaseInspector: Inspector2, IBaseInspector2
    {
        /* Filled with the name of the child class */
        protected readonly string _instanceName;
        /* The GUI */
        protected ATISView _view;
        /* Indicates whether the current session is a valid atis session or not*/
        protected bool _isAtisSession;
        /* The current session, valid only when _isAtisSession is true */
        protected Session _session;
        //protected Encoding _encoding = CONFIG.oHeaderEncoding;

        protected BaseInspector(string name)
        {
            _instanceName = name;
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
            string text = _isAtisSession ? this.Serialize(this._view.miContextShowHeader.Checked) : "Not an ATIS request.";
            this._view.Reset();
            this._view.txtRaw.Text = text.Replace("\0", " ");
            this._view.btnSpawnTextEditor.Enabled = _isAtisSession;
        }

        // open request outside fiddler
        public void SpawnTextEditor()
        {
            string path = String.Format("{0}{1}.txt", CONFIG.GetPath("SafeTemp"), _instanceName);
            InspectorUtils.OpenTextEditor(path, this.Serialize(this._view.miContextShowHeader.Checked));
        }

        public void OpenXml()
        {
            //string path = String.Format("{0}{1}.xml", CONFIG.GetPath("SafeTemp"), _instanceName);
        }

        #region implemented abstract members of Inspector2

        public override void AddToTab(System.Windows.Forms.TabPage o)
        {
            this._view = new ATISView(this);
            this._view.txtRaw.ReadOnly = true;
            this._view.txtRaw.BackColor = CONFIG.colorDisabledEdit;

            o.Text = "ATIS";
            o.Controls.Add(this._view);
            o.Controls[0].Dock = DockStyle.Fill;

            this._view.miContextShowHeader.CheckedChanged += new EventHandler(this.ContextShowHeaders_CheckedChanged);
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

            _isAtisSession = Protocol.IsAtisSession(oS); 
            if (_isAtisSession)
            {
                _session = oS;
            }

            this.UpdateDisplay();
        }

        public override void SetFontSize(float flSizeInPoints)
        {
            if (this._view != null)
            {
                this._view.txtRaw.Font = new Font(this._view.txtRaw.Font.FontFamily, flSizeInPoints);
            }
        }

        #endregion

        #region IBaseInspector2 implementation

        public void Clear()
        {
            this.OnClear();
            this._session = null;
            this._view.txtRaw.Clear();
            this._view.btnSpawnTextEditor.Enabled = false;
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

