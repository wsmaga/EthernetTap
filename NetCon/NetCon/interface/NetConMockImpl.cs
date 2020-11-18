using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCon.inter
{
    class NetConMockImpl : INetCon
    {
        private FrameListener frameListener;
        private Boolean shouldWork;
        private Boolean captureState;

        List<Byte[]> frames = new List<Byte[]>();

        private IntPtr mockDataPtr;
        public void sendAndReceiveMdio() { }
        public void sendFilter() { }
        public void sendRequest(RequestCode code, int port, bool value) { }
        public void sendSettings(string[] settings) { }

        public void setCaptureState(bool state) { this.captureState = state; }
        public void setOnFrameListener(FrameListener f)
        {
            frameListener = f;
        }

        public void startCapture()
        {
            loadData();
            shouldWork = true;
            foreach(var frame in frames)
            {
                if (!shouldWork)
                {
                    break;
                }

                mockDataPtr = Marshal.AllocHGlobal(frame.Length);   //memory leak !
                Marshal.Copy(frame, 0, mockDataPtr, frame.Length);
                if (captureState)
                {
                    frameListener(mockDataPtr, frame.Length);
                }
                Thread.Sleep(10);
            }
        }

        public void stopCapture()
        {
            shouldWork = false;
            Marshal.FreeHGlobal(mockDataPtr);
        }

        private void loadData()
        {
            Byte[] data = File.ReadAllBytes("./mock.pcap");
            List<Byte> frame = new List<Byte>();
            pushFrameHeader(frame);
            for (int i = 25; i < data.Length; i++){
                if(data[i] == 0x55)
                {
                    Byte[] headerCandidate = new Byte[8];
                    Array.ConstrainedCopy(data, i-1, headerCandidate, 0, 8);
                    if (checkForHeader(headerCandidate))
                    {
                        frames.Add(frame.ToArray());
                        frame = new List<Byte>();
                        pushFrameHeader(frame);
                        i += 7;
                        continue;
                    }
                    else
                    {
                        frame.Add(data[i]);
                    }
                }
                else
                {
                    frame.Add(data[i]);
                }
            }
        }

        private void pushFrameHeader(List<Byte> frame)
        {
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0x55);
            frame.Add(0xd5);
        }

        private bool checkForHeader(Byte[] headerCandidate)
        {
            bool isHeader = true;
            for (int j = 0; j < 7; j++)
            {
                isHeader &= headerCandidate[j] == 0x55;
            }
            isHeader &= headerCandidate[7] == 0xd5;
            return isHeader;
        }
    }
}
