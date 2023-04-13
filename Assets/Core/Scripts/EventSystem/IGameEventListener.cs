namespace HyperCasual.Core
{
    /// <summary>
    /// All classes that want to subscribe to an event must implement this interface   
    /// </summary>
    public interface IGameEventListener
    {
        /// <summary>
        /// The event handler that is called when the subscribed event is triggered
        /// </summary>
        void OnEventRaised();
    }
}