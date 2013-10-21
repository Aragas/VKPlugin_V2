using System;
using Rainmeter.Forms;
using Rainmeter.Methods;
using Rainmeter.Plugin;

namespace Rainmeter.Information
{
    public static class Info
    {
        private static int _friendsCount;
        private static int FriendsCount
        {
            get
            {
                if (_friendsCount == 0)
                    return GetFriendsCount();

                return _friendsCount;
            }
        }

        private static string _token;
        private static string Token 
        {
            get
            {
                if (_token == null)
                    return GetToken();
                return _token;
            }
        }

        private static string _id;
        private static string Id
        {
            get
            {
                if (_id == null)
                    return GetId();
                return _id;
            }
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

        private static string GetId()
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                return OAuth.Id;
            }

            return OAuth.Id;

        }

        private static int GetFriendsCount()
        {
            return Convert.ToInt32(Measure.FriendsCount);
        }

        private static readonly Friends Friends = new Friends();
        private static readonly Messages Messages = new Messages();

        #region UserData

        private static string[] _userArray;
        private static string[] UserArray
        {
            get
            {
                if (_userArray != null) 
                    return _userArray;

                Friends.Token = Token;
                Friends.Id = Id;
                Friends.Count = FriendsCount;

                _userArray = Friends.OnlineString();
                return _userArray;
            }
        }

        private static string[] _friendsUserData = new string[4];
        public static string[] FriendsUserData(int user)
        {
            if (user <= 0)
                user = 1;
            int i = (user*5)-5;
            _friendsUserData[0] = UserArray[i + 1] + " " + UserArray[i + 2];   // First Last Name.
            _friendsUserData[1] = UserArray[i + 0];                            // Friend Id.
            _friendsUserData[2] = UserArray[i + 3];                            // Photo Url.
            _friendsUserData[3] = UserArray[i + 4];                            // Online or Mobile. 
            return _friendsUserData;
        }

        #endregion

        #region Messages

        private static string _messagesUnReadCount;
        public static int MessagesUnReadCount
        {
            get
            {
                if (_messagesUnReadCount != null) return Convert.ToInt32(_messagesUnReadCount);

                Messages.Token = Token;
                Messages.Id = Id;
                _messagesUnReadCount = Convert.ToString(Messages.UnReadMessages());

                return Convert.ToInt32(_messagesUnReadCount);
            }
        }

        #endregion

        public static void Update()
        {
            _friendsUserData = null;
            _messagesUnReadCount = null;
        }
    }
}
