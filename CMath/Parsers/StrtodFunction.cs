using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMath.Parsers
{
    class StringToNumberParser : ParserFunction
    {
        protected override double evaluate(string data, ref int from)
        {
            //parse number as hex, bin or as a decimal fallback
            string s = Item.ToLower().Trim();
            if (s.StartsWith("0x")) {
                //Hex 
                return (double)Convert.ToInt64(s.Substring(2, s.Length - 2), 16);
            } else if (s.StartsWith("0b")) {
                //Binary
                return (double)Convert.ToInt64(s.Substring(2, s.Length - 2), 2);
            } else {
                double num;
                if (!Double.TryParse(Item, out num)) {
                    throw new ArgumentException("Could not parse token [" + Item + "]");
                }
                return num;
            }
        }
        public string Item { private get; set; }
    }
}
