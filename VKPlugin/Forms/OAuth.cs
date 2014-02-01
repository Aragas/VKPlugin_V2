﻿using System;
using System.Windows.Forms;

namespace Plugin.Forms
{
    /// <summary>
    ///     Get Token and Id.
    /// </summary>
    public partial class OAuth : Form
    {
        private static string _id;
        private static string _token;

        /// <summary>
        ///     Get your Id
        /// </summary>
        public static string Id 
        {
            get
            {
                if (_id == null)
                    OAuthRun();
                return _id;
            } 
        }

        /// <summary>
        ///     Get your Token
        /// </summary>
        public static string Token
        {
            get
            {
                if (_token == null)
                    OAuthRun();
                return _token;
            } 
        }

        private OAuth()
        {
            InitializeComponent();
            webBrowser1.Navigate(Url);
        }

        public static bool TokenIdExist
        {
            get { return (_token != null || _token != null); }
        }

        private static string Url
        {
            get
            {
                return "https://oauth.vk.com/authorize?client_id=3328403"
                       + "&redirect_uri=https://oauth.vk.com/blank.html"
                       + "&scope=friends,audio&display=popup&response_type=token"
#if DEBUG
 + "&revoke=1";
#else
 + "&revoke=0";
#endif
            }
        }

        /// <summary>
        ///     Run Form.
        /// </summary>
        public static void OAuthRun()
        {
            Application.Run(new OAuth());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveData()
        {
            string data = webBrowser1.Url.ToString().Split('#')[1];

            _token = data.Split('&')[0].Split('=')[1];
            _id = data.Split('=')[3];

            Close();
        }

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            if (webBrowser1.Url.ToString() == Url)
            {
                WindowState = FormWindowState.Normal;
            }
            else SaveData();
        }
    }
}