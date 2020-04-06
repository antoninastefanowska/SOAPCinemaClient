using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace IPAttachExtension
{
    public class IPAttachExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(IPAttachBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new IPAttachBehavior();
        }
    }
}
