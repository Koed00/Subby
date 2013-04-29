using System;
using Gtk;
using SubbyGTK;
using CookComputing.XmlRpc;

public partial class MainWindow: Gtk.Window
{	
	private Gtk.ListStore subtitlestore = new Gtk.ListStore (typeof (string), typeof (string));
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
		var subtitles=MainClass.Search(entry2.Text);
		foreach (var subtitle in subtitles) {
			subtitlestore.AppendValues (subtitle.title, subtitle.downloads.ToString ());
		}
	}
}
