using NetCon.model;
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
        List<Filter> filters;

        public FiltersConfiguration()
        {
            filters = new List<Filter>();
        }

        public FiltersConfiguration(Builder confBuilder)
        {
            filters = confBuilder.filters;
        }


        public Filter pass(Frame frame)
        {
            
            foreach(var filter in filters)
            {
                if (filter.pass(frame))
                {
                    return filter;
                }
            }

            return null;
        }

        public class Builder
        {
            public List<Filter> filters { get; } = new List<Filter>();
            public Builder AddFilter(Filter filter)
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
