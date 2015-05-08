using FuorieTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoundAnalysis
{

    public class Note
    {
        public double Frequency { get; set; }
        public string NoteName { get; set; }
        public int Octave { get; set; }

        public Note(double f,string name,int oct)
        {
            Octave = oct;
            Frequency = f;
            NoteName = name;
        }

        public override string ToString()
        {
            return NoteName + Octave;
        }
    }

    public class TuneInfo
    {
        public Note Note { get; set; }
        public double Shift { get; set; }

        public override string ToString()
        {
            return Note.ToString() + " "+Math.Round(Shift,1);
        }
    }

    public sealed class Bands
    {

        public List<Note> Notes = new List<Note>();


        public TuneInfo NoteByFreq(double freq)
        {
            var lmin = Notes.Last(k => k.Frequency <= freq);
            var fmax = Notes.First(k => k.Frequency >= freq);
            if (lmin == fmax)
                return new TuneInfo { Note = lmin,Shift = 0};

            var diff = (freq - lmin.Frequency)/(fmax.Frequency - lmin.Frequency);
            if (diff < 0.5)
            {
                return new TuneInfo { Note = lmin, Shift = diff };
            }
            else
            {
                return new TuneInfo { Note = fmax, Shift = diff-1 };
            }
        }
    }

    public sealed class BandManager
    {
        public string PickNoteWithVoting(Bands bands, FrequencyChart chart)
        {
            return chart.OrderByDescending(k => k.Level)
                            .Take(15)
                            .Select(k => bands.NoteByFreq(k.Frequency))
                            .GroupBy(k => k.Note.NoteName)
                            .Select(k => new { note = k.Key, count = k.Count() })
                            .OrderByDescending(k => k.count)
                            .First().note;
        }

		public TuneInfo PickSoloLeaderNote(Bands bands, FrequencyChart chart,int harmonics = 1)
		{
			return chart.OrderByDescending(k => k.Level)
				.Skip(harmonics-1)
                .Select(k => bands.NoteByFreq(k.Frequency))
				.First();
		}

        public Bands BoundGenerator(double maxFreq, int semitonesInBand)
        {

            var fact = Math.Pow(1.0595, semitonesInBand);
            var bands = new Bands();
            bands.Notes.Add(new Note(0,"-",0));
            var freqs = new List<double>(new[] { maxFreq });
            var names = new List<string>();
            for (var j = 0; freqs.Last() > 16; j++)
            {
                freqs.Add(freqs.Last() / fact);
                names.Add("");
            }
            freqs.Add(0);
            freqs.Reverse();                    
            return bands;
        }

		private IEnumerable<T> Unfold<T>(T init,Func<T,bool> ender,Func<T,T> next)
		{
			while (ender (init)) {
				yield return init;
				init = next (init);
			}
		}

		public Bands DrumTuningGenerator(double maxFreq)
		{


			var bands = new Bands();

			var e = Unfold (new Note (0, "", 0), k => k.Frequency < maxFreq,
				k =>new Note(k.Frequency < 70 
					?  k.Frequency + 0.1 
					: (k.Frequency < 1000 
						? k.Frequency + 1
						: k.Frequency + 10),"",0));
			bands.Notes = e.ToList();
			return bands;
		}

        public Bands TonesBoundGenerator()
        {
            var notes = new[]{
                "A#","B","C","C#","D","D#","E","F","F#","G","G#","A"
            };
            var bands = new Bands();
            bands.Notes.Add(new Note(0, "-", 0));
            bands.Notes.Add(new Note(27.5, "A", 0));
            var fact = 1.0595;          
            var octave = 0;
            var names = new List<string>{"-","A"};
            for (var j = 0; j < 9 * 12; j++)
            {
                bands.Notes.Add(new Note(bands.Notes.Last().Frequency * fact,notes[j % 12],octave));  
				if (j % 12 == 2)
					octave++;
            }                             
            return bands;
        }
    }
}
