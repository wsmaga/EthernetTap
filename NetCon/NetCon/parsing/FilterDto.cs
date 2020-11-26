using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace NetCon.parsing
{
    [XmlRoot("Filter")]
    public class FilterDto
    {
        [XmlElement("Condition")]
        public string Condition;
        [XmlElement("Targets")]
        public List<TargetDto> Targets;
    }
    public class TargetDto
    {
        [XmlElement("Bytes")]
        public int[] Bytes;
        [XmlElement("Type")]
        public string Type;
        [XmlElement("Name")]
        public string Name;
    }
    public static class FilterDtoExtensions
    {
        public static FilterDomain ToDomain(this FilterDto filterDto)
        {
            return FilterDomain.New(filterDto);
        }
    }

}
