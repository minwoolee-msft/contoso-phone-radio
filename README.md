# Contoso Phone Radio

## What is Contoso Phone Radio

Contoso Phone Radio is simple service that will accept phonecall, and then play music to the caller.

- Utilizes Azure Communication Service (ACS) Call Automation
- Able to answer incoming PSTN or VoIP calls
- Play media to the caller
- Simply deploy to Azure with provided ARM template

## Prerequisites

1. An Azure account with an active subscription.
2. Provisioned [Azure Communication Service resource](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/create-communication-resource)
3. (Optional) [Acquire a PSTN phone number from the Azure Communication Service resource](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/telephony/get-phone-number?tabs=windows&pivots=platform-azcli).

## Option 1: Deploy the Demo to Azure Web Apps

1. Make sure above Prerequisites are met.
2. Click on the **Deploy to Azure** Button

    [![Deploy To Azure](https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.svg?sanitize=true)](https://portal.azure.com/#create/Microsoft.Template/uri/https%3a%2f%2fraw.githubusercontent.com%2fminwoolee-msft%2fcontoso-phone-radio%2fmain%2fazuredeploy.json)

3. Fill out the values in the Deployment page
   - **Subscription / Resource Group**: Select your provisioned Subscription and Resource group here.
   - **Azure Communication Services Resource Name**: Name of your pre-provisioned ACS resource.
   - **Provision System Topic**: If you do not have EventGrid system topic created under ACS resource above, select YES. If you already have EventGrid system topic, select NO.
   - **System Topic Name**: Provide any unique name if you have selected YES above. If you already have EventGrid system topic under ACS, provide that name.

4. Review + Create.
5. Wait until provision finishes. May take up few minutes.
6. Once it is complete, You can test it by calling the PSTN phone number provisioned in Prerequisites.

## Option 2: Test the Demo on your Local Computer

**Reference**: [Build a customer interaction workflow using Call Automation](https://learn.microsoft.com/en-us/azure/communication-services/quickstarts/voice-video-calling/callflows-for-customer-interactions?pivots=programming-language-csharp)


1. Install latest Visual Studio if you do not have it.
2. Allow your localhost to accept incoming HTTP callbacks. You can use service like ngrok to have your localhost accessible to public network. This is required to accept events.
3. Get Envrionment Variable of the followings added in your Visual Studio
   - **AppServiceEndpoint**: your public endpoint from above ngrok.
   - **AzureCommunicationServiceKey**: your ACS resource connection string.
   - **PlayMediaFileEndpoint**: music .wav file to be played.
4. Start your service
5. On ACS portal, go events, create new webhook event with Incoming Call.
6. Webhook end point will be your public endpoint + /incomingcallevents. (i.e. https://contoso.com/incomingcallevents)
7. Verify that your incoming call eventgrid is successful on portal.
8. You can test it by calling the PSTN phone number provisioned in Prerequisites.

## Resources
- [Call Automation Overview](https://learn.microsoft.com/azure/communication-services/concepts/voice-video-calling/call-automation)
- [Incoming Call Concept](https://learn.microsoft.com/azure/communication-services/concepts/voice-video-calling/incoming-call-notification)
- [Build a customer interaction workflow using Call Automation](https://learn.microsoft.com/azure/communication-services/quickstarts/voice-video-calling/callflows-for-customer-interactions?pivots=programming-language-csha)
- [Redirect inbound telephony calls with Call Automation](https://learn.microsoft.com/azure/communication-services/how-tos/call-automation-sdk/redirect-inbound-telephony-calls?pivots=programming-language-csharp)
- [Quickstart: Play action](https://learn.microsoft.com/azure/communication-services/quickstarts/voice-video-calling/play-action?pivots=programming-language-csharp)
- [Quickstart: Recognize action](https://learn.microsoft.com/azure/communication-services/quickstarts/voice-video-calling/recognize-action?pivots=programming-language-csharp)
- [Read more about Call Recording in Azure Communication Services](https://learn.microsoft.com/azure/communication-services/concepts/voice-video-calling/call-recording)
- [Record and download calls with Event Grid](https://learn.microsoft.com/azure/communication-services/quickstarts/voice-video-calling/get-started-call-recording?pivots=programming-language-csharp)