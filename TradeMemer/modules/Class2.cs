using System;
using System.Collections.Generic;
using static Public_Bot.CustomCommandService;
using Public_Bot;
using Discord;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace TradeMemer.modules
{

    [DiscordCommandClass("Reporter","Class for reporting users")]
    public class MySecondCommandClass : CommandModuleBase
    {
        /*[DiscordCommand("sqltest")]
        public async Task SqlTest(params string[] args)
        {
            string tryer = string.Join(' ', args);
            Console.WriteLine(tryer);
            Class3.Bla(tryer);
        }*/
        [DiscordCommand("trade-report",description ="Reports the mentioned user for given reason and scamming",commandHelp ="trade-report [id/mention] [reason]",example = "trade-report @Scammer liar")]
        public async Task SqlTest(params string[] args)
        {
            ulong reportedId;
            if ((Context.Guild).MemberCount < 25 && (Context.Guild.Id != 732300342888497263 && (Context.Channel.Id != 732300343655923811 || Context.Channel.Id != 732300344138399855)))
            {
                await ReplyAsync("Due to prevention of scam reports, we only allow reports from servers with above 25 people.");
                return;
            }
            if (!(Context.User as SocketGuildUser).GuildPermissions.Administrator)
            {
                await ReplyAsync("You arent allowed to report dude");
                return;
            }
            if (!(Context.Message as IUserMessage).MentionedUserIds.Any())
            {
                if (ulong.TryParse(args[0], out ulong test))
                {
                    reportedId = test;
                }
                else
                {
                    await ReplyAsync("You gotta mention who to report.");
                    return;
                }
            }
            else
            {
                reportedId = (Context.Message as IUserMessage).MentionedUserIds.First();
            }
            IUser rA;
            try
            {
                rA = await (Context.Client as IDiscordClient).GetUserAsync(reportedId);
            }
            catch
            {
                await ReplyAsync("That user doesnt exist");
                return;
            }
            string x;
            string y;
            if (args.Length < 2) {
                x = await SqliteClass.Bla(reportedId, Context.Guild.Id, Context.User.Id, DateTime.UtcNow);
                y = "bad trading/scamming";
            }
            else {
                x = await SqliteClass.Bla(reportedId, Context.Guild.Id, Context.User.Id, DateTime.UtcNow, args[1]);
                y = string.Join(' ',args.Skip(1));
            }
            if (x == "E")
            {
                await ReplyAsync("This user has already been banned from Trade Memer.");
                return;
            }
            else if (x == "G")
            {
                await ReplyAsync("You can't report the same person twice from the same server.");
                return;
            }
            else
            {
                var m = new EmbedBuilder
                {
                    Title = $"User <@{reportedId}> reported",
                    Description = $"Above mentioned user has been reported for reason: {y}",
                    Color = Color.Red
                }.AddField("Report Id", $"For your reference, and further corrections use the below id:\n{x}");
                await ReplyAsync("", embed: m.Build());
            }
            await rA.SendMessageAsync($"You were reported for {y} by {Context.User.Username}{Context.User.Discriminator} in the server {Context.Guild.Name}\nTo appeal, u can go to https://forms.gle/gvcDFsqwqxCMr7dR7");

        }
        [DiscordCommand("profile",description ="Shows the previous reports of mentioned user",commandHelp ="profile [id/mention]",example = "profile @trader")]
        public async Task DiscordProfileGetSQL(params string[] _args)
        {
            ulong uzer;
            if (!(Context.Message as IUserMessage).MentionedUserIds.Any())
            {
                if (ulong.TryParse(_args[0], out ulong test))
                {
                    uzer = test;
                }
                else
                {
                    await ReplyAsync("You gotta mention whose profile u wanna check!");
                    return;
                }
            }
            else
            {
                uzer = (Context.Message as IUserMessage).MentionedUserIds.First();
            }
            Tuple<int, DateTimeOffset?> x = await SqliteClass.CheckUsr(uzer);
            if (x.Item1 != 0)
            {
                await ReplyAsync($"The user mentioned has {x.Item1} warning(s), with the latest one on {x.Item2.Value}");
            }
            else
            {
                await ReplyAsync($"The user mentioned has no warnings! They're good to trade with!");
            }
        }
        [DiscordCommand("appeal",description ="Gives the link for appealing against incorrect reports")]
        public async Task AppealCommand(params string[] arg)
        {
            await ReplyAsync("You may appeal at https://forms.gle/gvcDFsqwqxCMr7dR7 for any spam report!");
        }
        [DiscordCommand("my-reports",description ="Shows all your reports with IDs")]
        public async Task ReportCommand(params string[] argz)
        {
            var tup = await SqliteClass.SpeedCheck(Context.User.Id);
            if (tup.Item1 > 0)
            {
                string rids = "";
                tup.Item2.ForEach(x => rids += $"{x}\n");
                Embed m = new EmbedBuilder
                {
                    Title = $"{Context.User.Username}'s Reports",
                    Description = $"You have {tup.Item1} reports"
                }.AddField("Report Ids:", rids)
                .WithCurrentTimestamp()
                .Build();
                await ReplyAsync("", false, m);
                return;
            } else
            {
                await ReplyAsync($"You have no reports Mr Awesome! {await Class4.GetApi(Context as Discord.Commands.SocketCommandContext)}");
            }
        }
        [DiscordCommand("unreport")]
        public async Task Unreport(params string[] args)
        {
            if (Context.User.Id != 701029647760097361) return;
            await SqliteClass.UserUnreporter(args[0]);
            await ReplyAsync("Done sir.");
        }
        [DiscordCommand("sqlite")]
        public async Task SqliteSent(params string[] args)
        {
            if (Context.User.Id != 701029647760097361) return;
            await Context.User.SendFileAsync($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}tmdb.db");
            await ReplyAsync("Check ur DM!");
        }
    }
}
