﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Plugin.AudioPlayer;
using Plugin.Forms;
using Rainmeter;

namespace Plugin
{
    /// <summary>
    /// Main part of Measure.
    /// </summary>
    internal partial class MeasuresHandler
    {
        Player player = new Player();
        Friends friends = new Friends();
        Messages messages = new Messages();

        public static string FriendsCount { get; private set; }
        public static string SaveAudio { get; private set; }
        public static Dictionary<MeasureType, string> MeasurePath = new Dictionary<MeasureType, string>();

        internal enum MeasureType { PlayerType, FriendsType, MessagesType }
        private MeasureType _type;

        /// <summary>
        /// Called when a measure is created (i.e. when Rainmeter is launched or when a skin is refreshed).
        /// Initialize your measure object here.
        /// </summary>
        /// <param name="api">Rainmeter API</param>
        internal MeasuresHandler(Rainmeter.API api)
        {
            string type = api.ReadString("Type", "");
            switch (type.ToUpperInvariant())
            {
                case "LIST":
                    if (FriendsCount == null)
                        FriendsCount = api.ReadString("FriendsCount", "1");
                    break;

                case "PLAYER":
                    player.Init(api, ref _type);

                    TypeIsAlive(api, _type);
                    break;

                case "FRIENDS":
                    friends.Init(api, ref _type);
                    break;

                case "MESSAGES":
                    messages.Init(api, ref _type);
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
                case "PLAYER":
                    player.Reload(api, ref _type);
                    break;

                case "FRIENDS":
                    friends.Reload(api, ref _type);
                    break;

                case "MESSAGES":
                    messages.Reload(api, ref _type);
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
                case MeasureType.PlayerType:
                    return player.Double();

                case MeasureType.FriendsType:
                    return friends.Double();

                case MeasureType.MessagesType:
                    return messages.Double();
            }

            return 0.0;
        }

        internal string GetString()
        {
            switch (_type)
            {
                case MeasureType.PlayerType:
                    return player.String();

                case MeasureType.FriendsType:
                    return friends.String();

                case MeasureType.MessagesType:
                    return messages.String();
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
                VKPlayer.Execute(args);
            }
            else
            {
                VKPlayer.Execute(args);
            }
        }

        /// <summary>
        /// Called when a measure is disposed (i.e. when Rainmeter is closed or when a skin is refreshed).
        /// Dispose your measure object here.
        /// </summary>
        public void Finalize(Rainmeter.API api)
        {
            string type = api.ReadString("Type", "");
            switch (type.ToUpperInvariant())
            {
                case "PLAYER":
                    player.Dispose();
                    break;
            }
        }
        
        /// <summary>
        /// Called from TypeIsAlive(). (Using GetMethods())
        /// </summary>
        private void PlayerTypeDispose()
        {
            player.Dispose();
        }
    }

    /// <summary>
    /// Threading part of Measure.
    /// </summary>
    internal partial class MeasuresHandler
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
            if (ThreadAlive.ContainsKey(type) && ThreadAlive[type].IsAlive)
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

                        Thread.CurrentThread.Abort();
                    }
                })
                {
                    Name = "ThreadMonitor" + type,
                    IsBackground = true
                };
                ThreadAlive.Remove(type);
                ThreadAlive.Add(type, thread);
                ThreadAlive[type].Start();
            }
        }
    }

    /// <summary>
    /// Update part of Measure.
    /// </summary>
    internal partial class MeasuresHandler
    {
        private const int DefaultUpdateRate = 20;

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
