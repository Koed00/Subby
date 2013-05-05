using System;
using Gtk;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using SubbyGTK;
using System.Configuration;

public partial class MainWindow : Gtk.Window
{
	private string fname = string.Empty;
	private List<OpenSubtitlesClient.SearchResult> subtitles = new  List<OpenSubtitlesClient.SearchResult> ();

	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		languagebox.Changed += new System.EventHandler (LanguageChanged);
		Downloadbutton.Clicked += new System.EventHandler (DownloadSub);
		FileChooserD.FileSet += new System.EventHandler (SelectFile);
		MovieNodeView.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
		MovieNodeView.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
		MovieNodeView.AppendColumn ("Year", new Gtk.CellRendererText (), "text", 1);
		MovieNodeView.AppendColumn ("Season", new Gtk.CellRendererText (), "text", 2);
		MovieNodeView.AppendColumn ("Episode", new Gtk.CellRendererText (), "text", 3);
		MovieNodeView.AppendColumn ("Uploader", new Gtk.CellRendererText (), "text", 4);
		MovieNodeView.AppendColumn ("Downloads", new Gtk.CellRendererText (), "text", 5);
		DetailNode.AppendColumn ("", new Gtk.CellRendererText (), "text", 0);
		DetailNode.AppendColumn ("", new Gtk.CellRendererText (), "text", 1);
		MovieNodeView.ShowAll ();
		DetailNode.ShowAll ();

