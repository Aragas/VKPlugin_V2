using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
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
        public static string SaveAudio { get; private set; }
        public static Dictionary<MeasureType, string> MeasurePath = new Dictionary<MeasureType, string>();
        private static bool _created;

        private const int DefaultUpdateRate = 20;
        private int _userCount = 1;

        internal enum MeasureType { PlayerType, FriendsType, MessagesType }
        private MeasureType _type;

        private enum FriendsType { Name, Photo, Id, Status }
        private FriendsType _friendsType;

        private enum PlayerType { Settings, Artist, Title, Duration, Position, State, Repeat, Shuffle, Volume, Progress }
        private PlayerType _audioType;

        /// <summary>
        /// Called when a measure is created (i.e. when Rainmeter is launched or when a skin is refreshed).
        /// Initialize your measure object here.
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        internal Measure(Rainmeter.API api)
        {
            string type = api.ReadString("Type", "");
            switch (type.ToUpperInvariant())
            {
                case "LIST":
                    if (FriendsCount == null)
                        FriendsCount = api.ReadString("FriendsCount", "1");
                    break;

                case "PLAYER":
                    _type = MeasureType.PlayerType;

                    // Start TypeIsAlive() monitor.
                    TypeIsAlive(api, _type);

                    #region Path + LoadDll

                    if (!MeasurePath.ContainsKey(_type))
                    {
                        string path = api.ReadPath("Type", "");
                        path = path.Replace("\\" + path.Split('\\')[7], "\\");
                        MeasurePath.Add(_type, path);

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

                    break;

                case "FRIENDS":
                    _type = MeasureType.FriendsType;
                    _userCount = api.ReadInt("UserType", 1);

                    #region Path

                    if (!MeasurePath.ContainsKey(_type))
                    {
                        string path = api.ReadPath("Type", "");
                        path = path.Replace("\\" + path.Split('\\')[7], "\\");
                        MeasurePath.Add(_type, path);
                    }

                    #endregion Path

                    #region Update

                    if (!TwoUpdateRate.ContainsKey(_type))
                        TwoUpdateRate.Add(_type, new Dictionary<int, int>());
                    if (!TwoUpdateRate[_type].ContainsKey(_userCount))
                        TwoUpdateRate[_type].Add(_userCount, (int)api.ReadDouble("UpdateRate", DefaultUpdateRate));

                    if (!TwoUpdateCounter.ContainsKey(_type))
                        TwoUpdateCounter.Add(_type, new Dictionary<int, int>());
                    if (!TwoUpdateCounter[_type].ContainsKey(_userCount))
                        TwoUpdateCounter[_type].Add(_userCount, 20);

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

                    break;

                case "MESSAGES":
                    _type = MeasureType.MessagesType;

                    #region Update

                    if (!OneUpdateRate.ContainsKey(_type))
                        OneUpdateRate.Add(_type, (int)api.ReadDouble("UpdateRate", DefaultUpdateRate));

                    if (!OneUpdateCounter.ContainsKey(_type))
                        OneUpdateCounter.Add(_type, 20);

                    #endregion Update

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

                    #region Update

                    TwoUpdateCounter[_type][_userCount]++;

                    if (TwoUpdateRate[_type][_userCount] < TwoUpdateCounter[_type][_userCount])
                    {
                        Info.UpdateFriends();
                        string friendtype = api.ReadString("FriendType", "");
                        switch (friendtype.ToUpperInvariant())
                        {
                            case "NAME": _friendsType = FriendsType.Name; break;
                            case "PHOTO": _friendsType = FriendsType.Photo; break;
                            case "ID": _friendsType = FriendsType.Id; break;
                            case "STATUS": _friendsType = FriendsType.Status; break;

                            default:
                                API.Log
                                    (API.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                                break;
                        }
                        TwoUpdateCounter[_type][_userCount] = 0;
                    }

                    #endregion Update

                    break;

                case "MESSAGES":
                    _type = MeasureType.MessagesType;

                    #region Update

                    OneUpdateCounter[_type]++;

                    if (OneUpdateRate[_type] < OneUpdateCounter[_type])
                    {
                        Info.UpdateMessages();
                        OneUpdateCounter[_type] = 0;
                    }

                    #endregion Update

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
        internal double GetDouble()
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
                            //if (Player.Played) Player.PlayNext();
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
        internal void ExecuteBang(string args)
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                Player.Execute(args);
            }
            else
            {
                Player.Execute(args);
            }
        }

        /// <summary>
        /// Called when a measure is disposed (i.e. when Rainmeter is closed or when a skin is refreshed).
        /// Dispose your measure object here.
        /// </summary>
        ~Measure()
        {
        }
        
        /// <summary>
        /// Called from TypeIsAlive(). (Using GetMethods())
        /// </summary>
        private void PlayerTypeDispose()
        {
            Player.Dispose();
        }
    }

    /// <summary>
    /// Threading part of Measure.
    /// </summary>
    internal partial class Measure
    {
        private static readonly Dictionary<MeasureType, Thread> ThreadAlive = new Dictionary<MeasureType, Thread>();

        /// <summary>
        /// Call this to monitor is your skin is alive.
        /// If not, calls "yourtypename + Dispose" (i.e. SampleTypeDispose()) .
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        /// <param name="type">MeasureType</param>
        internal void TypeIsAlive(API api, MeasureType type)
        {
            if (ThreadAlive.ContainsKey(type))
                return;
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
                    Name = "ThreadMonitor" + type,
                    IsBackground = true
                };

                ThreadAlive.Add(type, thread);
                ThreadAlive[type].Start();
            }
        }
    }

    /// <summary>
    /// Update part of Measure.
    /// </summary>
    internal partial class Measure
    {
        private static Dictionary<MeasureType, int> OneUpdateRate = new Dictionary<MeasureType, int>();
        private static Dictionary<MeasureType, int> OneUpdateCounter = new Dictionary<MeasureType, int>();

        private static TwoKeyDictionary<MeasureType, int, int> TwoUpdateRate =
            new TwoKeyDictionary<MeasureType, int, int>();

        private static TwoKeyDictionary<MeasureType, int, int> TwoUpdateCounter =
            new TwoKeyDictionary<MeasureType, int, int>();

        private static ThreeKeyDictionary<MeasureType, int, int, int> ThreeUpdateRate =
            new ThreeKeyDictionary<MeasureType, int, int, int>();

        private static ThreeKeyDictionary<MeasureType, int, int, int> ThreeUpdateCounter =
            new ThreeKeyDictionary<MeasureType, int, int, int>();

        public class TwoKeyDictionary<TKey1, TKey2, TValue> :
            Dictionary<TKey1, Dictionary<TKey2, TValue>>
        {
        }

        public class ThreeKeyDictionary<TKey1, TKey2, TKey3, TValue> :
            Dictionary<TKey1, Dictionary<TKey2, Dictionary<TKey3, TValue>>>
        {
        }
    }
}
