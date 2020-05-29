using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.export
{
    class WriterNotInitializedException : Exception
    {
        public override string Message => "Writer not initialized. Invoke InitWrite on PcapWriter before writing";
    }
    interface IPcapWriter<T>
    {
        void InitWrite(string fileName);
        void EndWrite();
        void WriteFrame(T frame);
        bool isInitialized { get; }
    }
}
