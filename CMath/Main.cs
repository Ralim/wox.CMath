using System.Collections.Generic;
using Wox.Plugin;

namespace CMath
{
    public class Main : IPlugin
    {
        CMath cMath;

        public void Init(PluginInitContext context)
        {
            //Init any objects required here
            cMath = new CMath();
        }

        public List<Result> Query(Query query)
        {

            List<Result> res = new List<Result>();
            CMath.Width w = CMath.Width.formati32;// default 
            string q = query.RawQuery;

            if (q.Contains("int")) {
                //Might have a marker for output width
                if (q.Contains("uint8_t")) {
                    w = CMath.Width.formatu8;
                } else if (q.Contains("int8_t")) {
                    w = CMath.Width.formati8;
                } else if (q.Contains("uint16_t")) {
                    w = CMath.Width.formatu16;
                } else if (q.Contains("int16_t")) {
                    w = CMath.Width.formati16;
                } else if (q.Contains("uint32_t")) {
                    w = CMath.Width.formatu32;
                } else if (q.Contains("int32_t")) {
                    w = CMath.Width.formati32;
                } else if (q.Contains("int64_t")) {
                    w = CMath.Width.formati64;
                }
                //Truncate this out 
                int index = q.IndexOf("int") + 3;
                index = q.IndexOf(" ", index); // find following space 
                q = q.Substring(index);
            }

            try {
                string result;
                result = cMath.Evaluate(query.RawQuery, CMath.Format.Decimal, w);
                res.Add(new Result { Title = "Decimal", SubTitle = result, IcoPath = "icon.png" });
            } catch {
            }
            try {
                string result;
                result = cMath.Evaluate(query.RawQuery, CMath.Format.Hexadecimal, w);
                res.Add(new Result { Title = "Hex", SubTitle = result, IcoPath = "icon.png" });
            } catch {
            }
            try {
                string result;
                result = cMath.Evaluate(query.RawQuery, CMath.Format.Binary, w);
                res.Add(new Result { Title = "Binary", SubTitle = result, IcoPath = "icon.png" });
            } catch {
            }


            //if (res.Count == 0) {
             //   res.Add(new Result { Title = "No Result", SubTitle = query.RawQuery, IcoPath = "icon.png" });
            //}

            return res;
        }
    }
}
