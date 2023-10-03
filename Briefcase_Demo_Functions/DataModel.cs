using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Briefcase_Demo_Functions
{
    internal class DataModel
    {
        public class INFERENCE_RESULT_ITEM
        {
            public uint Class { get; set; }
            public double Confidence { get; set; }
            public double x { get; set; }
            public double y { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

        }
        public class INFERENCE_RESULT
        {
            public string DeviceId { get; set; }
            public string ModelId { get; set; }
            public bool Image { get; set; }
            public string T { get; set; }
            public List<INFERENCE_RESULT_ITEM> inferenceResults { get; set; }
        }
    }
}
