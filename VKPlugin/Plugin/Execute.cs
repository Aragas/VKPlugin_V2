using Rainmeter.AudioPlayer;
using Rainmeter.Forms;

namespace Rainmeter.Plugin
{
    public static class Execute
    {
        public static void Start(string command)
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
    }
}