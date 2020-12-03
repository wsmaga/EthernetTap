﻿using NetCon.model;
using System;
using System.Collections.Generic;
using System.IO;
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
        public TargetList Targets { get; set; }
        [XmlIgnore]
        public bool IsValid => (
            !string.IsNullOrWhiteSpace(Condition) && 
            Targets != null && 
            Targets.Items.Count > 0
           
            );
        public static FilterDto Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(FilterDto));
            using(TextReader reader=new StringReader(xml))
            {
                return (FilterDto)serializer.Deserialize(reader);
            }
        }
    }
    public class TargetList
    {
        [XmlElement("Target")]
        public List<TargetDto> Items { get; set; }
    }
    public class TargetDto
    {
        [XmlIgnore]
        public int[] Bytes { get; set; }
        [XmlElement("Bytes")]
        public string BytesString {
            get
            {
                return string.Join(",", Bytes);
            }
            set
            {
                Bytes = value.Split(',').Select(el => int.Parse(el)).ToArray();
            }
        }
        [XmlElement("Type")]
        public string Type;
        [XmlElement("Name")]
        public string Name; //TODO: VALIDATE NAMES!!!!!
        [XmlElement("Treshold")]
        public TresholdDto Treshold;
        [XmlElement("RegisterChanges")]
        public bool RegisterChanges;
        [XmlIgnore]
        public bool IsValid => (
            !string.IsNullOrWhiteSpace(Name) &&
            Bytes != null &&
            Bytes.Length > 0 &&
            !string.IsNullOrWhiteSpace(Type) &&
            Treshold != null
            );
    }

    public class TresholdDto
    {
        [XmlElement("Type")]
        public string Type;
        [XmlElement("Value")]
        public string Value;
        [XmlIgnore]
        public bool IsValid => (
            !string.IsNullOrWhiteSpace(Type) &&
            !string.IsNullOrWhiteSpace(Value)
            );

    }
    public static class FilterDtoExtensions
    {
        public static FilterDomain ToDomain(this FilterDto filterDto)
        {
            return FilterDomain.New(filterDto);
        }
    }

}
