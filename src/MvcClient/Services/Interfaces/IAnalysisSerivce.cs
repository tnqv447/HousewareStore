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
        Task<IEnumerable<ItemAnalysis>> CountItemsByBuyersAsync(string saleId);
        Task<IEnumerable<ItemAnalysis>> CountItemsBySalesAsync(string salesId);

        //==============================================================
        //for administrators ==================================================
        //

        //============================================================
    }
}