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
        Task<IList<AnalysisViewModel>> CountAllSales();
        //tong so item ma sales ban
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