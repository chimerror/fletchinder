using System;
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
            for (int currentVoiceIndex = 0; currentVoiceIndex < voices.Count; currentVoiceIndex++)
            {
                if (currentVoiceIndex == highestVoiceIndex)
                {
                    continue;
                }
                var times = notes[highestVoiceIndex]
                    .Union(notes[currentVoiceIndex])
                    .Select(n => n.TimeAs<T>(tempoMap))
                    .Distinct()
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

        public class Violation<T> :
            BaseViolation<T>,
            IComparable<HighestVoiceNotCrossedConvention.Violation<T>>,
            IEquatable<HighestVoiceNotCrossedConvention.Violation<T>> where T : ITimeSpan
        {
            public int HighVoiceIndex { get; set; }
            public int ViolatingVoiceIndex { get; set; }

            public int CompareTo(HighestVoiceNotCrossedConvention.Violation<T> other)
            {
                var baseComparison = base.CompareTo(other);
                var otherViolation = other as HighestVoiceNotCrossedConvention.Violation<T>;
                if (baseComparison != 0 || otherViolation == null)
                {
                    return baseComparison;
                }

                var highVoiceComparison = this.HighVoiceIndex.CompareTo(otherViolation.HighVoiceIndex);
                if (highVoiceComparison != 0)
                {
                    return highVoiceComparison;
                }

                return this.ViolatingVoiceIndex.CompareTo(otherViolation.ViolatingVoiceIndex);
            }

            public bool Equals(HighestVoiceNotCrossedConvention.Violation<T> other)
            {
                var baseEquality = base.Equals(other);
                if (!baseEquality)
                {
                    return baseEquality;
                }

                var otherViolation = other as HighestVoiceNotCrossedConvention.Violation<T>;
                if (otherViolation == null)
                {
                    return false;
                }
                else
                {
                    return this.HighVoiceIndex.Equals(other.HighVoiceIndex) &&
                        this.ViolatingVoiceIndex.Equals(other.ViolatingVoiceIndex);
                }
            }

            public override bool Equals(object obj)
            {
                if (obj == null)
                {
                    return false;
                }

                var otherViolation = obj as HighestVoiceNotCrossedConvention.Violation<T>;
                if (otherViolation == null)
                {
                    return false;
                }
                else
                {
                    return Equals(otherViolation);
                }
            }

            public override int GetHashCode()
            {
                return base.GetHashCode() ^ HighVoiceIndex ^ ViolatingVoiceIndex;
            }
        }
    }
}