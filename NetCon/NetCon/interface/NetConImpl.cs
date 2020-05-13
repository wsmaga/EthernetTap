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


        private FrameListener fl = (char fr) => { 
            Console.WriteLine(fr); 
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
