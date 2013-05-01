using System;
using System.Linq;
using Gtk;
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


	}
}
