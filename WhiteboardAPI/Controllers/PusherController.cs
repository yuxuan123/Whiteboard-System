using PusherServer;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WhiteboardAPI.Controllers
{
    public class PusherController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> HelloWorld()
        {
            var options = new PusherOptions
            {
                Cluster = "ap1",
                Encrypted = true
            };

            var pusher = new Pusher(
              "957914",
              "0a3b3bc361a655ea56ac",
              "1a2506af120a04af2906",
              options);

            var result = await pusher.TriggerAsync(
              "my-channel",
              "my-event",
              new { message = "hello world" });

            return Ok(result);
        }
    }
}

