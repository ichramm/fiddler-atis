using Fiddler;

namespace FiddlerATIS
{
    public sealed class ATISResponse : ATISInspectorBase, IResponseInspector2
    {
        public ATISResponse()
            : base("ATISResponse")
        {
        }

        // from: IResponseInspector2
        public HTTPResponseHeaders headers
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
            get { return session_.ResponseHeaders; }
        }

        protected override byte[] Body
        {
            get { return session_.ResponseBody; }
        }

        protected override void OnClear()
        {
        }

        protected override string Serialize(bool showHeader)
        {
            return Protocol.SerializeRespone(session_, showHeader);
        }

        #endregion
    }
}
