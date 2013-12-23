using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Plugin.AudioPlayer;
using Plugin.Forms;
using Plugin.Information;
using System;
using System.Threading;

namespace Plugin
{
    internal class Measure
    {
        public static string FriendsCount { get; private set; }
        public static string Path { get; private set; }
        public static string SaveAudio { get; private set; }

        int _userCount = 1;
        Thread _playerTypeIsAlive;

        internal enum Type
        {
            PlayerType,
            FriendsType,
            MessagesType
        }
        Type _type;

        enum FriendsType
        {
            Name,
            Photo,
            Id,
            Status
        }
        FriendsType _friendsType;

        enum PlayerType
        {
            Settings,
            Artist,
            Title,
            Duration,
            Position,
            State,
            Repeat,
            Shuffle,
            Volume,
            Progress
        }
        PlayerType _audioType;

        /// <summary>
        /// Called when a measure is created (i.e. when Rainmeter is launched or when a skin is refreshed). 
        /// Initialize your measure object here.
        /// </summary>
        internal Measure()
        {
        }

        internal void Initialize(Rainmeter.API rm)
        {
            Info.Initialize();

            if (Path == null)
            {
                string path = rm.ReadPath("Type", "");
                if (!String.IsNullOrEmpty(path))
                    Path = path.Replace("\\" + path.Split('\\')[7], "\\");
            }

            string type = rm.ReadString("Type", "");

            switch (type.ToUpperInvariant())
            {
                case "PLAYER":
                    TypeIsAlive(rm, Type.PlayerType, _playerTypeIsAlive);
                    _type = Type.PlayerType;
                    string playertype = rm.ReadString("PlayerType", "");

                    #region Player
                    switch (playertype.ToUpperInvariant())
                    {
                        case "SETTINGS":
                            _audioType = PlayerType.Settings;
                            if (SaveAudio == null)
                                SaveAudio = rm.ReadString("SaveAudio", "FALSE").ToUpperInvariant();
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
                            Rainmeter.API.Log
                                (Rainmeter.API.LogType.Error, "VKPlugin.dll PlayerType=" + playertype + " not valid");
                            break;
                    }
                    #endregion Player

                    break;

                case "FRIENDS":
                    _type = Type.FriendsType;
                    _userCount = rm.ReadInt("UserType", 1);
                    if (FriendsCount == null)
                        FriendsCount = rm.ReadString("FriendsCount", "1");
                    string friendtype = rm.ReadString("FriendType", "");

                    #region Friends
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
                            Rainmeter.API.Log
                                (Rainmeter.API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }
                    #endregion Friends

                    break;

                case "MESSAGES":
                    _type = Type.MessagesType;
                    break;

                default:
                    Rainmeter.API.Log
                        (Rainmeter.API.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
                    break;
            }
        }

        /// <summary>
        ///  Called when the measure settings are to be read directly after Initialize.
        ///  If DynamicVariables=1 is set on the measure, Reload is called on every update cycle (usually once per second).
        ///  Read and store measure settings here. To set a default maximum value for the measure, assign to maxValue.
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        /// <param name="maxValue">Max Value</param>
        internal void Reload(Rainmeter.API rm, ref double maxValue)
        {
            string type = rm.ReadString("Type", "");

            switch (type.ToUpperInvariant())
            {
                case "FRIENDS":
                    _type = Type.FriendsType;
                    _userCount = rm.ReadInt("UserType", 1);
                    string friendtype = rm.ReadString("FriendType", "");

                    #region Friends
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
                            Rainmeter.API.Log
                                (Rainmeter.API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }
                    #endregion Friends

                    break;

                case "MESSAGES":
                    _type = Type.MessagesType;
                    break;


                case "PLAYER":
                    //TypeIsAlive(rm, Type.PlayerType, _playerTypeIsAlive);
                    _type = Type.PlayerType;
                    string playertype = rm.ReadString("PlayerType", "");

                    switch (playertype.ToUpperInvariant())
                    {
                        // For autoplay.
                        case "STATE":
                            _audioType = PlayerType.State;
                            break;
                    }

                    break;

                default:
                    Rainmeter.API.Log
                        (Rainmeter.API.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
                    break;
            }

            //#region Update
            //UpdateRate = rm.ReadInt("UpdateRate", 20);
            //if (UpdateRate <= 0)
            //{
            //    UpdateRate = 20;
            //}
            //#endregion
        }

        /// <summary>
        /// Called on every update cycle (usually once per second).
        /// </summary>
        /// <returns>Return the numerical value for the measure here.</returns>
        internal double Update()
        {
            switch (_type)
            {
                case Type.MessagesType:
                    //if (UpdateCounter(Type.MessagesType) == 0)
                    {
                        return (Info.MessagesUnReadCount >= 1) ? 1 : 0;
                    }
                    break;

                case Type.PlayerType:
                    //if (UpdateCounter(Type.PlayerType) == 0)
                    {
                        #region Player
                        switch (_audioType)
                        {
                            case PlayerType.Duration:
                                return Player.Duration;

                            case PlayerType.Position:
                                return Math.Round(Player.Position);

                            case PlayerType.State:
                                if (Player.Played) Player.PlayNext();
                                return Player.State;

                            case PlayerType.Repeat:
                                return Player.Repeat ? 0.0 : 1.0;

                            case PlayerType.Shuffle:
                                return Player.Shuffle ? 0.0 : 1.0;

                            case PlayerType.Progress:
                                return Player.Progress;
                        }
                        #endregion Player
                    }
                    break;
            }

            //#region Update
            //UpdateCounter++;
            //if (UpdateCounter >= UpdateRate)
            //{
            //    UpdateCounter = 0;
            //    if (UpdatedString) UpdatedString = false;
            //}
            //#endregion

            return 0.0;
        }

        internal string GetString()
        {
            switch (_type)
            {
                case Type.FriendsType:
                    //if (UpdatedString(Type.FriendsType))
                    {
                        #region Friends
                        switch (_friendsType)
                        {
                            case FriendsType.Name:
                                return Info.FriendsUserData(_userCount)[0];

                            case FriendsType.Photo:
                                return Info.FriendsUserData(_userCount)[2];

                            case FriendsType.Id:
                                return Info.FriendsUserData(_userCount)[1];

                            case FriendsType.Status:
                                return Info.FriendsUserData(_userCount)[3];
                        }
                        #endregion Friends
                    }
                    break;

                case Type.PlayerType:
                    //if (UpdatedString(Type.PlayerType))
                    {
                        #region Player
                        switch (_audioType)
                        {
                            case PlayerType.Settings:
                                return "VKPlayer by Aragas (Aragasas)";

                            case PlayerType.Artist:
                                return Player.Artist ?? "Not Authorized";

                            case PlayerType.Title:
                                return Player.Title ?? "Click Play";
                        }
                        #endregion Player
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Called by Rainmeter when a !CommandMeasure bang is sent to the measure.
        /// </summary>
        /// <param name="args">String containing the arguments to parse.</param>
        internal void ExecuteBang(string command)
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                Player.Execute(command);
            }
            else
            {
                Player.Execute(command);
            }
        }

        internal void Finalize()
        {
        }

        //int[] arr = new int[Enum.GetNames(typeof(Type)).Length];
        //bool UpdatedString(Type type) {}
        //int UpdateCounter(Type type) {}
        //int UpdateRate;

        internal void TypeIsAlive(Rainmeter.API rm, Type type, Thread thread)
        {
            if (thread == null || !thread.IsAlive)
            {
                thread = new Thread(delegate()
                {
                    try
                    {
                        while (rm.ReadString(type.ToString(), "") != "")
                        {
                            Thread.Sleep(2000);
                        }
                    }
                    catch
                    {
                        Player.Dispose();
                        Thread.CurrentThread.Abort();
                    }
                });
                thread.Start();
            }
        }
    }
}
