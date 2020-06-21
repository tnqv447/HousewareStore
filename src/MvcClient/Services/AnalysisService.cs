using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;
using System.Linq;
using MvcClient.Infrastructure;
using MvcClient.ViewModels;

namespace MvcClient.Services
{
    public class AnalysisService : IAnalysisService
    {
        // private readonly string _serviceBaseUrl;
        private readonly IHttpClient _httpClient;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IItemService _itemService;
        public AnalysisService(IHttpClient httpClient, IOrderService orderService, IUserService userService,
        IItemService itemService)
        {
            _httpClient = httpClient;
            _orderService = orderService;
            _userService = userService;
            _itemService = itemService;

        }
        public async Task<IEnumerable<ItemAnalysis>> CountItemsByBuyersAsync(string saleId)
        {
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);
            // listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);
            var listBuyers = await _userService.GetBuyers();
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();
            if (listOrders != null)
            {
                results = listOrders.GroupBy(m => m.BuyerId)
                                        .Select(
                                            m => new ItemAnalysis
                                            {
                                                Name = (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))) == null ||
                                                        (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))).Count() == 0
                                                        ? "null" : (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))).FirstOrDefault().Name,
                                                UserId = m.First().BuyerId,
                                                TotalUnits = m.Sum(o => o.Units),
                                                TotalPrices = m.Sum(o => Math.Round(o.Units * o.UnitPrice, 2))
                                            }
                                        );
            }

            

            return results;
        }
        public async Task<IList<BaseAnalysis>> CountItemAllBuyers()
        {
            IList<BaseAnalysis> result = new List<BaseAnalysis>();
            var listBuyers = await _userService.GetBuyers();
            var listOrder = await _orderService.GetOrders();
            if (listOrder == null)
            {

            }
            else
            {
                listOrder = listOrder.Where(m => m.Status != OrderStatus.Rejected && m.Status != OrderStatus.Preparing);
                var listItem = await _itemService.GetAll();
                foreach (var buyer in listBuyers)
                {
                    var listOrderOfBuyers = listOrder.Where(m => m.BuyerId.Equals(buyer.UserId));
                    if (listOrderOfBuyers != null)
                    {
                        IList<OrderItem> orderItems = new List<OrderItem>();
                        foreach (var order in listOrderOfBuyers)
                        {
                            foreach (var item in order.OrderItems)
                            {
                                orderItems.Add(item);
                            }
                        }
                        var temp = listItem.GroupJoin(orderItems,
                                                        item => item.Id,
                                                        order => order.ItemId,
                                                        (item, orders) => new
                                                        {
                                                            item = item,
                                                            orderitemsUnitCount = (orders == null || orders.Count() == 0 ? 0 : orders.Sum(o => o.Units))
                                                        })
                                                        .Select(m => new ItemAnalysis
                                                        {
                                                            PictureUrl = m.item.PictureUrl,
                                                            ItemId = m.item.Id,
                                                            Name = m.item.Name,
                                                            UnitPrice = m.item.UnitPrice,
                                                            TotalUnits = m.orderitemsUnitCount,
                                                            TotalPrices = Math.Round(m.item.UnitPrice * m.orderitemsUnitCount, 2)
                                                        });
                        temp = temp.Where(m => m.TotalUnits > 0);
                        BaseAnalysis result_i = new BaseAnalysis
                        {
                            User = buyer,
                            Count = temp,
                            TotalUnits = temp.Sum(m => m.TotalUnits),
                            TotalPrices = temp.Sum(m => m.TotalPrices)
                        };
                        result.Add(result_i);
                    }

                }
            }


            return result;
        }
        public async Task<BaseAnalysis> CountItemInBuyer(string buyerId, string saleId)
        {
            var listOrder = await _orderService.GetOrderItemsForSales(saleId);
            listOrder = listOrder.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);
            var listItem = await _itemService.GetItemsSale(saleId);
            var listOrderBuyer = listOrder.Where(m => m.BuyerId.Equals(buyerId));
            var temp = listItem.GroupJoin(listOrderBuyer,
                                            item => item.Id,
                                            order => order.ItemId,
                                            (item, orders) => new
                                            {
                                                item = item,
                                                orderitemsUnitCount = (orders == null || orders.Count() == 0 ? 0 : orders.Sum(o => o.Units))

                                            })
                                            .Select(
                                                m => new ItemAnalysis
                                                {
                                                    PictureUrl = m.item.PictureUrl,
                                                    ItemId = m.item.Id,
                                                    Name = m.item.Name,
                                                    UnitPrice = m.item.UnitPrice,
                                                    TotalUnits = m.orderitemsUnitCount,
                                                    TotalPrices = Math.Round(m.item.UnitPrice * m.orderitemsUnitCount, 2)
                                                }
                                            );
            BaseAnalysis result = new BaseAnalysis
            {
                Count = temp,
                TotalUnits = temp.Sum(m => m.TotalUnits),
                TotalPrices = temp.Sum(m => m.TotalPrices)
            };
            return result;
        }
        
        public async Task<IList<BaseAnalysis>> CountAllSales()
        {
            IList<BaseAnalysis> results = new List<BaseAnalysis>();
            var listSales = await _userService.GetSales();
            foreach (var sale in listSales)
            {
                Console.WriteLine("\n sale: " + sale.Name);
                var count = await CountItemsBySalesAsync(sale.UserId);
                var totalUnits = 0;
                double totalPrices = 0;
                foreach (var item in count)
                {
                    totalUnits += item.TotalUnits;
                    totalPrices += Math.Round(item.TotalUnits * item.UnitPrice, 2);
                }
                if (count != null)
                {
                    BaseAnalysis result = new BaseAnalysis
                    {
                        User = sale,
                        Count = count,
                        TotalPrices = totalPrices,
                        TotalUnits = totalUnits
                    };

                    results.Add(result);
                }
                results.OrderByDescending(m => m.TotalPrices);
            }

            return results;
        }
        public async Task<IList<BaseAnalysis>> AllSales(DateTime date)
        {
            IList<BaseAnalysis> results = new List<BaseAnalysis>();
            var listSales = await _userService.GetSales();
            foreach (var sale in listSales)
            {
                Console.WriteLine("\n sale: " + sale.Name);
                var count = await SaleAsync(sale.UserId,date);
                var totalUnits = 0;
                double totalPrices = 0;
                foreach (var item in count)
                {
                    totalUnits += item.TotalUnits;
                    totalPrices += Math.Round(item.TotalUnits * item.UnitPrice, 2);
                }
                if (count != null)
                {
                    BaseAnalysis result = new BaseAnalysis
                    {
                        User = sale,
                        Count = count,
                        TotalPrices = totalPrices,
                        TotalUnits = totalUnits
                    };

                    results.Add(result);
                }
                results.OrderByDescending(m => m.TotalPrices);
            }

            return results;
        }
        
        public async Task<IEnumerable<ItemAnalysis>> CountItemsBySalesAsync(string saleId)
        {
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();

            if (listOrders == null)
            {
                Console.WriteLine("\nnull");
            }
            // Console.WriteLine("\nStatus: " + listOrders.ElementAt(0).ItemName + " ");
            else
            {
                listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);

                var listItems = await _itemService.GetItemsSale(saleId);

                if (listOrders != null && listItems != null)
                {
                    
                    results = listItems.GroupJoin(listOrders,
                                                    item => item.Id, // key của left table
                                                    orderitems => orderitems.ItemId,// key của right table
                                                    (item, orderitems) => //(Cái (a,b) này nó là table mới với 2 giá trị a,b)
                                                    new
                                                    {
                                                        item = item, //value 1
                                                        orderitemsUnitCount = (orderitems == null || orderitems.Count() == 0 ? 0 : orderitems.Sum(o => o.Units))
                                                    })
                                                    .Select(
                                                        m => new ItemAnalysis
                                                        {
                                                            PictureUrl = m.item.PictureUrl,
                                                            ItemId = m.item.Id, //gán các giá trị vào
                                                            Name = m.item.Name,
                                                            UnitPrice = m.item.UnitPrice,
                                                            TotalUnits = m.orderitemsUnitCount
                                                        }
                                                    );
                }
            }



            return results;
        }
        public async Task<IEnumerable<ItemAnalysis>> CountAllProducts(string saleId = null){
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();
            IEnumerable<Item> listItems = Enumerable.Empty<Item>();
            IList<OrderItem> listOrderItems = new List<OrderItem>();

            if(!String.IsNullOrEmpty(saleId)){
                listItems = await _itemService.GetItemsSale(saleId);
                
                var listOrderitems = await _orderService.GetOrderItemsForSales(saleId);
                listOrderitems = listOrderitems.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);
                results = listItems.GroupJoin(listOrderitems,
                                        item => item.Id,
                                        order => order.ItemId,
                                        (item,orders) => new {
                                            item = item,
                                            orderitemsUnitCount = (orders == null || orders.Count() == 0 ? 0 : orders.Sum(o => o.Units))
                                        })
                                        .Select(m => new ItemAnalysis{
                                            PictureUrl = m.item.PictureUrl,
                                            ItemId = m.item.Id,
                                            Name = m.item.Name,
                                            TotalUnits = m.orderitemsUnitCount,
                                            TotalPrices = Math.Round(m.orderitemsUnitCount * m.item.UnitPrice,2)
                                        });
            }
            else{
                listItems = await _itemService.GetAll();
                var listOrders = await _orderService.GetOrders();
                listOrders = listOrders.Where(m => m.Status != OrderStatus.Rejected && m.Status != OrderStatus.Preparing);
                foreach(var order in listOrders){
                    foreach(var item in order.OrderItems){
                        listOrderItems.Add(item);
                    }
                }
                results = listItems.GroupJoin(listOrderItems,
                                        item => item.Id,
                                        order => order.ItemId,
                                        (item,orders) => new {
                                            item = item,
                                            orderitemsUnitCount = (orders == null || orders.Count() == 0 ? 0 : orders.Sum(o => o.Units))
                                        })
                                        .Select(m => new ItemAnalysis{
                                            PictureUrl = m.item.PictureUrl,
                                            ItemId = m.item.Id,
                                            Name = m.item.Name,
                                            TotalUnits = m.orderitemsUnitCount,
                                            TotalPrices = Math.Round(m.orderitemsUnitCount * m.item.UnitPrice,2)
                                        });
            }
            
            
            return results;
        }
        public async Task<IEnumerable<ItemAnalysis>> SaleAsync(string saleId,DateTime date){
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            listOrders = listOrders.Where(m => DateTime.Compare(m.OrderDate,date) == 0);
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();

            if (listOrders == null)
            {
                Console.WriteLine("\nnull");
            }
            // Console.WriteLine("\nStatus: " + listOrders.ElementAt(0).ItemName + " ");
            else
            {
                listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);

                var listItems = await _itemService.GetItemsSale(saleId);

                if (listOrders != null && listItems != null)
                {
                    
                    results = listItems.GroupJoin(listOrders,
                                                    item => item.Id, 
                                                    orderitems => orderitems.ItemId,
                                                    (item, orderitems) => 
                                                    new
                                                    { 
                                                        item = item, //value 1
                                                        orderitemsUnitCount = (orderitems == null || orderitems.Count() == 0 ? 0 : orderitems.Sum(o => o.Units))
                                                    })
                                                    .Select(
                                                        m => new ItemAnalysis
                                                        {
                                                            ItemId = m.item.Id, 
                                                            Name = m.item.Name,
                                                            UnitPrice = m.item.UnitPrice,
                                                            TotalUnits = m.orderitemsUnitCount
                                                        }
                                                    );
                }
            }



            return results;
        }
        

    }
}