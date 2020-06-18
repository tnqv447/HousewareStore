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
        public AnalysisService(IHttpClient httpClient,IOrderService orderService,IUserService userService,
        IItemService itemService){
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
            foreach(var sale in listSales){
                var count = await CountItemsBySalesAsync(sale.UserId);
                if(count!=null){
                    AnalysisViewModel result = new AnalysisViewModel{
                        Sale = sale,
                        Count = count
                    };
                    
                    results.Add(result);
                }
                
            }
                
            Console.WriteLine("\nTEST: "+ results.ElementAt(0).Sale.UserId);
            return results;
        }
        // Chi tiết từng sale
        public async Task<IEnumerable<Management>> CountItemsBySalesAsync(string saleId)
        {
            var listOrders = await _orderService.GetOrderItemsForSales(saleId);
            var listItems = await _itemService.GetItemsSale(saleId);
            IEnumerable<Management> results = Enumerable.Empty<Management>();

            if(listOrders != null && listItems != null){
                    results = listItems.GroupJoin(listOrders,
                                                    it => it.Id,
                                                    oit => oit.ItemId,
                                                    (it,itGr) => new{
                                                        it = it,
                                                        itGr = itGr
                                                    })
                                                    .SelectMany(
                                                        m => m.itGr.DefaultIfEmpty(),
                                                        (m,oit) => new Management{
                                                            ItemId = m.it.Id,
                                                            Name = m.it.Name,
                                                            UnitPrice = m.it.UnitPrice,
                                                            TotalUnits = m.itGr.Sum(n => n.Units)
                                                        }
                                                    );
                    results =  results.GroupBy(m => new {m.ItemId, m.Name, m.UnitPrice}, (m,n) => new Management{
                        ItemId = m.ItemId,
                        Name = m.Name,
                        UnitPrice = m.UnitPrice,
                        TotalUnits = n.Sum(p => p.TotalUnits)
                    });
                }
            
                            
            
            return results;
        }
        //tong so item da ban
        // public async Task<int> CountDeliveredItemBySalesAsync(string salesId, DateTime? fromDate = null, DateTime? toDate = null)
        // {
        //     return 0;
        // }
        //tong doanh thu

        //for administrators ==================================================
        //
        
        
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