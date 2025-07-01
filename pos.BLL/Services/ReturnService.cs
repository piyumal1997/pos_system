using Microsoft.Data.SqlClient;
using pos_system.pos.BLL.Repositories;
using pos_system.pos.DAL;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace pos_system.pos.BLL.Services
{
    public class ReturnService
    {
        private readonly ReturnRepository _repository = new ReturnRepository();

        public ReturnResult ProcessReturn(int employeeId, int originalBillId, List<ReturnItem> returnItems)
        {
            return _repository.ProcessReturn(employeeId, originalBillId, returnItems);
        }

        public ReturnReceiptData GetReturnReceiptData(int returnId)
        {
            return _repository.GetReturnReceiptData(returnId);
        }
    }

}