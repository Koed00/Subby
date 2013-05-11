using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Net;
using Gtk;
using SubbyGTK;

public partial class MainWindow : Window
{
    private string _fname = string.Empty;
    private List<OpenSubtitlesClient.SearchResult> _subtitles = new List<OpenSubtitlesClient.SearchResult>();

    public MainWindow() : base(WindowType.Toplevel)
    {
        Build();
        languagebox.Changed += LanguageChanged;
        Downloadbutton.Clicked += DownloadSub;
        FileChooserD.FileSet += SelectFile;
        MovieNodeView.NodeSelection.Changed += OnSelectionChanged;
        MovieNodeView.AppendColumn("Title", new CellRendererText(), "text", 0);
        MovieNodeView.AppendColumn("Year", new CellRendererText(), "text", 1);
        MovieNodeView.AppendColumn("Season", new CellRendererText(), "text", 2);
        MovieNodeView.AppendColumn("Episode", new CellRendererText(), "text", 3);
        MovieNodeView.AppendColumn("Uploader", new CellRendererText(), "text", 4);
        MovieNodeView.AppendColumn("Downloads", new CellRendererText(), "text", 5);
        DetailNode.AppendColumn("", new CellRendererText(), "text", 0);
        DetailNode.AppendColumn("", new CellRendererText(), "text", 1);
        MovieNodeView.ShowAll();
        DetailNode.ShowAll();

        //FileChooser stuff
        var filt = new FileFilter {Name = "Movies"};
        filt.AddMimeType("video/x-matroska");
        filt.AddMimeType("video/x-msvideo");
        filt.AddMimeType("video/vnd.mpegurl");
        filt.AddMimeType("video/x-m4v");
        filt.AddMimeType("video/mp4");
        filt.AddMimeType("video/quicktime");
        filt.AddMimeType("video/mpeg");
        filt.AddMimeType("video/x-dv");
        filt.AddMimeType("video/x-sgi-movie");
        FileChooserD.AddFilter(filt);
    }

    public void PushStatus(uint i, string statustext)
    {
        statusbar1.Push(i, statustext);
    }


    public void PopulateLanguages()
    {
        string deflang = ConfigurationManager.AppSettings["sublanguage"];
        var opensub = new OpenSubtitlesClient();
        ArrayList langs = opensub.GetSubLanguages();
        var langstore = new ListStore(typeof (string), typeof (string));
        langstore.AppendValues("All", "all");
        int defrow = 0;
        int i = 1;
        foreach (Hashtable lang in langs)
        {
            langstore.AppendValues(lang["LanguageName"].ToString(), (lang["SubLanguageID"] ?? string.Empty).ToString());
            if (lang.ContainsValue(deflang))
                defrow = i;
            i++;
        }
        languagebox.Model = langstore;
        TreeIter iter;
        languagebox.Model.IterNthChild(out iter, defrow);
        languagebox.SetActiveIter(iter);
    }


    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings.Remove("sublanguage");
        config.AppSettings.Settings.Add("sublanguage", GetCurrentLang());
        config.Save(ConfigurationSaveMode.Modified);

