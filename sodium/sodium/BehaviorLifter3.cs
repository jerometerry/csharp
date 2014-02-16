namespace sodium
{
    public class BehaviorLifter3<TFirstBehavior,TSecondBehavior,TThirdBehavior,TResultBehavior> : 
        IFunction<TFirstBehavior, IFunction<TSecondBehavior, IFunction<TThirdBehavior, TResultBehavior>>>
    {
        private readonly ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> _behaviorFunction;

        public BehaviorLifter3(ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> behaviorFunction)
        {
            _behaviorFunction = behaviorFunction;
        }

        public IFunction<TSecondBehavior, IFunction<TThirdBehavior, TResultBehavior>> Apply(TFirstBehavior behavior)
        {
            return new Lifter1(_behaviorFunction, behavior);
        }

        private class Lifter1 : IFunction<TSecondBehavior, IFunction<TThirdBehavior, TResultBehavior>>
        {
            private readonly ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> _behaviorFunction;
            private readonly TFirstBehavior _behavior;

            public Lifter1(
                ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> behaviorFunction, 
                TFirstBehavior behavior)
            {
                _behaviorFunction = behaviorFunction;
                _behavior = behavior;
            }

            public IFunction<TThirdBehavior, TResultBehavior> Apply(TSecondBehavior behavior2)
            {
                return new Lifter2(_behaviorFunction, _behavior, behavior2);
            }

            private class Lifter2 : IFunction<TThirdBehavior, TResultBehavior>
            {
                private readonly ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> _behaviorFunction;
                private readonly TFirstBehavior _behavior1;
                private readonly TSecondBehavior _behavior2;

                public Lifter2(
                    ITernaryFunction<TFirstBehavior, TSecondBehavior, TThirdBehavior, TResultBehavior> behaviorFunction, 
                    TFirstBehavior behavior1, 
                    TSecondBehavior behavior2)
                {
                    _behaviorFunction = behaviorFunction;
                    _behavior1 = behavior1;
                    _behavior2 = behavior2;
                }

                public TResultBehavior Apply(TThirdBehavior behavior3)
                {
                    return _behaviorFunction.Apply(_behavior1, _behavior2, behavior3);
                }
            }
        }
    }
}