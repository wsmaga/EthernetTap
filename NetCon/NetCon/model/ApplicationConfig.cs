namespace NetCon.model
{
    public class ApplicationConfig
    {
        public int port;
        public int bufferSize;

        private FrameSource source;
        //TODO dodać konfigurację filtrów wysyłanych do FPGA jeśli będzie to konieczne

        public FrameSource Source
        {
            get {  return source; }

            set { source = value; }
        }

        public enum FrameSource{
            NET_FPGA,
            MOCK
        }
    }
}
