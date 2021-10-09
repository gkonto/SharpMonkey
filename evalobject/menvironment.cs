using System.Collections.Generic;
using evalobject;

#nullable enable

namespace menvironment
{
    public class MEnvironment
    {
        Dictionary<string, EvalObject> store = new Dictionary<string, EvalObject>();
        MEnvironment? outer;
        
        public MEnvironment(){}
        public MEnvironment(MEnvironment o)
        {
            outer = o;
        }

        public EvalObject? Get(string name)
        {
            EvalObject? ret = null;
            store.TryGetValue(name, out ret);
            if (ret == null && outer != null) {
                ret = outer.Get(name);
            }
            return ret;
        }

        public void Set(string name, EvalObject val)
        {
            store[name] = val;
        }
    }
}