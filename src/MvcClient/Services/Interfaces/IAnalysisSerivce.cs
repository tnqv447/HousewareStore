using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;

namespace MvcClient.Services
{
    public interface IAnalysisService
    {
        //for sales ==================================================
        //



        //============================================================
        //for managers ==================================================
        //
        //tong so item ma sales ban
        Task<int> CountItemsBySalesAsync(string salesId, ItemStatus status = ItemStatus.Approved);
        //tong so item da ban
        Task<int> CountDeliveredItemBySalesAsync(string salesId, DateTime? fromDate = null, DateTime? toDate = null);
        //tong doanh thu



        //==============================================================
        //for administrators ==================================================
        //

        //============================================================
    }
}