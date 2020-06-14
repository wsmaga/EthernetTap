using NetCon.util;
using System;

namespace NetCon.repo
{

    public class CaptureState
    {
        public class CaptureOn: CaptureState { }
        public class CaptureOff: CaptureState { }
        public class CaptureInitialized: CaptureState { }
        public class CaptureClosed: CaptureState { }
        public class CaptureError: CaptureState {
            private Exception error;
            public Exception Error { get { return error; } }
            public CaptureError(Exception err)
            {
                error = err;
            }
        }
    }

    interface IFrameRepository<T>
    {
        //void applyFilters(FiltersConfiguration<T> config);
        void InitCapture();
        void CloseCapture();
        void StartCapture();
        void StopCapture();
        Subject<T> FrameSubject { get; }
        Subject<CaptureState> CaptureState { get; }
    }
}
