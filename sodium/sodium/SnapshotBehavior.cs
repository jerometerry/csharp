namespace sodium
{
    public class SnapshotBehavior<TA, TB> : ILambda2<TA, TB, TB>
    {
        public TB Apply(TA a, TB b)
        {
            return b;
        }
    }
}