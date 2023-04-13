namespace HyperCasual.Core
{
    /// <summary>
    /// A link that is always open for transition.
    /// If the current state is linked to next step by this link type,
    /// The state machine moves to the next step once the execution of the current step is finished.
    /// </summary>
    public class Link : ILink
    {
        readonly IState m_NextState;
        
        /// <param name="nextState">the next state</param>
        public Link(IState nextState)
        {
            m_NextState = nextState;
        }

        public bool Validate(out IState nextState)
        {
            nextState = m_NextState;
            return true;
        }
    }
}