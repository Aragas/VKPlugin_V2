using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Plugin.AudioPlayer;
using Plugin.Forms;
using Plugin.Information;
using Rainmeter;

namespace Plugin
{
    /// <summary>
    /// Main part of Measure.
    /// </summary>
    internal partial class Measure
    {
        public static string FriendsCount { get; private set; }
        public static string Path { get; private set; }
        public static string SaveAudio { get; private set; }

        int _userCount = 1;

        internal enum MeasureType
        {
            PlayerType,
            FriendsType,
            MessagesType
        }
        MeasureType _type;

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
        /// Called when Rainmeter is launched. Just once.
        /// Is called before skin gets data.
        /// </summary>
        internal Measure()
        {
            if (!WasCreatedOnce.ContainsKey("Measure"))
            {
                // Put code here.


                WasCreatedOnce.Add("Measure", true);
            }
        }

        /// <summary>
        /// Called when a measure is created (i.e. when Rainmeter is launched or when a skin is refreshed). 
        /// Initialize your measure object here.
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        internal void Initialize(Rainmeter.API api)
        {
            #region Initialization
            if (!WasCreatedOnce.ContainsKey("Initialize"))
            {
                Info.Initialize();

                if (Updates.UpdateAvailable && Updates.DownloadUpdate)
                {
                    Process.Start(Updates.DownloadUrl);
                }

                UpdateRate = api.ReadInt("UpdateRate", 5);
                if (UpdateRate <= 0)
                {
                    UpdateRate = 5;
                }



                WasCreatedOnce.Add("Initialize", true);
            }

            if (Path == null)
            {
                string path = api.ReadPath("Type", "");
                if (!String.IsNullOrEmpty(path))
                    Path = path.Replace("\\" + path.Split('\\')[7], "\\");
            }
            #endregion Initialization

            string type = api.ReadString("Type", "");

            switch (type.ToUpperInvariant())
            {
                case "PLAYER":
                    _type = MeasureType.PlayerType;

                    TypeIsAlive(api, _type);

                    string playertype = api.ReadString("PlayerType", "");

                    #region Player
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

                    break;

                case "FRIENDS":
                    _type = MeasureType.FriendsType;

                    _userCount = api.ReadInt("UserType", 1);
                    if (FriendsCount == null)
                        FriendsCount = api.ReadString("FriendsCount", "1");

                    string friendtype = api.ReadString("FriendType", "");

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
                            API.Log
                                (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }
                    #endregion Friends

                    break;

                case "MESSAGES":
                    _type = MeasureType.MessagesType;

                    break;

                default:
                    API.Log
                        (API.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
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
        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            string type = api.ReadString("Type", "");

            switch (type.ToUpperInvariant())
            {
                case "FRIENDS":
                    _type = MeasureType.FriendsType;
                    _userCount = api.ReadInt("UserType", 1);
                    string friendtype = api.ReadString("FriendType", "");

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
                            API.Log
                                (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }
                    #endregion Friends

                    break;

                case "MESSAGES":
                    _type = MeasureType.MessagesType;
                    break;


                case "PLAYER":
                    _type = MeasureType.PlayerType;
                    string playertype = api.ReadString("PlayerType", "");

                    #region Player
                    switch (playertype.ToUpperInvariant())
                    {
                        // For autoplay.
                        case "STATE":
                            _audioType = PlayerType.State;
                            break;
                    }
                    #endregion Player

                    break;

                default:
                    API.Log
                        (API.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
                    break;
            }
        }

        /// <summary>
        /// Called on every update cycle (usually once per second).
        /// </summary>
        /// <returns>Return the numerical value for the measure here.</returns>
        internal double Update()
        {
            switch (_type)
            {
                case MeasureType.MessagesType:
                    return (Info.MessagesUnReadCount >= 1) ? 1 : 0;

                case MeasureType.PlayerType:

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

                    break;
            }

            return 0.0;
        }

        internal string GetString()
        {
            switch (_type)
            {
                case MeasureType.FriendsType:

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

                    break;

                case MeasureType.PlayerType:

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

                    break;
            }
            return null;
        }

        /// <summary>
        /// Called by Rainmeter when a !CommandMeasure bang is sent to the measure.
        /// </summary>
        /// <param name="command">String containing the arguments to parse.</param>
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

        /// <summary>
        /// Called when a measure is disposed (i.e. when Rainmeter is closed or when a skin is refreshed). 
        /// Dispose your measure object here.
        /// </summary>
        internal void Finalize()
        {
        }

        /// <summary>
        /// Called from TypeIsAlive(). (Using GetMethods())
        /// </summary>
        void PlayerTypeDispose()
        {
            Player.Dispose();
        }
    }

    /// <summary>
    /// Threading part of Measure.
    /// </summary>
    internal partial class Measure
    {
        static readonly Dictionary<MeasureType, Thread> ThreadAlive = new Dictionary<MeasureType, Thread>();

        /// <summary>
        /// Call this to monitor is your skin is alive. 
        /// If not, calls "yourtypename + Dispose" (i.e. SampleTypeDispose()) .
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        /// <param name="type">MeasureType</param>
        internal void TypeIsAlive(API api, MeasureType type)
        {
            if (!ThreadAlive.ContainsKey(type) ||
                !ThreadAlive[type].IsAlive ||
                ThreadAlive[type] == null)
            {
                Thread thread = new Thread(delegate()
                {
                    try
                    {
                        while (!String.IsNullOrEmpty(api.ReadString(type.ToString(), "")))
                        {
                            Thread.Sleep(2000);
                        }
                    }
                    catch
                    {
#if DEBUG
                        // Debug doesn't work well with GetMethod().
                        Player.Dispose();
#else
                        try
                        {
                            GetType()
                                .GetMethod(type + "Dispose", BindingFlags.Instance | BindingFlags.NonPublic)
                                .Invoke(this, null);
                        }
                        catch (NullReferenceException)
                        {
                            API.Log(API.LogType.Error, type + "Dispose() do not exist.");
                        }
#endif

                        Thread.CurrentThread.Abort();
                    }
                })
                {
                    IsBackground = true
                };

                ThreadAlive.Add(type, thread);
                thread.Start();
            }

        }
    }

    /// <summary>
    /// Use it if you need call/check anything just once
    /// </summary>
    internal partial class Measure
    {
        static readonly Dictionary<string, bool> WasCreatedOnce = new Dictionary<string, bool>();
    }

    /// <summary>
    /// Update part of Measure.
    /// </summary>
    internal partial class Measure
    {
        static readonly Dictionary<MeasureType, int> UpdatedDouble = new Dictionary<MeasureType, int>();
        static readonly Dictionary<MeasureType, int> UpdatedString = new Dictionary<MeasureType, int>();

        static int UpdateRate;

    }
}
