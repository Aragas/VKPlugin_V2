using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plugin.Forms;
using Plugin.Information;
using Plugin.Methods;
using Rainmeter;

namespace Plugin
{
    public class Friends : Measure
    {
        private enum FriendsType
        {
            Id,
            Name,
            Photo,
            Status
        }
        private FriendsType _friendsType;

        private int _count;
        private int _rate;

        private string ID;
        private string Name;
        private string Photo;
        private string Status;

        public override void Init(API api)
        {
            _rate = api.ReadInt("UpdateRate", 20);

            int friendsCount = api.ReadInt("FriendsCount", 5);

            _getFriends = new GetFriends
            {
                Token = OAuth.Token,
                Id = OAuth.Id,
                Count = friendsCount
            };

            #region Friends

            int userCount = api.ReadInt("UserType", 1);
            string friendtype = api.ReadString("FriendType", "").ToUpperInvariant();
            switch (friendtype)
            {
                case "ID":
                    ID = FriendsUserData(userCount)[0];
                    break;

                case "NAME":
                    Name = FriendsUserData(userCount)[1];
                    break;

                case "PHOTO":
                    Photo = FriendsUserData(userCount)[2];
                    break;

                case "STATUS":
                    Status = FriendsUserData(userCount)[3];
                    break;

                default:
                    API.Log
                        (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                    break;
            }

            #endregion Friends
        }

        public override void Reload(API api, ref double maxValue)
        {
            if (_count >= _rate)
            {
                UpdateFriends();
                _count = 0;
            }
            _count++;

            #region Friends

            int userCount = api.ReadInt("UserType", 1);
            string friendtype = api.ReadString("FriendType", "").ToUpperInvariant();
            switch (friendtype)
            {
                case "ID":
                    ID = FriendsUserData(userCount)[0];
                    break;

                case "NAME":
                    Name = FriendsUserData(userCount)[1];
                    break;

                case "PHOTO":
                    Photo = FriendsUserData(userCount)[2];
                    break;

                case "STATUS":
                    Status = FriendsUserData(userCount)[3];
                    break;

                default:
                    API.Log
                        (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                    break;
            }

            #endregion Friends
        }

        public override string GetString()
        {
            if (_getFriends != null)
            {
                switch (_friendsType)
                {
                    case FriendsType.Id:
                        return ID;

                    case FriendsType.Name:
                        return Name;

                    case FriendsType.Photo:
                        return Photo;

                    case FriendsType.Status:
                        return Status;
                }
            }

            return base.GetString();
        }

        private GetFriends _getFriends;


        private readonly string[] _friendsUserData = new string[4];
        private string[] _userArray;

        private string[] UserArray
        {
            get
            {
                if (_userArray != null)
                    return _userArray;

                _userArray = _getFriends.OnlineString();
                return _userArray;
            }
        }

        private string[] FriendsUserData1(int user)
        {
            if (user <= 0)
                user = 1;

            int i = (user * 5) - 5;

            _friendsUserData[0] = "1";                            // Friend Id.
            _friendsUserData[1] = "2";   // First Last Name.
            _friendsUserData[2] = "3";                            // Photo Url.
            _friendsUserData[3] = "4";                            // Online or Mobile.

            return _friendsUserData;
        }

        private string[] FriendsUserData(int user)
        {
            if (user <= 0)
                user = 1;

            int i = (user * 5) - 5;

            _friendsUserData[0] = UserArray[i + 0];                            // Friend Id.
            _friendsUserData[1] = UserArray[i + 1] + " " + UserArray[i + 2];   // First Last Name.
            _friendsUserData[2] = UserArray[i + 3];                            // Photo Url.
            _friendsUserData[3] = UserArray[i + 4];                            // Online or Mobile.

            return _friendsUserData;
        }

        private void UpdateFriends()
        {
            _userArray = null;
        }
    }

    public class Messages : Measure
    {
        private int _count;
        private int _rate;

        public override void Init(API api)
        {
            _rate = api.ReadInt("UpdateRate", 20);
        }

        public override void Reload(API api, ref double maxValue)
        {
            if (_count > _rate)
            {
                Info.UpdateMessages();
                _count = 0;
            }
            _count++;
        }

        public override double Update()
        {
            return Info.MessagesUnReadCount >= 1 ? 1 : base.Update();
        }
    }
}
