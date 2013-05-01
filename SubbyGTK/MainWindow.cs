using System;
using Gtk;
using SubbyGTK;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		var subclient = new OpenSubtitlesClient ();
		subclient.FileSearch ("");
		Gtk.TreeViewColumn movieColumn = new Gtk.TreeViewColumn ();
		movieColumn.Title = "Movie";
		Gtk.CellRendererText movieNameCell = new Gtk.CellRendererText ();
		movieColumn.PackStart (movieNameCell, true);

		Gtk.TreeViewColumn yearColumn = new Gtk.TreeViewColumn ();
		yearColumn.Title = "Year";
		Gtk.CellRendererText yearTitleCell = new Gtk.CellRendererText ();
		yearColumn.PackStart (yearTitleCell, true);

		Gtk.TreeViewColumn ratingColumn = new Gtk.TreeViewColumn ();
		ratingColumn.Title = "Rating";
		Gtk.CellRendererText ratingTitleCell = new Gtk.CellRendererText ();
		ratingColumn.PackStart (ratingTitleCell, true);

		tree.AppendColumn (movieColumn);
		tree.AppendColumn (yearColumn);
tree.AppendColumn (ratingColumn);

		movieColumn.AddAttribute (movieNameCell, "text", 0);
		yearColumn.AddAttribute (yearTitleCell, "text", 1);
		ratingColumn.AddAttribute (ratingTitleCell, "text", 2);

	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
		string fname="";
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
		statusbar1.Push (1, "Searching for filename.");
		Gtk.TreeStore musicListStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string),typeof(int));
		var opensub = new OpenSubtitlesClient ();
//		var subtitles = MainClass.Search (fname);
//		statusbar1.Push (2, "Found " + subtitles.Count + " titles");

//		Gtk.TreeIter iter;
//		foreach (var sub in subtitles) {
//			iter = musicListStore.AppendValues (sub.title, sub.year.ToString(), sub.rating.ToString(), sub.id);
//			var releases = sub.release.Split (' ');
//			foreach (var release in releases) {
//				musicListStore.AppendValues (iter, release);
//			}
//
//		}
		tree.Model = musicListStore;
		tree.ExpandAll ();

	}

	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		TreeIter iter;
		tree.Selection.GetSelected (out iter);
		var  movieid=(int) tree.Model.GetValue (iter, 3);
		//var podnap = new podnapisi.Client ();
		//var filename = podnap.GetFileName (movieid);
	}
}
