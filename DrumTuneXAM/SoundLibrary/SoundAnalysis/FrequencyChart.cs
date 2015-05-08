using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuorieTest
{
    public class FrequencyChart : List<FrequencyPair>
    {
        public double TotalLevel { get; set; }

		public double AverageLevel { get; set; }

		public FrequencyChart(double totallevel,double avglevel, IEnumerable<FrequencyPair> pairs)
            : base(pairs)
        {
            TotalLevel = totallevel;
			AverageLevel = avglevel;
        }

		public FrequencyChart(IEnumerable<FrequencyPair> pairs)
			: base(pairs)
		{
			if (pairs.Any ()) {
				AverageLevel = Math.Sqrt (pairs.Average (k => k.Level * k.Level));
				TotalLevel = pairs.Max (k => Math.Abs (k.Level));		
			}
		}

		public FrequencyChart ()
		{
			
		}
    }
}
