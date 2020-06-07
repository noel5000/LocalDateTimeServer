using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace TimeServer.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var restClient = new ClientRestFul("http://worldtimeapi.org/api/timezone/Africa/Casablanca", HttpVerb.GET, "");
            var result = restClient.MakeRequest();

            SYSTEMTIME st = new SYSTEMTIME();
            st.wYear = 2009; // must be short
            st.wMonth = 1;
            st.wDay = 1;
            st.wHour = 0;
            st.wMinute = 0;
            st.wSecond = 0;

            SetSystemTime(ref st);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }

}
