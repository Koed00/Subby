using System;
using Gtk;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using SubbyGTK;

public partial class MainWindow : Gtk.Window
{
	private string fname = string.Empty;

	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		var movieColumn = new Gtk.TreeViewColumn ();
		movieColumn.Title = "Title";
		var movieNameCell = new Gtk.CellRendererText ();
		movieColumn.PackStart (movieNameCell, true);

		var yearColumn = new Gtk.TreeViewColumn ();
		yearColumn.Title = "Year";
		var yearTitleCell = new Gtk.CellRendererText ();
		yearColumn.PackStart (yearTitleCell, true);

		var seasonColumn = new Gtk.TreeViewColumn ();
		seasonColumn.Title = "Season";
		var seasonTitleCell = new Gtk.CellRendererText ();
		seasonColumn.PackStart (seasonTitleCell, true);

		var episodeColumn = new Gtk.TreeViewColumn ();
		episodeColumn.Title = "Episode";
		var episodeTitleCell = new Gtk.CellRendererText ();
		episodeColumn.PackStart (episodeTitleCell, true);

		var ratingColumn = new Gtk.TreeViewColumn ();
		ratingColumn.Title = "Uploader";
		var ratingTitleCell = new Gtk.CellRendererText ();
		ratingColumn.PackStart (ratingTitleCell, true);

		var downloadsColumn = new Gtk.TreeViewColumn ();
		downloadsColumn.Title = "Downloads";
		var downloadsTitleCell = new Gtk.CellRendererText ();
		downloadsColumn.PackStart (downloadsTitleCell, true);

		tree.AppendColumn (movieColumn);
		tree.AppendColumn (yearColumn);
		tree.AppendColumn (seasonColumn);
		tree.AppendColumn (episodeColumn);
		tree.AppendColumn (downloadsColumn);
		tree.AppendColumn (ratingColumn);

		movieColumn.AddAttribute (movieNameCell, "text", 0);
		yearColumn.AddAttribute (yearTitleCell, "text", 1);
		seasonColumn.AddAttribute (seasonTitleCell, "text", 2);
		episodeColumn.AddAttribute (episodeTitleCell, "text", 3);
		downloadsColumn.AddAttribute (downloadsTitleCell, "text", 4);
		ratingColumn.AddAttribute (ratingTitleCell, "text", 5);


	}

	public void PushStatus (uint i, string statustext)
	{
		statusbar1.Push (i, statustext);
	}

	public void PopulateLanguages (string deflang)
	{
		var opensub = new OpenSubtitlesClient ();
		ArrayList langs = opensub.GetSubLanguages ();
		var langstore = new Gtk.ListStore (typeof (string), typeof(string));
		langstore.AppendValues ("All", "all");
		foreach (Hashtable lang in langs) {
			langstore.AppendValues (lang["LanguageName"].ToString(), (lang ["SubLanguageID"] ?? string.Empty).ToString ());
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
		var chooser = new FileChooserDialog (
			"Please select movie...",
			this,
			FileChooserAction.Open,
			"Cancel", ResponseType.Cancel,
			"Select", ResponseType.Accept);
		var filt = new FileFilter ();
		filt.Name = "Movies";
		filt.AddMimeType ("video/x-matroska");
		filt.AddMimeType ("video/x-msvideo");
		filt.AddMimeType ("video/vnd.mpegurl");
		filt.AddMimeType ("video/x-m4v");
		filt.AddMimeType ("video/mp4");
		filt.AddMimeType ("video/quicktime");
		filt.AddMimeType ("video/mpeg");
		filt.AddMimeType ("video/x-dv");
		filt.AddMimeType ("video/x-sgi-movie");
		chooser.AddFilter (filt);
		if (chooser.Run () == (int)ResponseType.Accept) {
			fname = chooser.Filename;
			entry2.Text = fname;
			chooser.Destroy ();
		} else {
			chooser.Destroy ();
			return;
		}

		GetSubs ();
	
	}

	private void GetSubs(){
		tree.Model = null;
		statusbar1.Push (1, "Searching for filename.");
		var MovieListStore = new Gtk.TreeStore (typeof (string), typeof(string), typeof(string), typeof(string),
		                                        typeof(string), typeof(string), typeof(string));
		var opensub = new OpenSubtitlesClient ();
		//get the selected language
		//TODO get last selected language from prefs
		var model = combobox2.Model;
		TreeIter itar;
		string lang;
		if(combobox2.GetActiveIter(out itar)) lang =(string)combobox2.Model.GetValue (itar,1);
		else{lang = "all";}
		//TODO save selected language to prefs
		List<OpenSubtitlesClient.SearchResult> subtitles = opensub.FileSearch (fname,lang);
		statusbar1.Push (2, "Found " + subtitles.Count + " titles");
		if(subtitles.Count==0) return;

		Gtk.TreeIter iter;
		foreach (OpenSubtitlesClient.SearchResult sub in subtitles) {
			iter = MovieListStore.AppendValues (sub.MovieName, sub.MovieYear, sub.SeriesSeason, sub.SeriesEpisode,sub.SubDownloadsCnt,
			                                    sub.UserNickName, sub.SubDownloadLink);

			MovieListStore.AppendValues (iter, "Release name:", sub.MovieReleaseName);
			if(!string.IsNullOrEmpty(sub.SubAuthorComment))MovieListStore.AppendValues (iter, "Author Comment:", sub.SubAuthorComment);
			if(sub.SubHearingImpaired!="0")MovieListStore.AppendValues (iter, "Hearing Imparied:", sub.SubHearingImpaired);
			MovieListStore.AppendValues (iter, "Language:",sub.LanguageName);
			if(sub.SubRating!="0.0")MovieListStore.AppendValues (iter, "Sub Rating:", sub.SubRating);
			if(sub.MovieImdbRating!="0.0")MovieListStore.AppendValues (iter, "IMDB rating:", sub.MovieImdbRating);
		}
		tree.Model = MovieListStore;

	}
	protected void OnButton2Clicked (object sender, EventArgs e)
	{
		TreeIter iter;
		tree.Selection.GetSelected (out iter);
		var downloadlinkg = (string)tree.Model.GetValue (iter, 6);
		string movietitle = System.IO.Path.GetFileNameWithoutExtension (fname);
		var Client = new WebClient ();
		string filename = "/tmp/tmp_" + movietitle + ".gz";
		Client.DownloadFile (downloadlinkg, filename);
		var fileToDecompress = new FileInfo (filename);
		string currentFileName = fileToDecompress.FullName;
		string newFileName = currentFileName.Remove (currentFileName.Length - fileToDecompress.Extension.Length);

		using (FileStream originalFileStream = fileToDecompress.OpenRead()) {
			using (FileStream decompressedFileStream = File.Create(newFileName)) {
				using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress)) {
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
		statusbar1.Push (3, "Downloaded " + newfile + ".");
	}

	protected void combobox2changed (object sender, EventArgs e)
	{
		if(!String.IsNullOrEmpty(fname)) GetSubs();
	}

	protected void MovieTitleSelected (object o, RowActivatedArgs args)
	{var DetailListStore = new Gtk.TreeStore (typeof (string), typeof(string));

	}
}