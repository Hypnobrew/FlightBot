using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using EzyFlightBot.Dialogs;
using System;
using EzyFlightBot.Services;

namespace EzyFlightBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IEzyFlight ezyflightService;
        public MessagesController(IEzyFlight ezyflight)
        {
            ezyflightService = ezyflight;
        }

        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity reply = activity.CreateReply();
                reply.Type = ActivityTypes.Typing;
                reply.Text = null;
                await connector.Conversations.ReplyToActivityAsync(reply);
                await Conversation.SendAsync(activity, () => new TravelDialog(ezyflightService));
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {

            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {

            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {

            }
            else if (message.Type == ActivityTypes.Typing)
            {
                
            }
            else if (message.Type == ActivityTypes.Ping)
            {

            }

            return null;
        }
    }
}