using CommonErrorsBot.CEB;

namespace CommonErrorsBot
{
    internal class Program
    {
        /// <summary>
        /// Main method to start the bot asynchronously 
        /// </summary>
        /// <param name="args"> Bot arguments, first argument is the bots token, 2nd is the bots prefix, 3rd is the api key</param>
        public static void Main(string[] args) => new Bot().StartAsync(args[0], args[1], args[2]).GetAwaiter().GetResult();
    }
}