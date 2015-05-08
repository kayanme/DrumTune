using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuorieTest
{
    public class FrequencyPair
    {
        public double Frequency;
        public double Level;

		public double DBLevel{ get { return 10 * Math.Log10 (Level/short.MaxValue); } }

        public FrequencyPair(double fr, double level)
        {
            Frequency = fr;
            Level = level;
        }

		public override string ToString ()
		{
			return string.Format ("{0:F3}:{1:F3}",Frequency,DBLevel);
		}
    }
}
