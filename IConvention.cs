using Melanchall.DryWetMidi.Core;

public interface IConvention
{
    bool MeetsConvention(MidiFile composition);
}