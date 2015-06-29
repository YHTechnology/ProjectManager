using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Lite.ExcelLibrary.BinaryDrawingFormat
{
	public partial class MsofbtArcRule : EscherRecord
	{
		public MsofbtArcRule(EscherRecord record) : base(record) { }

		public MsofbtArcRule()
		{
			this.Type = EscherRecordType.MsofbtArcRule;
		}

	}
}
