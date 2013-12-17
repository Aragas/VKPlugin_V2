using System.Collections.Generic;
using System.Xml;
using Rainmeter.ErrorHandler;

namespace Rainmeter.Methods
{
    public class Friends
    {
        /// <summary>
        ///     Set your Count.
        /// </summary>
        public int Count { private get; set; }

        /// <summary>
        ///     Set your Id.
        /// </summary>
        public string Id { private get; set; }

        /// <summary>
        ///     Set your Token.
        /// </summary>
        public string Token { private get; set; }

        public string[] OnlineString()
        {
            string Document = string.Join("", List().ToArray());

            XmlDocument doc0 = new XmlDocument();
            if (Document == null)
                return null;
            doc0.LoadXml(Document);

            List<string> Users = new List<string>();

            foreach (XmlNode node in doc0.SelectNodes("//user"))
            {
                Users.Add(node["uid"].InnerText);
                Users.Add(node["first_name"].InnerText);
                Users.Add(node["last_name"].InnerText);
                Users.Add(node["photo_50"].InnerText);
                Users.Add(node.SelectSingleNode("online_mobile") == null ? "Online" : "Mobile");
            }
            return Users.ToArray();
        }

        private List<string> List()
        {
            // Параметры конфигурации.
            const string method = "friends.get.xml?";
            string param = "uid=" + Id + "&order=hints" + "&fields=first_name,last_name,photo_50,online";

            //Получение документа.
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
                    Report.Friends.List();
                    return null;
            }

            #endregion ErrorCheck

            #region Filtering

            List<string> list = new List<string>();

            // Фильтрация документа по параметру.
            XmlNodeList nodeList = root.SelectNodes("/response/user[online='1']");

            list.Add("<main>");
            int x = 0;
            foreach (XmlNode node in nodeList)
            {
                list.Add(node.OuterXml);
                x = x + 1;
                if (x == Count) break;
            }
            list.Add("</main>");

            return list;

            #endregion Filtering
        }
    }
}