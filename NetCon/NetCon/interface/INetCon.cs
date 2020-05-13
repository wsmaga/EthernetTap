using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.inter
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void FrameListener(char frame);
    interface INetCon
{
        void startCapture();
        void stopCapture();
        void sendFilter();

        void setOnFrameListener(FrameListener f);
}
}
