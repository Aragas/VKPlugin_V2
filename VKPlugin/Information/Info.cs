using Plugin.Forms;
using Plugin.Methods;
using System;

namespace Plugin.Information
{
    /// <summary>
    /// Static class that store all needed friends and messages information.
    /// </summary>
    public static class Info
    {
        #region Internal

        private static readonly GetFriends GetFriends = new GetFriends
        {
            Token = Token,
            Id = Id,
        };

        private static readonly GetMessages GetMessages = new GetMessages
        {
            Token = Token
        };

        private static int _friendsCount;
        private static string _id;

        private static string _token;

        
        private static string Id
        {
            get
            {
                return _id ?? GetId();
            }
        }

        private static string Token
        {
            get
            {
                return _token ?? GetToken();
            }
        }

        private static string GetId()
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                return OAuth.Id;
            }

            return OAuth.Id;
        }

        private static string GetToken()
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                return OAuth.Token;
            }

            return OAuth.Token;
        }

        #endregion Internal

        #region UserData

        private static string[] _friendsUserData = new string[4];
        private static string[] _userArray;

        private static string[] UserArray
        {
            get
            {
                if (_userArray != null)
                    return _userArray;
                _userArray = GetFriends.OnlineString();
                return _userArray;
            }
        }

        /// <summary>
        /// Get info about a user.
        /// </summary>
        /// <param name="user">Number of user.</param>
        /// <returns></returns>
        public static string[] FriendsUserData(int user)
        {
            if (user <= 0)
                user = 1;
            int i = (user * 5) - 5;
            _friendsUserData[0] = UserArray[i + 1] + " " + UserArray[i + 2];   // First Last Name.
            _friendsUserData[1] = UserArray[i + 0];                            // Friend Id.
            _friendsUserData[2] = UserArray[i + 3];                            // Photo Url.
            _friendsUserData[3] = UserArray[i + 4];                            // Online or Mobile.
            return _friendsUserData;
        }

        #endregion UserData

        #region Messages

        private static string _messagesUnReadCount;

        /// <summary>
        /// Get nuber of your unread messages.
        /// </summary>
        public static int MessagesUnReadCount
        {
            get
            {
                if (_messagesUnReadCount != null)
                    return Convert.ToInt32(_messagesUnReadCount);
                _messagesUnReadCount = Convert.ToString(GetMessages.UnReadMessages());
                return Convert.ToInt32(_messagesUnReadCount);
            }
        }

        #endregion Messages

        public static void UpdateFriends()
        {
            _userArray = null;
        }

        public static void UpdateMessages()
        {
            _messagesUnReadCount = null;
        }
    }
}