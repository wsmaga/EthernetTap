using NetCon.inter;
using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.repo
{
    class FrameRepositoryImpl : IFrameRepository<Frame>
    {
        //Singleton boilerplate
        public static FrameRepositoryImpl instance { get; } = new FrameRepositoryImpl();

        //TODO tu można zmienić implementację przechwytywacza ramek na jakiś mock   //////////////
        private INetCon netConService = new NetConImpl();


        //TODO zadeklarować zmienną typu observable frame i aktualizować ją gdy przyjdzie callback

        //TODO można zrobić zmienną typu jakiegoś enum, która będzie informowała observerów o stanie przechwytywania (konfiguracje mdio, rozpoczęcie, etc. oraz błędy)

        FiltersConfiguration<Frame> filtersConfig = new FiltersConfiguration<Frame>();
        private FrameRepositoryImpl()
        {
            //zadeklarowanie naszego callbacka tutaj!   /////////////
            netConService.setOnFrameListener((IntPtr arr, int size) =>
            {
                byte[] data = new byte[size];
                Marshal.Copy(arr, data, 0, size);       //Kopiowanie danych. Może być problem z wydajnością w RT !!


                //TODO zaimplementować konwersję byte blob do obiektu klasy Frame. Wysłać przekonwertowaną ramkę do observerów

                return data.Length; //TODO ??? policzyć ile faktycznie bajtów odebrano i zwrócić tu! 
            });
        }

        public void applyFilters(FiltersConfiguration<Frame> config)
        {
            filtersConfig = config;
        }

        public async void startCapture()
        {
            try
            {
                netConService.sendRequest(RequestCode.BRIDGE_SWITCH, 1, true);
                netConService.sendRequest(RequestCode.BRIDGE_SWITCH, 2, true);
                netConService.sendRequest(RequestCode.BRIDGE_SWITCH, 3, true);
                netConService.sendRequest(RequestCode.BRIDGE_SWITCH, 4, true);

                netConService.sendAndReceiveMdio();

                string[] strVec = new string[] { "NetCon.exe", "set", "3", "0" };
                netConService.sendSettings(strVec);
                netConService.startCapture();
            }
            catch(Exception e)
            {
                //TODO obsużyć błąd

            }
            
        }

        public void stopCapture()
        {
            netConService.stopCapture();
        }

       
    }
}
