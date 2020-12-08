using NetCon.model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NetCon.parsing
{
    public class FilterDomain: IFilter
    {
        public PredicateTree Condition;
        public List<TargetDomain> Targets;
        public readonly int MaxIndex;
        public static FilterDomain New(FilterDto filterDto)
        {
            if (!(filterDto?.IsValid==true))
                throw new ArgumentNullException("One or more property in filterDto is null");
            var filter = FrameParser.CalculatePredicate(filterDto.Condition) ?? throw new ArgumentException("Could not calculate predicate");
            var results = new List<TargetDomain>();
            foreach (var dtoTarget in filterDto.Targets.Items)
            {
                results.Add(TargetDomain.New(dtoTarget) ?? throw new ArgumentException("Could not parse target to domain object"));
            }
            var maxIndex = CalculateMaxIndex(filterDto);
            return new FilterDomain(filter, results, maxIndex);
        }
        private FilterDomain(PredicateTree condition, List<TargetDomain> targets, int maxIndex)
        {
            Condition = condition;
            Targets = targets;
            MaxIndex = maxIndex;
        }
        private static int CalculateMaxIndex(FilterDto filterDto)
        {
            var targetsMaxIndex = filterDto.Targets.Items.SelectMany(x => x.Bytes).Max();
            Regex conditionIndexes = new Regex(@"(?<=\[)\d+(?=\])");
            var conditionMaxIndex = conditionIndexes
                .Matches(filterDto.Condition)
                .Cast<Match>()
                .Select(match => int.Parse(match.Value))
                .Max();
            return targetsMaxIndex > conditionMaxIndex ? targetsMaxIndex : conditionMaxIndex;
        }
        public List<TargetDataDto> GetUsefulData(Frame frame)
        {
            List<TargetDataDto> results = new List<TargetDataDto>();
            foreach(var t in Targets)
            {
                results.Add(t.GetTargetData(frame));
            }
            return results;
        }
        public bool Pass(Frame frame)
        {
            if (frame.RawData.Length <= this.MaxIndex)
                return false;
            else
                return Condition.Pass(frame);
        }
    }
    public class TargetDomain
    {
        public TargetTreshold Treshold;
        public int[] Bytes;
        public DataType Type;
        public string Name;
        public bool RegisterChanges;
        public static TargetDomain New(TargetDto targetDto)
        {
            if (!(targetDto?.IsValid == true))
                return null;
            DataType type=DataType.NONE;
            switch(targetDto.Type.ToLower())
            {
                case "byte_array": type = DataType.ByteArray;break;
                case "integer": type = DataType.Integer;break;
                case "string": type = DataType.String;break;
                case "boolean": type = DataType.Boolean;break;
                default: return null;
            }
            var treshold = TargetTreshold.New(type, targetDto.Treshold);
            return new TargetDomain(targetDto.Name,targetDto.Bytes, type,treshold,targetDto.RegisterChanges);
        }
        private TargetDomain(string name, int[] bytes, DataType type, TargetTreshold treshold, bool registerChanges=false)
        {
            Bytes = bytes;
            Type = type;
            Name = name;
            Treshold = treshold;
            RegisterChanges = registerChanges;
        }

        public TargetDataDto GetTargetData(Frame frame)
        {
            var rawData = new List<byte>();
            foreach (var index in Bytes)
                rawData.Add(frame.RawData[index]);
            dynamic value;
            switch (Type)
            {
                case DataType.ByteArray:
                    value = rawData.ToArray();
                    break;
                case DataType.Integer:
                    var prefixedHex = "0x" + BitConverter.ToString(rawData.ToArray()).Replace("-", "").ToLower();
                    value = Convert.ToInt32(prefixedHex, 16); 
                    break;
                case DataType.String:
                    value = string.Concat(rawData.Select(el=>(char)el));
                    break;
                case DataType.Boolean:
                    value = rawData.Count(el => el == 0) == rawData.Count();
                    break;
                default: throw new ArgumentException("Target type data extraction not implemented");
            }
            string tresholdType;
            switch (Treshold?.Type)
            {
                case TargetTreshold.TresholdType.GT: tresholdType = "gt"; break;
                case TargetTreshold.TresholdType.GE: tresholdType = "ge"; break;
                case TargetTreshold.TresholdType.LT: tresholdType = "lt"; break;
                case TargetTreshold.TresholdType.LE: tresholdType = "le"; break;
                default: tresholdType = "NONE"; break;
            }

            string dataType;
            switch (Type)
            {
                case DataType.ByteArray: dataType = "ByteArray"; break;
                case DataType.Integer: dataType = "Integer"; break;
                default: dataType = "NONE"; break;
            }
            return new TargetDataDto
            {
                Name = this.Name,
                Value = value,
                DataType = dataType,
                RawData = rawData.ToArray(),
                TriggeredTreshold = this.Treshold?.IsAboveTreshold(value)??false,
                TresholdValue = this.Treshold?.Value,
                RegisterChanges=this.RegisterChanges,
                TresholdType=tresholdType
            };

            
        }
        //todo implement treshold
        public class TargetTreshold
        {
            public DataType DataType;
            public TresholdType Type;
            public dynamic Value;
            public static TargetTreshold New(DataType dataType, TresholdDto tresholdDto)
            {
                if (!(tresholdDto?.IsValid == true))
                    return null;
                TresholdType tresholdType;
                switch (tresholdDto.Type.ToLower())
                {
                    case "gt":  tresholdType = TresholdType.GT;break;
                    case "ge": tresholdType = TresholdType.GE;break;
                    case "lt": tresholdType = TresholdType.LT; break;
                    case "le": tresholdType = TresholdType.LE; break;
                    default: tresholdType = TresholdType.NONE; break;
                }
                switch(dataType)
                {
                    case DataType.Integer:
                        int value;
                        if (int.TryParse(tresholdDto.Value, out value)) 
                            return new TargetTreshold { DataType = dataType, Value = value, Type = tresholdType };
                        else
                            return new TargetTreshold { DataType = dataType, Value = null, Type = TresholdType.NONE };
                    default:
                        return new TargetTreshold { DataType = dataType, Value = null, Type = TresholdType.NONE };
                }
               
            }
            public bool IsAboveTreshold(dynamic checkedValue)
            {
                switch(Type)
                {
                    case TresholdType.GT:
                        return checkedValue > Value;
                    case TresholdType.GE:
                        return checkedValue >= Value;
                    case TresholdType.LT:
                        return checkedValue < Value;
                    case TresholdType.LE:
                        return checkedValue <= Value;

                    default:
                        return false;
                }
            }
            public enum TresholdType { NONE, GT, LT, GE, LE}
            
        }
    }
}
