using BetterCommandService;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
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
            if (!truContext.Guild.Channels.Any(x => x.Name.ToLower() == "marketplace"))
            {
                try
                {
                    await truContext.Guild.CreateTextChannelAsync("marketplace");
                }
                catch (Discord.Net.HttpException)
                {
                    await truContext.Channel.SendMessageAsync("I dont have perms to make a channel. If u have trust issues, then create a channel called marketplace.");
                }
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
            await ReplyAsync($"Your sale of {quantity} {item}{s} for {price} currency has been mentioned in {chnl.Mention}!");
        }
        [DiscordCommand("trade")]
        public async Task SingleArgTrade(string help)
        {
            EmbedBuilder errorEmbed = new EmbedBuilder();
            errorEmbed.Title = "**Trade Memer help**";
            errorEmbed.Description = "``!trade`` \n``!trade [quantity] [item name] [price]``\nReact with :white_check_mark: to accept a deal and DM the seller.\nAfter a deal is finished, seller should react with :regional_indicator_d: to declare the deal as closed.\n\n``!idea``\nHave a suggesion for the bot? Tell it here!!!\n\n``!vote``\n Gives the link for bot's site\n\n``!ping``\nGets the ping of the bot";
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
        [Alias("support", "like")]
        public async Task vote()
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.Title = "**Like our bot? Vote for us!!**";
            embed.Description = "As the main aim of this bot is to give unconditional support for trading for currency bot items, we will **never** ask you to vote for a feature. If you truly like our bot, then please vote for us on top.gg (we're not verified there yet, so while u wait, click the link and review us on Bots on Discord)";
            embed.Url = "https://bots.ondiscord.xyz/bots/722732239376613406";
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }
        [DiscordCommand("idea")]
        public async Task idea(params string[] args)
        {
            await ReplyAsync("Your suggestion has been sent to the Devs and i've put in a good word ;)");
            var sug = Context.Message.Content.Remove(0,5);
            var mBed = new EmbedBuilder();
            mBed.Description = sug + "\n" + $"*Given by {Context.User.Username}#{Context.User.Discriminator}*";
            mBed.Title = $"**New bot suggestion from **{Context.Guild.Name}";
            var DMCG = (await Context.Client.GetGuildAsync(591660163229024287));
            var DJ = await DMCG.GetUserAsync(701029647760097361);
            var Ket = await DMCG.GetUserAsync(541998151716962305);
            await DJ.SendMessageAsync("",false, mBed.Build());
            await Ket.SendMessageAsync("",false, mBed.Build());
            await Task.Delay(2000);
        }
    }
}