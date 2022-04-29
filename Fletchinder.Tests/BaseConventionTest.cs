using System.Collections.Generic;
using Fletchinder.Conventions;
using FluentAssertions;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace Fletchinder.Tests
{
    public abstract class BaseConventionTest
    {
        protected void VerifyConvention<T>(
            IConvention convention,
            IList<TrackChunk> voices,
            IEnumerable<IViolation<T>> expectedViolations,
            string extraBecauseContext = "") where T : ITimeSpan
        {
            var actualViolations = convention
                .MeetsConvention<T>(voices, TempoMap.Default);
            if (!string.IsNullOrWhiteSpace(extraBecauseContext) && !extraBecauseContext.StartsWith(' '))
            {
                extraBecauseContext = $" {extraBecauseContext}";
            }
            expectedViolations
                .Should()
                .BeEquivalentTo(
                    actualViolations,
                    $"expected violations should match actual violations{extraBecauseContext}");
        }
    }
}
