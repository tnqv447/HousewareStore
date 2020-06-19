using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MvcClient.Models;
using MvcClient.Services;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using MvcClient.Authorization;
using MvcClient.ViewModels;
namespace MvcClient.Controllers
{
    [Authorize(Roles = "Sales")]
    public class OrderForSaleController : Controller
    {
        private readonly ILogger<OrderForSaleController> _logger;
        private readonly IOrderService _orderService;
        private readonly IAuthorizationService _authorizationService;
        private readonly IIdentityService<Buyer> _identityService;
        public OrderForSaleController(ILogger<OrderForSaleController> logger, IOrderService orderService,
                             IAuthorizationService authorizationService, IIdentityService<Buyer> identityService)
        {
            _logger = logger;
            _orderService = orderService;
            _authorizationService = authorizationService;
            _identityService = identityService;
        }
        public async Task<IActionResult> Index(int pageNumber = 1, string searchType = null, string searchString = null, string status = null, string sortType = null, string sortOrder = null)
        {
            var viewModel = await getViewModel(pageNumber, searchType, searchString, status, sortType, sortOrder);
            return View(viewModel);
        }
        public async Task<IActionResult> OrderForSalePaging(int pageNumber = 1, string searchType = null, string searchString = null, string status = null, string sortType = null, string sortOrder = null)
        {
            var viewModel = await getViewModel(pageNumber, searchType, searchString, status, sortType, sortOrder);
            return new JsonResult(viewModel);
        }
        public async Task<OrderForSaleViewModel> getViewModel(int pageNumber = 1, string searchType = null, string searchString = null, string status = null, string sortType = null, string sortOrder = null)
        {
            OrderForSaleViewModel viewModel = new OrderForSaleViewModel();
            var pageSize = 8;
            OrderItemStatus Status = OrderItemStatus.AllStatus;
            switch (status)
            {
                case "Preparing": Status = OrderItemStatus.Preparing; break;
                case "Accepted": Status = OrderItemStatus.Accepted; break;
                case "Shipping": Status = OrderItemStatus.Shipping; break;
                case "Delivered": Status = OrderItemStatus.Delivered; break;
                case "Rejected": Status = OrderItemStatus.Rejected; break;
                default: Status = OrderItemStatus.AllStatus; break;
            }
            SearchTypeOrderItem SearchType = SearchTypeOrderItem.ItemName;
            switch (searchType)
            {
                case "BuyerName": SearchType = SearchTypeOrderItem.BuyerName; break;
                default: SearchType = SearchTypeOrderItem.ItemName; break;
            }
            SortOrderOrderItem SortOrder = SortOrderOrderItem.Ascending;
            switch (sortOrder)
            {
                case "Ascending": SortOrder = SortOrderOrderItem.Ascending; break;
                case "Descending": SortOrder = SortOrderOrderItem.Descending; break;
                default: SortOrder = SortOrderOrderItem.Ascending; break;
            }
            SortTypeOrderItem SortType = SortTypeOrderItem.OrderId;
            switch (sortType)
            {
                case "BuyerName": SortType = SortTypeOrderItem.BuyerName; break;
                case "ItemName": SortType = SortTypeOrderItem.ItemName; break;
                case "Status": SortType = SortTypeOrderItem.Status; break;
                default: SortType = SortTypeOrderItem.OrderId; break;
            }
            var saleId = User.IsInRole(Constants.SalesRole) ? _identityService.Get(User).Id : null;
            viewModel.OrderItems = await _orderService.GetOrderItemsForSales(saleId, SearchType, searchString, Status, SortType, SortOrder);
            if (viewModel.OrderItems != null)
            {
                viewModel.OrderItemsPaging = PaginatedList<OrderItemForSales>.Create(viewModel.OrderItems, pageNumber, pageSize);
                viewModel.PageIndex = pageNumber;
                viewModel.PageTotal = viewModel.OrderItemsPaging.TotalPages;
            }
            else
            {
                viewModel.OrderItemsPaging = null;
            }
            return viewModel;
        }


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string itemStatus, string action)
        {
            OrderItemStatus status = OrderItemStatus.Preparing;
            if (action == "rejected")
            {
                status = OrderItemStatus.Rejected;
            }
            if (action == "update")
            {
                switch (itemStatus)
                {
                    case "Accepted": status = OrderItemStatus.Shipping; break;
                    case "Preparing": status = OrderItemStatus.Accepted; break;
                    case "Shipping": status = OrderItemStatus.Delivered; break;
                    case "Delivered": status = OrderItemStatus.Delivered; break;
                }
            }
            var orderItem = await _orderService.GetOrderItem(id);

            // var isAuthorize = await _authorizationService.AuthorizeAsync(User, orderItem, Operations.Reject);
            // if (!isAuthorize.Succeeded)
            // {
            //     return Forbid();
            // }
            orderItem.Status = status;
            await _orderService.UpdateOrderItem(orderItem.OrderId, orderItem);// ham nay ko update

            string ans = "Preparing";
            switch (orderItem.Status)
            {
                case OrderItemStatus.Shipping: ans = "Shipping"; break;
                case OrderItemStatus.Rejected: ans = "Rejected"; break;
                case OrderItemStatus.Accepted: ans = "Accepted"; break;
                case OrderItemStatus.Preparing: ans = "Preparing"; break;
                case OrderItemStatus.Delivered: ans = "Delivered"; break;
            }
        
            return new JsonResult(ans);
        }
        [HttpPost]
        public async Task<IActionResult> Reject(int id)
        {
            var orderItem = await _orderService.GetOrderItem(id);

            // var isAuthorize = await _authorizationService.AuthorizeAsync(User, orderItem, Operations.Reject);
            // if (!isAuthorize.Succeeded)
            // {
            //     return Forbid();
            // }

            orderItem.Status = OrderItemStatus.Rejected;
            await _orderService.UpdateOrderItem(orderItem.OrderId, orderItem);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Shipping(int id)
        {
            var orderItem = await _orderService.GetOrderItem(id);

            // var isAuthorize = await _authorizationService.AuthorizeAsync(User, orderItem, Operations.Reject);
            // if (!isAuthorize.Succeeded)
            // {
            //     return Forbid();
            // }

            orderItem.Status = OrderItemStatus.Shipping;
            await _orderService.UpdateOrderItem(orderItem.OrderId, orderItem);

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> Delivered(int id)
        {
            var orderItem = await _orderService.GetOrderItem(id);

            // var isAuthorize = await _authorizationService.AuthorizeAsync(User, orderItem, Operations.Reject);
            // if (!isAuthorize.Succeeded)
            // {
            //     return Forbid();
            // }

            orderItem.Status = OrderItemStatus.Delivered;
            await _orderService.UpdateOrderItem(orderItem.OrderId, orderItem);

            return RedirectToAction(nameof(Index));
        }
        //chết mẹ m làm mỗi thằng cái khác nhau à, t tưởng m làm chung update, the thif lam chung cung dc, cow ma ngaoi tru id con them 1 bien nua

        [HttpPost]
        public async Task<IActionResult> Accepted(int id)
        {
            var orderItem = await _orderService.GetOrderItem(id);

            // var isAuthorize = await _authorizationService.AuthorizeAsync(User, orderItem, Operations.Reject);
            // if (!isAuthorize.Succeeded)
            // {
            //     return Forbid();
            // }

            orderItem.Status = OrderItemStatus.Accepted;
            await _orderService.UpdateOrderItem(orderItem.OrderId, orderItem);

            return RedirectToAction(nameof(Index));
        }


    }
}