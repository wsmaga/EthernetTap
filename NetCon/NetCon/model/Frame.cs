using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    public enum DataType { NONE, ByteArray, Integer};
    public class Frame
    {
        //public DataType usefulDataType = DataType.NONE;
        public Frame(byte[] data)
        {
            rawData = data;
        }
        //public byte[] usefulData { get; set; }
        //nie usuwać tego
        private byte[] rawData;
        public byte[] RawData { get { return rawData; } }
    }
}
