using Melanchall.DryWetMidi.Core;

namespace Fletchinder.Conventions
{
    public interface IConvention
    {
        bool MeetsConvention(MidiFile composition);
    }
}