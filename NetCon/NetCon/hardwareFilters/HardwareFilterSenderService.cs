using NetCon.inter;
using NetCon.model;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.hardwareFilters
{
    class HardwareFilterSenderService
    {
        public static HardwareFilterSenderService instance { get; } = getService();
        private static HardwareFilterSenderService getService()
        {
            var config = ConfigFileHandler<ApplicationConfig>.ReadSettings();

            switch (config.Source)
            {
                case ApplicationConfig.FrameSource.MOCK: return new HardwareFilterSenderService(new NetConMockImpl());
                case ApplicationConfig.FrameSource.NET_FPGA: return new HardwareFilterSenderService(new NetConImpl());
                default: throw new ArgumentException("Unknown netcon impl");
            }
        }

        private HardwareFilterSenderService(INetCon _netcon)
        {
            netcon = _netcon;
        }

        private INetCon netcon;

        public void sendFilters(List<string> filterArgs)
        {
            List<string> args =  new List<string> { "NetCon.exe", "set" };
            
            foreach(string arg in filterArgs)
            {
                args.Add(arg);
            }

            netcon.sendSettings(args.ToArray());
        }
    }
}
