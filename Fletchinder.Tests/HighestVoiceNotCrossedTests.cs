using System.Linq;
using Fletchinder.Conventions;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using MT = Melanchall.DryWetMidi.MusicTheory;
using NUnit.Framework;

namespace Fletchinder.Tests
{
    public class HighestVoiceNotCrossedTests : BaseConventionTest
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
                .SetRootNote(MT.Note.Get(MT.NoteName.A, 4))
                .Note(MT.Interval.Zero) // A
                .Note(MT.Interval.One) // A#
                .Note(MT.Interval.Two) // B
                .Note(MT.Interval.Three) // C
                .Note(MT.Interval.Four) // C# <- Violation!
                .Note(MT.Interval.Three) // C
                .Note(MT.Interval.Two) // B
                .Note(MT.Interval.One) // A
                .Build();
            var voices = new [] { highVoice, lowVoice }
                .Select(p => p.ToTrackChunk(TempoMap.Default))
                .ToList();
            var convention = new HighestVoiceNotCrossedConvention();
            var expectedViolations = new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
            {
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                {
                    Convention = convention,
                    HighVoiceIndex = 0,
                    ViolatingVoiceIndex = 1,
                    TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                }
            };
            VerifyConvention<BarBeatTicksTimeSpan>(convention, voices, expectedViolations);
        }
    }
}