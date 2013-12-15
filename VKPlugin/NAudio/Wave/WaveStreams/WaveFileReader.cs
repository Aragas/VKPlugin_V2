using NAudio.FileFormats.Wav;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NAudio.Wave
{
    /// <summary>
    ///     This class supports the reading of WAV files,
    ///     providing a repositionable WaveStream that returns the raw data
    ///     contained in the WAV file
    /// </summary>
    public class WaveFileReader : WaveStream
    {
        private readonly List<RiffChunk> chunks = new List<RiffChunk>();
        private readonly long dataChunkLength;
        private readonly long dataPosition;
        private readonly bool ownInput;
        private readonly WaveFormat waveFormat;
        private Stream waveStream;

        /// <summary>Supports opening a WAV file</summary>
        /// <remarks>
        ///     The WAV file format is a real mess, but we will only
        ///     support the basic WAV file format which actually covers the vast
        ///     majority of WAV files out there. For more WAV file format information
        ///     visit www.wotsit.org. If you have a WAV file that can't be read by
        ///     this class, email it to the NAudio project and we will probably
        ///     fix this reader to support it
        /// </remarks>
        public WaveFileReader(String waveFile) :
            this(File.OpenRead(waveFile))
        {
            ownInput = true;
        }

        /// <summary>
        ///     Creates a Wave File Reader based on an input stream
        /// </summary>
        /// <param name="inputStream">The input stream containing a WAV file including header</param>
        public WaveFileReader(Stream inputStream)
        {
            waveStream = inputStream;
            var chunkReader = new WaveFileChunkReader();
            chunkReader.ReadWaveHeader(inputStream);
            waveFormat = chunkReader.WaveFormat;
            dataPosition = chunkReader.DataChunkPosition;
            dataChunkLength = chunkReader.DataChunkLength;
            chunks = chunkReader.RiffChunks;
            Position = 0;
        }

        /// <summary>
        ///     Gets a list of the additional chunks found in this file
        /// </summary>
        public List<RiffChunk> ExtraChunks
        {
            get { return chunks; }
        }

        /// <summary>
        ///     <see cref="WaveStream.WaveFormat" />
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }

        /// <summary>
        ///     This is the length of audio data contained in this WAV file, in bytes
        ///     (i.e. the byte length of the data chunk, not the length of the WAV file itself)
        ///     <see cref="WaveStream.WaveFormat" />
        /// </summary>
        public override long Length
        {
            get { return dataChunkLength; }
        }

        /// <summary>
        ///     Number of Samples (if possible to calculate)
        ///     This currently does not take into account number of channels, so
        ///     divide again by number of channels if you want the number of
        ///     audio 'frames'
        /// </summary>
        public long SampleCount
        {
            get
            {
                if (waveFormat.Encoding == WaveFormatEncoding.Pcm ||
                    waveFormat.Encoding == WaveFormatEncoding.Extensible ||
                    waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
                {
                    return dataChunkLength / BlockAlign;
                }
                // n.b. if there is a fact chunk, you can use that to get the number of samples
                throw new InvalidOperationException("Sample count is calculated only for the standard encodings");
            }
        }

        /// <summary>
        ///     Position in the WAV data chunk.
        ///     <see cref="Stream.Position" />
        /// </summary>
        public override long Position
        {
            get { return waveStream.Position - dataPosition; }
            set
            {
                lock (this)
                {
                    value = Math.Min(value, Length);
                    // make sure we don't get out of sync
                    value -= (value % waveFormat.BlockAlign);
                    waveStream.Position = value + dataPosition;
                }
            }
        }

        /// <summary>
        ///     Gets the data for the specified chunk
        /// </summary>
        public byte[] GetChunkData(RiffChunk chunk)
        {
            long oldPosition = waveStream.Position;
            waveStream.Position = chunk.StreamPosition;
            var data = new byte[chunk.Length];
            waveStream.Read(data, 0, data.Length);
            waveStream.Position = oldPosition;
            return data;
        }

        /// <summary>
        ///     Cleans up the resources associated with this WaveFileReader
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release managed resources.
                if (waveStream != null)
                {
                    // only dispose our source if we created it
                    if (ownInput)
                    {
                        waveStream.Close();
                    }
                    waveStream = null;
                }
            }
            else
            {
                Debug.Assert(false, "WaveFileReader was not disposed");
            }
            // Release unmanaged resources.
            // Set large fields to null.
            // Call Dispose on your base class.
            base.Dispose(disposing);
        }

        /// <summary>
        ///     Reads bytes from the Wave File
        ///     <see cref="Stream.Read" />
        /// </summary>
        public override int Read(byte[] array, int offset, int count)
        {
            if (count % waveFormat.BlockAlign != 0)
            {
                throw new ArgumentException(String.Format(
                    "Must read complete blocks: requested {0}, block align is {1}", count, WaveFormat.BlockAlign));
            }
            // sometimes there is more junk at the end of the file past the data chunk
            if (Position + count > dataChunkLength)
            {
                count = (int)(dataChunkLength - Position);
            }
            return waveStream.Read(array, offset, count);
        }

        /// <summary>
        ///     Attempts to read a sample into a float. n.b. only applicable for uncompressed formats
        ///     Will normalise the value read into the range -1.0f to 1.0f if it comes from a PCM encoding
        /// </summary>
        /// <returns>False if the end of the WAV data chunk was reached</returns>
        public bool TryReadFloat(out float sampleValue)
        {
            sampleValue = 0.0f;
            // 16 bit PCM data
            if (waveFormat.BitsPerSample == 16)
            {
                var value = new byte[2];
                int read = Read(value, 0, 2);
                if (read < 2)
                    return false;
                sampleValue = BitConverter.ToInt16(value, 0) / 32768f;
                return true;
            }
            // 24 bit PCM data
            if (waveFormat.BitsPerSample == 24)
            {
                var value = new byte[4];
                int read = Read(value, 0, 3);
                if (read < 3)
                    return false;
                if (value[2] > 0x7f)
                {
                    value[3] = 0xff;
                }
                else
                {
                    value[3] = 0x00;
                }
                sampleValue = BitConverter.ToInt32(value, 0) / (float)(0x800000);
                return true;
            }
            // 32 bit PCM data
            if (waveFormat.BitsPerSample == 32 && waveFormat.Encoding == WaveFormatEncoding.Extensible)
            {
                var value = new byte[4];
                int read = Read(value, 0, 4);
                if (read < 4)
                    return false;
                sampleValue = BitConverter.ToInt32(value, 0) / (Int32.MaxValue + 1f);
                return true;
            }
            // IEEE float data
            if (waveFormat.BitsPerSample == 32 && waveFormat.Encoding == WaveFormatEncoding.IeeeFloat)
            {
                var value = new byte[4];
                int read = Read(value, 0, 4);
                if (read < 4)
                    return false;
                sampleValue = BitConverter.ToSingle(value, 0);
                return true;
            }
            throw new ApplicationException("Only 16, 24 or 32 bit PCM or IEEE float audio data supported");
        }
    }
}