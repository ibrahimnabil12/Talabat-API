using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order;

namespace Talabat.Core.Specifications.Order_Specs
{
    public class orderWithPaymentIntentSpec : BaseSpecifications<Order>
    {
        public orderWithPaymentIntentSpec(string paymentIntentId) : base(O=>O.PaymentIntentId == paymentIntentId)
        {
            
        }

    }
}
