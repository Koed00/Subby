using System;
using Gtk;
using SubbyGTK;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

public partial class MainWindow: Gtk.Window
{	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		Gtk.TreeViewColumn movieColumn = new Gtk.TreeViewColumn ();
		movieColumn.Title = "Movie";
		Gtk.CellRendererText movieNameCell = new Gtk.CellRendererText ();
		movieColumn.PackStart (movieNameCell, true);

		Gtk.TreeViewColumn yearColumn = new Gtk.TreeViewColumn ();
		yearColumn.Title = "Year";
		Gtk.CellRendererText yearTitleCell = new Gtk.CellRendererText ();
		yearColumn.PackStart (yearTitleCell, true);

		Gtk.TreeViewColumn seasonColumn = new Gtk.TreeViewColumn ();
		seasonColumn.Title = "Season";
		Gtk.CellRendererText seasonTitleCell = new Gtk.CellRendererText ();
		seasonColumn.PackStart (seasonTitleCell, true);

		Gtk.TreeViewColumn episodeColumn = new Gtk.TreeViewColumn ();
		episodeColumn.Title = "Episode";
		Gtk.CellRendererText episodeTitleCell = new Gtk.CellRendererText ();
		episodeColumn.PackStart (episodeTitleCell, true);

		Gtk.TreeViewColumn ratingColumn = new Gtk.TreeViewColumn ();
		ratingColumn.Title = "Rating";
		Gtk.CellRendererText ratingTitleCell = new Gtk.CellRendererText ();
		ratingColumn.PackStart (ratingTitleCell, true);

		tree.AppendColumn (movieColumn);
		tree.AppendColumn (yearColumn);
		tree.AppendColumn (seasonColumn);
		tree.AppendColumn (episodeColumn);
		tree.AppendColumn (ratingColumn);

		movieColumn.AddAttribute (movieNameCell, "text", 0);
		yearColumn.AddAttribute (yearTitleCell, "text", 1);
		seasonColumn.AddAttribute (seasonTitleCell, "text", 2);
		episodeColumn.AddAttribute (episodeTitleCell, "text", 3);
		ratingColumn.AddAttribute (ratingTitleCell, "text", 4);

		var opensub = new OpenSubtitlesClient ();
		var langs = opensub.GetSubLanguages ();
		var langstore = new Gtk.ListStore (typeof (string), typeof(string));
		foreach (Hashtable lang in langs){

			langstore.AppendValues(lang["LanguageName"].ToString(), (lang["ISO639"]?? string.Empty).ToString());

		}
		combobox2.Model = langstore;

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
		ThreadPool.QueueUserWorkItem(x => 	statusbar1.Push (1, "Searching for filename."));
	
		Gtk.TreeStore musicListStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string),typeof(string),typeof(string),typeof(string));
		var opensub = new OpenSubtitlesClient ();
		var subtitles = opensub.FileSearch (fname);
		statusbar1.Push (2, "Found " + subtitles.Count + " titles");

		Gtk.TreeIter iter;
		foreach (var sub in subtitles) {
			iter = musicListStore.AppendValues (sub.MovieName, sub.MovieYear,sub.SeriesSeason,sub.SeriesEpisode, sub.SubRating, sub.SubDownloadLink);

			musicListStore.AppendValues (iter,"Author Comment:", sub.SubAuthorComment);
			musicListStore.AppendValues (iter,"Matched by", sub.MatchedBy);


		}
		tree.Model = musicListStore;
		//tree.ExpandAll ();

	}

	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		TreeIter iter;
		tree.Selection.GetSelected (out iter);
		var  downloadlinkg=(string) tree.Model.GetValue (iter, 5);

	}
}
