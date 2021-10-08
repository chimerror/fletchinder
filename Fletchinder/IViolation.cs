using Melanchall.DryWetMidi.Interaction;
using Fletchinder.Conventions;

namespace Fletchinder
{
    public interface IViolation<T> where T : ITimeSpan
    {
        T TimeSpan { get; set; }
        IConvention Convention { get; set; }
    }
}