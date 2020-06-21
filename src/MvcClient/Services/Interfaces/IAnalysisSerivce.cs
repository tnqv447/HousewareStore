using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;
using MvcClient.ViewModels;

namespace MvcClient.Services
{
    public interface IAnalysisService
    {
        //for sales ==================================================
        //



        //============================================================
        //for managers ==================================================
        //
        Task<IList<BaseAnalysis>> CountAllSales();
        Task<BaseAnalysis> CountItemInBuyer(string buyerId, string saleId);
        Task<IList<BaseAnalysis>> CountItemAllBuyers();
        Task<IEnumerable<ItemAnalysis>> CountAllProducts(string saleId = null);
        Task<IEnumerable<ItemAnalysis>> SaleAsync(string saleId,DateTime date);
        Task<IList<BaseAnalysis>> AllSales(DateTime date);
        Task<IEnumerable<ItemAnalysis>> CountItemsByBuyersAsync(string saleId);
        Task<IEnumerable<ItemAnalysis>> CountItemsBySalesAsync(string salesId);
        //tong so item da ban
        // Task<int> CountDeliveredItemBySalesAsync(string salesId, DateTime? fromDate = null, DateTime? toDate = null);
        //tong doanh thu

        // Task<IList<AnalysisViewModel>> CountAllProducts();


        //==============================================================
        //for administrators ==================================================
        //

        //============================================================
    }
}