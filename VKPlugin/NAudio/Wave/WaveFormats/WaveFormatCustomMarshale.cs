﻿using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
    /// <summary>
    ///     Custom marshaller for WaveFormat structures
    /// </summary>
    public sealed class WaveFormatCustomMarshaler : ICustomMarshaler
    {
        private static WaveFormatCustomMarshaler marshaler;

        /// <summary>
        ///     Clean up managed data
        /// </summary>
        public void CleanUpManagedData(object ManagedObj)
        {
        }

        /// <summary>
        ///     Clean up native data
        /// </summary>
        /// <param name="pNativeData"></param>
        public void CleanUpNativeData(IntPtr pNativeData)
        {
            Marshal.FreeHGlobal(pNativeData);
        }

        /// <summary>
        ///     Get native data size
        /// </summary>
        public int GetNativeDataSize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Marshal managed to native
        /// </summary>
        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            return WaveFormat.MarshalToPtr((WaveFormat)ManagedObj);
        }

        /// <summary>
        ///     Marshal Native to Managed
        /// </summary>
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return WaveFormat.MarshalFromPtr(pNativeData);
        }

        /// <summary>
        ///     Gets the instance of this marshaller
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            if (marshaler == null)
            {
                marshaler = new WaveFormatCustomMarshaler();
            }
            return marshaler;
        }
    }
}