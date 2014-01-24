using System;
using System.Collections.Generic;
using System.Reflection;
using Plugin.AudioPlayer;
using Plugin.Information;
using Rainmeter;

namespace Plugin
{
    internal partial class MeasuresHandler
    {
        /// <summary>
        /// Basic measure.
        /// </summary>
        class Measure : IDisposable
        {
            public virtual void Init(Rainmeter.API api) { }
            public virtual void Reload(Rainmeter.API api) { }
            public virtual double Double() { return 0.0; }
            public virtual string String() { return null; }
            public virtual void Dispose() { }
        }

        class Player : Measure
        {
            private enum PlayerType { Settings, Artist, Title, Duration, Position, State, Repeat, Shuffle, Volume, Progress }
            private PlayerType _audioType;

            public override void Init(Rainmeter.API api)
            {
                var type = MeasureType.PlayerType;

                #region Path + LoadDll

                if (!MeasurePath.ContainsKey(type))
                {
                    string path = api.ReadPath("Type", "");
                    path = path.Replace("\\" + path.Split('\\')[7], "\\");
                    MeasurePath.Add(type, path);

                    // Load NAudio library.
                    AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
                    {
                        var pathc = string.Format(path + "{0}.dll", "NAudio");
                        return Assembly.LoadFrom(pathc);
                    };
                }

                #endregion Path + LoadDll

                #region Player

                string playertype = api.ReadString("PlayerType", "");
                switch (playertype.ToUpperInvariant())
                {
                    case "SETTINGS":
                        _audioType = PlayerType.Settings;

                        SaveAudio = api.ReadString("SaveAudio", "FALSE").ToUpperInvariant();
                        break;

                    case "STATE":
                        _audioType = PlayerType.State;
                        break;

                    case "ARTIST":
                        _audioType = PlayerType.Artist;
                        break;

                    case "TITLE":
                        _audioType = PlayerType.Title;
                        break;

                    case "DURATION":
                        _audioType = PlayerType.Duration;
                        break;

                    case "POSITION":
                        _audioType = PlayerType.Position;
                        break;

                    case "REPEAT":
                        _audioType = PlayerType.Repeat;
                        break;

                    case "SHUFFLE":
                        _audioType = PlayerType.Shuffle;
                        break;

                    case "VOLUME":
                        _audioType = PlayerType.Volume;
                        break;

                    case "PROGRESS":
                        _audioType = PlayerType.Progress;
                        break;

                    default:
                        API.Log
                            (API.LogType.Error, "VKPlugin.dll PlayerType=" + playertype + " not valid");
                        break;
                }

                #endregion Player

            }

            public override void Reload(Rainmeter.API api)
            {
                string playertype = api.ReadString("PlayerType", "");

                #region Player

                switch (playertype.ToUpper())
                {
                    case "ARTIST":
                        _audioType = PlayerType.Artist;
                        break;

                    case "TITLE":
                        _audioType = PlayerType.Title;
                        break;

                    case "STATE":
                        _audioType = PlayerType.State;
                        break;

                    case "DURATION":
                        _audioType = PlayerType.Duration;
                        break;

                    case "POSITION":
                        _audioType = PlayerType.Position;
                        break;

                    case "REPEAT":
                        _audioType = PlayerType.Repeat;
                        break;

                    case "SHUFFLE":
                        _audioType = PlayerType.Shuffle;
                        break;

                    case "VOLUME":
                        _audioType = PlayerType.Volume;
                        break;

                    case "PROGRESS":
                        _audioType = PlayerType.Progress;
                        break;
                }

                #endregion Player

            }

            public override double Double()
            {
                #region Player
                switch (_audioType)
                {
                    case PlayerType.Duration:
                        return VKPlayer.Duration;

                    case PlayerType.Position:
                        return Math.Round(VKPlayer.Position);

                    case PlayerType.State:
                        return VKPlayer.State;

                    case PlayerType.Repeat:
                        return VKPlayer.Repeat ? 0.0 : 1.0;

                    case PlayerType.Shuffle:
                        return VKPlayer.Shuffle ? 0.0 : 1.0;

                    case PlayerType.Progress:
                        return VKPlayer.Progress;
                }
                #endregion Player

                return base.Double();
            }

