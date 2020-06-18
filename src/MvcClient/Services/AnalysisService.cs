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
        // Thống kê hết sale
        public async Task<IList<AnalysisViewModel>> CountAllSales()
        {
            IList<AnalysisViewModel> results = new List<AnalysisViewModel>();
            var listSales = await _userService.ManageUsers("Sales");
            foreach (var sale in listSales)
            {
                var count = await CountItemsBySalesAsync(sale.UserId);
                if (count != null)
                {
                    AnalysisViewModel result = new AnalysisViewModel
                    {
                        Sale = sale,
                        Count = count
                    };

                    results.Add(result);
                }

            }

            Console.WriteLine("\nTEST: " + results.ElementAt(0).Sale.UserId);
            return results;
        }
        // Chi tiết từng sale
        public async Task<IEnumerable<ItemAnalysis>> CountItemsBySalesAsync(string saleId)
        {
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            listOrders = listOrders.Where(m => m.Status != OrderItemStatus.Rejected && m.Status != OrderItemStatus.Preparing);

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
                                                    orderitemsUnitCount = (orderitems == null || orderitems.Count() == 0 ? 0 : orderitems.Sum(o => o.Units))//value 2
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