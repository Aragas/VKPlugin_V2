using System;
using System.Runtime.InteropServices;
using Rainmeter;

namespace Plugin
{
    public static class Plugin
    {
        static IntPtr StringBuffer = IntPtr.Zero;

        [DllExport]
        public static void Initialize(ref IntPtr data, IntPtr rm)
        {
            data = GCHandle.ToIntPtr(GCHandle.Alloc(new MeasuresHandler(new Rainmeter.API(rm))));
        }

        [DllExport]
        public static void Finalize(IntPtr data, IntPtr rm)
        {
            //MeasureHandler measure = (MeasureHandler)GCHandle.FromIntPtr(data).Target;
            //measure.Finalize(new Rainmeter.API(rm));

            GCHandle.FromIntPtr(data).Free();

            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }
        }

        [DllExport]
        public static void Reload(IntPtr data, IntPtr rm, ref double maxValue)
        {
            MeasuresHandler measure = (MeasuresHandler)GCHandle.FromIntPtr(data).Target;
            measure.Reload(new Rainmeter.API(rm), ref maxValue);
        }

        [DllExport]
        public static double Update(IntPtr data)
        {
            MeasuresHandler measure = (MeasuresHandler)GCHandle.FromIntPtr(data).Target;
            return measure.GetDouble();
        }

        [DllExport]
        public static IntPtr GetString(IntPtr data)
        {
            MeasuresHandler measure = (MeasuresHandler)GCHandle.FromIntPtr(data).Target;
            if (StringBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(StringBuffer);
                StringBuffer = IntPtr.Zero;
            }

            string stringValue = measure.GetString();
            if (stringValue != null)
            {
                StringBuffer = Marshal.StringToHGlobalUni(stringValue);
            }

            return StringBuffer;
        }

        [DllExport]
        public static void ExecuteBang(IntPtr data, IntPtr args)
        {
            MeasuresHandler measure = (MeasuresHandler)GCHandle.FromIntPtr(data).Target;
            measure.ExecuteBang(Marshal.PtrToStringUni(args));
        }
    }
}
