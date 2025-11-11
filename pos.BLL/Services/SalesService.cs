using pos_system.pos.DAL.Repositories;
using pos_system.pos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos_system.pos.BLL.Services
{
    public class SalesService
    {
        private readonly SalesRepository _salesRepository;

        public SalesService()
        {
            _salesRepository = new SalesRepository();
        }

        public ReturnToken GetTokenDetails(int tokenId)
        {
            return _salesRepository.GetTokenDetails(tokenId);
        }

        public List<QueuedBill> GetQueuedBills(int employeeId)
        {
            return _salesRepository.GetQueuedBills(employeeId);
        }

        public bool DeleteQueuedBill(int queueId)
        {
            return _salesRepository.DeleteQueuedBill(queueId);
        }

        public bool RestorePausedBill(int queueId, int employeeId, out int billId, out string cartData)
        {
            return _salesRepository.RestorePausedBill(queueId, employeeId, out billId, out cartData);
        }
    }
}
