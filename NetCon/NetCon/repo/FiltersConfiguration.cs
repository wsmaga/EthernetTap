using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.repo
{
    public class FiltersConfiguration<T>
    {
        List<Filter<T>> filters;

        public FiltersConfiguration()
        {
            filters = new List<Filter<T>>();
        }

        public FiltersConfiguration(Builder confBuilder)
        {
            filters = confBuilder.filters;
        }


        public bool pass(T frame)
        {
            bool pass = true;
            
            foreach(var filter in filters)
            {
                if (!filter.pass(frame))
                {
                    pass = false;
                    break;
                }
            }

            return pass;
        }

        public class Builder
        {
            public List<Filter<T>> filters { get; } = new List<Filter<T>>();
            public Builder AddFilter(Filter<T> filter)
            {
                filters.Add(filter);
                return this;
            }
        }
    }
}
