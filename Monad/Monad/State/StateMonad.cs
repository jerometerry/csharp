using System;
using Monad;

namespace Monad.State
{

/**
 * The state monad, which maintains a "cell" whose value can be updated
 * and queried via additional monad operations.
 *
 * @author Dave Herman
 */
public class StateMonad : Monad {

    public override Computation Unit(Object a) {
        return new Unit(a);
    }

    public override Computation Bind(Computation m, IComputationFactory f) {
        return new Bind(m, f);
    }

    /**
     * Produces a <i>lookup</i> computation, which evaluates the contents
     * of the state.
     *
     * @return a lookup computation.
     */
    public Computation Lookup() {
        return new Lookup();
    }

    /**
     * Produces a <i>mutate</i> computation, which updates the contents of
     * the state.
     *
     * @param newState the new contents of the state.
     * @return a mutate computation.
     */
    public Computation Mutate(Object newState) {
        return new Mutate(newState);
    }

}

}