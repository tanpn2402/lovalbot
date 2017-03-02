using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyChatBot.Models
{
    public class LuisObject
    {

        public static string urlBase = $"https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/";
        public static string appid = /*"8cb04c3c-9aa9-4d69-9206-24ac390485c5";*/ "ed5c8266-deb8-443e-97be-4db1c5851121";
        public static string subkey = /*"fc35624f8179492f96fc5152bdefd4f9";*/  "fc35624f8179492f96fc5152bdefd4f9";


        [JsonProperty("query")]
        public string query { get; set; }

        [JsonProperty("topScoringIntent")]
        public TopScoringIntent topScoringIntent { get; set; }


        [JsonProperty("intents")]
        public List<Intent> intents { get; set; }


        [JsonProperty("entities")]
        public List<Entity> entities { get; set; }



        public class TopScoringIntent
        {
            [JsonProperty("intent")]
            public string intent { get; set; }


            [JsonProperty("score")]
            public double score { get; set; }
        }

        public class Intent
        {
            [JsonProperty("intent")]
            public string intent { get; set; }

            [JsonProperty("score")]
            public double score { get; set; }
        }


        public class Entity
        {
            [JsonProperty("entity")]
            public string entity { get; set; }

            [JsonProperty("type")]
            public string type { get; set; }

            [JsonProperty("startIndex")]
            public int startIndex { get; set; }

            [JsonProperty("endIndex")]
            public int endIndex { get; set; }

            [JsonProperty("score")]
            public double score { get; set; }
        }
    }
}