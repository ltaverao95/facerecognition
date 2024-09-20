namespace Messaging.Core.Events
{
    public interface IOrderDispatchedEvent
    {
        Guid OrderId { get; }
        DateTime DispatchDateTime { get; }
    }
}