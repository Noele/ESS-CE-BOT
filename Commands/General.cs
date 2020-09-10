using System.Threading.Tasks;
using Discord.Commands;

namespace CommonErrorsBot.CEB.Commands
{
    public class General : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("Pong");
        }
        
    }
}