using System.Collections.Generic;

namespace monkey
{
    public class MEnvironment
    {
        Dictionary<string, EvalObject> store = new Dictionary<string, EvalObject>();
        protected MEnvironment outer;
        
        public MEnvironment(){}
        public MEnvironment(MEnvironment o)
        {
            outer = o;
        }

        public virtual EvalObject Get(Identifier ident)
        {
            EvalObject ret = null;
            store.TryGetValue(ident.value, out ret);
            if (ret == null && outer != null) {
                ret = outer.Get(ident);
            }
            return ret;
        }

        public void Set(string name, EvalObject val)
        {
            store[name] = val;
        }
    }

    public class MFunctionEnv : MEnvironment
    {
        List<EvalObject> function_params_store;

        public MFunctionEnv(MEnvironment o)
        {
            outer = o;
        }

        public void setArgumentVals(List<EvalObject> parameters)
        {
            function_params_store = parameters;
        }

        public override EvalObject Get(Identifier ident)
        {
            if (ident is FunctionParamIdentifier fpi) {
                return function_params_store[fpi.param_index];
            } else {
                return base.Get(ident);
            }
        }
    }
}