		//FileChooser stuff
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
		FileChooserD.AddFilter (filt);

	}

	[Gtk.TreeNode (ListOnly=true)]
	public class MovieTreeNode : Gtk.TreeNode
	{

		[Gtk.TreeNodeValue (Column=0)]
		public string Title{ get; set; }

		[Gtk.TreeNodeValue (Column=1)]
		public string Year { get; set; }

		[Gtk.TreeNodeValue (Column=2)]
		public string Season { get; set; }

		[Gtk.TreeNodeValue (Column=3)]
		public string Episode { get; set; }

		[Gtk.TreeNodeValue (Column=4)]
		public string Uploader { get; set; }

		[Gtk.TreeNodeValue (Column=5)]
		public string Downloads { get; set; }

		public string SubRating { get; set; }

		public string IMDBRating { get; set; }

		public string Lang { get; set; }

		public string DownloadLink { get; set; }

		public string SubFormat { get; set; }

		public bool SubHearingImpaired{ get; set; }

		public string AuthorCommments{ get; set; }

		public string Language{ get; set; }

		public string SubAddDate { get; set; }

		public string ReleaseName{ get; set; }
	}

	[Gtk.TreeNode (ListOnly=true)]
	public class DetailTreeNode : Gtk.TreeNode
	{
		public DetailTreeNode (string name, string value)
		{
			Name = name;
			Value = value;
		}

		[Gtk.TreeNodeValue (Column=0)]
		public string Name{ get; set; }

		[Gtk.TreeNodeValue (Column=1)]
		public string Value { get; set; }
	}

	public void PushStatus (uint i, string statustext)
	{
		statusbar1.Push (i, statustext);

	}


	public void PopulateLanguages ()
	{

		var deflang = ConfigurationManager.AppSettings ["sublanguage"].ToString ();
		var opensub = new OpenSubtitlesClient ();
		ArrayList langs = opensub.GetSubLanguages ();
		var langstore = new Gtk.ListStore (typeof(string), typeof(string));
		int defrow=0;
		int i = 0;
		foreach (Hashtable lang in langs) {
			langstore.AppendValues (lang ["LanguageName"].ToString (), (lang ["SubLanguageID"] ?? string.Empty).ToString ());
			if (lang.ContainsValue (deflang))
				defrow = i;
			i++;
		}
		languagebox.Model = langstore;
		Gtk.TreeIter iter;
		languagebox.Model.IterNthChild (out iter, defrow);
		languagebox.SetActiveIter (iter);
		
	}


	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		config.AppSettings.Settings.Remove("sublanguage");
		config.AppSettings.Settings.Add("sublanguage", GetCurrentLang());
		config.Save(ConfigurationSaveMode.Modified);
	
		Application.Quit ();
		a.RetVal = true;
	}

	protected void SelectFile (object sender, EventArgs e)
	{

		fname = FileChooserD.Filename;
		GetSubs ();
	
	}

	private void GetSubs ()
	{
		var store = new Gtk.NodeStore (typeof(MovieTreeNode));
		statusbar1.Push (1, "Searching for filename.");
		var opensub = new OpenSubtitlesClient ();
		subtitles = opensub.FileSearch (fname, GetCurrentLang());
		statusbar1.Push (2, "Found " + subtitles.Count + " titles");
		if (subtitles.Count > 0) {					
			foreach (OpenSubtitlesClient.SearchResult sub in subtitles) {
				var node = new MovieTreeNode ();
				node.Title = sub.MovieName;
				node.Year = sub.MovieYear;
				node.Season = sub.SeriesSeason;
				node.Episode = sub.SeriesEpisode;
				node.Uploader = sub.UserNickName;
				node.Downloads = sub.SubDownloadsCnt;
				node.DownloadLink = sub.SubDownloadLink;
				node.AuthorCommments = sub.SubAuthorComment;
				node.SubAddDate = sub.SubAddDate.ToShortDateString ();
				node.ReleaseName = sub.MovieReleaseName;
				node.IMDBRating = sub.MovieImdbRating;
				node.SubRating = sub.SubRating;
				node.Lang = sub.SubLanguageID;
				node.Language = sub.LanguageName;
				node.SubFormat = sub.SubFormat;
				node.SubHearingImpaired = sub.SubHearingImpaired == "1";
				store.AddNode (node);

			}
		}

		MovieNodeView.NodeStore = store;
		DetailNode.NodeStore = null;
		MovieNodeView.ShowAll ();
		DetailNode.ShowAll ();

		//TODO show details of first result

	}

	protected void DownloadSub (object sender, EventArgs e)
	{		 
		var selectednode = (MovieTreeNode)MovieNodeView.NodeSelection.SelectedNode;		
		if (selectednode == null)
			return;
		string movietitle = System.IO.Path.GetFileNameWithoutExtension (fname);
		var Client = new WebClient ();
		string filename = "/tmp/tmp_" + movietitle + ".gz";
		Client.DownloadFile (selectednode.DownloadLink, filename);
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
		string newfile = origlocation + "/" + movietitle + "." + selectednode.SubFormat;
		if (File.Exists (newfile)) {
			File.Delete (newfile);
		} //TODO: ask for overwrite?

		File.Move (newFileName, newfile);
		statusbar1.Push (3, "Downloaded " + newfile + ".");
	}

	protected void LanguageChanged (object sender, EventArgs e)
	{
		if (!String.IsNullOrEmpty (fname))
			GetSubs ();
	}

	private string GetCurrentLang(){
		var model = languagebox.Model;
		TreeIter itar;
		if (languagebox.GetActiveIter (out itar))
			return (string)languagebox.Model.GetValue (itar, 1);
		else {
			return "all";
		};

	}
	protected void OnSelectionChanged (object sender, EventArgs e)
	{
		var selectednode = (MovieTreeNode)MovieNodeView.NodeSelection.SelectedNode;
		if (selectednode == null)
			return;
		var store = new Gtk.NodeStore (typeof(DetailTreeNode));
		store.AddNode (new DetailTreeNode ("Added:", selectednode.SubAddDate));
		store.AddNode (new DetailTreeNode ("Release:", selectednode.ReleaseName));
		store.AddNode (new DetailTreeNode ("Comments:", selectednode.AuthorCommments));
		store.AddNode (new DetailTreeNode ("Language:", selectednode.Language));
		store.AddNode (new DetailTreeNode ("Rating:", selectednode.SubRating));
		store.AddNode (new DetailTreeNode ("IMDB:", selectednode.IMDBRating));
		store.AddNode (new DetailTreeNode ("Format:", selectednode.SubFormat));
		store.AddNode (new DetailTreeNode ("HearingImpaired:", selectednode.SubHearingImpaired.ToString ()));

		DetailNode.NodeStore = store;
		DetailNode.ShowAll ();

	}
}