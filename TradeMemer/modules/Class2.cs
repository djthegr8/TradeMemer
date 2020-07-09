using System;
using System.Collections.Generic;
using BetterCommandService;
using Discord;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TradeMemer.modules
{
    public partial class UserCriminalRecord
    {
        public List<Report> Reports { get; set; }
        public ulong UserId { get; set; }
    }

    public partial class Report
    {
        public DateTimeOffset Date { get; set; }
        public ulong GuildId { get; set; }
        public ulong ReporterID { get; set; }
    }

    [DiscordCommandClass()]
    public class MySecondCommandClass : CommandModuleBase
    {
        [DiscordCommand("trade-report")]
        public async Task JsonTest(params string[] args)
        {
            ulong reportedId;
            if (!(Context.User as Discord.WebSocket.SocketGuildUser).GuildPermissions.Administrator)
            {
                await ReplyAsync("You arent allowed to report dude");
                return;
            }
            if (!Context.Message.MentionedUserIds.Any())
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
                reportedId = Context.Message.MentionedUserIds.First();
            }
            var rA = await Context.Client.GetUserAsync(reportedId);
            if (rA.IsBot)
            {
                await ReplyAsync("I shall never report my bot friends!");
                return;
            }
            using (StreamReader r = new StreamReader($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}trader.json"))
            {
                string json = r.ReadToEnd();
                r.Close();
                List<UserCriminalRecord> lis = JsonConvert.DeserializeObject<List<UserCriminalRecord>>(json);
                Console.WriteLine(reportedId);
                if ((lis ?? new List<UserCriminalRecord>()).Any(x => x.UserId == reportedId))
                {
                    Console.WriteLine("FOUND EM!");
                    if ((lis.First(x => x.UserId == reportedId)).Reports.Count() == 3)
                    {
                        await ReplyAsync("That user has already been banned from trading!");
                        return;
                    }
                    if ((lis.First(x => x.UserId == reportedId)).Reports.Any(x => x.GuildId == Context.Guild.Id) && Context.User.Id != 701029647760097361)
                    {
                        await ReplyAsync("Hey man, you can't ban the same person twice in the same server!");
                        return;
                    }
                        (lis.First(x => x.UserId == reportedId)).Reports.Add(new Report
                        {
                            Date = Context.Message.Timestamp,
                            GuildId = Context.Guild.Id,
                            ReporterID = Context.User.Id
                        });
                    var val = (lis.First(x => x.UserId == reportedId)).Reports.Count;
                    string grammar;
                    if (val == 2)
                    {
                        grammar = "nd";
                    }
                    else if (val == 3)
                    {
                        grammar = "rd (they will be banned)";
                    }
                    else grammar = "rd (they will be banned)";
                    EmbedBuilder reportEmbed = new EmbedBuilder
                    {
                        Title = "User trading report",
                        Description = $"<@{reportedId}> has been warned, it is their {val}{grammar} warning.",
                        Footer = new EmbedFooterBuilder()
                    };
                    reportEmbed.Footer.Text = "If this is a spam report, you may be banned from Trade Memer";
                    await ReplyAsync("", false, reportEmbed.Build());
                }
                else
                {
                    var crim = new UserCriminalRecord
                    {
                        UserId = reportedId,
                        Reports = new List<Report>()
                    };
                    crim.Reports.Add(new Report
                    {
                        Date = Context.Message.Timestamp,
                        GuildId = Context.Guild.Id,
                        ReporterID = Context.User.Id
                    });
                    Console.WriteLine("{0} was added on {1} by {2}", crim.UserId, crim.Reports.Last().Date, crim.Reports.Last().GuildId);
                    var mBeD = new EmbedBuilder
                    {
                        Title = "First report for user",
                        Description = $"<@{reportedId}> has been warned, it is their first warning.",
                        Footer = new EmbedFooterBuilder()
                    };
                    mBeD.Footer.Text = "If this is a spam report, you may be banned from Trade Memer";
                    await ReplyAsync("", false, mBeD.Build());
                    try
                    {
                        lis.Add(crim);
                    }
                    catch (Exception)
                    {
                        lis = new List<UserCriminalRecord>();
                        lis.Add(crim);
                    }
                }
                var xAlpha = JsonConvert.SerializeObject(lis);
                Console.WriteLine(xAlpha);
                using (JsonTextWriter writer = new JsonTextWriter(new StreamWriter($"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}trader.json")))
                {
                    await writer.WriteRawAsync(xAlpha);
                }
            }
        }
    }
}
