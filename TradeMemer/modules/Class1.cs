using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using BetterCommandService;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using Discord.WebSocket;
using Discord.Commands;
using System.Net.Sockets;

namespace DMCG_Answer.modules
{

    [DiscordCommandClass()]
    public class MyCommandClass : CommandModuleBase
    {
        [DiscordCommand("trade")]
        public async Task TradeCommand(int quantity, string item, int price)
        {

            if (quantity == 0 || price == 0 || item == "")
            {
                EmbedBuilder errorEmbed = new EmbedBuilder();
                errorEmbed.Title = "**Trade Command**";
                errorEmbed.Description = "The command is \n!trade [quantity] [item name] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed.";
                var weird = await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
                return;
            }
            string chn = "";
            //if (item.Contains("bank") || item.Contains("note"))
            //{
            //    chn = Context.Guild.GetRole(717619391151341599).Mention;
            //}
            SocketCommandContext truContext = Context as SocketCommandContext;
            if (!truContext.Guild.Channels.Any(x => x.Name.ToLower() == "marketplace")) {
                await truContext.Guild.CreateTextChannelAsync("marketplace");
            }
            ulong chnlId = truContext.Guild.TextChannels.First(x => x.Name.ToLower() == "marketplace").Id;
            EmbedBuilder eb = new EmbedBuilder();
            eb.Title = "**" + item.ToUpper() + "** on sale";
            eb.Description = Context.User.Mention + " is placing **" + quantity + " " + item + "** on sale for " + price;
            eb.Color = Color.Green;
            var chnl = truContext.Guild.GetTextChannel(chnlId);
            var res = await chnl.SendMessageAsync(chn, false, eb.Build());
            await res.AddReactionAsync(new Emoji("✅"));
            string s;
            if (quantity > 1) { s = "s"; } else { s = ""; }
            await ReplyAsync($"Your sale of {quantity} {item}{s} for {price} DMC has been mentioned in {chnl.Mention}!");


        }
        [DiscordCommand("trade")]
        public async Task SingleArgTrade(string help)
        {
            EmbedBuilder errorEmbed = new EmbedBuilder();
            errorEmbed.Title = "**Trade command**";
            errorEmbed.Description = "The command is \n!trade [quantity] [item name] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed.";
            var weird = await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
        }
        [DiscordCommand("ping")]
        public async Task Ping()
        {
            await ReplyAsync($"Pong ``:{ (Context as SocketCommandContext).Client.Latency} ms``");
        }
        [DiscordCommand("trade")]
        public async Task NoArgTrade()
        {
            EmbedBuilder errorEmbed = new EmbedBuilder();
            errorEmbed.Title = "**Trade command**";
            errorEmbed.Description = "The command is \n!trade [quantity] [item name] [price]\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed.";
            var weird = await Context.Channel.SendMessageAsync("", false, errorEmbed.Build());
        }
        [DiscordCommand("vote")]
        [Alias("support","like")]
        public async Task vote()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "**Like our bot? Vote for us!!**";
            embed.Description = "As the main aim of this bot is to give unconditional support for trading for currency bot items, we will **never** ask you to vote for a feature. If you truly like our bot, then please vote for us on top.gg";
            embed.Url = "https://www.top.gg/";
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        
    }
}