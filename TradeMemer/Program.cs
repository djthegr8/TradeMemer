using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using BetterCommandService;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DMCG_Answer.modules;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace DMCG_Answer
{

    class Program
    {
        bool isHeistOn = false;
        IEmote YourEmoji = new Emoji("🏦");
        IEmote Dealdone = new Emoji("🇩");
        IEmote tick = new Emoji("✅");
        List<string> heist = new List<string>();
        string against = "";

        public static void Main(string[] args)
           => new Program().MainAsync().GetAwaiter().GetResult();
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
        private DiscordSocketClient _client;
        CustomCommandService _service = new CustomCommandService(new Settings()
        {
            DefaultPrefix = '!'
        });
        public async Task MainAsync()
        {

            //Console.WriteLine("The list of databases on this server is: ");
            //foreach (var db in dbList)
            //{
            //    Console.WriteLine(db);
            //}
            Console.Write(typeof(string).Assembly.ImageRuntimeVersion);

            _client = new DiscordSocketClient();

            _client.Log += Log;

            _client.MessageReceived += HandleCommandAsync;

            _client.ReactionAdded += HandleReactionAsync;





            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.
            var token = "NzIyNzMyMjM5Mzc2NjEzNDA2.Xuol1A.IVpfmEyG1JXIl_rucGGZ7xvwB2k";

            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            await _client.SetGameAsync("Trading stuff",null,ActivityType.CustomStatus);

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        internal async Task HandleCommandResult(ICommandResult result, SocketUserMessage msg)
        {
            //string logMsg = "";
            //logMsg += $"[UTC TIME - {DateTime.UtcNow.ToLongDateString() + " : " + DateTime.UtcNow.ToLongTimeString()}] ";
            string completed = resultformat(result.IsSuccess);
            //if (!result.IsSuccess)
            //    logMsg += $"COMMAND: {msg.Content} USER: {msg.Author.Username + "#" + msg.Author.Discriminator} COMMAND RESULT: {completed} ERROR TYPE: EXCEPTION: {result.Exception}";
            //else
            //    logMsg += $"COMMAND: {msg.Content} USER: {msg.Author.Username + "#" + msg.Author.Discriminator} COMMAND RESULT: {completed}";
            //var name = DateTime.Now.Day + "_" + DateTime.Now.Month + "_" + DateTime.Now.Year;
            //if (File.Exists(Global.CommandLogsDir + $"{Global.systemSlash}{name}.txt"))
            //{
            //    string curr = File.ReadAllText(Global.CommandLogsDir + $"{Global.systemSlash}{name}.txt");
            //    File.WriteAllText(Global.CommandLogsDir + $"{Global.systemSlash}{name}.txt", $"{curr}\n{logMsg}");
            //    Console.ForegroundColor = ConsoleColor.Magenta;
            //    Console.WriteLine($"Logged Command (from {msg.Author.Username})");
            //    Console.ForegroundColor = ConsoleColor.DarkGreen;
            //}
            //else
            //{
            //    File.Create(Global.MessageLogsDir + $"{Global.systemSlash}{name}.txt").Close();
            //    File.WriteAllText(Global.CommandLogsDir + $"{Global.systemSlash}{name}.txt", $"{logMsg}");
            //    Console.ForegroundColor = ConsoleColor.Cyan;
            //    Console.WriteLine($"Logged Command (from {msg.Author.Username}) and created new logfile");
            //    Console.ForegroundColor = ConsoleColor.DarkGreen;
            //}
            if (result.IsSuccess)
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.Color = Color.Green;
                eb.Title = "**Command Log**";
                eb.Description = $"The Command {msg.Content.Split(' ').First()} was used in {msg.Channel.Name} by {msg.Author.Username + "#" + msg.Author.Discriminator} \n\n **Full Message** \n `{msg.Content}`\n\n **Result** \n {completed}";
                eb.Footer = new EmbedFooterBuilder();
                eb.Footer.Text = "Command Autogen";
                eb.Footer.IconUrl = _client.CurrentUser.GetAvatarUrl();
                await _client.GetGuild(591660163229024287).GetTextChannel(712144160383041597).SendMessageAsync("", false, eb.Build());
            }

        }
        internal static string resultformat(bool isSuccess)
        {
            if (isSuccess)
                return "Success";
            if (!isSuccess)
                return "Failed";
            return "Unknown";
        }

        public async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            try
            {
                if (msg == null) return;
                if (msg.Channel.GetType() == typeof(SocketDMChannel)) {
                    await msg.Channel.SendMessageAsync("I really don't like working through DMs....So if you may, use me in a server mate!\nIf you have a suggestion, use !suggest to DM the devs!");
                    await Task.Delay(2000);
                    return; 
                }
                //var ca = msg.Content.ToCharArray();
                //if (ca.Length == 0) return;
                var context = new SocketCommandContext(_client, msg);
                if (_service.ContainsUsedPrefix(msg.ToString()))
                {
                    if (!context.User.IsBot)
                    {
                        Console.WriteLine("User is not a bot.");
                        new System.Threading.Thread(async () =>
                        {
                            var result = await _service.ExecuteAsync(context);
                            Console.WriteLine(context.User.Username + ": " + result.Result + " in channel " + context.Channel.Name + "(" + msg.Channel.GetType().ToString() + ")");
                            await HandleCommandResult(result, msg);
                        }).Start();
                    }

                    else Console.WriteLine("User is bot");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"We have encountered an error {e}");
                await msg.Channel.SendMessageAsync("Uhh there was an error. I have DMed my creator, DJ001 and he will solve this as soon as possible.");
                await Task.Delay(2000);
            }
        }
        public async Task HandleReactionAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            Console.WriteLine("Run HRA");
            var msg = await arg1.GetOrDownloadAsync();
            var mBed = msg.Embeds.First();
            Console.WriteLine("Embed success");
            Console.WriteLine(arg3.Emote.Name);
            if (arg2.Name == "marketplace" && mBed.Color == Color.Green)
            {
                var userSellerStr = Regex.Match(mBed.Description, @"\d+").Value;
                ulong userIdSeller = ulong.Parse(userSellerStr);
                var userSeller = await arg2.GetUserAsync(userIdSeller);
                var userIDReacter = arg3.UserId;
                var userReacter = await arg2.GetUserAsync(userIDReacter);
                if (userReacter.IsBot) return;
                if (userSeller.Id == userReacter.Id && arg3.Emote.Name == Dealdone.Name)
                {
                    await msg.DeleteAsync();
                    return;
                }
                Console.WriteLine("First if passed");
                if (arg3.Emote.Name == tick.Name)
                {
                    Console.WriteLine("Second if passed");
                    var DMCReacter = await userReacter.GetOrCreateDMChannelAsync();
                    var DMCSeller = await userSeller.GetOrCreateDMChannelAsync();
                    await DMCReacter.SendMessageAsync($"You have accepted a sale and a DM has been sent to {userSeller.Username}.\nYou can expect a reply shortly.");
                    await DMCSeller.SendMessageAsync($"{userReacter} has accepted your deal! Contact them for finalizing.");
                    await Task.Delay(5000);
                }
                else
                {
                    await msg.RemoveReactionAsync(arg3.Emote, userReacter);
                }
            }
        }

    }
}