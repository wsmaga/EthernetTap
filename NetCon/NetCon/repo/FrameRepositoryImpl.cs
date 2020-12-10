using NetCon.inter;
using NetCon.model;
using NetCon.util;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NetCon.repo
{
    class FrameRepositoryImpl : IFrameRepository<Frame>
    {
        public static FrameRepositoryImpl instance { get; } = getRepository();

       
        private static FrameRepositoryImpl getRepository()
        {
            var config = ConfigFileHandler<ApplicationConfig>.ReadSettings();
            
            switch(config.Source)
            {
                case ApplicationConfig.FrameSource.MOCK: return new FrameRepositoryImpl(new NetConMockImpl());
                case ApplicationConfig.FrameSource.NET_FPGA: return new FrameRepositoryImpl(new NetConImpl());
                default: throw new ArgumentException("Unknown frames source");
            }
        }


        private Subject<Frame> _subject = new Subject<Frame>();
        public Subject<Frame> FrameSubject => _subject;
        private Subject<CaptureState> captureState = new Subject<CaptureState>();
        public Subject<CaptureState> CaptureState => captureState;
        //TODO tu można zmienić implementację przechwytywacza ramek na jakiś mock   //////////////
        // private INetCon netConService = new NetConImpl();
        private INetCon netConService = null;

        //TODO można zrobić zmienną typu jakiegoś enum, która będzie informowała observerów o stanie przechwytywania (konfiguracje mdio, rozpoczęcie, etc. oraz błędy)

        //Konfiguracja filtrów


        internal FrameRepositoryImpl(INetCon netcon)
        {
            netConService = netcon;

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
                await Task.Run(() => netConService.startCapture());

            }
            catch (Exception e)
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
