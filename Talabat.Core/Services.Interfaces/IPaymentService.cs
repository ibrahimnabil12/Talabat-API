using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Services.Interfaces
{
    public interface IPaymentService
    {
        public Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId);
        public Task<Order> UpdatepaymentIntentToSuccessedOrFailed(string paymentIntentId, bool flag);
    }
}
