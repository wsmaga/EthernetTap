using System;
using System.Runtime.InteropServices;

namespace NetCon.inter
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int FrameListener(IntPtr frame, int array_size);

    enum RequestCode{
        CAPTURE_SWITCH,
        BRIDGE_SWITCH
    };
    interface INetCon
{
        void startCapture();
        void stopCapture();
        void sendFilter();

        void setCaptureState(bool state);

        void sendRequest(RequestCode code, int port, bool value);

        void sendSettings(string[] settings);

        void sendAndReceiveMdio();

        void setOnFrameListener(FrameListener f);
}
}
