using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CosmeticShop.Controllers
{
    public class SendSMSController : Controller
    {
        public IActionResult Index()
        {
            const string accountSid = "ACff7a2b137de5ac0d7fe4d1396756abe9";
            const string authToken = "4d8822f20cfb83f5e8d6f3f16b18c6de";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Hello Mình Là Sương",
                from: new Twilio.Types.PhoneNumber("+12028581210"),
                to: new Twilio.Types.PhoneNumber("+84867601320")
            );
            return View(message.Sid);
        }


    }
}