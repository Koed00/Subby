using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace podnapisi
{

	[Serializable, XmlRoot("results")]
	public class result
	{
		[XmlElement("pagination")]
		public pagination Pagination { get; set; }
		[XmlElement("subtitle")]
		public List<subtitle> Subtitles { get; set; }

		public class pagination
		{
			public int current { get; set; }
			public int count { get; set; }
			public int results { get; set; }
		}

		public class subtitle
		{
			public int id { get; set; }
			public string title { get; set; }
			public int year { get; set; }
		}
	}



}

