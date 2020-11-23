using System;
using System.Collections.Generic;
using System.Text;
using TeamC.SKS.DataAccess.Entities;

namespace TeamC.SKS.DataAccess.Interfaces
{
    public interface IWebhookRepository
    {
        long Create(Webhook hook);
        void Delete(long id);
        bool GetParcelByTrackingID(string trackingID);
        bool GetWebhookByID(long id);
        List<Webhook> GetWebhooksByTrackingID(string trackingID);
    }
}
