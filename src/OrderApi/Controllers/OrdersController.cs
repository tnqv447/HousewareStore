using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrderApi.Data.Repositories;
using OrderApi.DTOs;
using OrderApi.Models;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppSettings _settings;
        private readonly IOrderRepository _orderRepo;
        private readonly IOrderItemRepository _orderItemRepo;
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository orderRepo, IOrderItemRepository orderItemRepo, IOptions<AppSettings> settings, IMapper mapper)
        {
            _settings = settings.Value;
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderDTO>> GetOrders()
        {
            var orders = await _orderRepo.GetAllAsync();

            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(orders);
        }

        [HttpGet("buyerId/{buyerId}")]
        public async Task<IEnumerable<OrderDTO>> GetOrders(string buyerId)
        {
            var orders = await _orderRepo.GetByBuyerAsync(buyerId);

            return _mapper.Map<IEnumerable<Order>, IEnumerable<OrderDTO>>(orders);
        }

        [HttpGet("salesId/{salesId}")]
        public async Task<IEnumerable<OrderItemForSalesDTO>> GetOrdersBySales(string salesId)
        {
            var orderItems = await _orderRepo.GetBySalesAsync(salesId);
            if (orderItems == null || orderItems.Count() == 0)
            {
                return null;
            }
            else
            {
                var order = await _orderRepo.GetByAsync(orderItems.ElementAt(0).OrderId);
                var res = _mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemForSalesDTO>>(orderItems);
                this.TranferAdditionalInfo(order, res);
                return res;
            }
        }

        [HttpGet("{id}")]
        public async Task<OrderDTO> GetOrder(int id)
        {
            var order = await _orderRepo.GetByAsync(id);

            return _mapper.Map<Order, OrderDTO>(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDTO>> CreateOrder(OrderDTO orderDTO)
        {
            var order = _mapper.Map<OrderDTO, Order>(orderDTO);
            order.Status = OrderStatus.Preparing;
            order.OrderDate = DateTime.UtcNow;

            await _orderRepo.AddAsync(order);
            orderDTO = _mapper.Map<Order, OrderDTO>(order);

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, orderDTO);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _orderRepo.GetByAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            await _orderRepo.DeleteAsync(order);

            return NoContent();
        }

        [HttpPut("orderItems")]
        public async Task<IActionResult> UpdateOrderItem(int orderId, int itemId, OrderItemDTO dto)
        {
            if (!orderId.Equals(dto.OrderId) || !itemId.Equals(dto.ItemId))
            {
                return BadRequest();
            }

            // var Item = await _itemRepos.GetBy(id);
            var item = (await _orderRepo.GetByAsync(orderId)).OrderItems.Where(m => m.ItemId.Equals(itemId)).ElementAt(0);
            if (item == null)
            {
                return NotFound();
            }

            // _mapper.Map<Item>(ItemDTO);

            try
            {
                await _orderItemRepo.UpdateAsync(item);
            }
            catch (DbUpdateConcurrencyException) when (!_orderItemRepo.ItemExists(orderId, itemId))
            {
                return NotFound();
            }

            //update order status
            var order = await _orderRepo.GetByAsync(item.OrderId);
            var list = order.OrderItems;
            var count = list.Count();
            var res = OrderStatus.Preparing;
            int isRejected = 0;
            int isAccepted = 0;
            int isShipping = 0;
            int isDelivered = 0;
            foreach (var temp in list)
            {
                switch (temp.Status)
                {
                    case (OrderItemStatus.Rejected): isRejected++; break;
                    case (OrderItemStatus.Shipping): isShipping++; break;
                    case (OrderItemStatus.Accepted): isAccepted++; break;
                    case (OrderItemStatus.Delivered): isDelivered++; break;
                    default: break;
                }
            }
            if (isRejected.Equals(count)) res = OrderStatus.Rejected;
            else if (isDelivered.Equals(count)) res = OrderStatus.Delivered;
            else if (isShipping == count - isDelivered - isRejected) res = OrderStatus.Shipping;
            else if (isAccepted == count - isDelivered - isShipping - isRejected) res = OrderStatus.Accepted;

            order.Status = res;
            await _orderRepo.UpdateAsync(order);

            return NoContent();

        }

        //support functions
        private void TranferAdditionalInfo(Order order, IEnumerable<OrderItemForSalesDTO> items)
        {
            foreach (var item in items)
            {
                item.BuyerId = order.BuyerId;
                item.FirstName = order.FirstName;
                item.LastName = order.LastName;
                item.OrderDate = order.OrderDate;
                item.Address = order.Address;
                item.PaymentAuthCode = order.PaymentAuthCode;
            }
        }
    }
}