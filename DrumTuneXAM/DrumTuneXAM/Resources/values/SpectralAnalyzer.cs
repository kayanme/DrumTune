using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Transformations;
using SoundAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuorieTest
{

    public enum WindowType{Hann,Square}
    public class SpectralAnalyzer
    {
        private RealFourierTransformation trans;
        private double[] frqs;
        private double[] window;
        public SpectralAnalyzer(int windowSize, int discretization,WindowType winType)
        {
            trans = new RealFourierTransformation();
            frqs = trans.GenerateFrequencyScale(discretization, windowSize);
            if (winType == WindowType.Hann)
                window = Hanning(windowSize);
            else window = Square(windowSize);
        }

        private static double[] Hanning(int samples)
        {
            return Enumerable.Range(0, samples)
                .Select(k => 0.5 * (1 - Math.Cos(2 * Math.PI * k / (samples - 1))))
                .ToArray();
        }

        private static double[] Square(int samples)
        {
            return Enumerable.Range(0, samples)
                .Select(k => 1.0)
                .ToArray();
        }

        public static double[] SoundFromBytes(byte[] smp,bool stereo)
        {
            var shorts = smp.BTS(stereo).ToArray();
            var wavesample = shorts.Select(k => (double)k).ToArray();
            return wavesample;
        }

        public static double AverageLevelAnalysis(short[] sound)
        {                      
			return Math.Sqrt(sound.Average(k=>k * k));
        }

		public static double TotalLevelAnalysis(short[] sound)
		{                      
			return 2*Math.Sqrt(sound.Sum(k=>k * k));
		}

		public FrequencyChart ApplyWindow(FrequencyChart chart)
		{
			if (chart.Count != window.Count())
				throw new Exception ();
			var e = new FrequencyChart (chart.TotalLevel, chart.AverageLevel, chart);
			for (int i = 0; i < chart.Count; i++) {
				var d = e [i].Level * window [i];
				e [i].Level = d;
			}
			return e;
		}

		public double[] ApplyWindow(double[] sound)
		{
			if (sound.Count() != window.Count())
				throw new Exception ();
			var e = new double[sound.Length];
			for (int i = 0; i < sound.Length; i++) {
				sound[i] *= window [i];

			}
			return e;
		}

        public FrequencyChart MaxFrequenciesAnalyze(FrequencyChart chart, double cutoffLevel)
        {          
           
			var frClusters = chart.Where (k3 => k3.Level > cutoffLevel).ToArray ();
			if (!frClusters.Any ())
				return new FrequencyChart (0, 0, new List<FrequencyPair> ());
			frClusters = frClusters
                      .ClusterBy(k => k.Frequency, 0.01)
			  	      .Select(k => new FrequencyPair(k.Average(k3 => k3.Frequency), k.Average(k3 => k3.Level))).ToArray();				          

				var newChart = new FrequencyChart(chart.TotalLevel,chart.AverageLevel,
				                 frClusters.OrderBy(k=>k.Frequency)
                    );


			return newChart;
            
         

        }

		public FrequencyChart CutOffLows(FrequencyChart chart, double cutoffLevel)
		{
			var t = new FrequencyChart (chart.TotalLevel, chart.AverageLevel,
				chart);
			for (int i = 0, tCount = t.Count; i < tCount; i++) {
				var e = t [i];
				e.Level = e.Level > cutoffLevel ? e.Level : 0;
			}
			return t;
		}

       



		public FrequencyChart BandGrouping(Bands bands, FrequencyChart chart,double threshHold)
        {
            var freqs = bands.Frequencies;
			var workCharts = chart.Where (k => k.Level > threshHold).ToArray();
			var newChart = new FrequencyChart(chart.TotalLevel,chart.AverageLevel, new List<FrequencyPair>());
			var curBand = 0;
            var curUpBound = freqs[curBand];

			int curFreqInBand = 0;
			var curPair = new FrequencyPair(curUpBound,0);

			for (int i = 0; i < workCharts.Count; i++) {
				var sample = workCharts [i];
			
				if (sample.Frequency < curUpBound) {
					curPair.Level += sample.Level;

                }
                else if (freqs.Count > curBand)
                {
					
					curPair.Level /= curFreqInBand;
					curFreqInBand = 0;		
					while (sample.Frequency > curUpBound) {
						curBand++;
                        if (freqs.Count <= curBand)
                            curUpBound = freqs[freqs.Count - 1] * 10;
						else
                            curUpBound = freqs[curBand];
					}
					newChart.Add (curPair);
					curPair = new FrequencyPair (curUpBound, sample.Level);
				}
				curFreqInBand++;
			}

       
			return newChart;
        }




		public double[] ConvertFromSamples(short[] samples,double? normLevel=null)
		{
			var t = new double[samples.Length];
			double maxSample = normLevel??samples.Max ();
			if (maxSample > 0) {
				for (int i = 0; i < t.Length; i++) {
					t [i] = (double)samples [i] / maxSample;
				}
			}
			return t;
		}

		public double[] ConvertFromSamplesWithWindow(short[] samples)
		{
			var t = new double[samples.Length];
			double maxSample = 1;
			if (maxSample > 0) {
				for (int i = 0; i < t.Length; i++) {
					t [i] = (double)samples [i] * window[i] / maxSample;
				}
			}
			return t;
		}

        public FrequencyChart AnalyzeSound(double[] sound)
		{

			  
			var sample = sound;
			var amp = Math.Sqrt (sample.Average (k => k * k));
			var ampMax = sample.Max (k => Math.Abs( k));
			var l = AnalyzeUtils.FindMinMorePow (sound.Length);
			Array.Resize (ref sample, l);
			double[] real;
			double[] img;
                  
			trans.TransformForward (sample, out real, out img);
		//	var chart = new FrequencyChart (ampMax, amp,
	//			                             real.Zip (img, (r, i) => Math.Sqrt (r * r + i * i))
      //                          .Zip (frqs, (a, f) => new FrequencyPair (f, a))
      //                           .Where (k => k.Frequency > 0)
      //                           .ToArray ());

			var chart = new FrequencyChart (ampMax, amp,
								     	real.Zip (frqs, (a, f) => new FrequencyPair (f, Math.Abs(a)))
				                          .Where (k => k.Frequency > 0)
			                           .ToArray ());
			return chart;
               
            


		}



    }
}
