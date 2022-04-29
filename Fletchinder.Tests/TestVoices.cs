using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MT = Melanchall.DryWetMidi.MusicTheory;

namespace Fletchinder.Tests
{
    public static class TestVoices
    {
        public static TrackChunk SteadyEightNotes(MT.Note startingNote = null)
        {
            if (startingNote == null)
            {
                startingNote = MT.Note.Get(MT.NoteName.C, 4);
            }

            return new PatternBuilder()
                .SetNoteLength(MusicalTimeSpan.Quarter)
                .Note(startingNote)
                .Repeat(7)
                .Build()
                .ToTrackChunk(TempoMap.Default);
        }

        public static TrackChunk SteadyFourNotes(MT.Note startingNote = null)
        {
            if (startingNote == null)
            {
                startingNote = MT.Note.Get(MT.NoteName.C, 4);
            }

            return new PatternBuilder()
                .SetNoteLength(MusicalTimeSpan.Half)
                .Note(startingNote)
                .Repeat(3)
                .Build()
                .ToTrackChunk(TempoMap.Default);
        }

        public static TrackChunk RiseThenFallByHalfStepEightNotes(MT.Note startingNote = null)
        {
            if (startingNote == null)
            {
                startingNote = MT.Note.Get(MT.NoteName.C, 4);
            }

            return new PatternBuilder()
                .SetNoteLength(MusicalTimeSpan.Quarter)
                .SetRootNote(startingNote)
                .Note(MT.Interval.Zero)
                .Note(MT.Interval.One)
                .Note(MT.Interval.Two)
                .Note(MT.Interval.Three)
                .Note(MT.Interval.Four)
                .Note(MT.Interval.Three)
                .Note(MT.Interval.Two)
                .Note(MT.Interval.One)
                .Build()
                .ToTrackChunk(TempoMap.Default);
        }
    }
}