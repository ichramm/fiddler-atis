﻿using Fiddler;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FiddlerATIS
{
	public sealed class ATISRequest : BaseInspector, IRequestInspector2
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
			get { return _session.RequestHeaders; }
		}

		protected override byte[] Body
		{
			get { return _session.RequestBody; }
		}

		protected override void OnClear()
		{
		}

		protected override string Serialize()
		{
			return Protocol.SerializeRequest(_session);
		}

		#endregion
	}
}