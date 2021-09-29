using System.Linq;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Fletchinder.Conventions
{
    public class HighestVoiceNotCrossedConvention : IConvention
    {
        public bool MeetsConvention(MidiFile composition)
        {
            var voices = composition
                .GetTrackChunks()
                .ToList();
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
            var tempoMap = composition.GetTempoMap();
            var violationsFound = false;
            for (int currentVoiceIndex = 1; currentVoiceIndex < voices.Count; currentVoiceIndex++)
            {
                if (currentVoiceIndex == highestVoiceIndex)
                {
                    continue;
                }
                var times = notes[highestVoiceIndex]
                    .Union(notes[currentVoiceIndex])
                    .Select(n => n.TimeAs<BarBeatTicksTimeSpan>(tempoMap))
                    .ToList();
                foreach (var time in times)
                {
                    var highNotes = notes[highestVoiceIndex].AtTime(time, tempoMap).ToList();
                    var lowNotes = notes[currentVoiceIndex].AtTime(time, tempoMap).ToList();
                    if (highNotes.Count == 0 || lowNotes.Count == 0)
                    {
                        continue;
                    }
                    else if (highNotes.Any(hn => lowNotes.Any(ln => ln.NoteNumber > hn.NoteNumber)))
                    {
                        violationsFound = true;
                    }
                }
            }
            return !violationsFound;
        }
    }
}