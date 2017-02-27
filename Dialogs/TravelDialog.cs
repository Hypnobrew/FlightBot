using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using EzyFlightBot.Models;
using EzyFlightBot.Services;
using System.Web.Configuration;

namespace EzyFlightBot.Dialogs
{
    [Serializable]
    [LuisModel("XXXX", "XXXX")]
    public class TravelDialog : LuisDialog<object>
    {
        private string apiBaseuri;
        private string apiUsername;
        private string apiPassword;
        private readonly IEzyFlight ezyflightService;

        public TravelDialog(IEzyFlight ezyflight)
        {
            ezyflightService = ezyflight;
            apiBaseuri = WebConfigurationManager.AppSettings["ezyFlightUrl"];
            apiUsername = WebConfigurationManager.AppSettings["apiUsername"];
            apiPassword = WebConfigurationManager.AppSettings["apiPassword"];
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Qué? I know nothing..");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("I can do the following tasks:");
            await context.PostAsync("- Greet");
            await context.PostAsync("- Book flights");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Welcome! How can I help you?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("BookFlight")]
        public async Task BookFlight(IDialogContext context, LuisResult result)
        {
            await ParseData(context, result);
            await SelectNextQuestion(context);
        }

        private async Task ParseData(IDialogContext context, LuisResult result)
        {
            var travelData = GetData(context);
            var token = string.Empty;
            if (string.IsNullOrEmpty(travelData.Token))
            {
                travelData.Token = token = await ezyflightService.GetAuthToken(apiUsername, apiPassword, apiBaseuri);
            }

            if (result.TryFindEntity("Location::FromLocation", out EntityRecommendation fromLocation))
            {
                var candidate = await GetLocation(context, token, fromLocation.Entity);
                travelData.From = candidate.Name;
                travelData.FromCode = candidate.Code;
            }

            if (result.TryFindEntity("Location::ToLocation", out EntityRecommendation toLocation))
            {
                var candidate = await GetLocation(context, token, toLocation.Entity);
                travelData.To = candidate.Name;
                travelData.ToCode = candidate.Code;
            }

            if (result.TryFindEntity("builtin.datetime.date", out EntityRecommendation travelDate))
            {
                var date = DateTime.Parse(travelDate.Entity);
                travelData.TravelDate = date;
            }

            SetData(context, travelData);
        }

        private async Task SelectNextQuestion(IDialogContext context)
        {
            var travelData = GetData(context);

            if (string.IsNullOrEmpty(travelData.From))
            {
                await CreateFromDialog(context);
                return;
            }

            if (string.IsNullOrEmpty(travelData.To))
            {
                await CreateToDialog(context);
                return;
            }

            if (travelData.TravelDate == DateTime.MinValue)
            {
                await CreateWhenDialog(context);
                return;
            }

            CreateConfirmDialog(context);
        }

        private async Task AfterFromAsync(IDialogContext context, IAwaitable<Airport> argument)
        {
            var from = await argument;
            var travelData = GetData(context);
            travelData.From = from.Name;
            travelData.FromCode = from.Code;
            SetData(context, travelData);
            await SelectNextQuestion(context);
        }

        private async Task AfterToAsync(IDialogContext context, IAwaitable<Airport> argument)
        {
            var to = await argument;
            var travelData = GetData(context);
            travelData.To = to.Name;
            travelData.ToCode = to.Code;
            SetData(context, travelData);
            await SelectNextQuestion(context);
        }

        private async Task AfterWhenAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var departureInput = await argument;
            var parser = new Chronic.Parser();
            var dateResult = parser.Parse(departureInput);

            if(dateResult != null)
            {
                var parsedDate = dateResult.ToTime();
                var travelData = GetData(context);
                travelData.TravelDate = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day);
                SetData(context, travelData);
                await SelectNextQuestion(context);
                return;
            }

            PromptDialog.Text(
                    context,
                    AfterWhenAsync,
                    "Sorry did not get that. When do you want to go?");
        }

        private async Task AfterSelectFlightAsync(IDialogContext context, IAwaitable<string> argument)
        {
            SetData(context, new TravelData());
            await context.PostAsync("Flight booked!");
        }

        private async Task AfterSearchAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync("Searching..");
                var travelData = GetData(context);
                var availability = await ezyflightService.GetAvailability(travelData.Token, travelData.FromCode, travelData.ToCode, travelData.TravelDate, apiBaseuri);                
                var presentationData = availability.Where(x => !x.Soldout)
                                                   .Select(x => $"{x.DepartureDateTime.ToString("hh:mm")} {x.LowestPrice} COP");
                PromptDialog.Choice<string>(
                    context,
                    AfterSelectFlightAsync,
                    presentationData,
                    "Which flight do you want to book?");
                return;
            }
            else
            {
                SetData(context, new TravelData());
                await context.PostAsync("How can I help you?");
            }
            context.Wait(MessageReceived);
        }

        private async Task CreateFromDialog(IDialogContext context)
        {
            var travelData = GetData(context);
            var airports = await ezyflightService.GetFromAirports(travelData.Token, apiBaseuri);

            PromptDialog.Choice<Airport>(
                    context,
                    AfterFromAsync,
                    airports.Airports,
                    "Where do you want to go from?");
        }

        private async Task CreateToDialog(IDialogContext context)
        {
            var travelData = GetData(context);
            var airports = await ezyflightService.GetToAirports(travelData.Token, travelData.FromCode, apiBaseuri);
            
            PromptDialog.Choice<Airport>(
                    context,
                    AfterToAsync,
                    airports.Airports,
                    "Where do you want to go to?");
        }

        private async Task CreateWhenDialog(IDialogContext context)
        {
            PromptDialog.Text(
                    context,
                    AfterWhenAsync,
                    "When do you want to go?");
        }

        private void CreateConfirmDialog(IDialogContext context)
        {
            var travelData = GetData(context);
            PromptDialog.Confirm(
                    context,
                    AfterSearchAsync,
                    $"Confirm you want to go from {travelData.From} to {travelData.To}, {travelData.TravelDate.ToString("yyyy-MM-dd")} ?",
                    "Please answer yes or no!",
                    promptStyle: PromptStyle.Auto);
        }

        private async Task<AirportCandidate> GetLocation(IDialogContext context, string token, string location)
        {
            var travelData = GetData(context);
            var fromLocations = await ezyflightService.GetFromAirports(token, apiBaseuri);
            var fromLocationNames = fromLocations.Airports
                .Select(x => x.Name)
                .ToList();

            var candidate = DistanceCalculator.GetClosestMatch(location, fromLocationNames);
            var candidateCode = fromLocations.Airports
                .Where(x => x.Name == candidate)
                .First()
                .Code;

            return new AirportCandidate(candidate, candidateCode);
        }

        private TravelData GetData(IDialogContext context)
        {
            if(context.PrivateConversationData.TryGetValue<TravelData>("TravelData", out TravelData data))
            {
                return data;
            }
            return new TravelData();
        }

        private void SetData(IDialogContext context, TravelData data)
        {
            context.PrivateConversationData.SetValue<TravelData>("TravelData", data);
        }
    }
}