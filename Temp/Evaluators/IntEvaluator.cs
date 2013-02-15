using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temp
{
    public class IntEvaluator : IEvaluator
    {
        public int Value { get; set; }

        public IntEvaluator(int value)
        {
            this.Value = value;
        }

        public int Evaluate()
        {
            return Value;
        }


        public string ToTreeString(string ident)
        {
            return "\r\n" + ident + "INT: " + Value;
        }
    }
}
