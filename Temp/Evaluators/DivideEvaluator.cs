
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temp
{
    public class DivideEvaluator : IEvaluator
    {
        public IEvaluator Op1 { get; set; }
        public IEvaluator Op2 { get; set; }

        public DivideEvaluator(IEvaluator op1, IEvaluator op2)
        {
            Op1 = op1;
            Op2 = op2;
        }

        public int Evaluate()
        {
            return Op1.Evaluate() / Op2.Evaluate();
        }


        public string ToTreeString(string ident)
        {
            return "\r\n" + ident + "DIVIDE: " 
                + Op1.ToTreeString(ident + " ")
                + Op2.ToTreeString(ident + " ");
        }
    }
}
