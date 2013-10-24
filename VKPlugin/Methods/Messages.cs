using System;
using System.Diagnostics;
using System.Xml;

namespace Rainmeter.Methods
{
    public class Messages
    {
        /// <summary>
        ///     Set your Token.
        /// </summary>
        public string Token { private get; set; }

        public int UnReadMessages()
        {
            // Параметры.
            const string method = "account.getCounters.xml?";
            const string param = "&filter=messages";

            XmlDocument doc = new XmlDocument();
            doc.Load("https://api.vk.com/method/" + method + param + "&access_token=" + Token);

            #region ErrorCheck

            XmlNode root = doc.DocumentElement;
            XmlNodeList nodeListError = root.SelectNodes("error_code");

            const string checkerror = "<error_code>5</error_code>";
            const string checkerror2 = "<error_code>7</error_code>";

            foreach (XmlNode node in nodeListError)
            {
                if (node.OuterXml.Equals(checkerror) || node.OuterXml.Equals(checkerror2)) 
                    return 0;
            }

            #endregion ErrorCheck

            string countstring = "0";

            try
            {
                countstring = root["messages"].InnerText;
            }
            catch
            {
                Debug.Write("Message Error");
            }

            return Convert.ToInt32(countstring);
        }
    }
}