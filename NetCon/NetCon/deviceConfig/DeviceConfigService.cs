using NetCon.inter;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.deviceConfig
{
    class DeviceConfigService
    {
        private INetCon netCon;

        public DeviceConfigService(INetCon netCon)
        {
            this.netCon = netCon;
        }

        public void EnableEEE()
        {
            for (int i = 1; i <= 4; i++)
            {
                setEEEForPort(i, true);
            }
        }

        public void DisableEEE()
        {
            for (int i = 1; i <= 4; i++)
            {
                setEEEForPort(i, false);
            }
        }

        public void SetEthernetParametersForPort(int port, EthernetSpeed speed)
        {
            switch (speed)
            {
                case EthernetSpeed.SPEED_10:
                    netCon.sendMdio(port, 0, 256);
                    break;
                case EthernetSpeed.SPEED_100:
                    netCon.sendMdio(port, 0, 8448);
                    break;
                case EthernetSpeed.AUTONEGOTIATION:
                    netCon.sendMdio(port, 0, 4928);
                    break;
                    
            }
        }

        public void ResetDevice()
        {
            for(int i=1; i<=4; i++)
            {
                netCon.sendMdio(i, 0, 37184);
            }
        }

        private void setEEEForPort(int port, bool isEnabled)
        {
            netCon.sendMdio(port, 13, 7);
            netCon.sendMdio(port, 14, 60);
            netCon.sendMdio(port, 13, 16391);

            if (isEnabled)
            {
                netCon.sendMdio(port, 14, 6);
            } else
            {
                netCon.sendMdio(port, 14, 0);

            }
        }

    }
}
