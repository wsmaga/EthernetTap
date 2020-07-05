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
        public Subject<Frame> FrameSubject => _subject;
        private Subject<Frame> _ethernetFrameSubject = new Subject<Frame>();
        public Subject<Frame> EthernetFrameSubject => _ethernetFrameSubject;
        private Subject<CaptureState> captureState = new Subject<CaptureState>();
        public Subject<CaptureState> CaptureState => captureState;
        //TODO tu można zmienić implementację przechwytywacza ramek na jakiś mock   //////////////
        private INetCon netConService = new NetConImpl();

        //TODO można zrobić zmienną typu jakiegoś enum, która będzie informowała observerów o stanie przechwytywania (konfiguracje mdio, rozpoczęcie, etc. oraz błędy)

        //Konfiguracja filtrów
       

        private FrameRepositoryImpl()
        {
            //zadeklarowanie naszego callbacka tutaj!   /////////////
            netConService.setOnFrameListener((IntPtr arr, int size) =>
            {
                byte[] data = new byte[size];
                Marshal.Copy(arr, data, 0, size);       //Kopiowanie danych. Może być problem z wydajnością w RT !!

                //TODO zaimplementować konwersję byte blob do obiektu klasy Frame. Wysłać przekonwertowaną ramkę do observerów
                //TODO przepuścić ramkę przez filtry

                _subject.pushNextValue(new Frame(data));

                return data.Length;
            });
        }

        //Deprecated configuracja i filtrowanie przeniesiona do FrameParsera
        /*public void applyFilters(FiltersConfiguration<Frame> config)
        {
            filtersConfig = config;
        }*/

        public async void InitCapture()
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
                captureState.pushNextValue(new repo.CaptureState.CaptureInitialized());
                await Task.Run(()=>netConService.startCapture());
                
            }
            catch(Exception e)
            {
                captureState.pushNextValue(new repo.CaptureState.CaptureError(e));
            }
            
        }

        public void CloseCapture()
        {
            netConService.stopCapture();
            captureState.pushNextValue(new repo.CaptureState.CaptureClosed());
        }

        public void StartCapture()
        { 
            netConService.setCaptureState(true);
            captureState.pushNextValue(new repo.CaptureState.CaptureOn());
        }

        public void StopCapture()
        {
            netConService.setCaptureState(false);
            captureState.pushNextValue(new repo.CaptureState.CaptureOff());
        }
    }
}
