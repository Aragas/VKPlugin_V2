using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Plugin.AudioPlayer;
using Plugin.Forms;
using Plugin.Information;
using Plugin.Methods;
using Rainmeter;

namespace Plugin
{
    public static class MeasureHandler
    {
        //public static Friends Friends;
        //public static Messages Messages;

        public static Player Player;

        public static void GetMeasure(string type, ref Measure measure)
        {
            switch (type.ToUpper())
            {
                case "PLAYER":
                    if (Player == null)
                        Player = new Player();
                    measure = Player;
                    break;

                //case "FRIENDS":
                //    if (Friends == null)
                //        Friends = new Friends();
                //    measure = Friends;
                //    break;

                //case "MESSAGES":
                //    if (Messages == null)
                //        Messages = new Messages();
                //    measure = Messages;
                //    break;
            }
        }
    }

    public class Measure
    {
        public virtual void Init(Rainmeter.API api) { }
        public virtual void Reload(Rainmeter.API api, ref double maxValue) { }
        public virtual double Update() { return 0.0; }
        public virtual string GetString() { return null; }
        public virtual void ExecuteBang(string args) { }
        public virtual void Dispose() { }
    }

    public class Player : Measure
    {
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
        private PlayerType _audioType;

        public string SaveAudio { get; private set; }
        public string Path { get; private set; }

        public override void Init(Rainmeter.API api)
        {
            Path = api.ReadPath("Type", "");
            Path = Path.Replace("\\" + Path.Split('\\')[7], "\\");

            // Load NAudio library.
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                var pathc = string.Format(Path + "{0}.dll", "NAudio");
                return Assembly.LoadFrom(pathc);
            };
            
            #region Player

            string playertype = api.ReadString("PlayerType", "").ToUpperInvariant();
            switch (playertype)
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

        public override void Reload(Rainmeter.API api, ref double maxValue)
        {
            #region Player

            string playertype = api.ReadString("PlayerType", "");
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

        public override double Update()
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

            return base.Update();
        }

        public override string GetString()
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

            return base.GetString();
        }

        public override void ExecuteBang(string args)
        {
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                VKPlayer.Execute(args);
            }
            else
            {
                VKPlayer.Execute(args);
            }
        }

        public override void Dispose()
        {
            VKPlayer.Dispose();
        }
    }

    public static class Plugin
    {
        private static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            Rainmeter.API api = new Rainmeter.API(rm);

            string type = api.ReadString("Type", "");

            Measure measure = null;

            MeasureHandler.GetMeasure(type, ref measure);
            measure.Init(api);

            data = GCHandle.ToIntPtr(GCHandle.Alloc(measure));
        }

        [DllExport]
        public static void Finalize(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Dispose();

            GCHandle.FromIntPtr(data).Free();
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            return measure.Update();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }

        [DllExport]
        public static void ExecuteBang(IntPtr data, IntPtr args)
        {
            Measure measure = (Measure)GCHandle.FromIntPtr(data).Target;
            measure.ExecuteBang(Marshal.PtrToStringUni(args));
        }
    }
}
