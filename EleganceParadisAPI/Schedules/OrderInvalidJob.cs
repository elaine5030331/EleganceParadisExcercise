using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Interfaces;
using Coravel.Invocable;

namespace Infrastructure.Schedules
{
    public class OrderInvalidJob : IInvocable
    {
        private readonly IRepository<Order> _orderRepository;

        public OrderInvalidJob(IRepository<Order> orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Invoke()
        {
            var currentTime = DateTimeOffset.UtcNow.AddDays(-1);
            var invalidOrders = await _orderRepository.ListAsync(o => o.OrderStatus == OrderStatus.Pending && o.CreateAt.CompareTo(currentTime) > 0);
            foreach (var order in invalidOrders)
            {
                order.OrderStatus = OrderStatus.Failed;
            }
            await _orderRepository.UpdateRangeAsync(invalidOrders);
        }
    }
}
