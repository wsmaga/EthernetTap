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
    class DOMapper
    // TODO: Zmienić jak w TargetDataDto pojawią się enumy zamiast stringów
    {
        private static DOMapper instance;
        public static DOMapper GetInstance()
        {
            if (instance == null)
                instance = new DOMapper();
            return instance;
        }

        private DOMapper() { }

        public Target ToEntity(TargetDataDto targetDataDto)
        {
            //DataType dataType;
            //switch (targetDataDto.DataType.ToLower())
            //{
            //    case "byte_array": dataType = DataType.ByteArray; break;
            //    case "integer":
            //        switch (targetDataDto.RawData.Count())
            //        {
            //            case 2: dataType = DataType.Int16; break;
            //            case 4: dataType = DataType.Int32; break;
            //            case 8: dataType = DataType.Int64; break;
            //            default: return null;
            //        }
            //        break;
            //    case "string": dataType = DataType.String; break;
            //    case "boolean": dataType = DataType.Boolean; break;
            //    case "float":
            //        switch (targetDataDto.RawData.Count())
            //        {
            //            case 4: dataType = DataType.Single; break;
            //            case 8: dataType = DataType.Double; break;
            //            default: return null;
            //        }
            //        break;
            //    default:
            //        dataType = DataType.NONE;
            //        break;
            //}
            throw new NotImplementedException();
            return new Target();
            //return new Target(
            //    //targetDataDto.nameId,
            //    0,
            //    DateTime.Now,
            //    targetDataDto.RawData,
            //    //targetDataDto.DataType,
            //    dataType,
            //    //targetDataDto.arraySize,
            //    1
            //);
        }
    }
}
