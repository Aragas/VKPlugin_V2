using Rainmeter.Forms;
using Rainmeter.Methods;
using Rainmeter.Plugin;
using System;

namespace Rainmeter.Information
{
    public static class Info
    {
        #region Internal

        private static readonly Friends Friends = new Friends
        {
            Token = Token,
            Id = Id,
            Count = FriendsCount
        };

        private static readonly Messages Messages = new Messages
        {
            Token = Token
        };

        private static int _friendsCount;
        private static string _id;

        private static string _token;

        private static int FriendsCount
        {
            get
            {
                if (_friendsCount == 0)
                    return GetFriendsCount();

                return _friendsCount;
            }
        }

        private static string Id
        {
            get
            {
                if (_id == null)
                    return GetId();
                return _id;
            }
        }

        private static string Token
        {
            get
            {
                if (_token == null)
                    return GetToken();
                return _token;
            }
        }

        private static int GetFriendsCount()
        {
            return Convert.ToInt32(Measure.FriendsCount);
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
                _userArray = Friends.OnlineString();
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
                _messagesUnReadCount = Convert.ToString(Messages.UnReadMessages());
                return Convert.ToInt32(_messagesUnReadCount);
            }
        }

        #endregion Messages

        public static void Update()
        {
            _userArray = null;
            _messagesUnReadCount = null;
        }
    }
}