using System;
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
		}

		public static void Search (string filename)
		{
			string cleanname = Path.GetFileNameWithoutExtension (filename);
			var pclient = new podnapisi.Client ();
			var hashresult = pclient.Search (filename);
			var xmlresult = pclient.XMLSearch (cleanname);
			var sublist=new List<podnapisi.result.subtitle>();
			foreach (var subtitle in xmlresult.Subtitles) {

				if (subtitle.languageId == 2) {
					sublist.Add (subtitle);
				}

			}
		

		}
	}
}
