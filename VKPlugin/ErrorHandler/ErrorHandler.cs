namespace Plugin.ErrorHandler
{
    public static class Report
    {
        public static class Audio
        {
            public static void Count()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Can't get audio count");
            }

            public static void List()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Can't get audio list");
            }
        }

        public static class Friends
        {
            public static void Count()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Can't get friends count");
            }

            public static void List()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Can't get friends list");
            }
        }

        public static class Messages
        {
            public static void Count()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Can't get messages count");
            }
        }

        public static class Player
        {
            public static void Command()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll Invalid command");
            }

            public static void SetRepeat()
            {
                Rainmeter.API.Log
                        (Rainmeter.API.LogType.Error, "VKPlugin.dll SetRepeat format error");
            }

            public static void SetShuffle()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll SetShuffle format error");
            }

            public static void SetVolume()
            {
                Rainmeter.API.Log(Rainmeter.API.LogType.Error, "VKPlugin.dll SetVolume format error");
            }
        }
    }
}