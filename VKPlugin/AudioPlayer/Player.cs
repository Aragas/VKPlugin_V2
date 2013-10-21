﻿using System;
using System.IO;
using System.Net;
using System.Threading;
using NAudio.Wave;
using Rainmeter.Forms;
using Rainmeter.Methods;

namespace Rainmeter.AudioPlayer
{
    // To do:
    // Save mp3 to disk while playing from url.
    // Check if mp3 is saved and play local.
    // Play next mp3 after previous (find better method).

    public static class Player
    {
        internal static Playing Option = Playing.Init;
        internal static WaveChannel32 AudioStream;
        private static GetStream _gStream = new GetStream();
        private static WaveOut _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
        private static readonly Audio Au = new Audio();

        public static bool Played
        {
            get
            {
                if (Option != Playing.Ready) return false;
                return AudioStream != null && (Duration < AudioStream.CurrentTime.TotalSeconds);
            }
        }

        #region Internal

        private static string _token { get { return OAuth.Token; }}
        private static string _id { get { return OAuth.Id; } }
        private static string[] _array;
        private static int _numb;

        private static bool ArrayExists
        {
            get { return _array != null; }
        }

        private static string[] Array
        {
            get
            {
                if (ArrayExists) return _array;
                Au.Token = _token;
                Au.Id = _id;
                _array = Au.AudioList();
                return _array;
            }
        }

        private static string Url
        {
            get { return Array[_numb].Split('#')[4]; }
        }

        #endregion

        #region Variables

        public static bool Repeat;
        public static bool Shuffle;

        public static string Artist
        {
            get
            {
                if (ArrayExists) return Array[_numb].Split('#')[1];
                return null;
            }
        }

        public static string Title
        {
            get
            {
                if (ArrayExists) return Array[_numb].Split('#')[2];
                return null;
            }
        }

        public static string NextArtist
        {
            get
            {
                if (!ArrayExists) return null;
                if (_numb < Array.Length) return Array[_numb + 1].Split('#')[1];
                return null;
            }
        }

        public static string NextTitle
        {
            get
            {
                if (!ArrayExists) return null;
                if (_numb < Array.Length) return Array[_numb + 1].Split('#')[2];
                return null;
            }
        }

        public static double Duration
        {
            get
            {
                if (ArrayExists) return Convert.ToInt32(Array[_numb].Split('#')[3]);
                return 0.0;
            }
        }

        public static double State
        {
            get
            {
                if (_waveOut.PlaybackState == PlaybackState.Playing) return 1.0;
                if (_waveOut.PlaybackState == PlaybackState.Paused) return 2.0;
                return 0.0;
            }
        }

        public static double Position
        {
            get
            {
                if (_waveOut.PlaybackState == PlaybackState.Stopped) return 0.0;
                if (!Played) return AudioStream.CurrentTime.TotalSeconds;
                return 0.0;
            }
        }

        public static double Progress
        {
            get
            {
                if (Option == Playing.Ready)
                {
                    return Position/Duration;
                }
                return 0.0;
            }
        }

        #endregion

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
        }

        /// <summary>
        ///     Check if audiofile has ended. If true, starts next file. Better put in a loop.
        /// </summary>
        public static void PlayNext()
        {
            if (Repeat)
            {
                Stop();
                Play();
            }
            else if (Shuffle)
            {
                if (_waveOut.PlaybackState == PlaybackState.Stopped) return;
                if (_numb > Array.Length) return;

                var random = new Random();
                _numb = random.Next(0, Array.Length);

                Stop();
                _waveOut.Dispose();
                _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
                _gStream.Dispose();
                _gStream = new GetStream();
                Play();
            }
            else Next();
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
                            _play();
                            break;
                        case PlaybackState.Stopped:
                            Play();
                            break;
                    }
                    break;

                case Playing.Init:
                {
                    try
                    {
                        Play();
                    }
                    catch
                    {
                    }
                }
                    break;
            }
        }

        private static void Stop()
        {
            _waveOut.Stop();
        }

        private static void Next()
        {
            if (_waveOut.PlaybackState == PlaybackState.Stopped) return;
            if (_numb >= Array.Length) return;
            _numb += 1;

            Stop();
            _waveOut.Dispose();
            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            _gStream.Dispose();
            _gStream = null;
            _gStream = new GetStream();
            Play();
        }

        private static void Previous()
        {
            if (_waveOut.PlaybackState == PlaybackState.Stopped) return;
            if (_numb <= 0) return;
            _numb -= 1;

            Stop();
            _waveOut.Dispose();
            _waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback());
            _gStream.Dispose();
            _gStream = null;
            _gStream = new GetStream();
            Play();
        }

        private static void SetVolume(string value)
        {
#if DEBUG
            if (value.StartsWith("+") || value.StartsWith("-"))
            {
                value = value.Substring(1);
                AudioStream.Volume += Convert.ToSingle(Convert.ToInt32(value)/100);
            }
            else
            {
                AudioStream.Volume = Convert.ToSingle(Convert.ToInt32(value)/100);
            }
#else
            if (value.StartsWith("+") || value.StartsWith("-"))
            {
                try
                {
                    value = value.Substring(1);
                    AudioStream.Volume += Convert.ToSingle(Convert.ToInt32(value) / 100);
                }
                catch { }
            }
            else
            {
                try
                {
                    AudioStream.Volume = Convert.ToSingle(Convert.ToInt32(value) / 100);
                }
                catch { }
            }
#endif
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
            }
        }

        private static void Play()
        {
            _gStream.Url = Url;

            _waveOut.Init(_gStream.Wave());
            AudioStream.Volume = 0.7F;
            _waveOut.Play();
        }

        private static void _play()
        {
            _waveOut.Play();
        }

        private static void Pause()
        {
            _waveOut.Pause();
        }

        #endregion

        internal enum Playing
        {
            Init,
            Buffering,
            Ready
        }
    }

    internal class GetStream : IDisposable
    {
        private readonly Stream _ms = new MemoryStream();
        private WaveChannel32 _channel;
        private Mp3FileReader _reader;

        public string Url { private get; set; }

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

            GC.SuppressFinalize(this);
        }

        public WaveStream Wave()
        {
            #region Download

            ThreadStart download = delegate
            {
                WebResponse response = WebRequest.Create(Url).GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    var buffer = new byte[65536]; // 64KB chunks
                    int read;
                    while (stream != null && (read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        long pos = _ms.Position;
                        _ms.Position = _ms.Length;
                        _ms.Write(buffer, 0, read);
                        _ms.Position = pos;
                    }
                    Thread.CurrentThread.Abort();
                }
            };
            new Thread(download).Start();

            // Pre-buffering some data to allow NAudio to start playing
            while (_ms.Length < 65536*10)
            {
                Player.Option = Player.Playing.Buffering;

                Thread.Sleep(500);
            }
            if (_ms.Length > 65536*10) Player.Option = Player.Playing.Ready;

            #endregion

            _ms.Position = 0;
            _reader = new Mp3FileReader(_ms);
            _channel = new WaveChannel32(_reader);
            Player.AudioStream = _channel;
            return Player.AudioStream;
        }
    }
}