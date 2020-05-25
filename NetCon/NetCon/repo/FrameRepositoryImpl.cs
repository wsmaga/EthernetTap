using NetCon.inter;
using NetCon.model;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace NetCon.repo
{
    class FrameRepositoryImpl : IFrameRepository<Frame>
    {
        //Singleton boilerplate
        public static FrameRepositoryImpl instance { get; } = new FrameRepositoryImpl();

        private Subject<Frame> _subject = new Subject<Frame>();
        public Subject<Frame> subject => _subject;

        //TODO tu można zmienić implementację przechwytywacza ramek na jakiś mock   //////////////
        private INetCon netConService = new NetConImpl();

        //TODO można zrobić zmienną typu jakiegoś enum, która będzie informowała observerów o stanie przechwytywania (konfiguracje mdio, rozpoczęcie, etc. oraz błędy)

        //Konfiguracja filtrów
        FiltersConfiguration<Frame> filtersConfig = new FiltersConfiguration<Frame>();

        //Callbacki "observerów"
        private List<CurrentFrameListener<Frame>> currentFrameListeners = new List<CurrentFrameListener<Frame>>();
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
            for (int i = 0; i < 100; i++) {
                _subject.pushNextValue(new Frame());
            }
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

        private void notifyListeners(Frame frame)
        {
            foreach(var listener in currentFrameListeners)
            {
                listener(frame);
            }
        }

        public void stopCapture()
        {
            netConService.stopCapture();
        }
    }
}
