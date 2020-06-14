using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.parsing
{
    class FrameParser
    {
        public List<Frame> AllFrames = new List<Frame>() ;
        private string CurrFrame = "";
        private int FrameLength=-1;
        private enum FType {NONE, IPV4, ARP};
        private FType FrameType=FType.NONE;
        public void SendFrame(string rawDataString)
        {
            //string rawDataString = BitConverter.ToString(rawData).Replace("-", "").ToLower();
            if(CurrFrame.Length==0)
            {
                int index = rawDataString.IndexOf("55555555555555d5");
                if (index!=-1)
                {
                    string tempFrame = rawDataString.Substring(index);
                    ParseFrame(tempFrame);
                }
            }
            else
            {
                string tempFrame = CurrFrame+rawDataString;
                ParseFrame(tempFrame);
            }
        }
        private void ResetCurrFrame()
        {
            CurrFrame = "";
            FrameLength = -1;
            FrameType = FType.NONE;
        }
        private void ParseFrame(string tempFrame)
        {
            if (tempFrame.Length >= 52)
            {
                string FrameTypeHex = tempFrame.Substring(40, 4);
                switch (FrameTypeHex)
                {
                    case "0800":
                        FrameType = FType.IPV4;
                        int dataLength;
                        if (tempFrame.Length > 52)
                            dataLength = Convert.ToInt32(tempFrame.Substring(48, 4), 16) * 2;
                        else
                            dataLength = Convert.ToInt32(tempFrame.Substring(48), 16) * 2;
                        FrameLength = 52 + dataLength;
                        HandleFrameData(tempFrame);
                        break;

                    case "0806":
                        FrameType = FType.ARP;
                        FrameLength = 144; //doswiadczalnie wyznaczona liczba 
                        HandleFrameData(tempFrame);
                        break;
                    default:
                        ResetCurrFrame();
                        break;
                }
            }
            else
                CurrFrame = tempFrame;
        }
        private void HandleFrameData(string tempFrame)
        {
            if (tempFrame.Length <= FrameLength)
            {
                CurrFrame = tempFrame;
                if (tempFrame.Length == FrameLength)
                {

                    AllFrames.Add(new Frame(Enumerable.Range(0, CurrFrame.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(CurrFrame.Substring(x, 2), 16))
                     .ToArray()));
                    ResetCurrFrame();
                }

            }
            else
            {
                CurrFrame = tempFrame.Substring(0, FrameLength);
                string remainingData = tempFrame.Substring(FrameLength);
                AllFrames.Add(new Frame(Enumerable.Range(0, CurrFrame.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(CurrFrame.Substring(x, 2), 16))
                     .ToArray()));
                ResetCurrFrame();
                SendFrame(remainingData);
            }
        }
    }
}
