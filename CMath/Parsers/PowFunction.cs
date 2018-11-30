using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMath.Parsers
{

    class PowFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg1 = Parser.loadAndCalculate(data, ref from, ',');
            double arg2 = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);

            return Math.Pow(arg1, arg2);
        }
    }
}
