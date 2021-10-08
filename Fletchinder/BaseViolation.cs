using System;
using Melanchall.DryWetMidi.Interaction;
using Fletchinder.Conventions;

namespace Fletchinder
{
    public abstract class BaseViolation<T> : IViolation<T> where T : ITimeSpan
    {
        public T TimeSpan { get; set; }
        public IConvention Convention { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var otherViolation = obj as IViolation<T>;
            if (otherViolation == null)
            {
                throw new ArgumentException("Object is not a Violation");
            }
            else
            {
                return this.TimeSpan.CompareTo(otherViolation.TimeSpan);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var otherViolation = obj as IViolation<T>;
            if (otherViolation == null)
            {
                return false;
            }
            else
            {
                return this.Convention.Equals(otherViolation.Convention) &&
                    this.TimeSpan.Equals(otherViolation.TimeSpan);
            }
        }

        public override int GetHashCode()
        {
            return Convention.GetHashCode() ^ TimeSpan.GetHashCode();
        }
    }
}
