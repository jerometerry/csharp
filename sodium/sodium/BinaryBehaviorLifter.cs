namespace sodium
{
    class BinaryBehaviorLifter<TFirstBehavior,TSecondBehavior,TResultBehavior> : 
        IFunction<TFirstBehavior, IFunction<TSecondBehavior, TResultBehavior>>
    {
        private readonly IBinaryFunction<TFirstBehavior, TSecondBehavior, TResultBehavior> _behaviorFunction;

        public BinaryBehaviorLifter(IBinaryFunction<TFirstBehavior, TSecondBehavior, TResultBehavior> behaviorFunction)
        {
            _behaviorFunction = behaviorFunction;
        }

        public IFunction<TSecondBehavior, TResultBehavior> Apply(TFirstBehavior behavior)
        {
            return new Lifter(_behaviorFunction, behavior);
        }

        private class Lifter : IFunction<TSecondBehavior, TResultBehavior>
        {
            private readonly TFirstBehavior _behavior;
            private readonly IBinaryFunction<TFirstBehavior, TSecondBehavior, TResultBehavior> _behaviorFunction;

            public Lifter(
                IBinaryFunction<TFirstBehavior, TSecondBehavior, TResultBehavior> behaviorFunction, 
                TFirstBehavior behavior)
            {
                _behaviorFunction = behaviorFunction;
                _behavior = behavior;
            }

            public TResultBehavior Apply(TSecondBehavior behavior2)
            {
                return _behaviorFunction.Apply(_behavior, behavior2);
            }
        }
    }
}