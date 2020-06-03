using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly IMapper _mapper;

        public OrdersController(IOrderRepository orderRepo, IOptions<AppSettings> settings, IMapper mapper)
        {
            _settings = settings.Value;
            _orderRepo = orderRepo;
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
    }
}