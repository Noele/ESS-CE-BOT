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
            var uri = $"https://api.ocr.space/parse/imageurl?apikey={Bot.key}&url={attachment.Url}";
            var web = new WebClient();
            var responseString = web.DownloadString(uri);
            
            // Parse the result of that call to a JObject
            var result = JObject.Parse(responseString);
            
            // Attempt to get the parsed results
            var parsedResults = (JArray) result.GetValue("ParsedResults");
            
            // Get the first result of the parsed results
            var firstParsedResult = (JObject) parsedResults[0];
            
            // Get the text inside the parsed result
            var parsedText = (string) firstParsedResult.GetValue("ParsedText");
            
            // Read commonerrors.json
            JObject commonErrors;
            using (var r = new StreamReader("commonerrors.json"))
            {
                var json = r.ReadToEnd();
                commonErrors = JObject.Parse(json);
            }
            
            // If the key of any of the common errors is inside the parsed text, send that keys value
            foreach (var key in commonErrors)
            {
                if (parsedText.Contains(key.Key))
                {
                    await message.Channel.SendMessageAsync((string) key.Value);
                }
            }
        }    
    }
}