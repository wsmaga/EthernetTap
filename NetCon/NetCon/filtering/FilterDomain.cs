using NetCon.util;
using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetCon.parsing
{
    public class FilterDomain : IFilter
    {
        public PredicateTree Condition;
        public List<TargetDomain> Targets;
        public readonly int MaxIndex;
        public static FilterDomain New(FilterDto filterDto)
        {
            if (!(filterDto?.IsValid == true))
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
            foreach (var t in Targets)
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
    public partial class TargetDomain
    {
        public TargetThreshold Threshold;
        public int[] Bytes;
        public DataType Type;
        public string Name;
        public int Id;
        public bool RegisterChanges;
        public static TargetDomain New(TargetDto targetDto)
        {
            if (!(targetDto?.IsValid == true))
                return null;
            DataType type = DataType.NONE;
            switch (targetDto.Type.ToLower())
            {
                case "byte_array": type = DataType.ByteArray; break;
                case "integer":
                    switch (targetDto.Bytes.Count())
                    {
                        case 2: type = DataType.Int16; break;
                        case 4: type = DataType.Int32; break;
                        case 8: type = DataType.Int64; break;
                        default: return null;
                    }
                    break;
                case "string": type = DataType.String; break;
                case "boolean": type = DataType.Boolean; break;
                case "float":
                    switch (targetDto.Bytes.Count())
                    {
                        case 4: type = DataType.Single; break;
                        case 8: type = DataType.Double; break;
                        default: return null;
                    }
                    break;
                default: return null;
            }
            var threshold = TargetThreshold.New(type, targetDto.Threshold);
            return new TargetDomain(targetDto.Name, targetDto.Bytes, type, threshold,targetDto.Id, targetDto.RegisterChanges);
        }
        private TargetDomain(string name, int[] bytes, DataType type, TargetThreshold threshold, int id, bool registerChanges = false)
        {
            Bytes = bytes;
            Type = type;
            Name = name;
            Threshold = threshold;
            RegisterChanges = registerChanges;
            Id = id;
        }

        public TargetDataDto GetTargetData(Frame frame)
        {
            var rawData = new List<byte>();
            foreach (var index in Bytes)
                rawData.Add(frame.RawData[index]);
            dynamic value;
            var dataArr = rawData.ToArray();
            switch (Type)
            {
                case DataType.ByteArray:
                    value = rawData.ToArray();
                    break;
                case DataType.Int16:
                    if (BitConverter.IsLittleEndian)
                        dataArr = dataArr.Reverse().ToArray();
                    value = BitConverter.ToInt16(dataArr, 0);
                    break;
                case DataType.Int32:
                    if (BitConverter.IsLittleEndian)
                        dataArr = dataArr.Reverse().ToArray();
                    value = BitConverter.ToInt32(dataArr, 0);
                    break;
                case DataType.Int64:
                    if (BitConverter.IsLittleEndian)
                        dataArr = dataArr.Reverse().ToArray();
                    value = BitConverter.ToInt64(dataArr, 0);
                    break;
                case DataType.String:
                    value = string.Concat(rawData.Select(el => (char)el));
                    break;
                case DataType.Boolean:
                    value = rawData.Count(el => el == 0) == rawData.Count();
                    break;
                case DataType.Single:
                    if (BitConverter.IsLittleEndian)
                        dataArr = dataArr.Reverse().ToArray();
                    value = BitConverter.ToSingle(dataArr, 0);
                    break;
                case DataType.Double:
                    if (BitConverter.IsLittleEndian)
                        dataArr = dataArr.Reverse().ToArray();
                    value = BitConverter.ToDouble(dataArr, 0);
                    break;
                default: throw new ArgumentException("Target type data extraction not implemented");
            }
            /*string thresholdType;
            switch (Threshold?.Type)
            {
                case ThresholdType.GT: thresholdType = "gt"; break;
                case ThresholdType.GE: thresholdType = "ge"; break;
                case ThresholdType.LT: thresholdType = "lt"; break;
                case ThresholdType.LE: thresholdType = "le"; break;
                case ThresholdType.InClosed: thresholdType = "inclosed"; break;
                case ThresholdType.InOpen: thresholdType = "inopen"; break;
                case ThresholdType.OutClosed: thresholdType = "outclosed"; break;
                case ThresholdType.OutOpen: thresholdType = "outopen"; break;
                default: thresholdType = "NONE"; break;
            }

            string dataType;
            switch (Type)
            {
                case DataType.ByteArray: dataType = "ByteArray"; break;
                case DataType.Int16: dataType = "Int16"; break;
                case DataType.Int32: dataType = "Int32"; break;
                case DataType.Int64: dataType = "Int64"; break;
                case DataType.Boolean: dataType = "Boolean"; break;
                case DataType.String: dataType = "String"; break;
                case DataType.Single: dataType = "Single"; break;
                case DataType.Double: dataType = "Double"; break;
                default: dataType = "NONE"; break;
            }*/
            return new TargetDataDto
            {
                Id=this.Id,
                Name = this.Name,
                Value = value,
                DataType = this.Type,
                RawData = rawData.ToArray(),
                TriggeredThreshold = this.Threshold?.IsAboveThreshold(value) ?? false,
                ThresholdValue = this.Threshold?.Value,
                ThresholdValue2=this.Threshold.Value2,
                RegisterChanges = this.RegisterChanges,
                ThresholdType = this.Threshold.Type
            };


        }
        public partial class TargetThreshold
        {
            public DataType DataType;
            public ThresholdType Type;
            public dynamic Value;
            public dynamic Value2;
            public static TargetThreshold New(DataType dataType, ThresholdDto thresholdDto)
            {
                if (!(thresholdDto?.IsValid == true))
                    return null;
                ThresholdType thresholdType;
                switch (thresholdDto.Type.ToLower())
                {
                    case "gt": thresholdType = ThresholdType.GT; break;
                    case "ge": thresholdType = ThresholdType.GE; break;
                    case "lt": thresholdType = ThresholdType.LT; break;
                    case "le": thresholdType = ThresholdType.LE; break;
                    case "inclosed": thresholdType = ThresholdType.InClosed; break;
                    case "inopen": thresholdType = ThresholdType.InOpen; break;
                    case "outclosed": thresholdType = ThresholdType.OutClosed; break;
                    case "outopen": thresholdType = ThresholdType.OutOpen; break;
                    default: thresholdType = ThresholdType.NONE; break;
                }
                switch (dataType)
                {
                    case DataType.Int16:
                        if (short.TryParse(thresholdDto.Value, out var val1_int16))
                        {
                            if (thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.OutClosed || thresholdType == ThresholdType.OutOpen)
                            {
                                if (thresholdDto.Value2 != null && short.TryParse(thresholdDto.Value2, out var val2_int16))
                                    return new TargetThreshold { DataType = dataType, Value = val1_int16, Value2 = val2_int16, Type = thresholdType };
                            }
                            else
                                return new TargetThreshold { DataType = dataType, Value = val1_int16, Value2 = null, Type = thresholdType };
                        }
                        break;
                    case DataType.Int32:
                        if (int.TryParse(thresholdDto.Value, out var val1_int32))
                        {
                            if (thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.OutClosed || thresholdType == ThresholdType.OutOpen)
                            {
                                if (thresholdDto.Value2 != null && int.TryParse(thresholdDto.Value2, out var val2_int32))
                                    return new TargetThreshold { DataType = dataType, Value = val1_int32, Value2 = val2_int32, Type = thresholdType };
                            }
                            else
                                return new TargetThreshold { DataType = dataType, Value = val1_int32, Value2 = null, Type = thresholdType };
                        }
                        break;
                    case DataType.Int64:
                        if (long.TryParse(thresholdDto.Value, out var val1_int64))
                        {
                            if (thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.OutClosed || thresholdType == ThresholdType.OutOpen)
                            {
                                if (thresholdDto.Value2 != null && long.TryParse(thresholdDto.Value2, out var val2_int64))
                                    return new TargetThreshold { DataType = dataType, Value = val1_int64, Value2 = val2_int64, Type = thresholdType };
                            }
                            else
                                return new TargetThreshold { DataType = dataType, Value = val1_int64, Value2 = null, Type = thresholdType };
                        }
                        break;

                    case DataType.Single:
                        if (float.TryParse(thresholdDto.Value, out var val1_single))
                        {
                            if (thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.OutClosed || thresholdType == ThresholdType.OutOpen)
                            {
                                if (thresholdDto.Value2 != null && float.TryParse(thresholdDto.Value2, out var val2_single))
                                    return new TargetThreshold { DataType = dataType, Value = val1_single, Value2 = val2_single, Type = thresholdType };
                            }
                            else
                                return new TargetThreshold { DataType = dataType, Value = val1_single, Value2 = null, Type = thresholdType };
                        }
                        break;
                    case DataType.Double:
                        if (double.TryParse(thresholdDto.Value, out var val1_double))
                        {
                            if (thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.InClosed || thresholdType == ThresholdType.OutClosed || thresholdType == ThresholdType.OutOpen)
                            {
                                if (thresholdDto.Value2 != null && double.TryParse(thresholdDto.Value2, out var val2_double))
                                    return new TargetThreshold { DataType = dataType, Value = val1_double, Value2 = val2_double, Type = thresholdType };
                            }
                            else
                                return new TargetThreshold { DataType = dataType, Value = val1_double, Value2 = null, Type = thresholdType };
                        }
                        break;

                }
                return new TargetThreshold { DataType = dataType, Value = null, Value2 = null, Type = ThresholdType.NONE };
            }
            public bool IsAboveThreshold(dynamic checkedValue)
            {
                switch (Type)
                {
                    case ThresholdType.GT:
                        return checkedValue > Value;
                    case ThresholdType.GE:
                        return checkedValue >= Value;
                    case ThresholdType.LT:
                        return checkedValue < Value;
                    case ThresholdType.LE:
                        return checkedValue <= Value;
                    case ThresholdType.InClosed:
                        return Value <= checkedValue && checkedValue <= Value2;
                    case ThresholdType.InOpen:
                        return Value < checkedValue && checkedValue < Value2;
                    case ThresholdType.OutClosed:
                        return checkedValue <= Value || Value2 <= checkedValue;
                    case ThresholdType.OutOpen:
                        return checkedValue < Value || Value2 < checkedValue;
                    default:
                        return false;
                }
            }

        }
    }
}
