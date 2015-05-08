using MathNet.Numerics;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Transformations;
using SoundAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
            trans = new RealFourierTransformation(TransformationConvention.AsymmetricScaling );
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
				.Select(k => new FrequencyPair(k.OrderByDescending(k3 => k3.Level).First().Frequency, k.Max(k3 => k3.Level))).ToArray();				          

				var newChart = new FrequencyChart(chart.TotalLevel,chart.AverageLevel,
				                 frClusters.OrderBy(k=>k.Frequency)
                    );


			return newChart;
            
         

        }

		public FrequencyChart CutOffLows(FrequencyChart chart, double cutoffLevel)
		{
			var t = new FrequencyChart (chart.TotalLevel, chart.AverageLevel, chart);
			for (int i = 0, tCount = t.Count; i < tCount; i++) {
				var e = t [i];
				e.Level = cutoffLevel>0?(e.Level > cutoffLevel ? e.Level : 0):(e.DBLevel < cutoffLevel ? e.Level : 0);
			}
			return t;
		}

       



		public FrequencyChart BandGrouping(Bands bands, FrequencyChart chart)
        {
			Debug.Assert (bands.Notes.Count>=2);
            var freqs = bands.Notes.Select(k=>k.Frequency).ToArray();
		

			var curBand = 0;
            

			var t = new List<FrequencyPair> ();
			var lowPair = new FrequencyPair(bands.Notes[0].Frequency,0);
			var highPair = new FrequencyPair(bands.Notes[1].Frequency,0);
			t.Add (highPair);
			t.Add (lowPair);

			for (int i = 0; i < chart.Count; i++) {
				var sample = chart [i];

				while (sample.Frequency > highPair.Frequency) {
					if (curBand == bands.Notes.Count-1)
						goto C1;
					lowPair = highPair;
					curBand++;
					highPair = new FrequencyPair(bands.Notes [curBand].Frequency,0);
					t.Add (highPair);
				}


				lowPair.Level += sample.Level * Math.Round((sample.Frequency - lowPair.Frequency) / (highPair.Frequency - lowPair.Frequency));
				highPair.Level += sample.Level * Math.Round((highPair.Frequency - sample.Frequency) / (highPair.Frequency - lowPair.Frequency));
                
			}

       
			C1:         return  new FrequencyChart(t);
        }




		public double[] ConvertFromSamples(short[] samples)
		{
			var t = new double[window.Length];
		
			for (int i = 0; i < t.Length; i++) {
				if (samples.Length > i)
					t [i] = (double)samples [i];
				else
					t [i] = 0;
			}

			return t;
		}

		public double[] ConvertFromSamplesWithWindow(short[] samples)
		{
			var t = new double[window.Length];

			for (int i = 0; i < t.Length; i++) {
				if (samples.Length > i)
					t [i] = (double)samples [i] * window [i];
				else
					t [i] = 0;
				
			}
			return t;
		}

		public FrequencyChart AnalyzeSound(double[] sound,int offset)
		{


			var sample = new double[window.Length];
			Array.Copy(sound,offset,sample,0,window.Length);

		  

			double[] real;
			double[] img;
                  
			trans.TransformForward (sample, out real, out img);
	    	var chart = new FrequencyChart (
				real.Zip (img, (r, i) => Math.Sqrt (r * r + i * i) * 2 / (sample.Length))		
                               .Zip (frqs, (a, f) => new FrequencyPair (f, a))
                               .Where (k => k.Frequency > 0)
                               .ToArray ());


		
						
			return chart;
                          
		}


		public FrequencyChart CutEdges(FrequencyChart chart, double lowBound,double highBound)
		{
			var t = chart.Where(k=>k.Frequency >= lowBound && k.Frequency<=highBound);
			if (!t.Any ())
				return new FrequencyChart (0, 0, new List<FrequencyPair> ());
			return new FrequencyChart (
				t.Max(k=>k.Level),
				t.Average(k=>k.Level),
				t
			);

		}

		public FrequencyChart SelectPeaks(FrequencyChart[] charts)
		{
			var list = new List<FrequencyPair> ();
			for (int i = 0; i < charts [0].Count; i++) {
				list.Add (new FrequencyPair (charts [0] [i].Frequency, charts.Max (k => k [i].Level)));
			}
		
			return new FrequencyChart (list);
		}

		public FrequencyChart SelectAverage(FrequencyChart[] charts)
		{
			if (!charts.Any ())
				return new FrequencyChart ();
			var list = new List<FrequencyPair> ();
			for (int i = 0; i < charts [0].Count; i++) {
				list.Add (new FrequencyPair (charts [0] [i].Frequency, charts.Average (k => k [i].Level)));
			}

			return new FrequencyChart (list);
		}

		private T GetFromArray<T>(List<T> e,int index)
		{
			if (index < 0)
				return e [0];
			if (index >= e.Count)
				return e [e.Count - 1];
			return e [index];
		}

		public FrequencyChart Zoom(FrequencyChart chart)
		{
			var l = new List<FrequencyPair> ();
			for (int i = 0; i < chart.Count; i++) {
				if (GetFromArray (chart, i).Level < GetFromArray (chart, i + 1).Level && GetFromArray (chart, i).Level < GetFromArray (chart, i - 1).Level
		         || GetFromArray (chart, i).Level > GetFromArray (chart, i + 1).Level && GetFromArray (chart, i).Level > GetFromArray (chart, i - 1).Level)
					l.Add (GetFromArray (chart, i));
			}
			return new FrequencyChart (l);
		}

		public FrequencyChart LocalizeMaximums(FrequencyChart chart, int gapCount)
		{
			if (!chart.Any ())
				return chart;
			var list = new List<FrequencyPair> ();
			bool raising = true;
			FrequencyPair localMax = chart[0];
            FrequencyPair lastMeaninful = chart[0];
			for (var i = 0; i < chart.Count; i++) {
				double derivation;
				var cur = GetFromArray (chart, i);
				derivation = cur.Level - lastMeaninful.Level;

				if (raising) {
					if (derivation >= 0) {
						localMax = cur;
						lastMeaninful = localMax;
					} else {
						bool shouldContinueRaising = false;

						for (int j = i+1; j <= i + gapCount; j++) {
							if (GetFromArray (chart, j).Level - lastMeaninful.Level >= 0) {
								lastMeaninful = GetFromArray (chart, j);
								shouldContinueRaising = true;
								break;
							}
						}
						if (!shouldContinueRaising) {
							list.Add (localMax);
							lastMeaninful = cur;
							raising = false;
						}
					}
				} else {
					if (derivation <= 0) {
						lastMeaninful = cur;
					}
					else {
						bool shouldStartRaising = true;

						for (int j = i+1; j <= i + gapCount; j++) {
							if (GetFromArray (chart, j).Level - lastMeaninful.Level <= 0) {
								lastMeaninful = GetFromArray (chart, j);
								shouldStartRaising = false;
								break;
							}
						}
						if (shouldStartRaising) {
							localMax = cur;
							lastMeaninful = cur;
							raising = true;
						}
					}
				}
			}
            if (raising)
                list.Add(chart[chart.Count - 1]);
			return new FrequencyChart (list);
		}

    }
}
