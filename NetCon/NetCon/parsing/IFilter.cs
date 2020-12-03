using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.parsing
{
    public interface IFilter
    {
        bool Pass(Frame frame);
        List<TargetDataDto> GetUsefulData(Frame frame);
    }
}