            public override string String()
            {
                #region Player
                switch (_audioType)
                {
                    case PlayerType.Settings:
                        return "VKPlayer by Aragas (Aragasas)";

                    case PlayerType.Artist:
                        return VKPlayer.Artist ?? "Not Authorized";

                    case PlayerType.Title:
                        return VKPlayer.Title ?? "Click Play";
                }

                #endregion Player

                return base.String();
            }

            public override void Dispose()
            {
                VKPlayer.Dispose();
            }
        }

        class Friends : Measure
        {
            private enum FriendsType { Name, Photo, Id, Status }
            private FriendsType _friendsType;

            private int _userCount = 1;

            public override void Init(API api)
            {
                var type = MeasureType.FriendsType;
                _userCount = api.ReadInt("UserType", 1);

                #region Path

                if (!MeasurePath.ContainsKey(type))
                {
                    string path = api.ReadPath("Type", "");
                    path = path.Replace("\\" + path.Split('\\')[7], "\\");
                    MeasurePath.Add(type, path);
                }

                #endregion Path

                #region Update

                if (!TwoUpdateRate.ContainsKey(type))
                    TwoUpdateRate.Add(type, new Dictionary<int, int>());
                if (!TwoUpdateRate[type].ContainsKey(_userCount))
                    TwoUpdateRate[type].Add(_userCount, (int)api.ReadDouble("UpdateRate", DefaultUpdateRate));

                if (!TwoUpdateCounter.ContainsKey(type))
                    TwoUpdateCounter.Add(type, new Dictionary<int, int>());
                if (!TwoUpdateCounter[type].ContainsKey(_userCount))
                    TwoUpdateCounter[type].Add(_userCount, 20);

                #endregion Update

                #region Friends

                string friendtype = api.ReadString("FriendType", "");
                switch (friendtype.ToUpperInvariant())
                {
                    case "NAME":
                        _friendsType = FriendsType.Name;
                        break;

                    case "PHOTO":
                        _friendsType = FriendsType.Photo;
                        break;

                    case "ID":
                        _friendsType = FriendsType.Id;
                        break;

                    case "STATUS":
                        _friendsType = FriendsType.Status;
                        break;

                    default:
                        API.Log
                            (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                        break;
                }

                #endregion Friends
            }

            public override void Reload(API api)
            {
                var type = MeasureType.FriendsType;
                _userCount = api.ReadInt("UserType", 1);

                #region Update

                TwoUpdateCounter[type][_userCount]++;

                if (TwoUpdateRate[type][_userCount] < TwoUpdateCounter[type][_userCount])
                {
                    Info.UpdateFriends();
                    string friendtype = api.ReadString("FriendType", "");
                    switch (friendtype.ToUpperInvariant())
                    {
                        case "NAME":
                            _friendsType = FriendsType.Name;
                            break;
                        case "PHOTO":
                            _friendsType = FriendsType.Photo;
                            break;
                        case "ID":
                            _friendsType = FriendsType.Id;
                            break;
                        case "STATUS":
                            _friendsType = FriendsType.Status;
                            break;

                        default:
                            API.Log
                                (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }
                    TwoUpdateCounter[type][_userCount] = 0;
                }

                #endregion Update
            }

            public override string String()
            {
                #region Friends
                switch (_friendsType)
                {
                    case FriendsType.Name:
                        return Info.FriendsUserData(_userCount)[0];

                    case FriendsType.Id:
                        return Info.FriendsUserData(_userCount)[1];

                    case FriendsType.Photo:
                        return Info.FriendsUserData(_userCount)[2];

                    case FriendsType.Status:
                        return Info.FriendsUserData(_userCount)[3];
                }
                #endregion Friends

                return base.String();
            }
        }

        class Messages : Measure
        {
            public override void Init(API api)
            {
                var type = MeasureType.MessagesType;

                #region Update

                if (!OneUpdateRate.ContainsKey(type))
                    OneUpdateRate.Add(type, (int)api.ReadDouble("UpdateRate", DefaultUpdateRate));

                if (!OneUpdateCounter.ContainsKey(type))
                    OneUpdateCounter.Add(type, 20);

                #endregion Update
            }

            public override void Reload(API api)
            {
                var type = MeasureType.MessagesType;
                #region Update

                OneUpdateCounter[type]++;

                if (OneUpdateRate[type] < OneUpdateCounter[type])
                {
                    Info.UpdateMessages();
                    OneUpdateCounter[type] = 0;
                }

                #endregion Update
            }

            public override double Double()
            {
                return Info.MessagesUnReadCount >= 1 ? 1 : base.Double();
            }
        }
    }
    
}
