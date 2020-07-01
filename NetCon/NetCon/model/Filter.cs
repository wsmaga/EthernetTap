using NetCon.parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
    //TODO utworzyć klasy dla różnych rodzajów filtrów.
    public class Filter
    {
        public int maxIndex { private set; get; }
        public DataType targetDataType { private set; get; }
        public int[] targetIndexes { private set; get; }
        private PredicateTree filter;
        public Filter(PredicateTree pred, int max_index, int[] target_indexes, DataType data_type)
        {
            filter = pred;
            maxIndex = max_index;
            targetIndexes = target_indexes;
            targetDataType = data_type;
        }
        public bool pass(Frame frame)
        {
            if (frame.RawData.Length <= maxIndex)
                return false;
            else
                return filter.Pass(frame);
        }

        
    }
}
