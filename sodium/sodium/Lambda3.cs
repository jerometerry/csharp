namespace sodium {
	using System;

	public interface Lambda3<A,B,C,D> {
		D apply(A a, B b, C c);
	}

	public class Lambda3Impl<A,B,C,D> : Lambda3<A,B,C,D> 
	{
		private Func<A,B,C,D> _f;

		public Lambda3Impl(Func<A,B,C,D> f)
		{
			_f = f;
		}

		public D apply(A a, B b, C c)
		{
			return _f(a,b,c);
		}
	}

}