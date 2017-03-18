namespace HaxLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Models;
    using MySql.Data.MySqlClient;
    using SDatabase.MySQL;

    public sealed class Database : IDatabase
    {
        private ConnectionString connectionString;
        private ConnectionData connectionData;
        private MySqlConnection connection;

        public Database()
        {
            if (!System.IO.File.Exists(Constants.Database.ConfigFilename))
            {
                throw new Exception("Configuration file not found." + System.IO.Directory.GetCurrentDirectory());
            }

            this.connectionData = JFI.GetObject<ConnectionData>(Constants.Database.ConfigFilename);
            this.connectionString = new ConnectionString(this.connectionData);
            this.connection = new MySqlConnection(this.connectionString.Text);
            this.connection.Open();
        }

        public User Get(int id)
        {
            User res = null;
            if (this.UserExists(id))
            {
                using (var cmd = new MySqlCommand(string.Format("SELECT * FROM {0} WHERE id=@id", Constants.Database.UserTableName), this.connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            res = new User(reader.GetInt32(0), reader.GetString(1), reader.GetStringNullCheck(2), reader.GetInt32(3));
                        }
                    }
                }
            }

            return res;
        }

        public User Get(string name)
        {
            using (var cmd = new MySqlCommand(string.Format("SELECT id FROM {0} WHERE name=@name", Constants.Database.UserTableName), this.connection))
            {
                return this.Get(System.Convert.ToInt32(cmd.ExecuteScalar()));
            }
        }

        ////public Lobby GetLobby(int id)
        ////{
        ////    Lobby res = null;
        ////    if (this.LobbyExists(id))
        ////    {
        ////        using (var cmd = new MySqlCommand(string.Format("SELECT * FROM {0} WHERE id=@id", Constants.Database.LobbyTableName), this.connection))
        ////        {
        ////            cmd.Prepare();
        ////            cmd.Parameters.AddWithValue("@id", id);
        ////            using (var reader = cmd.ExecuteReader())
        ////            {
        ////                while (reader.Read())
        ////                {
        ////                    res = new Lobby(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetFloatNullCheck(3), reader.GetStringNullCheck(4), this.GetLobbyMembers(id));
        ////                }
        ////            }
        ////        }
        ////    }

        ////    return res;
        ////}

        public List<User> GetLobbyMembers(int id)
        {
            var res = new List<User>();
            using (var cmd = new MySqlCommand(string.Format("SELECT id FROM {0} WHERE lobbyid=@lobbyid", Constants.Database.UserTableName), this.connection))
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@lobbyid", id);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        res.Add(this.Get(reader.GetInt32(0)));
                    }
                }
            }

            return res;
        }

        public bool Add(User user)
        {
            if (!this.UserExists(user.ID))
            {
                using (var cmd = new MySqlCommand(string.Format("INSERT INTO {0} VALUES (@id, @name, @lobbyid, @balance);", Constants.Database.UserTableName), this.connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", user.ID);
                    cmd.Parameters.AddWithValue("@name", user.Name);
                    cmd.Parameters.AddWithValue("@lobbyid", user.LobbyID);
                    cmd.Parameters.AddWithValue("@balance", user.Balance);
                    cmd.ExecuteNonQuery();
                }

                    return true;
            }

            return false;
        }

        ////public bool AddLobby(Lobby lobby)
        ////{
        ////    if (!this.LobbyExists(lobby.ID))
        ////    {
        ////        using (var cmd = new MySqlCommand(string.Format("INSERT INTO {0} VALUES (@id, @hostid, @state, @totalpayamount, @receipturl", Constants.Database.LobbyTableName), this.connection))
        ////        {
        ////            cmd.Prepare();
        ////            cmd.Parameters.AddWithValue("@id", lobby.ID);
        ////            cmd.Parameters.AddWithValue("@hostid", lobby.HostID);
        ////            cmd.Parameters.AddWithValue("@state", lobby.State);
        ////            cmd.Parameters.AddWithValue("@totalpayamount", lobby.TotalPayAmount);
        ////            cmd.Parameters.AddWithValue("@receipturl", lobby.ReceiptUrl);
        ////            cmd.ExecuteNonQuery();
        ////        }
        ////        return true;
        ////    }

        ////    return false;
        ////}

        public bool Update(User user)
        {
            if (this.UserExists(user.ID))
            {
                this.Delete(user.ID);
                this.Add(user);
                return true;
            }

            return false;
        }

        ////public bool UpdateLobby(Lobby lobby)
        ////{
        ////    if (this.LobbyExists(lobby.ID))
        ////    {
        ////        this.DeleteLobby(lobby.ID);
        ////        this.AddLobby(lobby);
        ////        return true;
        ////    }

        ////    return false;
        ////}

        public bool Delete(int id)
        {
            if (this.UserExists(id))
            {
                using (var cmd = new MySqlCommand(string.Format("DELETE FROM {0} WHERE id=@id", Constants.Database.UserTableName), this.connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        ////public bool DeleteLobby(int id)
        ////{
        ////    if (this.LobbyExists(id))
        ////    {
        ////        var members = this.GetLobbyMembers(id);
        ////        foreach (var member in members)
        ////        {
        ////            member.Reset();
        ////            this.UpdateUser(member);
        ////        }

        ////        using (var cmd = new MySqlCommand(string.Format("DELETE FROM {0} WHERE id=@id", Constants.Database.LobbyTableName), this.connection))
        ////        {
        ////            cmd.ExecuteNonQuery();
        ////        }

        ////        return true;
        ////    }

        ////    return false;
        ////}

        public int Count()
        {
            return this.CountElements(Constants.Database.UserTableName);
        }

        ////public int CountLobbies()
        ////{
        ////    return this.Count(Constants.Database.LobbyTableName);
        ////}

        public bool Pay(int id, float amount)
        {
            if (this.MerchantExists(id))
            {
                using (var cmd = new MySqlCommand(string.Format("UPDATE {0} SET balance=@balance WHERE id=@id", Constants.Database.MerchantTableName), this.connection))
                {
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@balance", Math.Abs(amount));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }

            return false;
        }

        private int CountElements(string tableName)
        {
            using (var cmd = new MySqlCommand(string.Format("SELECT COUNT(*) FROM {0}", tableName), this.connection))
            {
                return System.Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private bool UserExists(int id)
        {
            return this.Exists(id, Constants.Database.UserTableName);
        }

        private bool MerchantExists(int id)
        {
            return this.Exists(id, Constants.Database.MerchantTableName);
        }

        ////private bool LobbyExists(int id)
        ////{
        ////    return this.Exists(id, Constants.Database.LobbyTableName);
        ////}

        private bool Exists(int id, string tableName)
        {
            using (var cmd = new MySqlCommand(string.Format("SELECT EXISTS(SELECT * FROM {0} WHERE id=@id);", tableName), this.connection))
            {
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@id", id);
                return System.Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }
    }
}
