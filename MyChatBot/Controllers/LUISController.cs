using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using MyChatBot.Models;
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
                    case "Help": await Help(context, result, obj); break;
                    case "Top": await Top(context, result, obj); break;
                }
            }
            catch(Exception ex)
            {
                // in case GetLuisObject(activity.Text)  return null object

            }


        }

        private async Task Top(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var reply = context.MakeMessage();
            reply.Attachments = new List<Attachment>();


            // display 15 product
            // but facebook messenger only display 10product in line
            // if you try to display 15product, fb messenger will display in 2 line

            // to solve this issue, we add a button at the end of line with content SHOW MORE

            
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;  // vertical
            for (int i = 0; i < 10; i++)
            {

                var productCard = new ThumbnailCard
                {
                    Title = "Áo lên cổ trụ - S11" + i,
                    Subtitle = "220 000 vnđ",
                    Text = "Váy xanh kết hợp với áo trắng cổ trụ - xu hướng mới của các bạn nữ.",
                    Images = new List<CardImage> { new CardImage("http://www.bangtoshop.com/assets/images/ao-so-mi-nu-co-tru-tay-ngan-dep-xinh-xan-nhat-hien-nay-he-2016-11.jpg") },
                    Buttons = new List<CardAction> {
                        new CardAction(ActionTypes.OpenUrl, "Xem tại lovalshop", value: "http://www.bangtoshop.com/trang-chu-Finibus-Bonorum-et-Malorum/15"),
                        new CardAction(ActionTypes.ImBack, "Đặt hàng", value: "đặt sản phẩm " + "S123")
                    }
                };


                reply.Attachments.Add(productCard.ToAttachment());
            }

            // show more
            CardAction moreButton = new CardAction()
            {
                Value = "",
                Type = "postBack",
                Title = "Thêm"
            };
            //cardButtons.Add(moreButton);
            /*List<CardAction> moreBtnList = new List<CardAction>();
            moreBtnList.Add(moreButton);

            ThumbnailCard moreCard = new ThumbnailCard()
            {
                Title = "Xem thêm",
                Subtitle = "",
                Buttons = moreBtnList
            };
            //Attachment plAttachment = moreCard.ToAttachment();
            reply.Attachments.Add(moreCard.ToAttachment());*/



            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);

        }

        private async Task Help(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            

            if (activity.Text.Contains("order"))
            {
                // order help
                await context.PostAsync("Bạn hãy nhập Đặt sản phẩm S123 để đặt ngay sản phẩm có mã S213. ");
                await context.PostAsync("Bạn cũng có thể nhập Đặt sản phẩm này nếu bạn đang xem chi tiết một sản phẩm nào đó.");

            }
            else if (activity.Text.Contains("detail"))
            {
                // detail help
                await context.PostAsync("Bạn hãy nhập Chi tiết sản phẩm S123 hoặc đơn giản hơn bằng cách nhập sản phẩm S123 để xem chi tiết một sản phẩm.");
            }
            else if (activity.Text.Contains("payment"))
            {
                // payment help
                await context.PostAsync("Bạn hãy nhập Thanh toán hoặc Tính tiền để thực hiện việc thanh toán.");
            }
            else if (activity.Text.Contains("customer"))
            {
                // payment help
                await context.PostAsync("Địa chỉ liên hệ: tanpn2402@gmail.com\nTel: 0978 505 815");
            }
            else
            {
                // default
                List<CardAction> helpButtons = new List<CardAction>();
                CardAction orderBtn = new CardAction()
                {
                    Value = "help order",
                    Type = "imBack",
                    Title = "Đặt hàng"
                };

                CardAction detailBtn = new CardAction()
                {
                    Value = "help detail",
                    Type = "imBack",
                    Title = "Chi tiết sản phẩm"
                };

                CardAction paymentBtn = new CardAction()
                {
                    Value = "help payment",
                    Type = "imBack",
                    Title = "Thanh toán"
                };

                CardAction customerCareBtn = new CardAction()
                {
                    Value = "help customer care",
                    Type = "imBack",
                    Title = "Trung tâm chăm sóc khách hàng"
                };
                helpButtons.Add(orderBtn);
                helpButtons.Add(detailBtn);
                helpButtons.Add(paymentBtn);
                helpButtons.Add(customerCareBtn);



                var helpCard = new HeroCard()
                {
                    Title = "Bạn muốn trợ giúp gì ?",
                    Buttons = helpButtons
                };

                Attachment plAttachment = helpCard.ToAttachment();

                var reply = activity.CreateReply();
                reply.Attachments = new List<Attachment>();

                reply.Attachments.Add(plAttachment);

                await context.PostAsync(reply);
            }

            


            context.Wait(ActivityReceivedAsync);
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

            
            if(obj.entities.Count == 0)
            {
                reply.Text = "Bạn muốn hỏi sản phẩm nào?";
            }
            else
            {
                if(obj.entities.ElementAt(0).type == "Product")
                {
                    // show image of this product

                    var heroCard = new HeroCard
                    {
                        Title = "Sản phẩm " + obj.entities[0].entity,
                        Subtitle = "Số lượng: 13",
                        Text = "Váy xanh kết hợp với áo trắng cổ trụ - xu hướng mới của các bạn nữ ", // đặt mô tả sản phẩm ở đây
                        Images = new List<CardImage> { new CardImage("http://www.bangtoshop.com/assets/images/ao-so-mi-nu-co-tru-tay-ngan-dep-xinh-xan-nhat-hien-nay-he-2016-11.jpg") },
                        Buttons = new List<CardAction> {
                                new CardAction(ActionTypes.OpenUrl, "Xem tại lovalshop", value: "http://www.bangtoshop.com/assets/images/ao-so-mi-nu-co-tru-tay-ngan-dep-xinh-xan-nhat-hien-nay-he-2016-11.jpg") ,
                                new CardAction(ActionTypes.ImBack, "Đặt hàng", value: "đặt sản phẩm " + "S123")
                                }
                    };

                    reply.Attachments.Add(heroCard.ToAttachment());

                }
              
            }
           
            //reply.Text = text;
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

            await context.PostAsync(reply);
            context.Wait(ActivityReceivedAsync);
        }

        // handle Payment
        private async Task Payment(IDialogContext context, IAwaitable<object> result, LuisObject obj)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();
            reply.Attachments = new List<Attachment>();

            long total = 100000; // tổng tiền phải trả
            int tax = 100000; // thuế
            string orderCode = "123-asd-123";

            List<ReceiptItem> receiptList = new List<ReceiptItem>();

            for (int i = 0; i < 5; i++)
            {
                receiptList.Add(new ReceiptItem()
                {
                    Title = "Váy xanh - V10" + i,   // đặt tên sp + mã sp ở đây
                    Subtitle = "SL: " + i,          // đặt số lượng ở đây
                    Text = null,
                    Image = new CardImage(url: "http://www.bangtoshop.com/assets/images/ao-so-mi-nu-co-tru-tay-ngan-dep-xinh-xan-nhat-hien-nay-he-2016-11.jpg"),
                    Price = "$" + "220000",
                    //Quantity = i + "",
                    Tap = null
                }
                );
            }


            var receiptCard = new ReceiptCard
            {
                Title = "Xác nhận đơn hàng",
                Facts = new List<Fact> { new Fact("Order Number", orderCode), new Fact("Payment Method", "Tiền mặt"), new Fact("Currency","VND") },
                Items = receiptList,
                Tax = "$" + tax.ToString() ,
                Total = "$" + total.ToString() ,
                Buttons = new List<CardAction>
                {
                    new CardAction(
                        ActionTypes.OpenUrl,
                        "Xác nhận",
                        null,  // image
                        "https://azure.microsoft.com/en-us/pricing/"  // link
                        )
                }
            };

            reply.Attachments.Add(receiptCard.ToAttachment());

            //reply.Text = "Bạn hãy nhập địa chỉ email để chúng tôi liên lạc với bạn";
            //await context.PostAsync(reply.Attachments.ToString());
            
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
