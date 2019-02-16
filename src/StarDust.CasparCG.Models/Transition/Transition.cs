
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models
{

    /// <summary>
    /// Represent a transition in CapsarCG Server
    /// </summary>
    [Serializable]
    public class Transition
    {

        public Transition()
        {
        }


        /// <summary>
        /// Create a transtion description to send the command to CasparCG
        /// </summary>
        /// <param name="type">Kind of transition</param>
        /// <param name="duration"> Duration in frame</param>
        public Transition(TransitionType type, int duration)
        {
            this.Type = type;
            this.Duration = duration;
        }

        /// <summary>
        /// Create a transtion description to send the command to CasparCG
        /// </summary>
        /// <param name="type">Kind of transition</param>
        /// <param name="duration"> Duration in frame</param>
        /// <param name="direction"> Direction of the transition</param>
        public Transition(TransitionType type, int duration, TransitionDirection direction)
        {
            this.Type = type;
            this.Duration = duration;
        }

        /// <summary>
        /// Direction of the transition
        /// </summary>
        [DataMember]
        public TransitionDirection Direction { get; set; } = TransitionDirection.RIGHT;

        /// <summary>
        /// Kind of transition
        /// </summary>
        [DataMember]
        public TransitionType Type { get; set; } = TransitionType.CUT;

        /// <summary>
        /// Duration in frame
        /// </summary>
        [DataMember]
        public int Duration { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.Type.ToAmcpValue(), this.Duration.ToString(), this.Direction.ToAmcpValue());
        }
    }
}
