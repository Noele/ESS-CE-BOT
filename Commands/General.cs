using System.Threading.Tasks;
using Discord.Commands;

namespace CommonErrorsBot.CEB.Commands
{
    public class General : ModuleBase<SocketCommandContext>
    {
        // Ping
        [Command("ping")]
        public async Task Ping()
        {
            // Pong
            await ReplyAsync("Pong");
        }
        // What you expected actual documentation on this ????? :kekw:
    }
}