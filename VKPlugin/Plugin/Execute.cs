using Rainmeter.AudioPlayer;
using Rainmeter.Forms;

namespace Rainmeter.Plugin
{
    public static class Execute
    {
        public static void Start(string command)
        {
#if DEBUG
            if (!OAuth.TokenIdExist)
            {
                OAuth.OAuthRun();
                Player.Execute(command);
            }
            else
            {
                Player.Execute(command);
            }
#else
            if (!OAuth.TokenIdExist)
            {
                try
                {
                    OAuth.OAuthRun();
                    Player.Execute(command);
                }
                catch {}
            }
            else
            {
                try
                {
                    Player.Execute(command);
                }
                catch {}
            }
#endif
        }
    }
}