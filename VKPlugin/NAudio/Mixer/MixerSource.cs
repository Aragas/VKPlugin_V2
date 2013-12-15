// created on 10/12/2002 at 21:00
using System;
using System.Runtime.InteropServices;

namespace NAudio.Mixer
{
    /// <summary>
    /// Represents a Mixer source
    /// </summary>
    public class MixerSource
    {
        private MixerInterop.MIXERLINE mixerLine;
        private IntPtr mixerHandle;
        private int nDestination;
        private int nSource;

        /// <summary>
        /// Creates a new Mixer Source
        /// </summary>
        /// <param name="nMixer">Mixer ID</param>
        /// <param name="nDestination">Destination ID</param>
        /// <param name="nSource">Source ID</param>
        public MixerSource(IntPtr mixerHandle, int nDestination, int nSource)
        {
            mixerLine = new MixerInterop.MIXERLINE();
            mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
            mixerLine.dwDestination = nDestination;
            mixerLine.dwSource = nSource;
            this.mixerHandle = mixerHandle;
            this.nDestination = nDestination;
            this.nSource = nSource;
        }

        /// <summary>
        /// Source Name
        /// </summary>
        public String Name
        {
            get
            {
                return mixerLine.szName;
            }
        }

        /// <summary>
        /// Source short name
        /// </summary>
        public String ShortName
        {
            get
            {
                return mixerLine.szShortName;
            }
        }

        /// <summary>
        /// Number of controls
        /// </summary>
        public int ControlsCount
        {
            get
            {
                return mixerLine.cControls;
            }
        }

        /// <summary>
        /// Retrieves the specified control
        /// </summary>
        /// <param name="nControl"></param>
        /// <returns></returns>
        public MixerControl GetControl(int nControl)
        {
            if (nControl < 0 || nControl >= ControlsCount)
            {
                throw new ArgumentOutOfRangeException("nControl");
            }
            return null;
        }

        /// <summary>
        /// Number of channels
        /// </summary>
        public int Channels
        {
            get
            {
                return mixerLine.cChannels;
            }
        }
    }
}