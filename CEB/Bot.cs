using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace CommonErrorsBot.CEB
{
    public class Bot
    {
        public static string Key;

        public static JObject CommonErrors;
        
        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        private string _prefix;
        private string _token;
        
        /// <summary>
        /// Starts the bot Asynchronously 
        /// </summary>
        /// <param name="token">Token the bot will log in with</param>
        /// <param name="prefix">Prefix the bot will use to respond to messages</param>
        public async Task StartAsync(string token, string prefix, string key)
        {
            LoadFile();
                
            Key = key;
            _prefix = prefix;
            _token = token;
            _client = new DiscordSocketClient(); // Initialise the client
            _commands = new CommandService();    // Create a CommandService
            _services = new ServiceCollection()  // Collection of the services we have
                .AddSingleton(_client)           // Adds the Bot as a service
                .AddSingleton(_commands)         // Adds the commands as a service
                .BuildServiceProvider();         // Builds the service Provider

            _client.Log += OnLog; // Signup to the Log event, this outputs almost all messages to the console

            await RegisterEvents(); // Register our events
            
            await RegisterCommandsAsync(); // Register our commands

            await _client.LoginAsync(TokenType.Bot, _token); // Login to discord

            await _client.StartAsync(); // Start the bot Asynchronously 

            await Task.Delay(-1); // Delay the end of the task for an infinite amount of time

        }

        /// <summary>
        /// Registers Events
        /// </summary>
        private async Task RegisterEvents()
        {
            _client.MessageReceived += new CEBEvents.EventHandler().OnMessage;
        }

        /// <summary>
        /// Outputs a logged message to the console.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>property if the task was completed highly unlikely to fail.</returns>
        private static Task OnLog(LogMessage message)
        {
            Console.WriteLine(message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers commands to the CommandService
        /// </summary>
        private async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
        
        /// <summary>
        /// Handles an incoming command
        /// </summary>
        /// <param name="arg"></param>
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);
            if (message != null && message.Author.IsBot) return;

            var argPos = 0;
            if (message.HasStringPrefix(_prefix, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                if (result.Error.Equals(CommandError.UnmetPrecondition))
                    if (message != null)
                        await message.Channel.SendMessageAsync(result.ErrorReason);
            }
        }
        
        /// <summary>
        /// Loads the contents of commonerrors.json into the CommonErrors var
        /// </summary>
        public static void LoadFile()
        {
            // Read commonerrors.json
            using (var r = new StreamReader("commonerrors.json"))
            {
                var json = r.ReadToEnd();
                CommonErrors = JObject.Parse(json);
            }
        }
    }
}