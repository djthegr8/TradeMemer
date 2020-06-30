using BetterCommandService;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using System;
using System.Diagnostics.Contracts;

namespace DMCG_Answer.modules
{
    [DiscordCommandClass()]
    public class MyCommandClass : CommandModuleBase
    {
        readonly Random regret = new Random();
        readonly Embed erroR = new EmbedBuilder {
            Title = "**Trade Memer help**",
            Description = "**Trading Commands**\n``!trade`` is the command for putting a sale \n``!trade [quantity] [itemname] [price]``\nWhile ``!buying`` is the command for requesting a trade\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed.\n\n\n**Non trading commands**\n``!idea``\nHave a suggestion for the bot? Tell it here!!!\n\n``!vote``\n Gives the link for bot's site\n\n``!ping``\nGets the ping of the bot"
        }.AddField("Join our support server!", "[If not done already, **join our support server today!**](https://discord.gg/QtzrtGr)").Build();
        readonly Embed buyError = new EmbedBuilder
        {
            Title = "**Buy Command**",
            Description = "The command is \n!buy [quantity] [itemname] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed."
        }.Build();
        readonly Embed vote = new EmbedBuilder
            {
                Title = "**Like our bot? Vote for us!!**",
                Description = "As the main aim of this bot is to give unconditional support for trading for currency bot items, we will **never** ask you to vote for a feature. If you truly like our bot, then please vote for us on top.gg (we're not verified there yet, so while u wait, click the link and review us on Bots on Discord)",
                Url = "https://bots.ondiscord.xyz/bots/722732239376613406"
            }.AddField("Join our support server!", "[If not done already, **join our support server today!**](https://discord.gg/QtzrtGr)").Build();
        readonly Embed tradeError = new EmbedBuilder
                {
                    Title = "**Trade Command**",
                    Description = "The command is \n!trade [quantity] [itemname] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed."
                }.Build();

        [DiscordCommand("buying")]
        public async Task BuyCommand(uint quantity, string item, string price)
        {
            if (quantity == 0 || item == "")
            {               
                var weird = await Context.Channel.SendMessageAsync("", false, buyError);
                return;
            }
            string chn = "";
            //if (item.Contains("bank") || item.Contains("note"))
            //{
            //    chn = Context.Guild.GetRole(717619391151341599).Mention;
            //}
            SocketCommandContext truContext = Context as SocketCommandContext;
            ulong chnlId = await SearchChannel(truContext);
            if (chnlId == 1)
            {
                return;
            }
            EmbedBuilder eb = new EmbedBuilder
            {
                Title = $"**{item.ToUpper()}** in demand!",
                Color = Color.Blue,
                Description = Context.User.Mention + " wants to buy **" + quantity + " " + item + "** and is willing to pay " + price,
                ThumbnailUrl = Context.User.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder()
            };
            eb.Footer.Text = "Made with ❤️ by TradeMemer";
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            var res = await chnl.SendMessageAsync(chn, false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            await res.AddReactionAsync(new Emoji("🇩"));
            await ReplyAsync($"Your buyer's request of {quantity} {item} for {price} currency has been mentioned in {chnl.Mention}!");
            await Bleh();
        }
        [DiscordCommand("trade")]
        public async Task TradeCommand(uint quantity, string item, string price)
        {
            if (quantity == 0 || item == "")
            {
                var weird = await Context.Channel.SendMessageAsync("", false, tradeError);
                return;
            }
            //if (item.Contains("bank") || item.Contains("note"))
            //{
            //    chn = Context.Guild.GetRole(717619391151341599).Mention;
            //}
            SocketCommandContext truContext = Context as SocketCommandContext;
            ulong chnlId = await SearchChannel(truContext);
            if (chnlId == 1)
            {
                return;
            }
            EmbedBuilder eb = new EmbedBuilder
            {
                Title = "**" + item.ToUpper() + "** on sale",
                Color = Color.Green,
                Description = Context.User.Mention + " is placing **" + quantity + " " + item + "** on sale for " + price,
                ThumbnailUrl = Context.User.GetAvatarUrl(),
                Footer = new EmbedFooterBuilder(),
            };
            eb.Footer.Text = "Made with ❤️ by TradeMemer";
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            var res = await chnl.SendMessageAsync("", false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            await res.AddReactionAsync(new Emoji("🇩"));
            await ReplyAsync($"Your sale of {quantity} {item} for {price} currency has been mentioned in {chnl.Mention}!");
            await Bleh();
        }
        [DiscordCommand("trade")]
        public async Task SingleArgTrade(string help)
        {
            await NoArgTrade();
        }

        [DiscordCommand("ping")]
        public async Task Ping()
        {
            await ReplyAsync($"TradePong: ``{ (Context as SocketCommandContext).Client.Latency} ms``");
        }
        [DiscordCommand("trade")]
        public async Task NoArgTrade()
        {
            await Context.Channel.SendMessageAsync("", false, erroR);
        }
        [DiscordCommand("buying")]
        public async Task NoArgBuyTrade()
        {
            await NoArgTrade();
        }
        [DiscordCommand("buying")]
        public async Task OneArgBuyTSrade(string arg)
        {
            await NoArgTrade();
        }
        [DiscordCommand("vote")]
        [Alias("support", "like")]
        public async Task Vote()
        {
            await Context.Channel.SendMessageAsync("", false, vote);
        }
        [DiscordCommand("idea")]
        public async Task Idea(params string[] args)
        {
            await ReplyAsync("Your suggestion has been sent to the Devs and i've put in a good word ;)");
            var sug = Context.Message.Content.Remove(0,5);
            var mBed = new EmbedBuilder
            {
                Description = sug + "\n" + $"*Given by {Context.User.Username}#{Context.User.Discriminator}*",
                Title = $"**New bot suggestion from **{Context.Guild.Name}"
            };
            var tM = await Context.Client.GetGuildAsync(725567523264659566);
            var sugChan = await tM.GetChannelAsync(725584761640452118) as SocketTextChannel;
            await sugChan.SendMessageAsync("", false, mBed.Build());
            await Task.Delay(2000);
        }
        public async Task Bleh() {
            if (regret.Next(0, 10) > 6)
            {
                if (regret.Next(0, 10) > 4)
                {
                    await ReplyAsync("While you wait for a someone to see your deal, why not join our supercool support server!\nhttps://discord.gg/QtzrtGr");
                }
                else
                {
                    await ReplyAsync("Hey trader, cmon and join our official Trade Memer support server now!\nhttps://discord.gg/QtzrtGr");
                }
            }
        }
        public async Task<ulong> SearchChannel(SocketCommandContext truContext) {
            if (!truContext.Guild.Channels.Any(x => x.Name.ToLower().Contains("marketplace")))
            {
                try
                {
                    await truContext.Guild.CreateTextChannelAsync("marketplace");
                    return truContext.Guild.TextChannels.First(x => x.Name == "marketplace").Id;
                }
                catch (Exception)
                {
                    await truContext.Channel.SendMessageAsync("I dont have perms to make a channel. If u have trust issues, then create a channel including the phrase marketplace.");
                    return 1;
                }
            }
            return truContext.Guild.TextChannels.First(x => x.Name.ToLower().Contains("marketplace")).Id;
        }
    }
}