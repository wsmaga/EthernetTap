using NetCon.parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.model
{
/*    //DEPRECATED DO NOT USE
    [Serializable]
    public class Filter
    {
        public readonly string filterString;
        public int maxIndex { private set; get; }
        public DataType targetDataType { private set; get; }
        public int[] targetIndexes { private set; get; }
        private PredicateTree filter;
        public Filter(PredicateTree pred, int max_index, int[] target_indexes, DataType data_type, string filter_string)
        {
            filter = pred;
            maxIndex = max_index;
            targetIndexes = target_indexes;
            targetDataType = data_type;
            filterString = filter_string;
        }
        public bool Pass(Frame frame)
        {
            if (frame.RawData.Length <= maxIndex)
                return false;
            else
                return filter.Pass(frame);
        }
        public List<TargetDataDto> GetUsefulData(Frame frame)
        {
            List<TargetDataDto> result = new List<TargetDataDto>();
            var bytes=new byte[targetIndexes.Length];
            for (int i = 0; i < targetIndexes.Length; i++)
            {
                bytes[i] = frame.RawData[targetIndexes[i]];
            }
            result.Add(new TargetDataDto
            {
                RawData=bytes,
                Value=bytes,
                TriggeredThreshold=false,
                ThresholdValue=null
            });
            return result;
        }

    }*/
}