        Application.Quit();
        a.RetVal = true;
    }

    protected void SelectFile(object sender, EventArgs e)
    {
        _fname = FileChooserD.Filename;
        GetSubs();
    }

    private void GetSubs()
    {
        var store = new NodeStore(typeof (MovieTreeNode));
        statusbar1.Push(1, "Searching for filename.");
        var opensub = new OpenSubtitlesClient();
        _subtitles = opensub.FileSearch(_fname, GetCurrentLang());
        statusbar1.Push(2, "Found " + _subtitles.Count + " titles");
        if (_subtitles.Count > 0)
        {
            foreach (OpenSubtitlesClient.SearchResult sub in _subtitles)
            {
                var node = new MovieTreeNode
                               {
                                   Title = sub.MovieName,
                                   Year = sub.MovieYear,
                                   Season = sub.SeriesSeason,
                                   Episode = sub.SeriesEpisode,
                                   Uploader = sub.UserNickName,
                                   Downloads = sub.SubDownloadsCnt,
                                   DownloadLink = sub.SubDownloadLink,
                                   AuthorCommments = sub.SubAuthorComment,
                                   SubAddDate = sub.SubAddDate.ToShortDateString(),
                                   ReleaseName = sub.MovieReleaseName,
                                   IMDBRating = sub.MovieImdbRating,
                                   SubRating = sub.SubRating,
                                   Lang = sub.SubLanguageID,
                                   Language = sub.LanguageName,
                                   SubFormat = sub.SubFormat,
                                   SubHearingImpaired = sub.SubHearingImpaired == "1",
                                   IDMovieImdb = sub.IDMovieImdb
                               };
                store.AddNode(node);
            }
        }

        MovieNodeView.NodeStore = store;
        DetailNode.NodeStore = null;
        MovieNodeView.ShowAll();
        DetailNode.ShowAll();

        //TODO show details of first result
    }

    protected void DownloadSub(object sender, EventArgs e)
    {
        var tempdir = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine);
        if(string.IsNullOrEmpty(tempdir)) tempdir = "/tmp";
        
        var selectednode = (MovieTreeNode) MovieNodeView.NodeSelection.SelectedNode;
        if (selectednode == null)
            return;
        string movietitle = System.IO.Path.GetFileNameWithoutExtension(_fname);
        var client = new WebClient();
        string filename = tempdir+"/tmp_" + movietitle + ".gz";
        client.DownloadFile(selectednode.DownloadLink, filename);
        var fileToDecompress = new FileInfo(filename);
        string currentFileName = fileToDecompress.FullName;
        string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

        using (FileStream originalFileStream = fileToDecompress.OpenRead())
        {
            using (FileStream decompressedFileStream = File.Create(newFileName))
            {
                using (var decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                {
                    decompressionStream.CopyTo(decompressedFileStream);
                }
            }
        }
        fileToDecompress.Delete();
        string origlocation = System.IO.Path.GetDirectoryName(_fname);
        string newfile = origlocation + "/" + movietitle + "." + selectednode.SubFormat;
        if (File.Exists(newfile))
        {
            File.Delete(newfile);
        } //TODO: ask for overwrite?

        File.Move(newFileName, newfile);
        statusbar1.Push(3, "Downloaded " + newfile + ".");
    }

    protected void LanguageChanged(object sender, EventArgs e)
    {
        if (!String.IsNullOrEmpty(_fname))
            GetSubs();
    }

    private string GetCurrentLang()
    {
        TreeIter itar;
        if (languagebox.GetActiveIter(out itar))
            return (string) languagebox.Model.GetValue(itar, 1);
        return "all";
    }

    protected void OnSelectionChanged(object sender, EventArgs e)
    {
        var selectednode = (MovieTreeNode) MovieNodeView.NodeSelection.SelectedNode;
        if (selectednode == null)
            return;
        var store = new NodeStore(typeof (DetailTreeNode));
        store.AddNode(new DetailTreeNode("Added:", selectednode.SubAddDate));
        store.AddNode(new DetailTreeNode("Release:", selectednode.ReleaseName));
        store.AddNode(new DetailTreeNode("Comments:", selectednode.AuthorCommments));
        store.AddNode(new DetailTreeNode("Language:", selectednode.Language));
        store.AddNode(new DetailTreeNode("Rating:", selectednode.SubRating));
        store.AddNode(new DetailTreeNode("IMDB:", selectednode.IMDBRating));
        store.AddNode(new DetailTreeNode("Format:", selectednode.SubFormat));
        store.AddNode(new DetailTreeNode("HearingImpaired:", selectednode.SubHearingImpaired.ToString()));

        DetailNode.NodeStore = store;
        DetailNode.ShowAll();
    }

    public int ShowError(string message)
    {
        var md = new MessageDialog(this,
                                   DialogFlags.DestroyWithParent,
                                   MessageType.Error,
                                   ButtonsType.Close, message);

        int result = md.Run();
        md.Destroy();
        return result;
    }

    #region Nested type: DetailTreeNode

    [TreeNode(ListOnly = true)]
    public class DetailTreeNode : TreeNode
    {
        public DetailTreeNode(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [TreeNodeValue(Column = 0)]
        public string Name { get; set; }

        [TreeNodeValue(Column = 1)]
        public string Value { get; set; }
    }

    #endregion

    #region Nested type: MovieTreeNode

    [TreeNode(ListOnly = true)]
    public class MovieTreeNode : TreeNode
    {
        [TreeNodeValue(Column = 0)]
        public string Title { get; set; }

        [TreeNodeValue(Column = 1)]
        public string Year { get; set; }

        [TreeNodeValue(Column = 2)]
        public string Season { get; set; }

        [TreeNodeValue(Column = 3)]
        public string Episode { get; set; }

        [TreeNodeValue(Column = 4)]
        public string Uploader { get; set; }

        [TreeNodeValue(Column = 5)]
        public string Downloads { get; set; }

        public string SubRating { get; set; }

        public string IMDBRating { get; set; }
        public string IDMovieImdb { get; set; }

        public string Lang { get; set; }

        public string DownloadLink { get; set; }

        public string SubFormat { get; set; }

        public bool SubHearingImpaired { get; set; }

        public string AuthorCommments { get; set; }

        public string Language { get; set; }

        public string SubAddDate { get; set; }

        public string ReleaseName { get; set; }
    }

    #endregion
}