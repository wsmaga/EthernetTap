using NetCon.util;

namespace NetCon.parsing
{
    public class TargetDataRepository
    {
        private static TargetDataRepository instance;
        public static TargetDataRepository GetInstance()
        {
            if (instance == null)
                instance = new TargetDataRepository();
            return instance;
        }
        public Subject<TargetDataDto> FrameDataSubject=new Subject<TargetDataDto>();

    }
}
