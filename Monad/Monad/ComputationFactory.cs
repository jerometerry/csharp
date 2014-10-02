using System;

namespace Monad
{
    public class ComputationFactory : IComputationFactory
    {
        private readonly Computation _comp;

        public ComputationFactory(Computation comp)
        {
            _comp = comp;
        }

        public Computation Make(Object o)
        {
            return _comp;
        }
    }
}
