using System.Collections.Generic;
using Fletchinder.Conventions;
using Melanchall.DryWetMidi.Core;
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

        [TestCaseSource(nameof(HighestVoiceTestCases))]
        public void ViolationsCaught(
            HighestVoiceNotCrossedConvention convention,
            TrackChunk[] voices,
            IEnumerable<HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>> expectedViolations,
            string extraBecauseContext = "")
        {
            VerifyConvention<BarBeatTicksTimeSpan>(convention, voices, expectedViolations, extraBecauseContext);
        }

        private static IEnumerable<object> HighestVoiceTestCases()
        {
            var convention = new HighestVoiceNotCrossedConvention();
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4))
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 0,
                        ViolatingVoiceIndex = 1,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    }
                },
                "when a single voice in track 1 rises above the high voice in track 0"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[] {
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4)),
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5))
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 1,
                        ViolatingVoiceIndex = 0,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    }
                },
                "when a single voice in track 0 rises above the high voice in track 1"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4)),
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4))
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 1,
                        ViolatingVoiceIndex = 0,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    },
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 1,
                        ViolatingVoiceIndex = 2,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    }
                },
                "when multiple voices rise above a steady high voice"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 4)),
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                },
                "when two steady voices are an octave apart and do not cross"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.GSharp, 4)),
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                },
                "when a rising and falling voice does not cross a steady voice above it"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4)),
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.SteadyEightNotes(MT.Note.Get(MT.NoteName.C, 4)),
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 1,
                        ViolatingVoiceIndex = 0,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    }
                },
                "when multiple higher voices are crossed, generating only a violation for the highest voice"
            };
            yield return new object[]
            {
                convention,
                new TrackChunk[]
                {
                    TestVoices.SteadyFourNotes(MT.Note.Get(MT.NoteName.C, 5)),
                    TestVoices.RiseThenFallByHalfStepEightNotes(MT.Note.Get(MT.NoteName.A, 4))
                },
                new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>[]
                {
                    new HighestVoiceNotCrossedConvention.Violation<BarBeatTicksTimeSpan>()
                    {
                        Convention = convention,
                        HighVoiceIndex = 0,
                        ViolatingVoiceIndex = 1,
                        TimeSpan = BarBeatTicksTimeSpan.Parse("1.0.0")
                    }
                },
                "when a high voice with longer notes is crossed by one with shorter notes"
            };
        }
    }
}