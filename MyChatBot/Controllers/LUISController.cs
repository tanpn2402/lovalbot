using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MyChatBot.Controllers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace MyChatBot
{
    //[LuisModel("ed5c8266-deb8-443e-97be-4db1c5851121", "fc35624f8179492f96fc5152bdefd4f9")]
    [Serializable]
    public class LUISController : IDialog /*LuisDialog<object>*/
    {
        // implemented from IDialog
        // it seem been a constructor
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ActivityReceivedAsync);
        }

        private async Task ActivityReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
           try
            {
                switch (GetLuisObject(activity.Text).topScoringIntent.intent)
                {
                    case "None": await None(context, result); break;
                    case "SearchHotels": await SearchHotels(context, result); break;
                    case "Help": await Help(context, result); break;
                    case "ShowHotelsReviews": await ShowHotelsReviews(context, result); break;
                }
            }
            catch(Exception ex)
            {
                // in case GetLuisObject(activity.Text)  return null object

            }


        }


        // handle none intent
        private async Task None(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();
            
            reply.Text = "Không nhận dạng được";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);

        }

        // handle SearchHotels intent
        private async Task SearchHotels(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            reply.Text = "SearchHotels";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        // handle none Help
        private async Task Help(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            reply.Text = "Help";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        // handle none ShowHotelsReviews
        private async Task ShowHotelsReviews(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            reply.Text = "ShowHotelsReviews";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }







        // get luis object from luis web
        public LuisObject GetLuisObject(string query)
        {


            var url = $"{LuisObject.urlBase}{LuisObject.appid}?subscription-key={LuisObject.subkey}&q={query}&verbose=true";

            // Call API
            try
            {
                // Send JSON to endpoint
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";

                // Handle Response from endpoint
                WebResponse response = request.GetResponse();
                var dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                // Deserialize JSON
                var infos = JsonConvert.DeserializeObject<LuisObject>(responseFromServer);

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();

                return infos;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}