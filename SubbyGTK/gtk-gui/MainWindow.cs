
// This file has been generated by the GUI designer. Do not modify.
public partial class MainWindow
{
	private global::Gtk.VBox vbox1;
	private global::Gtk.HBox hbox1;
	private global::Gtk.ComboBox languagebox;
	private global::Gtk.Button FileButton;
	private global::Gtk.Entry entry2;
	private global::Gtk.ScrolledWindow GtkScrolledWindow;
	private global::Gtk.NodeView MovieNodeView;
	private global::Gtk.ScrolledWindow scrolledwindow1;
	private global::Gtk.NodeView DetailNode;
	private global::Gtk.HBox hbox3;
	private global::Gtk.Statusbar statusbar1;
	private global::Gtk.Button Downloadbutton;

	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = "Subby";
		this.WindowPosition = ((global::Gtk.WindowPosition)(1));
		this.AllowShrink = true;
		this.DefaultWidth = 720;
		this.DefaultHeight = 480;
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.vbox1 = new global::Gtk.VBox ();
		this.vbox1.Name = "vbox1";
		this.vbox1.Spacing = 6;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox1 = new global::Gtk.HBox ();
		this.hbox1.Name = "hbox1";
		this.hbox1.Spacing = 6;
		// Container child hbox1.Gtk.Box+BoxChild
		this.languagebox = global::Gtk.ComboBox.NewText ();
		this.languagebox.Name = "languagebox";
		this.hbox1.Add (this.languagebox);
		global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.languagebox]));
		w1.Position = 0;
		w1.Expand = false;
		w1.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.FileButton = new global::Gtk.Button ();
		this.FileButton.CanFocus = true;
		this.FileButton.Name = "FileButton";
		this.FileButton.UseUnderline = true;
		this.FileButton.Label = "Choose File";
		this.hbox1.Add (this.FileButton);
		global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.FileButton]));
		w2.Position = 1;
		w2.Expand = false;
		w2.Fill = false;
		// Container child hbox1.Gtk.Box+BoxChild
		this.entry2 = new global::Gtk.Entry ();
		this.entry2.CanFocus = true;
		this.entry2.Name = "entry2";
		this.entry2.IsEditable = true;
		this.entry2.InvisibleChar = '●';
		this.hbox1.Add (this.entry2);
		global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.entry2]));
		w3.Position = 2;
		this.vbox1.Add (this.hbox1);
		global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox1]));
		w4.Position = 0;
		w4.Expand = false;
		w4.Fill = false;
		// Container child vbox1.Gtk.Box+BoxChild
		this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
		this.GtkScrolledWindow.Name = "GtkScrolledWindow";
		this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
		this.MovieNodeView = new global::Gtk.NodeView ();
		this.MovieNodeView.CanFocus = true;
		this.MovieNodeView.Name = "MovieNodeView";
		this.GtkScrolledWindow.Add (this.MovieNodeView);
		this.vbox1.Add (this.GtkScrolledWindow);
		global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.GtkScrolledWindow]));
		w6.Position = 1;
		// Container child vbox1.Gtk.Box+BoxChild
		this.scrolledwindow1 = new global::Gtk.ScrolledWindow ();
		this.scrolledwindow1.CanFocus = true;
		this.scrolledwindow1.Name = "scrolledwindow1";
		this.scrolledwindow1.ShadowType = ((global::Gtk.ShadowType)(1));
		// Container child scrolledwindow1.Gtk.Container+ContainerChild
		this.DetailNode = new global::Gtk.NodeView ();
		this.DetailNode.Name = "DetailNode";
		this.DetailNode.EnableSearch = false;
		this.DetailNode.HeadersVisible = false;
		this.scrolledwindow1.Add (this.DetailNode);
		this.vbox1.Add (this.scrolledwindow1);
		global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.scrolledwindow1]));
		w8.Position = 2;
		// Container child vbox1.Gtk.Box+BoxChild
		this.hbox3 = new global::Gtk.HBox ();
		this.hbox3.Name = "hbox3";
		this.hbox3.Spacing = 6;
		// Container child hbox3.Gtk.Box+BoxChild
		this.statusbar1 = new global::Gtk.Statusbar ();
		this.statusbar1.Name = "statusbar1";
		this.statusbar1.Spacing = 6;
		this.hbox3.Add (this.statusbar1);
		global::Gtk.Box.BoxChild w9 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.statusbar1]));
		w9.Position = 0;
		// Container child hbox3.Gtk.Box+BoxChild
		this.Downloadbutton = new global::Gtk.Button ();
		this.Downloadbutton.CanFocus = true;
		this.Downloadbutton.Name = "Downloadbutton";
		this.Downloadbutton.UseUnderline = true;
		this.Downloadbutton.Label = "Download";
		this.hbox3.Add (this.Downloadbutton);
		global::Gtk.Box.BoxChild w10 = ((global::Gtk.Box.BoxChild)(this.hbox3 [this.Downloadbutton]));
		w10.Position = 1;
		w10.Expand = false;
		w10.Fill = false;
		this.vbox1.Add (this.hbox3);
		global::Gtk.Box.BoxChild w11 = ((global::Gtk.Box.BoxChild)(this.vbox1 [this.hbox3]));
		w11.Position = 3;
		w11.Expand = false;
		w11.Fill = false;
		this.Add (this.vbox1);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
	}
}
