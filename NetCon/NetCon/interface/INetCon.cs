using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.inter
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FrameListener(IntPtr frame, int array_size);
    interface INetCon
{
        void startCapture();
        void stopCapture();
        void sendFilter();

        void setOnFrameListener(FrameListener f);
}
}
