using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Public_Bot;
using Discord.Commands;
using DiscordBotsList.Api;
using System.Threading.Tasks;
using DiscordBotsList.Api.Objects;
using System.Reflection.Metadata.Ecma335;

namespace TradeMemer.modules
{
    [DiscordCommandClass("Top.GG/DBL","Api usage class")]
    public class Class4: CommandModuleBase
    {
        public static AuthDiscordBotListApi DblApi = new AuthDiscordBotListApi(722732239376613406, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjcyMjczMjIzOTM3NjYxMzQwNiIsImJvdCI6dHJ1ZSwiaWF0IjoxNTk0OTkyOTczfQ.c3qlr_X0XRk9h3flsWR1Tt88mLRODAi6JrOuIrpgX28");
        public static async Task<string> GetApi(SocketCommandContext ctxt)
        {
            IDblSelfBot me = await DblApi.GetMeAsync();
            var hasVoted = await DblApi.HasVoted(ctxt.User.Id);
            if (hasVoted)
            {
                return "*We ❤️ you for voting!!!*";
            } else
            {
                return "";
            }
        }
        public static async Task<bool> HasVoted(ulong ide)
        {
            IDblBot me = await DblApi.GetMeAsync();
            return await DblApi.HasVoted(ide);
        }
    }
}
