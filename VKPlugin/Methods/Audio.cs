using System;
using System.Xml;
using Plugin.ErrorHandler;

namespace Plugin.Methods
{
    public class GetAudio
    {
        private bool _disposed;

        /// <summary>
        ///     Set your Id.
        /// </summary>
        public string Id { private get; set; }

        /// <summary>
        ///     Set your Token.
        /// </summary>
        public string Token { private get; set; }

        public string[] AudioList()
        {
            // Parameters.
            const string method = "audio.get.xml?";
            string param = "owner_id=" + Id + "&count=" + "200";

            // Getting document.
            var doc = new XmlDocument();
            doc.Load("https://api.vk.com/method/" + method + param + "&access_token=" + Token);

            #region ErrorCheck
            XmlNode root = doc.DocumentElement;
            XmlNodeList nodeListError = root.SelectNodes("error_code");

            const string checkerror = "<error_code>5</error_code>";
            const string checkerror2 = "<error_code>7</error_code>";

            foreach (XmlNode node in nodeListError)
            {
                if (node.OuterXml.Equals(checkerror) || node.OuterXml.Equals(checkerror2))
                    Report.Audio.List();
                    return null;
            }
            #endregion ErrorCheck

            var arr3 = new string[0];

            #region Filtering
            foreach (XmlNode node in doc.SelectNodes("//audio"))
            {
                const string space = "#";
                string artist = node["artist"].InnerText;
                string title = node["title"].InnerText;
                string duration = node["duration"].InnerText;
                string url = node["url"].InnerText.Split('?')[0];

                if (artist.Contains("&amp;")) artist = artist.Replace("&amp;", "&");
                if (title.Contains("&amp;")) title = title.Replace("&amp;", "&");

                Array.Resize(ref arr3, arr3.Length + 1);
                arr3[arr3.Length - 1] = space + artist + space + title + space + duration + space + url;
            }
            #endregion Filtering

            return arr3;
        }

        // Don't work.
        private string AudioCount()
        {
            // Parameters.
            const string method = "audio.getCount.xml?";
            string param = "owner_id=" + Id;

            // Getting document.
            var doc = new XmlDocument();
            doc.Load("https://api.vk.com/method/" + method + param + "&access_token=" + Token);

            #region ErrorCheck

            XmlNode root = doc.DocumentElement;
            XmlNodeList nodeListError = root.SelectNodes("error_code");

            const string checkerror = "<error_code>5</error_code>";
            const string checkerror2 = "<error_code>7</error_code>";

            foreach (XmlNode node in nodeListError)
            {
                if (node.OuterXml.Equals(checkerror) || node.OuterXml.Equals(checkerror2))
                    Report.Audio.Count();
                    return null;
            }

            #endregion ErrorCheck

            string countstring = "0";

            //try
            //{
            countstring = root.InnerText;
            //}
            //catch
            //{
            //    MessageBox.Show("AudioCount Error");
            //}

            return countstring;
        }
    }
}