﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMath.Parsers
{
    class AbsFunction : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            double arg = Parser.loadAndCalculate(data, ref from, Parser.END_ARG);
            return Math.Abs(arg);
        }
    }
}
