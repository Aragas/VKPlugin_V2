using NAudio.CoreAudioApi;
using NAudio.Wave;
using Plugin.ErrorHandler;
using Plugin.Forms;
using Plugin.Methods;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Plugin.AudioPlayer
{
    // To do:
    // Optimize download function. Then will work SetPosition. !!! (Maybe in another life)

    // Play next audio don't work. Freezes if click play after next audio.
    // AudioList and Variables seems good.
    // Maybe optimize bool FileExists. Takes too much funcs.

    public static class VKPlayer
    {
        internal enum Playing { Init, Ready }
        internal static Playing Status;
        
        private static readonly MMDevice DefaultDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint
            (DataFlow.Render, Role.Multimedia);

        private static Audio _audio; 
        private static GetFile _gFile;
        private static GetStream _gStream;
        private static WaveStream _waveStream; 
        private static WaveChannel32 _audioChannel32;
        private static readonly IWavePlayer WavePlayer = new WaveOut();

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
            get { return Reverse(Reverse(Array[_numb].Split('#')[4]).Split('/')[0]); }
        }

        private static string Id
        {
            get { return OAuth.Id; } 
            
        }

        private static string Token
        {
            get { return OAuth.Token; }
        }

        private static string Url
        {
            get { return Array[_numb].Split('#')[4]; }
        }

        #endregion AudioList

        #region Variables

        public static bool Repeat { get; private set; }
        public static bool Shuffle { get; private set; }

        public static string Artist
        {
            get { return ArrayExists ? Array[_numb].Split('#')[1] : null ; }
        }

        public static double Duration
        {
            get { return ArrayExists ? Convert.ToInt32(Array[_numb].Split('#')[3]) : 0.0; }
        }

        public static PlaybackState PlaybackState
        {
            get { return WavePlayer.PlaybackState; }
        }

        public static bool Played
        {
            get
            {
                if (Status != Playing.Ready)
                    return false;
                return _waveStream != null && (Duration < _waveStream.CurrentTime.TotalSeconds);
            }
        }

        public static double Position
        {
            get
            {
                if (!Played && PlaybackState != PlaybackState.Stopped)
                    return _waveStream.CurrentTime.TotalSeconds;
                return 0.0;
            }
        }

        public static double Progress
        {
            get { return Position / Duration; }
        }

        public static bool SaveAudio
        {
            get { return Convert.ToBoolean(MeasuresHandler.SaveAudio); }
        }

        public static double State
        {
            get
            {
                switch (PlaybackState)
                {
                    case PlaybackState.Playing:
                        return 1.0;

                    case PlaybackState.Paused:
                        return 0.0;

                        case PlaybackState.Stopped:
                        return 0.0;

                    default:
                        return 0.0;
                }
            }
        }

        public static string Title
        {
            get { return ArrayExists ? Array[_numb].Split('#')[2] : null; }
        }

        #endregion Variables

        #region Execute

        /// <summary>
        ///     Execute your command.
        /// </summary>
        /// <param name="command">Your command.</param>
        public static void Execute(string command)
        {
            if (String.IsNullOrEmpty(command)) return;

            if (command == "PlayPause") PlayPause();
            else if (command == "Play") PlayPause();
            else if (command == "Pause") PlayPause();
            else if (command == "Stop") Stop();
            else if (command == "Next") Next();
            else if (command == "Previous") Previous();
            else if (command.StartsWith("SetVolume")) SetVolume(command.Remove(0, 10));
            else if (command.Contains("SetShuffle")) SetShuffle(command.Remove(0, 11));
            else if (command.Contains("SetRepeat")) SetRepeat(command.Remove(0, 10));
            #if DEBUG
            else if (command.Contains("SetPosition")) SetPosition(command.Remove(0, 12));
            #endif
            else Report.Player.Command();
        }

        /// <summary>
        /// Check if audio file has ended. If true, starts next file. Better put in a event.
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
            if (PlaybackState == PlaybackState.Stopped)
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
            WavePlayer.Pause();
        }

        private static void Play()
        {
            WavePlayer.Play();
        }

        private static void PlayNew()
        {
            DisposeAudio();
            Status = Playing.Init;

            if (FileExists)
            {
                _gFile = new GetFile();
                _gFile.Loaded +=_gFile_Loaded;
                _waveStream = _gFile.Wave(FilePath);
                _audioChannel32 = new WaveChannel32(_waveStream) {PadWithZeroes = false};
                WavePlayer.Init(_audioChannel32);
                WavePlayer.PlaybackStopped += _waveOut_PlaybackStopped;

            }
            else
            {
                _gStream = new GetStream();
                _gStream.EnoughDataToPlay +=_gStream_EnoughDataToPlay;
                _waveStream = _gStream.Wave(Url);
                _audioChannel32 = new WaveChannel32(_waveStream) {PadWithZeroes = false};
                WavePlayer.Init(_audioChannel32);
                WavePlayer.PlaybackStopped += _waveOut_PlaybackStopped;
            }

            _audioChannel32.Volume = DefaultDevice.AudioEndpointVolume.MasterVolumeLevelScalar;

            WavePlayer.Play();
        }

        private static void _gStream_EnoughDataToPlay(object sender, EventArgs e)
        {
            Status = Playing.Ready;
        }

        private static void _gFile_Loaded(object sender, EventArgs e)
        {
            Status = Playing.Ready;
        }

        private static void _waveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            PlayNext();
        }

        private static void PlayPause()
        {
            switch (Status)
            {
                case Playing.Ready:

                    switch (PlaybackState)
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
            if (PlaybackState == PlaybackState.Stopped)
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
                    _audioChannel32.Volume += (float)Convert.ToInt32(value) / (float)100;
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
                    _audioChannel32.Volume -= (float)Convert.ToInt32(value) / (float)100;
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
                    _audioChannel32.Volume = (float)Convert.ToInt32(value) / (float)100;
                }
                catch (FormatException)
                {
                    Report.Player.SetVolume();
                }
            }
        }

        // Don't work.
        private static void SetPosition(string value)
        {
#if DEBUG
            if (Status != Playing.Ready) return;
            if (value.StartsWith("+") || value.StartsWith("-"))
            {
                bool plus = (value.Contains("+"));
                value = value.Substring(1);
                double seconds = Convert.ToDouble(value) / 100.0 * Duration;

                if (plus)
                    _waveStream.CurrentTime += TimeSpan.FromSeconds(seconds);
                else
                    _waveStream.CurrentTime -= TimeSpan.FromSeconds(seconds);
            }
            else
            {
                double seconds = Convert.ToDouble(value) / 100.0 * Duration;
                _waveStream.CurrentTime = TimeSpan.FromSeconds(seconds);
            }
#else
            if (Option != Playing.Ready) return;
            if (value.StartsWith("+") || value.StartsWith("-"))
            {
                try
                {
                    bool plus = (value.Contains("+"));
                    value = value.Substring(1);
                    double seconds = Convert.ToDouble(value)/100.0*Duration;

                    if (plus) _waveStream.CurrentTime = _waveStream.CurrentTime.Add(TimeSpan.FromSeconds(seconds));
                    else _waveStream.CurrentTime = _waveStream.CurrentTime.Subtract(TimeSpan.FromSeconds(seconds));
                }
                catch{}
            }
            else
            {
                try
                {
                    double seconds = Convert.ToDouble(value)/100.0*Duration;
                    _waveStream.CurrentTime = TimeSpan.FromSeconds(seconds);
                }
                catch{}
            }
#endif
        }

        private static void Stop()
        {
            WavePlayer.Stop();
            _waveStream.Position = 0;
        }

        #endregion Execute

        #region File

        public static string FilePath
        {
            get
            {
                string path = MeasuresHandler.MeasurePath[MeasuresHandler.MeasureType.PlayerType] + "Music\\";

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path + FileName;
            }
        }

        private static bool FileExists
        {
            get { return (File.Exists(FilePath)); }
        }

        #endregion File

        public static void Dispose()
        {
            DisposeAudio();

            _array = null;
        }

        private static void DisposeAudio()
        {
            if (WavePlayer != null)
            {
                WavePlayer.Stop();
            }

            if (_waveStream != null)
            {
                _waveStream.Dispose();
            }

            if (_audioChannel32 != null)
            {
                _audioChannel32.Dispose();
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

    internal sealed class GetFile : IDisposable
    {
        public event EventHandler Loaded;

        private void OnLoaded(EventArgs e)
        {
            if (Loaded != null)
                Loaded(this, e);
        }

        private Mp3FileReader _reader;

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }
            //if (_channel != null)
            //{
            //    _channel.Dispose();
            //}
        }

        public Mp3FileReader Wave(string path)
        {
            _reader = new Mp3FileReader(path);
            OnLoaded(EventArgs.Empty);

            return _reader;
        }
    }

    internal sealed class GetStream : IDisposable
    {
        public event EventHandler Downloaded;
        public event EventHandler Downloading;
        public event EventHandler EnoughDataToPlay;
        public event EventHandler Saved;

        private void OnDownloaded(EventArgs e)
        {
            if (Downloaded != null)
                Downloaded(this, e);
        }
        private void OnDownloading(EventArgs e)
        {
            if (Downloading != null)
                Downloading(this, e);
        }
        private void OnEnoughDataToPlay(EventArgs e)
        {
            if (EnoughDataToPlay != null)
                EnoughDataToPlay(this, e);
        }
        private void OnSaved(EventArgs e)
        {
            if (Saved != null)
                Saved(this, e);
        }

        private readonly Thread _downloadThread;
        private Stream _ms = new MemoryStream();
        private Stream _ms1 = new MemoryStream();
        private Mp3FileReader _reader;

        public GetStream()
        {
            _downloadThread = VKPlayer.SaveAudio ? new Thread(DownloadSave) : new Thread(Download);
        }

        public long Position { get { return _ms.Position; } set { _ms.Position = value; }}

        private string Url { get; set; }

        public void Dispose()
        {
            if (_downloadThread.IsAlive && _downloadThread != null)
                _downloadThread.Abort();

            if (_reader != null)
            {
                _reader.Dispose();
            }
        }

        public Mp3FileReader Wave(string url)
        {
            Url = url;

            if (!_downloadThread.IsAlive && _downloadThread != null)
                _downloadThread.Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (_ms.Length < 32 * 1024 * 8) { }

            if (_ms.Length > 32 * 1024 * 8)
                OnEnoughDataToPlay(EventArgs.Empty);
            
            _ms.Position = 0;
            _reader = new Mp3FileReader(_ms);
            return _reader;
        }

        private static void CopyStream(Stream input, Stream output)
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
            using (WebResponse response = WebRequest.Create(Url).GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                OnDownloading(EventArgs.Empty);

                var buffer = new byte[16 * 1024]; // 16Kb chunks
                int read;
                while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    long pos = _ms.Position;
                    _ms.Position = _ms.Length;
                    _ms.Write(buffer, 0, read);
                    _ms.Position = pos;
                }

                OnDownloaded(EventArgs.Empty);
            }
        }

        private void DownloadSave()
        {
            using (WebResponse response = WebRequest.Create(Url).GetResponse())
            using (Stream stream = response.GetResponseStream())
            {
                OnDownloading(EventArgs.Empty);

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

                OnDownloaded(EventArgs.Empty);

                using (Stream file = File.OpenWrite(VKPlayer.FilePath))
                {
                    CopyStream(_ms1, file);
                }

                OnSaved(EventArgs.Empty);
            }
        }
    }
}