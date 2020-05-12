using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.inter
{
    public delegate void FrameListener(int frame);
    interface INetCon
{
        void startCapture();
        void stopCapture();
        void sendFilter();

        void setOnFrameListener(FrameListener f);
}
}
