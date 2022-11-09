using Azure.Communication.CallAutomation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPhoneRadio.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CallbackEventsController : ControllerBase
    {
        const string ACSKeyVariableName = "AzureCommunicationServiceKey";
        const string MediaEndpoint = "PlayMediaFileEndpoint";

        private readonly ILogger<CallbackEventsController> _logger;

        private CallAutomationClient _callingClient;
        private string? acsKey;
        private Uri mediaEndpoint;

        public CallbackEventsController(ILogger<CallbackEventsController> logger)
        {
            _logger = logger;

            acsKey = Environment.GetEnvironmentVariable(ACSKeyVariableName);
            var mediaEndpointString = Environment.GetEnvironmentVariable(MediaEndpoint);

            if (string.IsNullOrEmpty(acsKey) || string.IsNullOrEmpty(mediaEndpointString))
            {
                throw new ArgumentNullException($"{ACSKeyVariableName} or/and {mediaEndpointString} are not defined in appsettings.");
            }

            mediaEndpoint = new Uri(mediaEndpointString);
            _callingClient = new CallAutomationClient(acsKey);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult OnCallbackEvent([FromBody] object request)
        {
            try
            {
                var httpContent = new BinaryData(request.ToString()).ToStream();
                var eventBase = CallAutomationEventParser.Parse(BinaryData.FromStream(httpContent));

                _logger.LogInformation($"EventType[{eventBase}]---[{request.ToString()}]");

                if (eventBase is CallConnected)
                {
                    // call is connected ! play this msg;
                    playMedia(eventBase.CallConnectionId);
                }
                else if (eventBase is PlayCompleted)
                {
                    // play media completed - play again
                    playMedia(eventBase.CallConnectionId);
                }
            }
            catch (Exception ex)
            {
                // handle exception...
                _logger.LogError($"Exception [{ex.ToString()}]");
            }

            return Ok();
        }

        private void playMedia(string callConnectionId)
        {
            _callingClient.GetCallConnection(callConnectionId).GetCallMedia().PlayToAllAsync(new FileSource(mediaEndpoint), new PlayOptions { Loop = false });
        }
    }
}