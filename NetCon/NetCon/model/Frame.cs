using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    public class Frame
    {
        public Frame(byte[] data)
        {
            rawData = data;
        }

        private byte[] rawData;
        public byte[] RawData { get { return rawData; } }
    }
}
