using System;
using Melanchall.DryWetMidi.Interaction;
using Fletchinder.Conventions;

namespace Fletchinder
{
    public interface IViolation<T> : IComparable where T : ITimeSpan
    {
        T TimeSpan { get; set; }
        IConvention Convention { get; set; }
    }
}