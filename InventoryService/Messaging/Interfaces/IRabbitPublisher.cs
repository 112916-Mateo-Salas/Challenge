namespace InventoryService.Messaging.Interfaces
{
    public interface IRabbitPublisher
    {
          Task Publish(object data, string routingKey);
    }
}
