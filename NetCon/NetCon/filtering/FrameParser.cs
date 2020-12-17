using Kaitai;
using NetCon.model;
using NetCon.repo;
using NetCon.util;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetCon.filtering
{
    class FrameParser
    {
        public List<Frame> AllFrames = new List<Frame>() ;
        private string CurrFrame = "";
        private int FrameLength=-1;
        private enum FType {NONE, IPV4, ARP, EtherCAT };
        private FType FrameType=FType.NONE;
        public TargetDataRepository TargetDataRepo=TargetDataRepository.GetInstance();

        private FiltersConfiguration filtersConfig;
        public FrameParser(Subject<Frame> _subject)
        {
            new SubjectObserver<Frame>(Frame =>
            {
                ParseFrame(BitConverter.ToString(Frame.RawData).Replace("-", "").ToLower());
            }).Subscribe(_subject);
        }
        private void ParseFrame(string rawDataString)
        {
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
                    case "88a4":
                        FrameType = FType.EtherCAT;
                        FrameLength = 144;
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
            Frame temp = new Frame(Enumerable.Range(0, Frame.Length)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(Frame.Substring(x, 2), 16))
                     .ToArray());
            AllFrames.Add(temp);
            FilterFrame(temp);
            ResetCurrFrame();
        }
        private void FilterFrame(Frame frame)
        {
            if (filtersConfig != null)
            { 
                IFilter passedFilter = filtersConfig.pass(frame);
                if (passedFilter!=null)
                {
                    var data = passedFilter.GetUsefulData(frame);
                    foreach(var d in data)
                        TargetDataRepo.FrameDataSubject.pushNextValue(d);
                }
            }
        }
        
        public void LoadFilterConfiguration(FiltersConfiguration configuration)
        {
            filtersConfig = configuration;
        }

        

        static public PredicateTree CalculatePredicate(string input)
        {
            Operation op = Operation.NONE;
            string inputCopy;
            if(input.StartsWith("And(")&&input.EndsWith(")"))
            {
                op = Operation.AND;
                inputCopy = input.Substring(4, input.Length - 5);
            }
            else if(input.StartsWith("Or(") && input.EndsWith(")"))
            {
                op = Operation.OR;
                inputCopy = input.Substring(3, input.Length - 4);
            }
            else
            {
                return null;
            }
           
            List<string> expressions = new List<string>();
            int bracketNo = 0;
            int indexOffset = 0;
            for(int i=0;i<inputCopy.Length;i++)
            {
                char ztest=inputCopy[i];
                switch(inputCopy[i])
                {
                    case ',':
                        if(bracketNo==0)
                        {
                            expressions.Add(inputCopy.Substring(0+indexOffset, i-indexOffset));
                            indexOffset = i + 1;
                        }
                        break;
                    case '(':
                        bracketNo++;
                        break;
                    case ')':
                        bracketNo--;
                        break;
                }
                if (i == inputCopy.Length - 1 && bracketNo == 0)
                {
                    expressions.Add(inputCopy.Substring(0 + indexOffset, i - indexOffset + 1));
                }
            }
            if (expressions.Count == 0)
                return null;
            PredicateTree[] predicates = new PredicateTree[expressions.Count];
            Regex rCanCalculate = new Regex(@"^\[\d+\]=[0-9,a-f]{2}$");
            for(int i=0; i<predicates.Length;i++)
            {
                if(rCanCalculate.IsMatch(expressions[i]))
                {
                    Regex rArguments = new Regex(@"[0-9,a-f]+");
                    MatchCollection mCol = rArguments.Matches(expressions[i]);
                    int arg1 = int.Parse(mCol[0].Value);
                    byte arg2 = byte.Parse(mCol[1].Value,System.Globalization.NumberStyles.HexNumber);
                    predicates[i] = new PredicateTree(new Predicate<Frame>(frame=>frame.RawData[arg1].Equals(arg2)));
                }
                else
                {
                    PredicateTree recurRes=CalculatePredicate(expressions[i]);
                    if (recurRes == null)
                        return null;
                    else
                        predicates[i] = recurRes;
                }
            }
            /*List<PredicateTree> functionResult=new List<PredicateTree>();
            for(int i=1;i<predicates.Length;i++)
            {
                switch(op)
                {
                    case Operation.OR:
                        functionResult = new Predicate<Frame>(frame => functionResult(frame) || predicates[i](frame));
                        break;
                    case Operation.AND:
                        functionResult = new Predicate<Frame>(frame => functionResult(frame) && predicates[i](frame));
                        break;
                }
            }*/
            return new PredicateTree(predicates,op);

        }
    }
}
