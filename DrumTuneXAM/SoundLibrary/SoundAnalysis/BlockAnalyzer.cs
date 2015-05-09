using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FuorieTest;

namespace SoundLibrary.SoundAnalysis
{
    public class BlockAnalyzer
    {
        private SpectralAnalyzer _analyzer;
        private int _size;
        public BlockInfo AnalyzeBlock(short[] amp)
        {
            var blocks1 = new List<FrequencyChart>();
            var blocks2 = new List<FrequencyChart>();

            while (amp.Any())
            {
                var ampPart = amp.Take(_size).ToArray();
                amp = amp.Skip(_size / 4).ToArray();

                var sound = _analyzer.ConvertFromSamples(ampPart);

                var spectrum = _analyzer.AnalyzeSound(sound, 0);
                blocks1.Add(_analyzer.CutEdges(spectrum, 100, 400));
                blocks2.Add(_analyzer.CutEdges(spectrum, 400, 800));
            }

            var totalSpectrum1 = _analyzer.SelectAverage(blocks1.ToArray());
            var totalSpectrum2 = _analyzer.SelectAverage(blocks2.ToArray());

            return new BlockInfo(Math.Round(totalSpectrum1.OrderByDescending(k => k.Level).First().Frequency),totalSpectrum1);
            
        }

        public BlockAnalyzer(int blockSize,int discr)
        {
            _analyzer = new SpectralAnalyzer(blockSize, discr);
            _size = blockSize;
        }
    }
}