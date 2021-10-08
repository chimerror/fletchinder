using NUnit.Framework;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using MT = Melanchall.DryWetMidi.MusicTheory;
using System.Linq;
using Fletchinder.Conventions;

namespace Fletchinder.Tests
{
    public class HighestVoiceNotCrossedTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ViolationsCaught()
        {
            var highVoice = new PatternBuilder()
                .SetNoteLength(MusicalTimeSpan.Quarter)
                .Note(MT.Note.Get(MT.NoteName.C, 5))
                .Repeat(7)
                .Build();
            var lowVoice = new PatternBuilder()
                .SetNoteLength(MusicalTimeSpan.Quarter)
                .Note(MT.Note.Get(MT.NoteName.A, 4))
                .Note(MT.Interval.One) // A#
                .Note(MT.Interval.One) // B
                .Note(MT.Interval.One) // C
                .Note(MT.Interval.One) // D <- Violation!
                .Note(-MT.Interval.One) // C
                .Note(-MT.Interval.One) // B
                .Note(-MT.Interval.One) // A#
                .Build();
            var voices = new [] { highVoice, lowVoice }
                .Select(p => p.ToTrackChunk(TempoMap.Default))
                .ToList();
            var convention = new HighestVoiceNotCrossedConvention();
            var violations = convention.MeetsConvention<MusicalTimeSpan>(voices, TempoMap.Default);

            Assert.Pass();
        }
    }
}