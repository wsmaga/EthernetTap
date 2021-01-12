using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    class HardwareFiltersDefinition
    {
        int port;
        int minFrameLength;
        string filter1;
        string filter2;
        string filter3;
        string filter4;

        public int Port { get => port; set => port = value; }
        public int MinFrameLength { get => minFrameLength; set => minFrameLength = value; }
        public string Filter1 { get => filter1; set => filter1 = value; }
        public string Filter2 { get => filter2; set => filter2 = value; }
        public string Filter3 { get => filter3; set => filter3 = value; }
        public string Filter4 { get => filter4; set => filter4 = value; }
    }
}
