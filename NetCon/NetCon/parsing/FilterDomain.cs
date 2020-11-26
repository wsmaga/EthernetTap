using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetCon.parsing
{
    public class FilterDomain
    {
        public PredicateTree Filter;
        public List<TargetDomain> Targets;
        public readonly int MaxIndex;
        public static FilterDomain New(FilterDto filterDto)
        {
            if (filterDto == null || string.IsNullOrWhiteSpace(filterDto.Condition) || filterDto.Targets == null || filterDto.Targets.Count == 0)
                throw new ArgumentNullException("Some Ting Null");
            var filter = FrameParser.CalculatePredicate(filterDto.Condition) ?? throw new ArgumentException("Could not calculate predicate coz its probably shit");
            var results = new List<TargetDomain>();
            foreach (var dtoTarget in filterDto.Targets)
            {
                results.Add(TargetDomain.New(dtoTarget) ?? throw new ArgumentException("Could not parse target to domain object coz its probably shit"));
            }
            var maxIndex = CalculateMaxIndex(filterDto);
            return new FilterDomain(filter, results, maxIndex);
        }
        private FilterDomain(PredicateTree filter, List<TargetDomain> targets, int maxIndex)
        {
            Filter = filter;
            Targets = targets;
            MaxIndex = maxIndex;
        }
        private static int CalculateMaxIndex(FilterDto filterDto)
        {
            var targetsMaxIndex = filterDto.Targets.SelectMany(x => x.Bytes).Max();
            Regex conditionIndexes = new Regex(@"(?<=\[)\d+(?=\])");
            var conditionMaxIndex = conditionIndexes.Matches(filterDto.Condition).Cast<Match>().Select(match => int.Parse(match.Value)).Max();
            return targetsMaxIndex > conditionMaxIndex ? targetsMaxIndex : conditionMaxIndex;
            //dear future me: good luck understanding whats going on
        }
    }
    public class TargetDomain
    {
        public int[] Bytes;
        public DataType Type;
        public string Name;
        public static TargetDomain New(TargetDto targetDto)
        {
            if (targetDto == null||string.IsNullOrWhiteSpace(targetDto.Name))
                return null;
            DataType type=DataType.NONE;
            switch(targetDto.Type)
            {
                case "bytes": type = DataType.Bytes;break;
                default: return null;
            }
            if (targetDto.Bytes == null || targetDto.Bytes.Length == 0)
                return null;
            return new TargetDomain(targetDto.Bytes, type);
        }
        private TargetDomain(int[] bytes, DataType type)
        {
            Bytes = bytes;
            Type = type;
        }
        //possibly json??
        public KeyValuePair<string,object> GetTargetData(Frame frame)
        {
            switch(Type)
            {
                case DataType.Bytes:
                    var data = new List<byte>();
                    foreach (var index in Bytes)
                        data.Add(frame.RawData[index]);
                    return new KeyValuePair<string, object>(Name, data.ToArray());
                    break;
                default: throw new ArgumentException("Target type data extraction not implemented");
            }
        }
    }
}
