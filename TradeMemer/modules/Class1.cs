using static Public_Bot.CustomCommandService;
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
using Public_Bot;

namespace TradeMemer.modules
{
    [DiscordCommandClass("Trading","Class with all the stuff")]
    public class MyCommandClass : CommandModuleBase
    {
        readonly Random regret = new Random();
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

        [DiscordCommand("buying",
            description ="This command is for requesting people to sell the item you are wishing to buy.",
            commandHelp ="buying [quantity] [itemname] [price-each/barter]",
            example ="buying 10 pepe 2m"
            )]
        [Alt("b")]
        [Alt("buy")]
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
            SocketCommandContext truContext = Context;
            ulong chnlId = await SearchChannel(truContext);
            if (chnlId == 1)
            {
                return;
            }
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            if (isTrader) await ReplyAsync($"Your buyer's request of {quantity} {item} for {price} currency has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            else await ReplyAsync($"Your barter request of {quantity} {item} for {price} has been mentioned in {chnl.Mention}!\n{await Class4.GetApi(Context as SocketCommandContext)}");
            await Bleh(Context);
            EmbedBuilder eb = new EmbedBuilder
            {
                Title = $"**{item.ToUpper()}** in demand!",
                Color = Color.Blue,
                Footer = new EmbedFooterBuilder()
            };
            if (isTrader) eb.Description = $"{Context.User.Mention} wants to buy **{quantity} {item} ** and is willing to pay {price} each";
            else eb.Description = $"{Context.User.Mention} wants to buy **{quantity} {item} ** and is willing to barter {price}";
            eb.Footer.Text = "Made with ❤️ by TradeMemer";
            var res = await chnl.SendMessageAsync(chn, false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            await res.AddReactionAsync(new Emoji("❌"));
        }
        [DiscordCommand("trade",
            description ="This is the command for placing an item for sale.",
            commandHelp ="trade [quantity] [itemname] [price-each/barter]",
            example ="trade 5 fish 5.1k"
            )]
        [Alt("s")]
        [Alt("sell")]
        [Alt("selling")]
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

        [DiscordCommand("ping",description ="See how fast the bot's responses are!!")]
        public async Task Ping()
        {
            await ReplyAsync($"TradePong: ``{ (Context as SocketCommandContext).Client.Latency} ms``");
        }
        [DiscordCommand("trade")]
        public async Task NoArgTrade()
        {
            var prefix = await SqliteClass.PrefixGetter(Context.Guild.Id);
            Embed erroR = new EmbedBuilder
            {
                Title = "**Trade Memer help**"
            }
        .AddField("Trading Mechanism", $"React with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :x: to declare the deal as closed.\n\n")
        .AddField("Trading Commands", $"1.``{prefix}trade``\n2.``{prefix}buying``\n\n")
        .AddField("Non trading commands", $"1.`{prefix}prefix`\n2.`{prefix}idea`\n3.`{prefix}vote`\n4.`{prefix}ping`\n5. `{prefix}patch`\n")
        .AddField("Secure-trade Commands", $"1.`{prefix}profile`\n2.`{prefix}my-reports`\n3.`{prefix}trade-report`(Admins only)\n4.`{prefix}appeal`\n\n")
        .AddField("Command-Specific Help", $"Specific help can be received by doing `{prefix}help [cmdname]`")
        .AddField("Links", "[Support Server](https://discord.gg/PbunDXN) | [Invite link](https://tiny.cc/TMAdmin) | [GitHub](https://tiny.cc/TMGitHub)")
        .Build();
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
        [DiscordCommand("vote",
            description ="Like our bot? Vote for it on Discord Bot List!"
            )]
        [Alt("voting")]
        public async Task Vote(params string[] args)
        {
            await Context.Channel.SendMessageAsync("", false, vote);
        }
        [DiscordCommand("idea",description ="Have a suggestion to make? Do it here")]
        [Alt("suggest")]
        [Alt("suggestion")]
        public async Task Idea(params string[] args)
        {
            await ReplyAsync("Your suggestion has been sent to the Devs and i've put in a good word ;)");
            await Bleh(Context);
            var sug = Context.Message.Content;
            var mBed = new EmbedBuilder
            {
                Description = sug + "\n" + $"*Given by {Context.User.Username}#{Context.User.Discriminator}*",
                Title = $"**New bot suggestion from **{Context.Guild.Name}"
            };
            var tM = await (Context.Client as IDiscordClient).GetGuildAsync(732300342888497263);
            var sugChan = await tM.GetChannelAsync(734634237772431490) as SocketTextChannel;
            await sugChan.SendMessageAsync("", false, mBed.Build());
            await Task.Delay(2000);
        }
        [DiscordCommand("invite",description ="Invite the bot to your server!")]
        public async Task Me(params string[] args)
        {
            invite.ImageUrl = Context.Client.CurrentUser.GetAvatarUrl();
            await ReplyAsync("", false, invite.Build());
        }
        [DiscordCommand("prefix",description ="Change the bot's prefix here!",commandHelp ="prefix &")]
        public async Task PrefixUpd(string x)
        {
            await SqliteClass.PrefixAdder(Context.Guild.Id, x);
            EmbedBuilder f = new EmbedBuilder
            {
                Title = "Prefix Changed Successfully!",
                Description = $"The bot prefix has successfully been changed to {x}",
                ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl()
            };
            await ReplyAsync("", false, f.Build());
        }

        [DiscordCommand("help")]
        public async Task Helper(string cmd)
        {
            if (!Commands.Any(x => (x.CommandName.ToLower() == cmd.ToLower() || x.Alts.Any(x => x.ToLower() == cmd.ToLower())) && x.CommandDescription != ""))
            {
                var d = new EmbedBuilder
                {
                    Title = "Command not found",
                    Description = $"The command {cmd} was not found!",
                    Footer = new EmbedFooterBuilder()
                };
                d.Footer.Text = "Help Command by Trade Memer";
                d.Footer.Build();
                await ReplyAsync("", embed: d.Build());
            } else
            {
                var prefixure = await SqliteClass.PrefixGetter(Context.Guild.Id);
                var commandSelected = Commands.First(x => (x.CommandName.ToLower() == cmd.ToLower() || x.Alts.Any(x => x.ToLower() == cmd.ToLower())) && x.CommandDescription != "");
                var aliasStr = prefixure + string.Join($", {prefixure}", commandSelected.Alts);
                var embeds = new EmbedBuilder();
                embeds.AddField("Command", prefixure + commandSelected.CommandName + '\t');
                embeds.AddField("Description", commandSelected.CommandDescription, true);
                if (!string.IsNullOrEmpty(commandSelected.CommandHelpMessage)) embeds.AddField("Usage", $"`{prefixure}{commandSelected.CommandHelpMessage}`");
                if (!string.IsNullOrEmpty(commandSelected.example)) embeds.AddField("Example", $"`{prefixure}{commandSelected.example}`");
                if (commandSelected.Alts.Count > 0) embeds.AddField("Aliases", aliasStr);
                embeds.AddField("Links", "[Support Server](https://discord.gg/PbunDXN) | [Invite link](https://tiny.cc/TMAdmin) | [GitHub](https://tiny.cc/TMGitHub)");
                embeds.Footer = new EmbedFooterBuilder { Text = "Help Command by Trade Memer" };
                await ReplyAsync("", false, embeds.Build());
            }
        }
        [DiscordCommand("patch",
            description ="See the update logs here!",
            commandHelp ="patch [version-number]",
            example ="patch 1.3"
            )]
        public async Task PatchFunc(double vnum)
        {
            EmbedBuilder x = new EmbedBuilder();
            switch (vnum)
            {
                case 1.1:
                    x.Description = "The very first Trade Memer patch!\nAll it included was trade embeds!!";
                    break;
                case 1.2:
                    x.Description = "The first version to run on a RPi, was tremendously improved from v1.1 to receive Reaction based trades!";
                    break;
                case 1.3:
                    x.Description = "One of the more updated versions, with all sorts of reporting features (JSON)";
                    break;
                case 1.4:
                    x.Description = "1. Shifted to SQLite for better reliability\n2. Custom Prefix\n3. Professional commands.";
                    break;
            }
            x.Title = "Patch Information";
            await ReplyAsync("", embed: x.Build());
        }
        //Below are supporter functions, not really commands.
        
        public async Task Bleh(ICommandContext context)
        {
            var xyz = await context.Client.GetGuildAsync(732300342888497263);
            await xyz.DownloadUsersAsync();
            var abc = await xyz.GetUsersAsync();
            if (regret.Next(0, 6) <= 4)
            {
                if (regret.Next(0, 10) > 5)
                {
                    if (abc.Any(x => x.Id == context.User.Id)) return;
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