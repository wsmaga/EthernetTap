using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.filtering
{
    public interface IFilter
    {
        bool Pass(Frame frame);
        List<TargetDataDto> GetUsefulData(Frame frame);
    }
}
