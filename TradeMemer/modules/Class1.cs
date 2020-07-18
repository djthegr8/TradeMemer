using BetterCommandService;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMemer.modules;
using Newtonsoft.Json;
using System.IO;


namespace TradeMemer.modules
{
    [DiscordCommandClass()]
    public class MyCommandClass : CommandModuleBase
    {
        readonly Random regret = new Random();
        readonly Embed erroR = new EmbedBuilder
        {
            Title = "**Trade Memer help**"
        }
        .AddField("Trading Commands", "``!trade`` is the command for putting a sale \n`!trade [quantity] [itemname] [price]`\nExample - !trade 12 banknotes 20k\nWhile ``!buying`` is the command for requesting a trade\nExample - !buying 2 pepe 60000\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :x: to declare the deal as closed.\n\n")
        .AddField("Secure-trade Commands", "`!profile` - This command allows you to check the past warns of the trader.\n`!profile [mention/id]`\nExample - !profile @DJ001\n`!my-reports` - Get details of your past warns.\n`trade-report`(Admins only) - Report scammers.You can report a scammer only once in 1 server.\n`!trade-report [@scammmer or ID] [optional reason]`\nExample - !trade-report @DJ001\n`!appeal` - If you feel you are warned or banned falsely then you can appeal here with proof.\n\n")
        .AddField("Non trading commands", "`idea` - Help us in making trade better by sharing your unique suggestions with us.\nExample - !idea my suggestion is make bot better\n`!vote` - Motivate us for making trade better by voting the bot.\n`!ping` - Allows you to check the speed of bot.\n\n")
        .Build();
        readonly Embed buyError = new EmbedBuilder
        {
            Title = "**Buy Command**",
            Description = "The command is \n!buy [quantity] [itemname] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :x: to declare the deal as closed."
        }.Build();
        readonly Embed vote = new EmbedBuilder
        {
            Title = "**Like our bot? Vote for us!!**",
            Description = "As the main aim of this bot is to give unconditional support for trading for currency bot items, we will **never** ask you to vote for a feature. If you truly like our bot, then please vote for us on top.gg!!!!",
            Url = "https://top.gg/bot/722732239376613406"
        }.Build();
        readonly Embed tradeError = new EmbedBuilder
        {
            Title = "**Trade Command**",
            Description = "The command is \n!trade [quantity] [itemname] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed."
        }.Build();
        readonly EmbedBuilder invite = new EmbedBuilder
        {
            Title = "Invite TradeMemer!",
            Description = "Click the above link to invite TradeMemer to your servers!!",
            Url = "https://tiny.cc/TMAdmin",
            Footer = new EmbedFooterBuilder
            {
                Text = "For any server or perms config, dont hesitate DMing DJ001#0915"
            }
        };

