using System;
using System.Xml;
using Plugin.ErrorHandler;

namespace Plugin.Methods
{
    public class GetMessages
    {
        /// <summary>
        ///     Set your Token.
        /// </summary>
        public string Token { private get; set; }

        public int UnReadMessages()
        {
            // Parameters.
            const string method = "account.getCounters.xml?";
            const string param = "&filter=messages";

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
                    Report.Messages.Count();
                    return 0;
            }
            #endregion ErrorCheck

            string countstring = "0";

            if (!root.HasChildNodes)
                return Convert.ToInt32(countstring);

            XmlElement xmlElement = root["messages"];
            if (xmlElement != null)
                countstring = root["messages"].InnerText;

            return Convert.ToInt32(countstring);
        }
    }
}