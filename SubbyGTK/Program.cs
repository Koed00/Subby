using Gtk;

namespace SubbyGTK
{
    internal class MainClass
    {
        public static void Main(string[] args)
        {
            Application.Init();
            var win = new MainWindow();
            if (OpenSubtitlesClient.CheckForConnection())
            {
                win.PopulateLanguages();
                win.Show();
            }
            else
            {
                win.ShowError("Can't connect to Opensubtitles.org\nPlease check your internet connection");
                win.Destroy();
                Application.Quit();
                return;
            }
            Application.Run();
        }
    }
}