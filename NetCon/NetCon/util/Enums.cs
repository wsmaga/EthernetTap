namespace NetCon.util
{
    public enum DataType
    {
        NONE,
        ByteArray,
        String,
        Boolean,
        Int16,
        Int32,
        Int64,
        Single,
        Double
    };
    public enum ThresholdType
    {
        NONE,
        GT,
        LT,
        GE,
        LE,
        InOpen,
        InClosed,
        OutOpen,
        OutClosed
    }

    public enum DBConnectionStatus
    {
        OK,
        BadServer,
        BadDB
    }
}
