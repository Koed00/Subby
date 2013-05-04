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
	private List<OpenSubtitlesClient.SearchResult> subtitles = new  List<OpenSubtitlesClient.SearchResult> ();

	public MainWindow () : base(Gtk.WindowType.Toplevel)
	{
		Build ();
		languagebox.Changed += new System.EventHandler (LanguageChanged);
		Downloadbutton.Clicked += new System.EventHandler (DownloadSub);
		FileButton.Clicked += new System.EventHandler (SelectFile);
		MovieNodeView.NodeSelection.Changed += new System.EventHandler (OnSelectionChanged);
		MovieNodeView.AppendColumn ("Title", new Gtk.CellRendererText (), "text", 0);
		MovieNodeView.AppendColumn ("Year", new Gtk.CellRendererText (), "text", 1);
		MovieNodeView.AppendColumn ("Season", new Gtk.CellRendererText (), "text", 2);
		MovieNodeView.AppendColumn ("Episode", new Gtk.CellRendererText (), "text", 3);
		MovieNodeView.AppendColumn ("Uploader", new Gtk.CellRendererText (), "text", 4);
		MovieNodeView.AppendColumn ("Downloads", new Gtk.CellRendererText (), "text", 5);
		DetailNode.AppendColumn("",new Gtk.CellRendererText (), "text", 0);
		DetailNode.AppendColumn("",new Gtk.CellRendererText (), "text", 1);
		MovieNodeView.ShowAll ();
		DetailNode.ShowAll ();




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
		public string SubFormat {get;set;}
		public bool SubHearingImpaired{ get; set; }
		public string AuthorCommments{ get; set; }
		public string Language{ get; set; }
		public string SubAddDate {get;set;}
		public string ReleaseName{ get; set; }
	}

	[Gtk.TreeNode (ListOnly=true)]
	public class DetailTreeNode : Gtk.TreeNode
	{
		public DetailTreeNode(string name, string value){
			Name=name;
			Value=value;
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

	public void PopulateLanguages (string deflang)
	{
		var opensub = new OpenSubtitlesClient ();
		ArrayList langs = opensub.GetSubLanguages ();
		var langstore = new Gtk.ListStore (typeof(string), typeof(string));
		langstore.AppendValues ("All", "all");
		foreach (Hashtable lang in langs) {
			langstore.AppendValues (lang ["LanguageName"].ToString (), (lang ["SubLanguageID"] ?? string.Empty).ToString ());
		}
		languagebox.Model = langstore;
		statusbar1.Push (4, "Ready.");
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}

	protected void SelectFile (object sender, EventArgs e)
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

	private void GetSubs ()
	{
		var store = new Gtk.NodeStore (typeof(MovieTreeNode));
		statusbar1.Push (1, "Searching for filename.");
		var opensub = new OpenSubtitlesClient ();
		//get the selected language
		//TODO get last selected language from prefs
		var model = languagebox.Model;
		TreeIter itar;
		string lang;
		if (languagebox.GetActiveIter (out itar))
			lang = (string)languagebox.Model.GetValue (itar, 1);
		else {
			lang = "all";
		}
		//TODO save selected language to prefs
		subtitles = opensub.FileSearch (fname, lang);
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
				node.SubAddDate = sub.SubAddDate.ToShortDateString();
				node.ReleaseName = sub.MovieReleaseName;
				node.IMDBRating = sub.MovieImdbRating;
				node.SubRating = sub.SubRating;
				node.Lang = sub.SubLanguageID;
				node.Language = sub.LanguageName;
				node.SubFormat = sub.SubFormat;
				node.SubHearingImpaired=sub.SubHearingImpaired=="1";
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
		string newfile = origlocation + "/" + movietitle + "."+selectednode.SubFormat; //TODO add actual extension type
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

	protected void OnSelectionChanged (object sender, EventArgs e)
	{
		var selectednode = (MovieTreeNode)MovieNodeView.NodeSelection.SelectedNode;
		if (selectednode == null)
			return;
		var store = new Gtk.NodeStore (typeof(DetailTreeNode));
		store.AddNode(new DetailTreeNode("Added:", selectednode.SubAddDate));
		store.AddNode (new DetailTreeNode ("Release:", selectednode.ReleaseName));
		store.AddNode(new DetailTreeNode("Comments:", selectednode.AuthorCommments));
		store.AddNode(new DetailTreeNode("Language:", selectednode.Language));
		store.AddNode(new DetailTreeNode("Rating:", selectednode.SubRating));
		store.AddNode(new DetailTreeNode("IMDB:", selectednode.IMDBRating));
		store.AddNode (new DetailTreeNode ("Format:", selectednode.SubFormat));
		store.AddNode (new DetailTreeNode ("HearingImpaired:", selectednode.SubHearingImpaired.ToString ()));

			DetailNode.NodeStore = store;
			DetailNode.ShowAll ();

	}
}