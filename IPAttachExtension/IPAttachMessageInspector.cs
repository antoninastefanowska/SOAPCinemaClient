using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace IPAttachExtension
{
    public class IPAttachMessageInspector : IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            object messageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out messageObject))
            {
                HttpRequestMessageProperty msg = messageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(msg.Headers["IP-Address"]))
                    msg.Headers["IP-Address"] = GetIPAddress();
            }
            else
            {
                HttpRequestMessageProperty msg = new HttpRequestMessageProperty();
                msg.Headers.Add("IP-Address", GetIPAddress());
                request.Properties.Add(HttpRequestMessageProperty.Name, msg);
            }
            Debug.WriteLine(GetIPAddress());
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        private string GetIPAddress()
        {
            string hostName = Dns.GetHostName();
            return Dns.GetHostAddresses(hostName)[0].ToString();
        }
    }
}
