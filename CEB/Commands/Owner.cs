using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
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
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task _reloadFile()
        {
            Bot.LoadFile();
        }
        
        /// <summary>
        /// Gets the list of common errors
        /// </summary>
        /// <returns></returns>
        [Command("GetCommonErrors")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task _getCommonErrors()
        {
            var output = new StringBuilder();
            output.Append("```\n");
            foreach (var key in Bot.CommonErrors)
            {
                output.Append($"{key.Key}\n");
                if (output.Length > 1800)
                {
                    output.Append("```");
                    await Context.Channel.SendMessageAsync(output.ToString()); // We went over 1800 characters   
                    output = new StringBuilder();                                   // Discord only allows 2000 per message
                    output.Append("```\n");                                         // This makes sure the output can be sent in full
                }
            }
            output.Append("```");

            await Context.Channel.SendMessageAsync(output.ToString());
        }
        
        /// <summary>
        /// Sends a message of the key and property of a common error
        /// </summary>
        /// <param name="key"></param>
        /// <param name="property"></param>
        [Command("GetCommonErrorProperty")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task _getCommonErrorProperty(string key, string property)
        {
            string search;
            try
            {
                search = (string) Bot.CommonErrors.SelectToken(key).SelectToken(property); // Attempt to get the key and property 
            }
            catch (Exception e)
            {
                search = $"Failed to find {key}, {property}"; // We failed so lets default to an error message
            }
            search = search ?? $"Failed to find {key}, {property}"; // This is to solve a bug where the second arg is correct, and the first is wrong, this ensures we get an error message
            await Context.Channel.SendMessageAsync(search); // Send the output
        }
        
        /// <summary>
        /// Adds a common error to the file and reloads it
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        [Command("AddCommonError")]
        [RequireUserPermission(GuildPermission.Administrator)]
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
        [RequireUserPermission(GuildPermission.Administrator)]
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
