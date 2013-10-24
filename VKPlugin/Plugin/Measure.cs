using Rainmeter.API;
using Rainmeter.AudioPlayer;
using Rainmeter.Information;
using System;

namespace Rainmeter.Plugin
{
    internal class Measure
    {
        private int _userType = 1;
        private AudioPlayer _audioType;
        private FriendsType _friendsType;
        private Type _type;

        private enum AudioPlayer
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
                        case AudioPlayer.Status:
                            return "VKPlayer by Aragas (Aragasas)";

                        case AudioPlayer.Artist:
                            return Player.Artist ?? "Not Authorized";

                        case AudioPlayer.Title:
                            return Player.Title ?? "Click Play";

                        case AudioPlayer.NextArtist:
                            return Player.NextArtist ?? "Not Authorized";

                        case AudioPlayer.NextTitle:
                            return Player.NextTitle ?? "Click Play";
                    }

                    #endregion Player

                    break;
            }
            return null;
        }

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
                            _audioType = AudioPlayer.Status;
                            break;

                        case "state":
                            _audioType = AudioPlayer.State;
                            break;

                        case "artist":
                            _audioType = AudioPlayer.Artist;
                            break;

                        case "title":
                            _audioType = AudioPlayer.Title;
                            break;

                        case "duration":
                            _audioType = AudioPlayer.Duration;
                            break;

                        case "position":
                            _audioType = AudioPlayer.Position;
                            break;

                        case "repeat":
                            _audioType = AudioPlayer.Repeat;
                            break;

                        case "shuffle":
                            _audioType = AudioPlayer.Shuffle;
                            break;

                        case "volume":
                            _audioType = AudioPlayer.Volume;
                            break;

                        case "progress":
                            _audioType = AudioPlayer.Progress;
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
                        case AudioPlayer.Duration:
                            return Player.Duration;

                        case AudioPlayer.Position:
                            return Math.Round(Player.Position);

                        case AudioPlayer.State:
                            if (Player.Played) Player.PlayNext();
                            return Player.State;

                        case AudioPlayer.Repeat:
                            return Player.Repeat ? 0.0 : 1.0;

                        case AudioPlayer.Shuffle:
                            return Player.Shuffle ? 0.0 : 1.0;

                        case AudioPlayer.Progress:
                            return Player.Progress;
                    }

                    #endregion Player

                    break;
            }
            return 0.0;
        }
    }
}