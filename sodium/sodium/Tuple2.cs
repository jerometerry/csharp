namespace sodium
{
	public sealed class Tuple2<A,B> {
		public Tuple2(A a, B b) {
			this.a = a;
			this.b = b;
		}
		public A a;
		public B b;
	}
}