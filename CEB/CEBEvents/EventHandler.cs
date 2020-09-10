using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonErrorsBot.CEB.CEBEvents
{
    
    public class EventHandler
    {
        private List<string> _allowedFileExtensions= new List<string> {".png", ".jpg", ".jpeg"};
        
        /// <summary>
        /// Called when the bot detects a message
        /// Attempts to read an attached image for common errors
        /// </summary>
        /// <param name="message"></param>
        public async Task OnMessage(SocketMessage message)
        {
            // If the message has no attachments, return
            if (message.Attachments.Count == 0) return;
            
            // Create a variable for the first attachment
            var attachment = message.Attachments.First();
            
            // If the attachment is not an image, return
            if(!_allowedFileExtensions.Any(x => attachment.Filename.EndsWith(x))) return;
            
            // Make a call to the api
            var uri = $"https://api.ocr.space/parse/imageurl?apikey={Bot.Key}&url={attachment.Url}";
            var web = new WebClient();
            var responseString = web.DownloadString(uri);

            // Deserialize the result of that call to a Data Object
            var dataObject = JsonConvert.DeserializeObject<Data>(responseString);

            // Get the text inside the dataObject 
            var parsedText = dataObject.ParsedResults[0].ParsedText;

            // For each key in CommonErrors, if they objects search appears in the parsed text, send the objects reponse
            foreach (var key in Bot.CommonErrors)
            {
                var keyObject = (JObject) key.Value;
                if (parsedText.Contains((string) keyObject.SelectToken("Search")))
                {
                    await message.Channel.SendMessageAsync((string) keyObject.SelectToken("Response"));
                }
            }
        }    
    }
}