using Azure.Communication.CallAutomation;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContosoPhoneRadio.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IncomingCallEventsController : ControllerBase
    {
        const string ACSKeyVariableName = "AzureCommunicationServiceKey";
        const string AppServiceEndpointVariableName = "AppServiceEndpoint";

        private readonly ILogger<IncomingCallEventsController> _logger;

        private CallAutomationClient _callingClient;
        private string? acsKey;
        private string? appServiceEndpoint;

        public IncomingCallEventsController(ILogger<IncomingCallEventsController> logger)
        {
            _logger = logger;

            acsKey = Environment.GetEnvironmentVariable(ACSKeyVariableName);
            appServiceEndpoint = Environment.GetEnvironmentVariable(AppServiceEndpointVariableName);

            if (string.IsNullOrEmpty(acsKey) || string.IsNullOrEmpty(appServiceEndpoint))
            {
                throw new ArgumentNullException($"{ACSKeyVariableName} or/and {AppServiceEndpointVariableName} are not defined in appsettings.");
            }

            _callingClient = new CallAutomationClient(acsKey);
        }

        [HttpGet("helloworld")]
        [AllowAnonymous]
        public IActionResult hellowowrld()
        {
            _logger.LogInformation($"Hello World Called.");
            return Ok("I am alive!");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult OnIncomingCallRequest([FromBody] object request)
        {
            try
            {
                // parse the request
                var httpContent = new BinaryData(request.ToString()).ToStream();
                EventGridEvent cloudEvent = EventGridEvent.ParseMany(BinaryData.FromStream(httpContent)).FirstOrDefault();

                if (cloudEvent.EventType == SystemEventNames.EventGridSubscriptionValidation)
                {
                    // this section is for handling initial handshaking with Event webhook registration
                    var eventData = cloudEvent.Data.ToObjectFromJson<SubscriptionValidationEventData>();
                    var responseData = new SubscriptionValidationResponse
                    {
                        ValidationResponse = eventData.ValidationCode
                    };

                    if (responseData.ValidationResponse != null)
                    {
                        _logger.LogInformation($"Event Handshake");
                        return Ok(responseData);
                    }
                }
                else if (cloudEvent.EventType.Equals("Microsoft.Communication.IncomingCall"))
                {
                    _logger.LogInformation($"There is a new incoming call!");

                    // This is for incoming call
                    var eventData = request.ToString();
                    if (eventData != null)
                    {

                        // TODO: fix this when Event grid support incoming call event parsing
                        string incomingCallContext = eventData.Split("\"incomingCallContext\":\"")[1].Split("\"")[0];

                        AnswerCallResult result1 = _callingClient.AnswerCall(incomingCallContext, new Uri($"{appServiceEndpoint}/CallbackEvents"));

                        _logger.LogInformation($"Answered Call with CallConnectionId[{result1.CallConnection.CallConnectionId}]");
                    }
                }
            }
            catch (Exception ex)
            {
                // handle exception...
                _logger.LogError($"Exception [{ex.ToString()}]");
            }

            return Ok();
        }
    }
}