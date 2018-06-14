using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseController
{
    public class ConfirmationSlip
    {
        public string requestor;
        public string sender;

        public bool value;
        public ControllableType senderType;

        public ConfirmationSlip(
            string requestor, 
            string sender, 
            ControllableType controllableType, 
            bool value)
        {
            this.requestor = requestor;
            this.sender = sender;
            this.senderType = controllableType;
            this.value = value;
        }

        public override string ToString()
        {
            return sender + " replying to " + requestor + ". Controllable: " +  senderType + " value = " + value.ToString();

        }
    }
}
