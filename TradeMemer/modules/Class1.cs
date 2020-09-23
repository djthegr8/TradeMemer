using static Public_Bot.CustomCommandService;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMemer.modules;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using System.IO;
using Public_Bot;
using GuildPermissions = Public_Bot.GuildPermissions;
using System.Security.Cryptography.X509Certificates;

namespace TradeMemer.modules
{
    [DiscordCommandClass("Role Editor","Class for editing of Roles")]
    public class RoleEditor: CommandModuleBase
    {
        [GuildPermissions(GuildPermission.ManageRoles)]
        [Alt("del")]
        [DiscordCommand("delete",commandHelp ="delete <@role/id>", description ="Deletes the mentioned role",example ="delete @DumbRole")]
        public async Task DelRole(params string[] args)
        {
            SocketRole DeleteRole;
            if (args.Length == 0)
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "What role?",
                    Description = "Mention the role you wish to delete",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            DeleteRole = GetRole(args[0]);
            if (DeleteRole == null)
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "What role?",
                    Description = "Mention the role you wish to delete",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            if (Context.Guild.CurrentUser.Roles.All(idk => idk.CompareTo(DeleteRole) < 0))
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "Hey, thats above me",
                    Description = $"The bot's highest role => {Context.Guild.CurrentUser.Roles.Max().Name}\nThe role you wish to delete => {DeleteRole.Name}",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            if (!(Context.User as SocketGuildUser).Roles.Any(rl => rl.Position > DeleteRole.Position) && Context.Guild.OwnerId != Context.User.Id)
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "Not gonna happen kid",
                    Description = "You're below the role you want to delete!",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            else
            {
                var nm = DeleteRole.Name;
                await DeleteRole.DeleteAsync();
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = $"Role deleted successfully!",
                    Description = $"The role `{nm}` was successfully deleted",
                    Color = Blurple
                }.WithCurrentTimestamp().Build());
            }
        }

        /*[DiscordCommand("create",description = "New role creation wizard",example ="create",commandHelp ="create")]
        [GuildPermissions(GuildPermission.ManageRoles)]
        public async Task CreateRole(params string[] args)
        {
            await ReplyAsync("Alright, lets make you a role! What should it be called?");
            await 
        }*/
        [GuildPermissions(GuildPermission.ManageRoles)]
        [DiscordCommand("addperms",commandHelp ="addperms <@role/id> <Permission>",description ="Adds the given permission to the requested role")]
        public async Task AddPerms(params string[] args)
        {
            switch (args.Length)
            {
                case 0 or 1:
                    await ReplyAsync("", false, new EmbedBuilder
                    {
                        Title = "U need to give the Role and Permission",
                        Description = $"The way to use the command is \n{await SqliteClass.PrefixGetter(Context.Guild.Id)}addperms <@role/id> <Permission>`",
                        Color = Color.Red
                    }.WithCurrentTimestamp().Build());
                    return;
            }
            var roleA = GetRole(args[0]);
            if (roleA == null)
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "That role is invalid",
                    Description = $"The way to use the command is \n{await SqliteClass.PrefixGetter(Context.Guild.Id)}addperms <@role/id> <Permission>`",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            var gp = GetPermission(args[1]);
            if (gp.Item2 == false)
            {
                await ReplyAsync("", false, new EmbedBuilder
                {
                    Title = "That permission is invalid",
                    Description = $"The list of permissions is ~ ```{string.Join('\n', Enum.GetNames(typeof(GuildPermission)))}```",
                    Color = Color.Red
                }.WithCurrentTimestamp().Build());
                return;
            }
            switch (gp.Item1)
            {
                case GuildPermission.AddReactions:
                    roleA.Permissions.Modify(addReactions: true);
                    break;
                case GuildPermission.Administrator:
                    roleA.Permissions.Modify(administrator: true);
                    break;
                case GuildPermission.AttachFiles:
                    roleA.Permissions.Modify(attachFiles: true);
                    break;
                case GuildPermission.BanMembers:
                    roleA.Permissions.Modify(banMembers: true);
                    break;
                case GuildPermission.ChangeNickname:
                    roleA.Permissions.Modify(changeNickname: true);
                    break;
                case GuildPermission.Connect:
                    roleA.Permissions.Modify(connect: true);
                    break;
                case GuildPermission.CreateInstantInvite:
                    roleA.Permissions.Modify(createInstantInvite: true);
                    break;
                case GuildPermission.DeafenMembers:
                    roleA.Permissions.Modify(deafenMembers: true);
                    break;
                case GuildPermission.EmbedLinks:
                    roleA.Permissions.Modify(embedLinks: true);
                    break;
                case GuildPermission.KickMembers:
                    roleA.Permissions.Modify(kickMembers: true);
                    break;
                case GuildPermission.ManageChannels:
                    roleA.Permissions.Modify(manageChannels: true);
                    break;
                case GuildPermission.ManageEmojis:
                    roleA.Permissions.Modify(manageEmojis: true);
                    break;
                case GuildPermission.ManageGuild:
                    roleA.Permissions.Modify(manageGuild: true);
                    break;
                case GuildPermission.ManageMessages:
                    roleA.Permissions.Modify(manageMessages: true);
                    break;
                case GuildPermission.ManageNicknames:
                    roleA.Permissions.Modify(manageNicknames: true);
                    break;
                case GuildPermission.ManageRoles:
                    roleA.Permissions.Modify(manageRoles: true);
                    break;
                case GuildPermission.ManageWebhooks:
                    roleA.Permissions.Modify(manageWebhooks: true);
                    break;
                case GuildPermission.MentionEveryone:
                    roleA.Permissions.Modify(mentionEveryone: true);
                    break;
                case GuildPermission.MoveMembers:
                    roleA.Permissions.Modify(moveMembers: true);
                    break;
                case GuildPermission.MuteMembers:
                    roleA.Permissions.Modify(muteMembers: true);
                    break;
                case GuildPermission.PrioritySpeaker:
                    roleA.Permissions.Modify(prioritySpeaker: true);
                    break;
                case GuildPermission.ReadMessageHistory:
                    roleA.Permissions.Modify(readMessageHistory: true);
                    break;
                case GuildPermission.ReadMessages or GuildPermission.ViewChannel:
                    roleA.Permissions.Modify(viewChannel: true);
                    break;
                case GuildPermission.SendMessages:
                    roleA.Permissions.Modify(sendMessages: true);
                    break;
                case GuildPermission.SendTTSMessages:
                    roleA.Permissions.Modify(sendTTSMessages: true);
                    break;
                case GuildPermission.Speak:
                    roleA.Permissions.Modify(speak: true);
                    break;
                case GuildPermission.Stream:
                    roleA.Permissions.Modify(stream: true);
                    break;
                case GuildPermission.UseExternalEmojis:
                    roleA.Permissions.Modify(useExternalEmojis: true);
                    break;
                case GuildPermission.UseVAD:
                    roleA.Permissions.Modify(useVoiceActivation: true);
                    break;
                case GuildPermission.ViewAuditLog:
                    roleA.Permissions.Modify(viewAuditLog: true);
                    break;
            }
            string perms = "```\n";
            string permsRight = "";
            var props = typeof(Discord.GuildPermissions).GetProperties();
            var boolProps = props.Where(x => x.PropertyType == typeof(bool));
            var pTypes = boolProps.Where(x => (bool)x.GetValue(roleA.Permissions) == true).ToList();
            var nTypes = boolProps.Where(x => (bool)x.GetValue(roleA.Permissions) == false).ToList();
            var pd = boolProps.Max(x => x.Name.Length) + 1;
            if (nTypes.Count == 0)
                perms += "Administrator: ✅```";
            else
            {
                foreach (var perm in pTypes)
                    perms += $"{perm.Name}:".PadRight(pd) + " ✅\n";
                perms += "```";
                permsRight = "```\n";
                foreach (var nperm in nTypes)
                    permsRight += $"{nperm.Name}:".PadRight(pd) + " ❌\n";
                permsRight += "```";
            }
            await ReplyAsync("", false, new EmbedBuilder
            {
                Title = $"Permission {args[1]} Added to Role {roleA.Name}!",
                Description = $"**{roleA.Name}'s permissions**\n{perms}\n**",
                Color = Color.Red
            }.WithCurrentTimestamp().Build());
            return;
        }
    }
}