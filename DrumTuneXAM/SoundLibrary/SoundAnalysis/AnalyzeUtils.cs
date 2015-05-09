using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuorieTest
{
    public static class AnalyzeUtils
    {
		public static int FindMaxLessPow(int e)
		{
			return Enumerable.Range (0, 20)
				.Select (k => (int)Math.Pow (2, k))
				.Last (k => k < e);
		}

		public static int FindMinMorePow(int e)
		{
			return Enumerable.Range (0, 20)
				.Select (k => (int)Math.Pow (2, k))
				.First (k => k >= e);
		}

        public static IEnumerable<double[]> Windows(this double[] source, double[] func, int shift)
        {

            for (int i = 0; i + func.Length < source.Length; i += shift)
            {
                var subSource = new double[func.Length];
                for (int j = 0; j < shift; j++)
                {
                    subSource[j] = func[j] * source[i + j];
                }

                yield return subSource;
            }
        }

        public static short[] BTS(this byte[] b,bool stereo)
        {
            if (!stereo)
               return Enumerable.Range(0, b.Length / 2)
                             .Select(k => BitConverter.ToInt16(b, k * 2)).ToArray();

            return Enumerable.Range(0, b.Length / 4)
                            .Select(k => BitConverter.ToInt16(b, k * 4)).ToArray();

        }

        public static IEnumerable<T[]> ClusterBy<T>(this IEnumerable<T> source, Func<T, double> clusteringKey, double thresh)
        {
			
            var buff = new List<T>();
			var acc = new List<double>();
            foreach (var t in source.OrderBy(clusteringKey))
            {
                var k = clusteringKey(t);
                if (!buff.Any())
                {
                    buff.Add(t);
                    acc.Add(k);
                }
                else
                {
					if (Math.Abs((acc.Mean() - k) / acc.Mean()) > thresh)
                    {
                        yield return buff.ToArray();
                        buff.Clear();
                        buff.Add(t);
                        acc.Clear();
                        acc.Add(k);
                    }
                    else
                    {
                        buff.Add(t);
                        acc.Add(k);
                    }
                }
            }
            yield return buff.ToArray();
        }


        public static List<List<T>> ClusterGroup<T>(this IEnumerable<IEnumerable<T>> columns, Func<T, double> keySelector, double threshold)
        {
            var rows = new List<List<T>>();
            var accs = new List<List<double>>();
            foreach (var column in columns)
            {
                if (!rows.Any())
                {
                    foreach (var r in column)
                    {
                        var row = new List<T>();
                        row.Add(r);
                        rows.Add(row);
						var acc = new List<double>();
                        acc.Add(keySelector(r));
                        accs.Add(acc);
                    }
                }
                else
                {
                    foreach (var r in column)
                    {
                        foreach (var exRowNum in Enumerable.Range(0, accs.Count()))
                        {
                            var acc = accs[exRowNum];
                            var row = rows[exRowNum];
                            var key = keySelector(r);
							if (Math.Abs((acc.Mean() - key) / acc.Mean()) < threshold)
                            {
                                row.Add(r);
                                acc.Add(key);
                                break;
                            }
                            if (exRowNum == accs.Count() - 1)
                            {
                                row = new List<T>();
                                row.Add(r);
                                rows.Add(row);
								acc = new List<double>();
                                acc.Add(keySelector(r));
                                accs.Add(acc);
                            }
                        }
                    }
                }
            }
            return rows;
        }

    }
}
