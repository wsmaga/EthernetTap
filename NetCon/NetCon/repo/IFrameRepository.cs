using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.repo
{
    interface IFrameRepository<T>
    {

        void applyFilters(FiltersConfiguration<T> config);
        void startCapture();
        void stopCapture();

        //TODO dodać metodę dodającą observera dla ramki 
    }
}
