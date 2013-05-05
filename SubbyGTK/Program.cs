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
			win.PopulateLanguages ();
			win.Show ();
			Application.Run ();
			//setup node

	
		}


	}
}
