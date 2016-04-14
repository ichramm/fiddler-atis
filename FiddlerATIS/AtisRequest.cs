using Fiddler;

namespace FiddlerATIS
{
    public sealed class ATISRequest : ATISInspectorBase, IRequestInspector2
    {
        public ATISRequest()
            : base("ATISRequest")
        {
        }

        // from: IRequestInspector2
        public HTTPRequestHeaders headers
        {
            get { return null; }
            set
            {
                value = null;
                this.UpdateDisplay();
            }
        }

        #region implemented abstract members of BaseInspector

        protected override HTTPHeaders Headers
        {
            get { return session_.RequestHeaders; }
        }

        protected override byte[] Body
        {
            get { return session_.RequestBody; }
        }

        protected override void OnClear()
        {
        }

        protected override string Serialize(bool showHeader)
        {
            return Protocol.SerializeRequest(session_, showHeader);
        }

        #endregion
    }
}
