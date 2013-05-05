
// This file has been generated by the GUI designer. Do not modify.
public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.VBox vbox3;
	private global::Gtk.HBox hbox2;
	private global::Gtk.FileChooserButton FileChooserD;
	private global::Gtk.ComboBox languagebox;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.NodeView MovieNodeView;
	private global::Gtk.ScrolledWindow GtkScrolledWindow1;
	private global::Gtk.NodeView DetailNode;
	private global::Gtk.HBox hbox1;
	private global::Gtk.Button Downloadbutton;
	private global::Gtk.Statusbar statusbar1;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = "Subby";
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		this.BorderWidth = ((uint)(2));
		this.AllowShrink = true;
		this.DefaultWidth = 720;
		this.DefaultHeight = 480;
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.vbox3 = new global::Gtk.VBox ();
		this.vbox3.Name = "vbox3";
		this.vbox3.Spacing = 6;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox2 = new global::Gtk.HBox ();
		this.hbox2.Name = "hbox2";
		this.hbox2.Spacing = 6;
		// Container child hbox2.Gtk.Box+BoxChild
		this.FileChooserD = new global::Gtk.FileChooserButton ("Select a File", ((global::Gtk.FileChooserAction)(0)));
		this.FileChooserD.Name = "FileChooserD";
		this.FileChooserD.LocalOnly = false;
		this.hbox2.Add (this.FileChooserD);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.FileChooserD]));
		w1.Position = 0;
		// Container child hbox2.Gtk.Box+BoxChild
		this.languagebox = global::Gtk.ComboBox.NewText ();
		this.languagebox.Name = "languagebox";
		this.hbox2.Add (this.languagebox);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.languagebox]));
		w2.Position = 1;
		w2.Expand = false;
		w2.Fill = false;
		this.vbox3.Add (this.hbox2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox2]));
		w3.Position = 0;
		w3.Expand = false;
		w3.Fill = false;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.MovieNodeView = new global::Gtk.NodeView ();
		this.MovieNodeView.CanFocus = true;
		this.MovieNodeView.Name = "MovieNodeView";
		this.MovieNodeView.EnableSearch = false;
		this.MovieNodeView.SearchColumn = 0;
		this.GtkScrolledWindow.Add (this.MovieNodeView);
		this.vbox3.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow]));
		w5.Position = 1;
		// Container child vbox3.Gtk.Box+BoxChild
		this.GtkScrolledWindow1 = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow1.Name = "GtkScrolledWindow1";
		this.GtkScrolledWindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow1.Gtk.Container+ContainerChild
		this.DetailNode = new global::Gtk.NodeView ();
		this.DetailNode.CanFocus = true;
		this.DetailNode.Name = "DetailNode";
		this.DetailNode.EnableSearch = false;
		this.DetailNode.HeadersVisible = false;
		this.GtkScrolledWindow1.Add (this.DetailNode);
		this.vbox3.Add (this.GtkScrolledWindow1);
		global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.GtkScrolledWindow1]));
		w7.Position = 2;
		// Container child vbox3.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.Downloadbutton = new global::Gtk.Button ();
		this.Downloadbutton.CanFocus = true;
		this.Downloadbutton.Name = "Downloadbutton";
		this.Downloadbutton.UseUnderline = true;
		// Container child Downloadbutton.Gtk.Container+ContainerChild
		global::Gtk.Alignment w8 = new global::Gtk.Alignment (0.5F, 0.5F, 0F, 0F);
		// Container child GtkAlignment.Gtk.Container+ContainerChild
		global::Gtk.HBox w9 = new global::Gtk.HBox ();
		w9.Spacing = 2;
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Image w10 = new global::Gtk.Image ();
		w10.Pixbuf = global::Stetic.IconLoader.LoadIcon (this, "gtk-save", global::Gtk.IconSize.Button);
		w9.Add (w10);
		// Container child GtkHBox.Gtk.Container+ContainerChild
		global::Gtk.Label w12 = new global::Gtk.Label ();
		w12.LabelProp = "Download";
		w12.UseUnderline = true;
		w9.Add (w12);
		w8.Add (w9);
		this.Downloadbutton.Add (w8);
		this.hbox1.Add (this.Downloadbutton);
		global::Gtk.Box.BoxChild w16 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.Downloadbutton]));
		w16.Position = 1;
		w16.Fill = false;
		this.vbox3.Add (this.hbox1);
		global::Gtk.Box.BoxChild w17 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
		w17.PackType = ((global::Gtk.PackType)(1));
		w17.Position = 3;
		w17.Expand = false;
		w17.Fill = false;
		this.vbox1.Add (this.vbox3);
		global::Gtk.Box.BoxChild w18 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.vbox3]));
		w18.Position = 0;
		// Container child vbox1.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.vbox1.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w19 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.statusbar1]));
		w19.Position = 1;
		w19.Expand = false;
		w19.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
