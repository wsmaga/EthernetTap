using System;
using NetCon.export.entities;
using NetCon.filtering;

namespace NetCon.export.services
{
    class EntityMapper
    {
        private static EntityMapper instance;
        public static EntityMapper GetInstance()
        {
            if (instance == null)
                instance = new EntityMapper();
            return instance;
        }

        private EntityMapper() { }

        public Target ToNewTarget(TargetDataDto targetDataDto)
        {
            return new Target
            {
                DataType = targetDataDto.DataType,
                Date = DateTime.Now,
                RawData = targetDataDto.RawData,
                TargetNameID = targetDataDto.Id
            };
        }

        public VariableEvent ToNewVariableEvent(TargetDataDto targetDataDto)
        {
            return new VariableEvent
            {
                thresholdDataType = targetDataDto.DataType,
                thresholdType = targetDataDto.ThresholdType,
                ThresholdValue = targetDataDto.ThresholdValue,
                ThresholdValue2 = targetDataDto.ThresholdValue2
            };
        }

        public TargetName ToNewTargetName(TargetDataDto targetDataDto)
        {
            return new TargetName
            {
                CreationDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Name = targetDataDto.Name,
                TargetNameID = targetDataDto.Id
            };
        }
    }
}
