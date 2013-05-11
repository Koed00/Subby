using System;
using System.Linq;
using Gtk;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
namespace SubbyGTK
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainWindow win = new MainWindow ();
			if (OpenSubtitlesClient.CheckForConnection()) {
				win.PopulateLanguages ();
				win.Show ();
			} else {
				win.ShowError ("Can't connect to Opensubtitles.org\nPlease check your internet connection");
				win.Destroy ();
				Application.Quit();
				return;
			}
			Application.Run ();
	
		}


	}
}
