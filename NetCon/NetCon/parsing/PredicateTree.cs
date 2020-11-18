using NetCon.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCon.parsing
{
    public enum Operation { NONE, AND, OR }

    public class PredicateTree
    {
        Operation operation;
        private readonly PredicateTree[] children;
        private readonly Predicate<Frame> corePredicate;
        public PredicateTree(Predicate<Frame> predicate)
        {
            corePredicate = predicate;
        }
        public PredicateTree(PredicateTree[] _children, Operation _operation)
        {
            if (_children != null && _children.Length > 0)
                operation = _operation;
            else
                operation = Operation.NONE;
            children = _children;
        }
        public bool Pass(Frame frame)
        {
            if (corePredicate == null)
            {
                switch (operation)
                {
                    case Operation.AND:
                        foreach(PredicateTree c in children)
                        {
                            if (!c.Pass(frame))
                                return false;
                        }
                        return true;
                    case Operation.OR:
                        foreach (PredicateTree c in children)
                        {
                            if (c.Pass(frame))
                                return true;
                        }
                        return false;
                    default:
                        return false;
                }
            }
            else
                return corePredicate(frame);
        }
    }
}
