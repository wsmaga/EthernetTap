using NetCon.model;
using NetCon.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.repo
{
    public delegate void CurrentFrameListener<T>(T frame);
    interface IFrameRepository<T>
    {

        void applyFilters(FiltersConfiguration<T> config);
        void startCapture();
        void stopCapture();

        Subject<T> subject { get; }
    }
}
