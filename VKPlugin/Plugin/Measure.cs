using Rainmeter.API;
using Rainmeter.AudioPlayer;
using Rainmeter.Information;
using System;

namespace Rainmeter.Plugin
{
    internal class Measure
    {
        public static RainmeterAPI RM;
        private PlayerType _audioType;
        private FriendsType _friendsType;
        private Type _type;
        private int _userType = 1;

        /// <summary>
        /// Called when a measure is created (i.e. when Rainmeter is launched or when a skin is refreshed). Initialize your measure object here.
        /// </summary>
        internal Measure()
        {
        }

        private enum FriendsType
        {
            Name,
            Photo,
            Id,
            Status
        }

        private enum PlayerType
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

        private enum Type
        {
            Player,
            Friends,
            Messages
        }

        public static string FriendsCount { get; private set; }

        public static string Path { get; private set; }
        
        public static string SaveAudio { get; private set; }

        /// <summary>
        /// Called by Rainmeter when a !CommandMeasure bang is sent to the measure.
        /// </summary>
        /// <param name="args">String containing the arguments to parse.</param>
        internal static void ExecuteBang(string command)
        {
            Execute.Start(command);
        }

        internal string GetString()
        {
            switch (_type)
            {
                case Type.Friends:

                    #region Friends

                    switch (_friendsType)
                    {
                        case FriendsType.Name:
                            return Info.FriendsUserData(_userType)[0];

                        case FriendsType.Photo:
                            return Info.FriendsUserData(_userType)[2];

                        case FriendsType.Id:
                            return Info.FriendsUserData(_userType)[1];

                        case FriendsType.Status:
                            return Info.FriendsUserData(_userType)[3];
                    }

                    #endregion Friends

                    break;

                case Type.Player:

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
        ///  Called when the measure settings are to be read directly after Initialize.
        ///  If DynamicVariables=1 is set on the measure, Reload is called on every update cycle (usually once per second).
        ///  Read and store measure settings here. To set a default maximum value for the measure, assign to maxValue.
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        /// <param name="maxValue">Max Value</param>
        internal void Reload(RainmeterAPI rm, ref double maxValue)
        {
            RM = rm;

            Info.Update();

            if (Path == null)
            {
                string path = rm.ReadPath("PlayerType", "");
                if (!String.IsNullOrEmpty(path))
                {
                    Path = path.Replace("\\" + path.Split('\\')[7], "\\");
                }
            }

            string playertype = rm.ReadString("PlayerType", "");
            string friendtype = rm.ReadString("FriendType", "");
            string type = rm.ReadString("Type", "");
            switch (type.ToUpperInvariant())
            {
                case "PLAYER":
                    _type = Type.Player;

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
                            RainmeterAPI.Log
                                (RainmeterAPI.LogType.Error, "VKPlugin.dll PlayerType=" + playertype + " not valid");
                            break;
                    }

                    #endregion Player

                    break;

                case "FRIENDS":
                    _type = Type.Friends;
                    if (FriendsCount == null)
                        FriendsCount = rm.ReadString("FriendsCount", "1");
                    _userType = rm.ReadInt("UserType", 1);

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
                            RainmeterAPI.Log
                                (RainmeterAPI.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }

                    #endregion Friends

                    break;

                case "MESSAGES":
                    _type = Type.Messages;
                    break;

                default:
                    RainmeterAPI.Log
                        (RainmeterAPI.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
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
                case Type.Messages:
                    return (Info.MessagesUnReadCount >= 1) ? 1 : 0;

                case Type.Player:

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
    }
}