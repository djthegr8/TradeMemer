using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks; 
using Microsoft.Data.Sqlite;
namespace TradeMemer.modules
{
    class SqliteClass
    {
        /*public static SqliteCommand Connect()
        {
            
        }*/
        public static string fph = "Data Source = tmdb.db";
        public static async Task<string> Bla(ulong usrId, ulong GuildId, ulong ReporterId, DateTime timeZ, string reason = "not given")
        {
            var gu = Guid.NewGuid().ToString();
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT Count(*) FROM Crims WHERE UserId = {usrId};";
            var reedr = await cmd.ExecuteReaderAsync();
            await reedr.ReadAsync();
            var counter = reedr.GetInt16(0);
            Console.WriteLine(counter);
            await reedr.CloseAsync();
            if (counter < 3)
            {
                cmd.CommandText = $"SELECT GuildId FROM Crims WHERE UserId = {usrId}";
                var readz = await cmd.ExecuteReaderAsync();
                while (await readz.ReadAsync())
                {
                    if (ulong.Parse(readz.GetInt64(0).ToString()) == GuildId && ReporterId != 701029647760097361)
                    {
                        return "G";
                    }
                }
                await readz.CloseAsync();
                cmd.CommandText = $"INSERT INTO Crims(ReportId,UserId,GuildId,ReporterID,Time,Reason) VALUES(\"{gu}\",{usrId},{GuildId},{ReporterId},\"{timeZ:o}\",\"{reason}\");";
                Console.WriteLine(cmd);
                await cmd.ExecuteNonQueryAsync();
                await con.CloseAsync();
                return gu;
            }
            else
            {
                con.Close();
                return "E";
            }
            
        }
        public static async Task<Tuple<int,DateTimeOffset?>> CheckUsr(ulong userID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT Time,Count(*) FROM Crims WHERE UserId = {userID}";
            var read = await cmd.ExecuteReaderAsync();
            await read.ReadAsync();
            var nullOrNot = read.GetInt32(1) == 0;
            if (!nullOrNot)
            {
                var taim = read.GetDateTimeOffset(0);
                var count = read.GetInt16(1);
                await con.CloseAsync();
                return new Tuple<int, DateTimeOffset?>(count, taim);
            }
            else
            {
                await con.CloseAsync();
                return new Tuple<int, DateTimeOffset?>(0, null);
            }
            
        }
        public static async Task<Tuple<int, List<Guid>>> SpeedCheck (ulong userID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"select ReportID from Crims where UserId = {userID};";
            var read = await cmd.ExecuteReaderAsync();
            var guidlist = new List<Guid>();
            if (!read.HasRows)
            {
                await read.CloseAsync();
                await con.CloseAsync(); 
                return new Tuple<int, List<Guid>>(0, guidlist);
            }
            while (await read.ReadAsync())
            {
                guidlist.Add(read.GetGuid(0));
            }
            await read.CloseAsync();
            await con.CloseAsync();
            return new Tuple<int, List<Guid>>(guidlist.Count, guidlist);
        }
        public static async Task<Discord.EmbedBuilder> GuidInfoGetter(Guid repguid)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"select * from Crims where ReportID = \"{repguid}\";";
            var read = await cmd.ExecuteReaderAsync();
            if (!read.HasRows) return new Discord.EmbedBuilder { Title = "Could not find report with ID", Description = $"No report was found with the ID {repguid}" };
            await read.ReadAsync();
            var uzierID = read.GetInt64(1);
            var reporterID = read.GetInt64(3);
            var GuildID = read.GetInt64(2);
            var Time4Stuff = read.GetDateTime(4);
            var X = new Discord.EmbedBuilder
            {
                Title = $"Report `{repguid}`"
            };
            X.AddField(new Discord.EmbedFieldBuilder { Name = "Reported User Id:", Value = uzierID, IsInline=true});
            X.AddField(new Discord.EmbedFieldBuilder { Name = "Reported Server Id:", Value = GuildID, IsInline = true });
            X.AddField("Time", $"This report was done on {Time4Stuff.Day}-{Time4Stuff.Month}-{Time4Stuff.Year}");
            return X; 
        }
        public static async Task<string> PrefixGetter(ulong GuilID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"select Prefix from prefixes where guildid = {GuilID}";
            var read = await cmd.ExecuteReaderAsync();
            await read.ReadAsync();
            if (!read.HasRows) return "!";
            var pref = read.GetString(0);
            await read.CloseAsync();
            await con.CloseAsync();
            return pref;
        }
        public static async Task PrefixAdder(ulong GuLDID, string prefix)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"replace into prefixes (guildid,Prefix) values ({GuLDID},\"{prefix}\");";
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
            return;
        }
        public static async Task UserUnreporter(string repID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"delete from Crims where ReportID=\"{repID}\";";
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
            return;
        }
        public static async Task<bool> IsOnCooldown(ulong userID, ulong guildID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"SELECT EXISTS ( SELECT* FROM cooldown WHERE (UserId = {userID} AND GuildID = {guildID}));";
            var outp = await cmd.ExecuteReaderAsync();
            await outp.ReadAsync();
            short hasOrNot = outp.GetInt16(0);
            await outp.CloseAsync();
            await con.CloseAsync();
            return hasOrNot == 1;
        }
        public static async Task AddCooldown(ulong userID, ulong guildID)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"insert into cooldown(UserId,GuildID) values({userID},{guildID});";
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = $"select Cooldown FROM prefixes where guildid = {guildID};";
            var reedr = await cmd.ExecuteReaderAsync();
            await reedr.ReadAsync();
            var cdmins = reedr.GetInt16(0);
            await reedr.CloseAsync();
            await Task.Delay(cdmins * 60000);
            cmd.CommandText = $"delete from cooldown where(UserId={userID} AND GuildID={guildID});";
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
            return;
        }
        public static async Task EditCoolDOWN(ulong guildID, int mins)
        {
            using var con = new SqliteConnection(fph);
            await con.OpenAsync();
            using var cmd = new SqliteCommand();
            cmd.Connection = con;
            cmd.CommandText = $"update prefixes set Cooldown = {mins} where guildid = {guildID};";
            await cmd.ExecuteNonQueryAsync();
            cmd.CommandText = $"delete from cooldown where GuildID={guildID}";
            await cmd.ExecuteNonQueryAsync();
            await con.CloseAsync();
            return;
        }
    }
}
