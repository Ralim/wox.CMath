using CMath.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMath
{

    public class ParserFunction
    {
        public ParserFunction()
        {
            m_impl = this;
        }

        // A "virtual" Constructor
        internal ParserFunction(string data, ref int from, string item, char ch)
        {
            if (item.Length == 0 && ch == Parser.START_ARG) {
                // There is no function, just an expression in parentheses
                m_impl = s_idFunction;
                return;
            }

            if (m_functions.TryGetValue(item, out m_impl)) {
                // Function exists and is registered (e.g. pi, exp, etc.)
                return;
            }

            // Function not found, will try to parse this as a number.
            s_strtodFunction.Item = item;
            m_impl = s_strtodFunction;
        }

        public static void addFunction(string name, ParserFunction function)
        {
            m_functions[name] = function;
        }

        public double getValue(string data, ref int from)
        {
            return m_impl.evaluate(data, ref from);
        }

        protected virtual double evaluate(string data, ref int from)
        {
            // The real implementation will be in the derived classes.
            return 0;
        }

        private ParserFunction m_impl;
        private static Dictionary<string, ParserFunction> m_functions = new Dictionary<string, ParserFunction>();

        private static StringToNumberParser s_strtodFunction = new StringToNumberParser();
        private static IdentityFunction s_idFunction = new IdentityFunction();
    }

}
