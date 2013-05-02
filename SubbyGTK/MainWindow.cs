using System;
using Gtk;
using SubbyGTK;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.IO.Compression;

public partial class MainWindow: Gtk.Window
{	
	private	string fname = string.Empty;

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


	}
	public void PushStatus(uint i, string statustext){

		statusbar1.Push (i, statustext);
	}

	public void PopulateLanguages(string deflang){

		var opensub = new OpenSubtitlesClient ();
		var langs = opensub.GetSubLanguages ();
		var langstore = new Gtk.ListStore (typeof (string), typeof(string));
		foreach (Hashtable lang in langs) {

			langstore.AppendValues (lang["LanguageName"].ToString(), (lang ["ISO639"] ?? string.Empty).ToString ());

		}
		combobox2.Model = langstore;
		statusbar1.Push (4, "Ready.");
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void OnButton1Clicked (object sender, EventArgs e)
	{
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
	
		Gtk.TreeStore musicListStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
		var opensub = new OpenSubtitlesClient ();
		var subtitles = opensub.FileSearch (fname);
		statusbar1.Push (2, "Found " + subtitles.Count + " titles");

		Gtk.TreeIter iter;
		foreach (var sub in subtitles) {
			iter = musicListStore.AppendValues (sub.MovieName, sub.MovieYear, sub.SeriesSeason, sub.SeriesEpisode, sub.SubRating, sub.SubDownloadLink);

			musicListStore.AppendValues (iter, "Author Comment:", sub.SubAuthorComment);
			musicListStore.AppendValues (iter, "Language", sub.LanguageName);


		}
		tree.Model = musicListStore;
		//tree.ExpandAll ();

	}

	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		TreeIter iter;
		tree.Selection.GetSelected (out iter);
		var downloadlinkg = (string)tree.Model.GetValue (iter, 5);
		var movietitle = System.IO.Path.GetFileNameWithoutExtension (fname);
		WebClient Client = new WebClient ();
		var filename = "/tmp/tmp_" + movietitle + ".gz";
		Client.DownloadFile (downloadlinkg, filename);
		var fileToDecompress = new FileInfo (filename);
		string currentFileName = fileToDecompress.FullName;
		string newFileName = currentFileName.Remove (currentFileName.Length - fileToDecompress.Extension.Length);

		using (FileStream originalFileStream = fileToDecompress.OpenRead()) {
			using (FileStream decompressedFileStream = File.Create(newFileName)) {
				using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress)) {
					decompressionStream.CopyTo (decompressedFileStream);

				}
			}
		}
		fileToDecompress.Delete ();
		string origlocation = System.IO.Path.GetDirectoryName (fname);
		string newfile = origlocation + "/" + movietitle + ".srt"; //TODO add actual extension type
		if (File.Exists (newfile)) {
			File.Delete (newfile);
		} //TODO: ask for overwrite?

		File.Move (newFileName, newfile);
		statusbar1.Push (3, "Downloaded " + newfile+". Done");


	}
}
