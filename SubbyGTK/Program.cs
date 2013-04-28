using System;
using System.Linq;
using Gtk;
using CookComputing.XmlRpc;
using System.IO;
using System.Collections.Generic;
namespace SubbyGTK
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
			//setup node

		}

		public static List<podnapisi.result.subtitle>Search (string filename)
		{
			string cleanname = Path.GetFileName(filename);
			var pclient = new podnapisi.Client ();
			var hashresult = pclient.Search (filename);
			var xmlresult = pclient.XMLSearch (cleanname);
			var sublist= xmlresult.Subtitles.Where(subtitle => subtitle.languageId == 2).ToList();	
			return sublist;
		}
	}
}