        [DiscordCommand("buying")]
        public async Task BuyCommand(uint quantity, string item, string price)
        {
            if (quantity == 0 || item == "")
            {
                await Context.Channel.SendMessageAsync("", false, buyError);
                return;
            }
            string chn = "";
            //if (item.Contains("bank") || item.Contains("note"))
            //{
            //    chn = Context.Guild.GetRole(717619391151341599).Mention;
            //}
            bool isTrader;
            (price, isTrader) = await Preprocess(price);
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
                Footer = new EmbedFooterBuilder()
            };
            if (isTrader) eb.Description = $"{Context.User.Mention} wants to buy **{quantity} {item} ** and is willing to pay {price} each";
            else eb.Description = $"{Context.User.Mention} wants to buy **{quantity} {item} ** and is willing to barter {price}";
            eb.Footer.Text = "Made with ❤️ by TradeMemer";
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            var res = await chnl.SendMessageAsync(chn, false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            await res.AddReactionAsync(new Emoji("❌"));
            if (isTrader) await ReplyAsync($"Your buyer's request of {quantity} {item} for {price} currency has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            else await ReplyAsync($"Your barter request of {quantity} {item} for {price} has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            await Bleh(Context);
        }
        [DiscordCommand("trade")]
        public async Task TradeCommand(uint quantity, string item, string price)
        {
            if (quantity == 0 || item == "")
            {
                await Context.Channel.SendMessageAsync("", false, tradeError);
                return;
            }
            bool isTrader;
            (price, isTrader) = await Preprocess(price);
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
                Footer = new EmbedFooterBuilder(),
            };
            eb.Footer.Text = "Made with ❤️ by TradeMemer";
            if (isTrader) eb.Description = $"{Context.User.Mention} is placing **{quantity} {item} ** on sale for {price} each";
            else eb.Description = $"{Context.User.Mention} is placing **{quantity} {item} ** on sale and is willing to exchange {price}";
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            var res = await chnl.SendMessageAsync("", false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            await res.AddReactionAsync(new Emoji("❌"));
            if (isTrader) await ReplyAsync($"Your sale of {quantity} {item} for {price} currency has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            else await ReplyAsync($"Your barter of {quantity} {item} for {price} has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            await Bleh(Context);
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
        public async Task Vote()
        {
            await Context.Channel.SendMessageAsync("", false, vote);
        }
        [DiscordCommand("idea")]
        public async Task Idea(params string[] args)
        {
            await ReplyAsync("Your suggestion has been sent to the Devs and i've put in a good word ;)\nAnd while you wait, join our support server!\nhttps://discord.gg/PbunDXN");
            var sug = Context.Message.Content.Remove(0, 5);
            var mBed = new EmbedBuilder
            {
                Description = sug + "\n" + $"*Given by {Context.User.Username}#{Context.User.Discriminator}*",
                Title = $"**New bot suggestion from **{Context.Guild.Name}"
            };
            var tM = await Context.Client.GetGuildAsync(730634262788833281);
            var sugChan = await tM.GetChannelAsync(730700586135322636) as SocketTextChannel;
            await sugChan.SendMessageAsync("", false, mBed.Build());
            await Task.Delay(2000);
        }
        [DiscordCommand("invite")]
        public async Task Me(params string[] args)
        {
            invite.ImageUrl = Context.Client.CurrentUser.GetAvatarUrl();
            await ReplyAsync("", false, invite.Build());
        }
        [DiscordCommand("prefix")]
        public async Task PrefixUpd(char x)
        {
            await Class3.PrefixAdder(Context.Guild.Id, x);
        }
        //Below are supporter functions, not really commands.
        
        public async Task Bleh(ICommandContext context)
        {
            
            if (regret.Next(0, 6) <= 3)
            {
                if (regret.Next(0, 10) > 5)
                {
                    if ((context.User as SocketGuildUser).MutualGuilds.Any(x => x.Id == 732300342888497263)) return;
                    await ReplyAsync("While you wait for a someone to see your deal, why not join our supercool support server!\nhttps://discord.gg/PbunDXN");
                }
                else
                {
                    if (await Class4.HasVoted(context.User.Id)) return;
                    await ReplyAsync("Whats up memer! Vote for our bot NOW! :)\nhttps://tiny.cc/TMDBL");
                }
            }
        }
        public async Task<ulong> SearchChannel(SocketCommandContext truContext)
        {
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
        public async Task<(string, bool)> Preprocess(string s)
        {
            bool alf;
            string tor = "";
            (tor, alf) = await Task.Run(() =>
           {
               int rem = s.Length - 1;
               bool isVal = float.TryParse(s.Remove(s.Length - 1, 1), out float test);
               bool isFullNum = float.TryParse(s, out float fullnum);
               if (isFullNum)
               {
                   return (fullnum.ToString("#,##0"), true);
               }
               else if (isVal && (s.ToLower()[rem] == 'k' || s.ToLower()[rem] == 'm'))
               {
                   test *= (Convert.ToInt32(s.ToLower()[rem] == 'k') * 1000) + (Convert.ToInt32(s.ToLower()[rem] == 'm') * 1000000);
                   return (test.ToString("#,##0"), isVal);
               }
               else
               {
                   isVal = false;
                   return (s, isVal);
               }
           });
            return (tor, alf);
        }
    }
}