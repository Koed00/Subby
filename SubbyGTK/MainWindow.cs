using System;
using Gtk;
using SubbyGTK;
public partial class MainWindow: Gtk.Window
{	

	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
		 string fname;
		 string fpath;
		FileChooserDialog chooser = new FileChooserDialog (
			"Please select movie...",
			this,
			FileChooserAction.Open,
			"Cancel", ResponseType.Cancel,
			"Select", ResponseType.Accept);
		if (chooser.Run () == (int)ResponseType.Accept) {

			fname = chooser.Filename;
			entry2.Text = fname;
		
		} 
		chooser.Destroy ();
		MainClass.Search(entry2.Text);
	}
}
