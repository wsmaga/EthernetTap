using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NetCon.parsing.TargetDomain;

namespace NetCon.parsing
{
    public class TargetDataDto
    {
        public string Name;
        public dynamic Value;
        public string DataType; //change to enum after presentation
        public byte[] RawData;
        public bool TriggeredThreshold;
        public dynamic ThresholdValue;
        public string ThresholdType; //change to enum after presentation
        public bool RegisterChanges;
    }
}
