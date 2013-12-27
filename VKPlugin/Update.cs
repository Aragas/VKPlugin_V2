using System;
using System.Net;
using System.Windows.Forms;

namespace Plugin
{
    public static class Updates
    {
        public static bool UpdateAvailable()
        {
            return string.Equals(DllVersion, UpdateVersion);
        }

        public static bool DownloadUpdate()
        {
            DialogResult dialogResult = MessageBox.Show("New update available" + Environment.NewLine + " Download?",
                "VKPlugin", MessageBoxButtons.YesNo);

            return dialogResult == DialogResult.Yes;

        }

        public static string DownloadUrl()
        {
            return "https://github.com/Aragas/VKPlugin_V2/raw/NM/Update/" + UpdateText;
        }

        static readonly WebClient Wc = new WebClient();

        const string UpdateFileUrl = "https://raw.github.com/Aragas/VKPlugin_V2/NM/Update/Info.txt";

        static readonly string DllVersion = typeof(Measure).Assembly.GetName().Version.ToString();

        static readonly string UpdateText = Wc.DownloadString(UpdateFileUrl);

        static readonly string UpdateVersion = UpdateText.Split('_')[1].Remove(5) + ".0";

    }
}
