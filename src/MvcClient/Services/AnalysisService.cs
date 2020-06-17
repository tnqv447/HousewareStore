using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MvcClient.Models;

namespace MvcClient.Services
{
    public class AnalysisService : IAnalysisService
    {
        //for sales ==================================================
        //

        //for managers ==================================================
        //
        //tong so item ma sales ban
        public async Task<int> CountItemsBySalesAsync(string salesId, ItemStatus status = ItemStatus.Approved)
        {
            return 0;
        }
        //tong so item da ban
        public async Task<int> CountDeliveredItemBySalesAsync(string salesId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            return 0;
        }
        //tong doanh thu

        //for administrators ==================================================
        //
    }
}