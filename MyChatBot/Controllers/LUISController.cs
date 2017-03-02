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
                LuisObject obj = GetLuisObject(activity.Text);
                switch (obj.topScoringIntent.intent)
                {
                    case "None": await None(context, result, obj); break;
                    case "Product": await Product(context, result, obj); break;
                    case "Order": await Order(context, result, obj); break;
                    case "Payment": await Payment(context, result, obj); break;
                    case "Question": await Question(context, result, obj); break;
                    case "Hello": await Hello(context, result, obj); break;

                }
            }
            catch(Exception ex)
            {
                // in case GetLuisObject(activity.Text)  return null object

            }


        }

        private async Task Question(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            string text = "";
            if (obj.query.Contains("tuổi"))
            {
                text = "Mình mới được sinh ra cách đây 2 ngày thôi ahihi";
            }
            else if (obj.query.Contains("tên"))
            {
                text = "Mình tên là LovalBot";
            }
            else if(obj.query.Contains("giới tính") || obj.query.Contains("nam") || obj.query.Contains("nữ"))
            {
                text = "Mình là chat bot và mình không có giới tính nhé";
            }
            else
            {
                text = "Mình là LovalBot, mình mới được 2 ngày tuổi, không có giới tính. Mình do Phạm Nhật Tân phát triển.";
            }


            reply.Text = text;
            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        private async Task Hello(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            reply.Text = "Chào bạn, mình có thể giúp gì cho bạn ?";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }


        // handle none intent
        private async Task None(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();
            
            reply.Text = "Không nhận dạng được";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);

        }

        // handle Product intent
        private async Task Product(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            string text= "";
            
            if(obj.entities.Count == 0)
            {
                text = "Bạn muốn hỏi sản phẩm nào?";
            }
            else
            {
                if(obj.entities.ElementAt(0).type == "Product")
                {
                    text = "Hiện tại sản phẩm " + obj.entities[0].entity + " đang tạm hết hàng, 10 ngày sau thì sản phẩm này có hàng lại.";
                }
                //switch(obj.entities.)
                //{
                //    case  "Product":


                //        break;
                //}
            }

            reply.Text = text;
            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        // handle none Order
        private async Task Order(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            string text = "";

            if (obj.entities.Count == 0)
            {
                text = "Mã sản phẩm không xác định, hãy chắc rằng bạn nhập đúng mã";
            }
            else
            {
                if (obj.entities.ElementAt(0).type == "Product")
                {
                    text = "Mã sản phẩm " + obj.entities[0].entity + " không tồn tại, hãy chắc rằng bạn nhập đúng mã.";
                }
                //switch(obj.entities.)
                //{
                //    case  "Product":


                //        break;
                //}
            }

            reply.Text = text;

            //reply.Text = "Đang đặt hàng, vui lòng đợi";


            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        // handle none Payment
        private async Task Payment(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            reply.Text = "Bạn hãy nhập địa chỉ email để chúng tôi liên lạc với bạn";


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