using NetCon.model;
using NetCon.filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.repo
{
    public class FiltersConfiguration
    {
        List<IFilter> filters;

        public FiltersConfiguration()
        {
            filters = new List<IFilter>();
        }

        public FiltersConfiguration(Builder confBuilder)
        {
            filters = confBuilder.filters;
        }


        public IFilter pass(Frame frame)
        {
            
            foreach(var filter in filters)
            {
                if (filter.Pass(frame))
                {
                    return filter;
                }
            }

            return null;
        }

        public class Builder
        {
            public List<IFilter> filters { get; } = new List<IFilter>();
            public Builder AddFilter(IFilter filter)
            {
                if(filter!=null)
                    filters.Add(filter);
                return this;
            }

            public FiltersConfiguration Build()
            {
                return new FiltersConfiguration(this);
            }
        }

    }
}
