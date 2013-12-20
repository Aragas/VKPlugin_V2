using NAudio.CoreAudioApi;
using NAudio.Wave;
using Rainmeter.ErrorHandler;
using Rainmeter.Forms;
using Rainmeter.Methods;
using Rainmeter.Plugin;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Rainmeter.AudioPlayer
{
    // To do:
    // Save mp3 to disk while playing from url. (Done).
    // Check if mp3 is saved and play local. !!! (Done)
    // Play next mp3 after previous (find better method). !!! (Maybe in another life)
    // Optimize download function. Then will work SetPosition. !!! (Maybe in another life)

    public static class Player
    {
        internal static WaveChannel32 AudioStream;
        internal static Playing Option = Playing.Init;

        private static readonly MMDevice DefaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint
            (DataFlow.Render, Role.Multimedia);

        private static Audio _audio; 
        private static GetFile _gFile = new GetFile();
        private static GetStream _gStream = new GetStream();
        private static WaveOut _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());

        internal enum Playing
        {
            Init,
            Buffering,
            Ready
        }

        #region AudioList

        private static string[] _array;

        private static int _numb;

        private static string[] Array
        {
            get
            {
                if (ArrayExists) return _array;

                _audio = new Audio
                {
                    Token = Token,
                    Id = Id
                };
                _array = _audio.AudioList();
                return _array;
            }
        }

        private static bool ArrayExists
        {
            get { return _array != null; }
        }

        private static string FileName
        {
            get
            {
                return Reverse(Reverse(Array[_numb].Split('#')[4]).Split('/')[0]);
            }
        }

        private static string Id { get { return OAuth.Id; } }

        private static string Token { get { return OAuth.Token; } }

        private static string Url
        {
            get
            {
                return Array[_numb].Split('#')[4];
            }
        }

        #endregion AudioList

        #region Variables

        public static bool Repeat;
        public static bool Shuffle;

        public static string Artist
        {
            get
            {
                return Array[_numb].Split('#')[1];
            }
        }

        public static double Duration
        {
            get
            {
                return Convert.ToInt32(Array[_numb].Split('#')[3]);
            }
        }

        public static bool Played
        {
            get
            {
                if (Option != Playing.Ready)
                    return false;
                return AudioStream != null && (Duration < AudioStream.CurrentTime.TotalSeconds);
            }
        }

        public static double Position
        {
            get
            {
                if (_waveOut.PlaybackState == PlaybackState.Stopped)
                    return 0.0;
                if (!Played)
                    return AudioStream.CurrentTime.TotalSeconds;
                return 0.0;
            }
        }

        public static double Progress
        {
            get
            {
                return Position / Duration;
            }
        }

        public static bool SaveAudio
        {
            get
            {
                return Convert.ToBoolean(Measure.SaveAudio);
            }
        }

        public static double State
        {
            get
            {
                switch (_waveOut.PlaybackState)
                {
                    case PlaybackState.Playing:
                        return 1.0;

                    case PlaybackState.Paused:
                        return 2.0;

                    default:
                        return 0.0;
                }
            }
        }

        public static string Title
        {
            get
            {
                return Array[_numb].Split('#')[2];
            }
        }

        #endregion Variables

        #region Execute

        /// <summary>
        ///     Execute your command.
        /// </summary>
        /// <param name="command">Your command.</param>
        /// <param name="token">Your token.</param>
        /// <param name="id">Your id.</param>
        public static void Execute(string command)
        {
            if (command == "PlayPause") PlayPause();
            else if (command == "Play") PlayPause();
            else if (command == "Pause") PlayPause();
            else if (command == "Stop") Stop();
            else if (command == "Next") Next();
            else if (command == "Previous") Previous();
            else if (command.Contains("SetVolume")) SetVolume(command.Remove(0, 10));
            else if (command.Contains("SetShuffle")) SetShuffle(command.Remove(0, 11));
            else if (command.Contains("SetRepeat")) SetRepeat(command.Remove(0, 10));
            else Report.Player.Command();
        }

        /// <summary>
        ///     Check if audio file has ended. If true, starts next file. Better put in a loop.
        /// </summary>
        public static void PlayNext()
        {
            if (Repeat)
            {
                Stop();
                PlayNew();
            }
            else if (Shuffle)
            {
                var random = new Random();
                _numb = random.Next(0, Array.Length);

                Stop();
                PlayNew();
            }
            else Next();
        }

        private static void Next()
        {
            // Check if stopped.
            if (_waveOut.PlaybackState == PlaybackState.Stopped)
                return;

            // Check if we are at the end of our playlist.
            if (_numb >= Array.Length)
                return;

            _numb += 1;

            Stop();
            PlayNew();
        }

        private static void Pause()
        {
            _waveOut.Pause();
        }

        private static void Play()
        {
            _waveOut.Play();
        }

        private static void PlayNew()
        {
            DisposeAll();

            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());

            if (FileExists)
            {
                _gFile = new GetFile();
                _waveOut.Init(_gFile.Wave(FilePath));
            }
            else
            {
                _gStream = new GetStream();
                _waveOut.Init(_gStream.Wave(Url));
            }

            AudioStream.Volume = DefaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;

            _waveOut.Play();
        }

        private static void PlayPause()
        {
            switch (Option)
            {
                case Playing.Ready:

                    switch (_waveOut.PlaybackState)
                    {
                        case PlaybackState.Playing:
                            Pause();
                            break;

                        case PlaybackState.Paused:
                            Play();
                            break;

                        case PlaybackState.Stopped:
                            PlayNew();
                            break;
                    }
                    break;

                case Playing.Init:
                    PlayNew();
                    break;
            }
        }

        private static void Previous()
        {
            // Check if stopped.
            if (_waveOut.PlaybackState == PlaybackState.Stopped)
                return;

            // Check if we are at the beginning of our playlist.
            if (_numb <= 0)
                return;

            _numb -= 1;

            Stop();
            PlayNew();
        }

        private static void SetRepeat(string value)
        {
            switch (value)
            {
                case "1":
                    Repeat = true;
                    break;

                case "0":
                    Repeat = false;
                    break;

                case "-1":
                    if (Repeat) Repeat = false;
                    else
                    {
                        Repeat = true;
                        Shuffle = false;
                    }
                    break;

                default:
                    Report.Player.SetRepeat();
                    break;
            }
        }

        private static void SetShuffle(string value)
        {
            switch (value)
            {
                case "1":
                    Shuffle = true;
                    break;

                case "0":
                    Shuffle = false;
                    break;

                case "-1":
                    if (Shuffle) Shuffle = false;
                    else
                    {
                        Shuffle = true;
                        Repeat = false;
                    }
                    break;

                default:
                    Report.Player.SetShuffle();
                    break;
            }
        }

        private static void SetVolume(string value)
        {
            if (value.StartsWith("+"))
            {
                try
                {
                    value = value.Substring(1);
                    AudioStream.Volume += (float)Convert.ToInt32(value) / (float)100;
                }
                catch (FormatException)
                {
                    Report.Player.SetVolume();
                }
            }
            else if (value.StartsWith("-"))
            {
                try
                {
                    value = value.Substring(1);
                    AudioStream.Volume -= (float)Convert.ToInt32(value) / (float)100;
                }
                catch (FormatException)
                {
                    Report.Player.SetVolume();
                }
            }
            else
            {
                try
                {
                    AudioStream.Volume = (float)Convert.ToInt32(value) / (float)100;
                }
                catch (FormatException)
                {
                    Report.Player.SetVolume();
                }
            }
        }

        private static void Stop()
        {
            _waveOut.Stop();
        }

        #endregion Execute

        #region File

        public static string FilePath
        {
            get
            {
                string path = Measure.Path + "Music\\";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path + FileName;
            }
        }

        private static bool FileExists
        {
            get
            {
                return (File.Exists(FilePath));
            }
        }

        #endregion File

        public static void Dispose()
        {
            DisposeAll();

            if (_array != null)
            {
                _array = null;
            }

            if (AudioStream != null)
            {
                AudioStream.Dispose();
            }
        }

        private static void DisposeAll()
        {
            if (_waveOut != null)
            {
                _waveOut.Dispose();
            }

            if (_gStream != null)
            {
                _gStream.Dispose();
            }

            if (_gFile != null)
            {
                _gFile.Dispose();
            }
        }

        private static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            System.Array.Reverse(charArray);
            return new string(charArray);
        }
    }

    internal class GetFile : IDisposable
    {
        private WaveChannel32 _channel;
        private Mp3FileReader _reader;

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        public WaveChannel32 Wave(string url)
        {
            _reader = new Mp3FileReader(url);
            _channel = new WaveChannel32(_reader);
            Player.AudioStream = _channel;
            return Player.AudioStream;
        }
    }

    internal class GetStream : IDisposable
    {
        private readonly Thread _downloadThread;
        private readonly Stream _ms = new MemoryStream();
        private readonly Stream _ms1 = new MemoryStream();
        private WaveChannel32 _channel;
        private Mp3FileReader _reader;

        public GetStream()
        {
            _downloadThread = Player.SaveAudio ? new Thread(DownloadSave) : new Thread(Download);
        }

        private string Url { get; set; }

        public void Dispose()
        {
            if (_downloadThread.IsAlive)
                _downloadThread.Abort();

            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
            if (_channel != null)
            {
                _channel.Dispose();
                _channel = null;
            }
        }

        public WaveChannel32 Wave(string url)
        {
            Url = url;

            if (!_downloadThread.IsAlive)
                _downloadThread.Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (_ms.Length < 32 * 1024 * 8)
            {
                Player.Option = Player.Playing.Buffering;

                Thread.Sleep(100); //Find better method.
            }

            if (_ms.Length > 32 * 1024 * 8)
                Player.Option = Player.Playing.Ready;

            _ms.Position = 0;
            _reader = new Mp3FileReader(_ms);
            _channel = new WaveChannel32(_reader);
            Player.AudioStream = _channel;
            return Player.AudioStream;
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32 * 1024];
            int read;
            while (input != null && (read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }

        private void Download()
        {
            WebResponse response = WebRequest.Create(Url).GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                var buffer = new byte[32 * 1024]; // 32Kb chunks
                int read;
                while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    long pos = _ms.Position;
                    _ms.Position = _ms.Length;
                    _ms.Write(buffer, 0, read);
                    _ms.Position = pos;
                }
            }
        }

        private void DownloadSave()
        {
            WebResponse response = WebRequest.Create(Url).GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                var buffer = new byte[32 * 1024]; // 32Kb chunks
                int read;
                while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    long pos = _ms.Position;
                    _ms.Position = _ms.Length;
                    _ms1.Position = _ms1.Length;
                    _ms.Write(buffer, 0, read);
                    _ms1.Write(buffer, 0, read);
                    _ms.Position = pos;
                    _ms1.Position = pos;
                }

                using (Stream file = File.OpenWrite(Player.FilePath))
                {
                    CopyStream(_ms1, file);
                }
            }
        }
    }
}