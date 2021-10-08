using System.Collections.Generic;
using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Fletchinder.Conventions
{
    public class HighestVoiceNotCrossedConvention : IConvention
    {
        public IEnumerable<IViolation<T>> MeetsConvention<T>(IList<TrackChunk> voices, TempoMap tempoMap) where T : ITimeSpan
        {
            var notes = voices
                .Select(t => t.GetNotes())
                .ToList();
            var highestVoiceIndex = 0;
            var highestVoiceNoteNumber = notes[0].First().NoteNumber;
            for (int currentVoiceIndex = 1; currentVoiceIndex < voices.Count; currentVoiceIndex++)
            {
                var candidateNoteNumber = notes[currentVoiceIndex].First().NoteNumber;
                if (candidateNoteNumber > highestVoiceNoteNumber)
                {
                    highestVoiceIndex = currentVoiceIndex;
                    highestVoiceNoteNumber = candidateNoteNumber;
                }
            }
            for (int currentVoiceIndex = 1; currentVoiceIndex < voices.Count; currentVoiceIndex++)
            {
                if (currentVoiceIndex == highestVoiceIndex)
                {
                    continue;
                }
                var times = notes[highestVoiceIndex]
                    .Union(notes[currentVoiceIndex])
                    .Select(n => n.TimeAs<T>(tempoMap))
                    .ToList();
                foreach (var time in times)
                {
                    var highNotes = notes[highestVoiceIndex].AtTime(time, tempoMap).ToList();
                    var lowNotes = notes[currentVoiceIndex].AtTime(time, tempoMap).ToList();
                    if (highNotes.Count() == 0 || lowNotes.Count() == 0)
                    {
                        continue;
                    }
                    else if (highNotes.Any(hn => lowNotes.Any(ln => ln.NoteNumber > hn.NoteNumber)))
                    {
                        yield return new Violation<T>()
                        {
                            TimeSpan = time,
                            Convention = this,
                            HighVoiceIndex = highestVoiceIndex,
                            ViolatingVoiceIndex = currentVoiceIndex
                        };
                    }
                }
            }
        }

        public class Violation<T> : IViolation<T> where T : ITimeSpan
        {
            public T TimeSpan { get; set; }
            public IConvention Convention { get; set; }
            public int HighVoiceIndex { get; set; }
            public int ViolatingVoiceIndex { get; set; }
        }
    }
}