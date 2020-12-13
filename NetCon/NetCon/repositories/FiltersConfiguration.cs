using NetCon.model;
using NetCon.parsing;
using System.Collections.Generic;

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
