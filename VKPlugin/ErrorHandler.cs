using Rainmeter.API;

namespace Rainmeter.ErrorHandler
{
    public static class Report
    {
        public static class Audio
        {
            public static void Count()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Can't get audio count");
            }

            public static void List()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Can't get audio list");
            }
        }

        public static class Friends
        {
            public static void Count()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Can't get friends count");
            }

            public static void List()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Can't get friends list");
            }
        }

        public static class Measure
        {
            public static void WrongType(string type)
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Type=" + type + " not valid");
            }
        }

        public static class Messages
        {
            public static void Count()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Can't get messages count");
            }
        }

        public static class Player
        {
            public static void Command()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll Invalid command");
            }

            public static void SetRepeat()
            {
                RainmeterAPI.Log
                        (RainmeterAPI.LogType.Error, "VKPlugin.dll SetRepeat format error");
            }

            public static void SetShuffle()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll SetShuffle format error");
            }

            public static void SetVolume()
            {
                RainmeterAPI.Log(RainmeterAPI.LogType.Error, "VKPlugin.dll SetVolume format error");
            }
        }
    }
}