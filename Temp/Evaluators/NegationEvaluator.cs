using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temp
{
    public class NegationEvaluator : IEvaluator
    {
        public IEvaluator Evaluator { get; set; }

        public NegationEvaluator(IEvaluator evaluator)
        {
            this.Evaluator = evaluator;
        }

        public int Evaluate()
        {
            return -this.Evaluator.Evaluate();
        }


        public string ToTreeString(string ident)
        {
            return "\r\n" + ident + "Negation: " 
                + Evaluator.ToTreeString(ident + " ");
        }
    }
}
