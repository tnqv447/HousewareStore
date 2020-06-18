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
        public async Task<IEnumerable<OrderItemForSalesDTO>> GetOrdersBySales(string salesId, SearchTypeOrderItem searchType = SearchTypeOrderItem.ItemName, string searchString = null,
                                OrderItemStatus status = OrderItemStatus.AllStatus, SortTypeOrderItem sortType = SortTypeOrderItem.OrderId, SortOrderOrderItem sortOrder = SortOrderOrderItem.Ascending)
        {
            var orderItems = await _orderRepo.GetBySalesAsync(salesId);
            if (orderItems == null || orderItems.Count() == 0)
            {
                return null;
            }
            else
            {
                var res = _mapper.Map<IEnumerable<OrderItem>, IEnumerable<OrderItemForSalesDTO>>(orderItems);
                await this.TranferAdditionalInfo(res);
                if (status != OrderItemStatus.AllStatus)
                {
                    res = res.Where(m => m.Status == status).ToList();
                }
                if (!String.IsNullOrEmpty(searchString))
                {
                    switch (searchType)
                    {
                        case (SearchTypeOrderItem.BuyerName):
                            res = res.Where(m => (m.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) || m.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList(); break;
                        case (SearchTypeOrderItem.ItemName):
                            res = res.Where(m => m.ItemName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList(); break;
                        default: break;
                    }
                }
                res = this.SortOrderItem(res, sortType, sortOrder);
                return res;
            }
        }
        private IEnumerable<OrderItemForSalesDTO> SortOrderItem(IEnumerable<OrderItemForSalesDTO> dtos, SortTypeOrderItem sortType, SortOrderOrderItem sortOrder)
        {

            switch (sortOrder)
            {
                case (SortOrderOrderItem.Descending):
                    {
                        switch (sortType)
                        {
                            case (SortTypeOrderItem.BuyerName):
                                dtos = dtos.OrderByDescending(m => m.FirstName.ToLower()).ToList(); break;
                            case (SortTypeOrderItem.ItemName):
                                dtos = dtos.OrderByDescending(m => m.ItemName.ToLower()).ToList(); break;
                            case (SortTypeOrderItem.Status):
                                dtos = dtos.OrderByDescending(m => m.Status).ToList(); break;
                            default:
                                dtos = dtos.OrderByDescending(m => m.OrderId).ToList(); break;
                        }
                        break;
                    }
                default:
                    {
                        switch (sortType)
                        {
                            case (SortTypeOrderItem.BuyerName):
                                dtos = dtos.OrderBy(m => m.FirstName.ToLower()).ToList(); break;
                            case (SortTypeOrderItem.ItemName):
                                dtos = dtos.OrderBy(m => m.ItemName.ToLower()).ToList(); break;
                            case (SortTypeOrderItem.Status):
                                dtos = dtos.OrderBy(m => m.Status).ToList(); break;
                            default:
                                dtos = dtos.OrderBy(m => m.OrderId).ToList(); break;
                        }
                        break;
                    }

            }
            return dtos;
        }

        [HttpGet("{id}")]
        public async Task<OrderDTO> GetOrder(int id)
        {
            var order = await _orderRepo.GetByAsync(id);

            return _mapper.Map<Order, OrderDTO>(order);
        }
        [HttpGet("orderItem/{id}")]
        public async Task<OrderItemDTO> GetOrderItem(int id)
        {
            var orderItem = await _orderItemRepo.GetByAsync(id);

            return _mapper.Map<OrderItem, OrderItemDTO>(orderItem);
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
        public async Task<IActionResult> UpdateOrderItem(int orderId, int itemId, OrderItemStatus status)
        {
            // if (!orderId.Equals(dto.OrderId) || !itemId.Equals(dto.ItemId))
            // {
            //     return BadRequest();
            // }

            // var Item = await _itemRepos.GetBy(id);
            // t van chua hieu nhi, lẽ ra get item tư orderitem ID la dc r, sao phai lay itemID == itemID nữa
            // hinh là no lay order r sau do tư order mơi lay orderItem, t di ăn cơm cai, dm nay t an thi m an di deo
            var item = (await _orderRepo.GetByAsync(orderId)).OrderItems.Where(m => m.ItemId == itemId).ElementAt(0);

            if (item == null)
            {
                return NotFound();
            }

            // _mapper.Map<Item>(ItemDTO);

            try
            {
                item.Status = status;
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
        private async Task TranferAdditionalInfo(IEnumerable<OrderItemForSalesDTO> items)
        {
            foreach (var item in items)
            {
                var order = await _orderRepo.GetByAsync(item.OrderId);
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