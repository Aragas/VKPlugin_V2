using Rainmeter.API;
using Rainmeter.AudioPlayer;
using Rainmeter.Information;
using System;

namespace Rainmeter.Plugin
{
    internal class Measure
    {
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

        private enum PlayerType
        {
            Status,
            Artist,
            Title,
            NextArtist,
            NextTitle,
            Duration,
            Position,
            State,
            Repeat,
            Shuffle,
            Volume,
            Progress
        }

        private enum FriendsType
        {
            Name,
            Photo,
            Id,
            Status
        }

        private enum Type
        {
            Player,
            Friends,
            Messages
        }

        public static string FriendsCount { get; private set; }

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
                        case PlayerType.Status:
                            return "VKPlayer by Aragas (Aragasas)";

                        case PlayerType.Artist:
                            return Player.Artist ?? "Not Authorized";

                        case PlayerType.Title:
                            return Player.Title ?? "Click Play";

                        case PlayerType.NextArtist:
                            return Player.NextArtist ?? "Not Authorized";

                        case PlayerType.NextTitle:
                            return Player.NextTitle ?? "Click Play";
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
            Info.Update();

            string playertype = rm.ReadString("PlayerType", "");
            string friendtype = rm.ReadString("FriendType", "");
            string type = rm.ReadString("Type", "");
            switch (type.ToLowerInvariant())
            {
                case "player":
                    _type = Type.Player;

                    #region Player

                    switch (playertype.ToLowerInvariant())
                    {
                        case "status":
                            _audioType = PlayerType.Status;
                            break;

                        case "state":
                            _audioType = PlayerType.State;
                            break;

                        case "artist":
                            _audioType = PlayerType.Artist;
                            break;

                        case "title":
                            _audioType = PlayerType.Title;
                            break;

                        case "duration":
                            _audioType = PlayerType.Duration;
                            break;

                        case "position":
                            _audioType = PlayerType.Position;
                            break;

                        case "repeat":
                            _audioType = PlayerType.Repeat;
                            break;

                        case "shuffle":
                            _audioType = PlayerType.Shuffle;
                            break;

                        case "volume":
                            _audioType = PlayerType.Volume;
                            break;

                        case "progress":
                            _audioType = PlayerType.Progress;
                            break;

                        default:
                            RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll PlayerType=" + playertype + " not valid");
                            break;
                    }

                    #endregion Player

                    break;

                case "friends":
                    _type = Type.Friends;
                    if (FriendsCount == null)
                        FriendsCount = rm.ReadString("FriendsCount", "1");
                    _userType = rm.ReadInt("UserType", 1);

                    #region Friends

                    switch (friendtype.ToLowerInvariant())
                    {
                        case "name":
                            _friendsType = FriendsType.Name;
                            break;

                        case "photo":
                            _friendsType = FriendsType.Photo;
                            break;

                        case "id":
                            _friendsType = FriendsType.Id;
                            break;

                        case "status":
                            _friendsType = FriendsType.Status;
                            break;

                        default:
                            RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll FriendType=" + friendtype + " not valid");
                            break;
                    }

                    #endregion Friends

                    break;

                case "messages":
                    _type = Type.Messages;
                    break;

                default:
                    RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKOnline.dll Type=" + type + " not valid");
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