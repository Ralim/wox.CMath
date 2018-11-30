using CMath.Parsers;
using System;

namespace CMath
{
    /// <summary>
    /// CMath parser class
    /// Provides functions for C like math operations (with extensions)
    /// Supports 0x... and 0b1.... notation, otherwise decimal.
    /// Works in integers only.
    /// </summary>
    public class CMath
    {
        public enum Format
        {
            Binary,
            Decimal,
            Hexadecimal,
        }
        public enum Width
        {
            formatu8,  // uint8_t
            formati8,  // int8_t
            formatu16, // uint16_t
            formati16, // int16_t
            formatu32, // uint32_t
            formati32, // int32_t
            formati64, // int64_t
        }
        public CMath()
        {
            ParserFunction.addFunction("exp", new ExpFunction());
            ParserFunction.addFunction("pow", new PowFunction());
            ParserFunction.addFunction("abs", new AbsFunction());
            ParserFunction.addFunction("sqrt", new SqrtFunction());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="statement">The maths to evaluate</param>
        /// <param name="outputFormat">The output format (Hex, dec,bin) </param>
        /// <param name="outputWidth">The final bit width the value is stored in </param>
        /// <returns>A string of the result</returns>
        public String Evaluate(String statement, Format outputFormat, Width outputWidth)
        {
            //Swap out symbols 
            statement = statement.Replace("<<", "#").Replace(">>", "$"); // Magic symbols
            double result = Parser.process(statement);

            Int64 value = (Int64)result;


            // Format the resulting value down to the desired type
            int outputBase = 10;
            String preString = "";
            switch (outputFormat) {
                case Format.Binary:
                    outputBase = 2;
                    preString = "0b";
                    break;
                case Format.Decimal:
                    outputBase = 10;
                    break;
                case Format.Hexadecimal:
                    outputBase = 16;
                    preString = "0x";
                    break;
                default:
                    break;
            }
            switch (outputWidth) {
                case Width.formatu8: {
                        Byte ans = (Byte)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formati8: {
                        SByte ans = (SByte)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formatu16: {
                        UInt16 ans = (UInt16)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formati16: {
                        Int16 ans = (Int16)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formatu32: {
                        UInt32 ans = (UInt32)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formati32: {
                        Int32 ans = (Int32)(value);// down convert
                        return preString + Convert.ToString(ans, outputBase).ToUpper();
                    }
                case Width.formati64: {
                        return preString + Convert.ToString(value, outputBase).ToUpper();
                    }
                default:
                    break;
            }
            return "";
        }
    }
}
