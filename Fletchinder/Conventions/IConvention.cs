using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;

namespace Fletchinder.Conventions
{
    public interface IConvention
    {
        IEnumerable<IViolation<T>> MeetsConvention<T>(IList<TrackChunk> voices, TempoMap tempoMap) where T : ITimeSpan;
    }
}