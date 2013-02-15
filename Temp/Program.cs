using Antlr.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter input:");


            string line = Console.ReadLine();

            while(line.Trim() != "" && line.Trim().ToLower() != "quit")
            {
                var charStream = new Antlr.Runtime.ANTLRStringStream(line);
                var lexer = new SimpleCalcLexer(charStream);
                var tokenStream = new Antlr.Runtime.CommonTokenStream(lexer);
                var parser = new SimpleCalcParser(tokenStream);
                
                var cal = parser.calculate();


                CommonTreeNodeStream nodeStream = new CommonTreeNodeStream(cal.Tree);

                SimpleCalcWalker walker = new SimpleCalcWalker(nodeStream);

                int value = walker.calc();

                Console.WriteLine(cal.Tree.ToStringTree());


                Console.WriteLine("RESULT: {0}", value);
                Console.WriteLine();
                Console.WriteLine();
                line = Console.ReadLine();
            }
        }
    }
}
