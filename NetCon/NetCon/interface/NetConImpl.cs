using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.inter
{
    class NetConImpl : INetCon
    {

        //Ciało callbacku
        private FrameListener fl = (IntPtr arr, int size) => {
            byte[] data = new byte[size];
            Marshal.Copy(arr, data, 0, size);       //Kopiowanie danych. Może być problem z wydajnością w RT !!
            String dataToStr = System.Text.Encoding.Default.GetString(data);
            Console.WriteLine("");
            return data.Length; //TODO policzyć ile faktycznie bajtów odebrano i zwrócić tu! 
        };

        [DllImport(".\\..\\..\\..\\Debug\\NETCONLIB.dll",EntryPoint ="startCapture", CallingConvention = CallingConvention.Cdecl)]
        private static extern void _startCapture(
            int port,
            [MarshalAs(UnmanagedType.FunctionPtr)]
            FrameListener listener, 
            int bufSize);

        [DllImport(".\\..\\..\\..\\Debug\\NETCONLIB.dll", EntryPoint = "stopCapture")]
        private static extern int _stopCapture();

        public void sendFilter()
        {
            throw new NotImplementedException();
        }

        public void setOnFrameListener(FrameListener f)
        {
            fl = f;
        }

        public void startCapture()
        {
            _startCapture(1, fl, 1024 * 1024 * 16);
        }

        public void stopCapture()
        {
            var ret = _stopCapture();
        }
    }
}
