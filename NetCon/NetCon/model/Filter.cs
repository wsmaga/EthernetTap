using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    //TODO utworzyć klasy dla różnych rodzajów filtrów.
    public class Filter<T>
    {
        Predicate<T> filterPredicate;
        public Filter(Predicate<T> pred)
        {
            filterPredicate = pred;
        }
        public bool pass(T frame)
        {
            return filterPredicate(frame);
        }
        
    }
}
