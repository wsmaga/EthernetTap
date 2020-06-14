using Kaitai;
using NetCon.model;
using NetCon.repo;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Linq;


namespace NetCon.parsing
{
    class FrameParser
    {
        public List<Frame> AllFrames = new List<Frame>() ;
        private string CurrFrame = "";
        private int FrameLength=-1;
        private enum FType {NONE, IPV4, ARP};
        private FType FrameType=FType.NONE;
        public Subject<EthernetFrame> EthernetFrameSubject=new Subject<EthernetFrame>();

        FiltersConfiguration<EthernetFrame> filtersConfig;
        public FrameParser(Subject<Frame> _subject)
        {
            FiltersConfiguration<EthernetFrame>.Builder builder = new FiltersConfiguration<EthernetFrame>.Builder();
            builder.AddFilter(new Filter<EthernetFrame>(frame => frame.EtherType==EthernetFrame.EtherTypeEnum.Arp)); //Enumerable.SequenceEqual(frame.SrcMac,new byte[] {0,1,5,27,159,150}))
            filtersConfig = new FiltersConfiguration<EthernetFrame>(builder);
            new SubjectObserver<Frame>(Frame =>
            {
                ParseFrame(BitConverter.ToString(Frame.RawData).Replace("-", "").ToLower());
            }).Subscribe(_subject);
        }
        private void ParseFrame(string rawDataString)
        {
            //string rawDataString = BitConverter.ToString(rawData).Replace("-", "").ToLower();
            if(CurrFrame.Length==0)
            {
                int index = rawDataString.IndexOf("55555555555555d5");
                if (index!=-1)
                {
                    string tempFrame = rawDataString.Substring(index);
                    GroupFrame(tempFrame);
                }
            }
            else
            {
                string tempFrame = CurrFrame+rawDataString;
                GroupFrame(tempFrame);
            }
        }
        private void ResetCurrFrame()
        {
            CurrFrame = "";
            FrameLength = -1;
            FrameType = FType.NONE;
        }
        private void GroupFrame(string tempFrame)
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
                    SaveFrame(CurrFrame);

            }
            else
            {
                CurrFrame = tempFrame.Substring(0, FrameLength);
                string remainingData = tempFrame.Substring(FrameLength);
                SaveFrame(CurrFrame);
                ParseFrame(remainingData);
            }
        }
        private void SaveFrame(string Frame)
        {
            AllFrames.Add(new Frame(Enumerable.Range(0, Frame.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(Frame.Substring(x, 2), 16))
                     .ToArray()));
            EthernetFrame temp = new EthernetFrame(new KaitaiStream(Enumerable.Range(16, Frame.Length - 16 - 8)
                      .Where(x => x % 2 == 0)
                      .Select(x => Convert.ToByte(Frame.Substring(x, 2), 16))
                      .ToArray()));
            //EthernetFrameSubject.pushNextValue(temp);
            FilterFrame(temp);
            ResetCurrFrame();
        }
        private void FilterFrame(EthernetFrame frame)
        {
            if (filtersConfig.pass(frame))
                EthernetFrameSubject.pushNextValue(frame);
        }
    }
}
