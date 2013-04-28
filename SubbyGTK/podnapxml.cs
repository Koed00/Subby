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
			public int movieId{ get; set;}
			public string url { get; set;}
			public string uploaderId { get; set; }
			public string uploaderName{ get; set; }
			public string release{ get; set; }
			public int languageId{ get; set; }
			public string languageName { get; set; }
			public long time { get; set; }
			public int tvSeason { get; set; }
			public int tvEpisode { get; set; }
			public int tvSpecial{ get; set; }
			public int cds{ get; set; }
			public string format{ get; set; }
			public string fps{ get; set; }
			public int rating { get; set; }
			public string flags { get; set; }
			public int downloads { get; set; }
		}
	}



}

