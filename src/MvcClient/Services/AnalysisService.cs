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
        //for sales ==================================================
        //

        //for managers ==================================================
        //
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
            // listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);
            var listBuyers = await _userService.GetBuyers();
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();
            if( listOrders != null){
                results = listOrders.GroupBy(m => m.BuyerId)
                                        .Select(
                                            m =>  new ItemAnalysis{
                                                Name = (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))) == null ||
                                                        (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))).Count() == 0 
                                                        ? "null" : (listBuyers.Where(s => s.UserId.Equals(m.First().BuyerId))).FirstOrDefault().Name,
                                                UserId = m.First().BuyerId,
                                                TotalUnits = m.Sum(o => o.Units),
                                                TotalPrices = m.Sum(o => Math.Round(o.Units * o.UnitPrice,2))
                                            }
                                        );
            }
            
            // if (listOrders != null && listBuyers != null)
            // {
            //     //Đầu tiên là left join 2 bảng vừa lấy về
            //     results = listBuyers.GroupJoin(listOrders,
            //                                     buyer => buyer.UserId, // key của left table
            //                                     orderitems => orderitems.BuyerId,// key của right table
            //                                     (buyer, orderitems) => //(Cái (a,b) này nó là table mới với 2 giá trị a,b)
            //                                     new
            //                                     { //new này là tạo table mới chứa 2 giá trị dưới
            //                                         buyer = buyer, //value 1
            //                                         orderitemsUnitCount = (orderitems == null || orderitems.Count() == 0 ? 0 : orderitems.Sum(o => o.Units)),
            //                                         orderItemsTotal = (orderitems == null || orderitems.Count() == 0 ? 0: orderitems.Sum(o => Math.Round(o.UnitPrice * o.Units,2)))
            //                                     })
            //                                     .Select(
            //                                         m => new ItemAnalysis
            //                                         {
            //                                             UserId = m.buyer.UserId, //gán các giá trị vào
            //                                             Name = m.buyer.Name,
            //                                             TotalUnits = m.orderitemsUnitCount,
            //                                             TotalPrices = m.orderItemsTotal
            //                                         }
            //                                     );
            // }

            return results;
        }
        public async Task<IList<AllSaleAnal>> CountItemAllBuyers(){
            IList<AllSaleAnal> result = new List<AllSaleAnal>();
            var listBuyers = await _userService.GetBuyers();
            var listOrder = await _orderService.GetOrders();
            var listItem = await _itemService.GetAll();
            foreach(var buyer in listBuyers){
                var listOrderOfBuyers = listOrder.where(m => m.BuyerId.Equals(buyer.UserId));
                
            }
            
            return result;
        }
        public async Task<AllSaleAnal> CountItemInBuyer(string buyerId, string saleId){
            var listOrder = await _orderService.GetOrderItemsForSales(saleId);
            var listItem = await _itemService.GetItemsSale(saleId);
            var listOrderBuyer = listOrder.Where(m => m.BuyerId.Equals(buyerId));
            var temp = listItem.GroupJoin(listOrderBuyer,
                                            item => item.Id,
                                            order => order.ItemId,
                                            (item,orders) => new{
                                                item = item,
                                                orderitemsUnitCount = (orders == null || orders.Count() == 0 ? 0 : orders.Sum(o => o.Units))
                                                
                                            })
                                            .Select(
                                                m => new ItemAnalysis{
                                                    ItemId = m.item.Id,
                                                    Name = m.item.Name,
                                                    UnitPrice = m.item.UnitPrice,
                                                    TotalUnits = m.orderitemsUnitCount,
                                                    TotalPrices = Math.Round(m.item.UnitPrice * m.orderitemsUnitCount,2)
                                                }
                                            );
            AllSaleAnal result = new AllSaleAnal{
                Count = temp,
                TotalUnits = temp.Sum(m => m.TotalUnits),
                TotalPrices = temp.Sum(m => m.TotalPrices)
            };
            return result;
        }
        // Thống kê hết sale
        public async Task<IList<AllSaleAnal>> CountAllSales()
        {
            IList<AllSaleAnal> results = new List<AllSaleAnal>();
            var listSales = await _userService.GetSales();
            foreach (var sale in listSales)
            {
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
                    AllSaleAnal result = new AllSaleAnal
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
        // Chi tiết từng sale
        public async Task<IEnumerable<ItemAnalysis>> CountItemsBySalesAsync(string saleId)
        {
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            // listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);

            var listItems = await _itemService.GetItemsSale(saleId);
            IEnumerable<ItemAnalysis> results = Enumerable.Empty<ItemAnalysis>();

            if (listOrders != null && listItems != null)
            {
                //Đầu tiên là left join 2 bảng vừa lấy về
                results = listItems.GroupJoin(listOrders,
                                                item => item.Id, // key của left table
                                                orderitems => orderitems.ItemId,// key của right table
                                                (item, orderitems) => //(Cái (a,b) này nó là table mới với 2 giá trị a,b)
                                                new
                                                { //new này là tạo table mới chứa 2 giá trị dưới
                                                    item = item, //value 1
                                                    orderitemsUnitCount = (orderitems == null || orderitems.Count() == 0 ? 0 : orderitems.Sum(o => o.Units))
                                                })
                                                .Select(
                                                    m => new ItemAnalysis
                                                    {
                                                        ItemId = m.item.Id, //gán các giá trị vào
                                                        Name = m.item.Name,
                                                        UnitPrice = m.item.UnitPrice,
                                                        TotalUnits = m.orderitemsUnitCount
                                                    }
                                                );
            }

            return results;
        }




    }
    public static class LinqEx
    {
        //cac ham left join,right join, inner join, outer join
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            this IEnumerable<TOuter> outer,
            IEnumerable<TInner> inner,
            Func<TOuter, TKey> outerKeySelector,
            Func<TInner, TKey> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer
                .GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => new
                {
                    a,
                    b
                })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => resultSelector(x.a, b));
        }
    }
}