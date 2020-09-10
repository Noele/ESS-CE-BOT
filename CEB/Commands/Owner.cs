using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Audio.Streams;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CommonErrorsBot.CEB.Commands
{
    public class Owner : ModuleBase<SocketCommandContext>
    {
        
        /// <summary>
        /// Reloads commonerrors.json
        /// </summary>
        /// <returns></returns>
        [Command("ReloadFile")]
        public async Task _reloadFile()
        {
            Bot.LoadFile();
        }
        
        /// <summary>
        /// Gets the list of common errors
        /// </summary>
        /// <returns></returns>
        [Command("GetCommonErrors")]
        public async Task _getCommonErrors()
        {
            var output = new StringBuilder();
            output.Append("```\n");
            foreach (var key in Bot.CommonErrors)
            {
                output.Append($"{key.Key}\n");
            }
            output.Append("```");

            await Context.Channel.SendMessageAsync(output.ToString());
        }
        
        /// <summary>
        /// Adds a common error to the file and reloads it
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Command("AddCommonError")]
        public async Task _addCommonError([Remainder]string args)
        {
            // Split the args at |
            var splitArgs = args.Split('|');
            
            // If they user provided more then a key, search and response, exit out
            if (splitArgs.Length != 3)
            {
                await Context.Channel.SendMessageAsync("Too many/few args, please supply a Key, Search and Response\nExample: ACoolkey | A Cool Search | A Cool Response");
                return;
            }
                
            // Create a key search and response, this helps with readability and gives us time to trim to neaten things up
            var key = splitArgs[0].Trim();
            var search = splitArgs[1].Trim();
            var response = splitArgs[2].Trim();
            
            // Create a json object of our data
            var jsonObject = new JObject
            {
                {
                    key, new JObject
                    {
                        {"Search", search},
                        {"Response", response}
                    }
                }
            };
            
            // Merge the json data with the current data
            Bot.CommonErrors.Merge(jsonObject);
            
            // Save the data to the file
            System.IO.File.WriteAllText("commonerrors.json", Bot.CommonErrors.ToString());
            
            // Reload the data into memory
            Bot.LoadFile();
            
            //Send a conformation message
            await Context.Channel.SendMessageAsync($"I have added {key} to the common errors and reloaded the file.");
        }
        
        /// <summary>
        /// Removes a common error from the file by its key and reloads it
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Command("RemoveCommonError")]
        public async Task _removeCommonError([Remainder] string args)
        {
            // Attempt to remove the argument
            var res = Bot.CommonErrors.Remove(args);
            
            // If it returns false, that means it failed to remove the key, so lets send a message and return
            if (!res)
            {
                await Context.Channel.SendMessageAsync($"Failed to find {args}, No common error has been removed.");
                return;
            }
            
            // It returned true, that means it was successful, lets save the new data to commonerrors.json
            System.IO.File.WriteAllText("commonerrors.json", Bot.CommonErrors.ToString());
            
            // Reload the file
            Bot.LoadFile();
            
            // Send a confirmation message
            await Context.Channel.SendMessageAsync(
                $"I have removed {args} from the common errors and reloaded the file.");
        }
    }
}