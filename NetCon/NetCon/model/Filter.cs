using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    //TODO utworzyć klasy dla różnych rodzajów filtrów.
    public abstract class Filter<T>
    {
        public abstract bool pass(T frame);
        
    }
}
