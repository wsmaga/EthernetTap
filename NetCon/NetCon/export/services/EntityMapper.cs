using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCon.util;
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

            // TODO: Change TargetDataDto.ThresholdValue and ThresholdValue2 type to byte[]
            byte[] ThresholdValue = new byte[0];
            byte[] ThresholdValue2 = new byte[0];
            switch (targetDataDto.DataType)
            {
                case DataType.NONE:
                    throw new ArgumentException("TargetDataDto's DataType cannot be NONE", "DataType");
                    break;
                case DataType.Int32:
                    int int1 = targetDataDto.ThresholdValue;
                    int int2 = (targetDataDto.ThresholdValue2 ?? int1);
                    ThresholdValue = BitConverter.GetBytes(int1);
                    ThresholdValue2 = BitConverter.GetBytes(int2);
                    break;
                default:
                    throw new NotImplementedException("DataType not yet implemented in EntityMapper.ToNewVariableEvent");
                    break;
            }

            return new VariableEvent
            {
                thresholdDataType = targetDataDto.DataType,
                thresholdType = targetDataDto.ThresholdType,
                ThresholdValue = ThresholdValue,
                ThresholdValue2 = ThresholdValue2
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
