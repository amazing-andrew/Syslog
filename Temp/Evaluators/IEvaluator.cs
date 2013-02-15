using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temp
{
    public interface IEvaluator
    {
        int Evaluate();

        string ToTreeString(string ident);
    }
}
