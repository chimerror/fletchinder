using System.Linq;
using Fletchinder.Conventions;
using FluentAssertions;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Interaction;
using MT = Melanchall.DryWetMidi.MusicTheory;
using NUnit.Framework;

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
            var violations = convention
                .MeetsConvention<BarBeatTicksTimeSpan>(voices, TempoMap.Default)
                .ToList();

            violations.Count.Should().Be(1, "there should be only one violation");
            var violation = violations[0] as HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>;
            violation.Should().NotBeNull("it should be castable to the right violation");
            violation.Convention.Should().BeSameAs(convention, "the original convention should be returned");
            violation.HighVoiceIndex.Should().Be(0, "the highest voice should be determined correctly");
            violation.ViolatingVoiceIndex.Should().Be(1, "the violating voice should be determined correctly");
            violation.TimeSpan.Should().Be(
                BarBeatTicksTimeSpan.Parse("1.0.0"),
                "the violation should be marked at the right time");
        }
    }
}