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
        Task<IList<AllSaleAnal>> CountAllSales();
        Task<AllSaleAnal> CountItemInBuyer(string buyerId, string saleId);
        Task<IList<AllSaleAnal>> CountItemAllBuyers();
        //tong so item ma sales ban
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