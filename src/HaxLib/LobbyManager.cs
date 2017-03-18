namespace HaxLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;

    public sealed class LobbyManager : ILobbyManager
    {
        private Dictionary<string, Lobby> lobbies;
        private IDatabase database;

        public LobbyManager(IDatabase database)
        {
            this.lobbies = new Dictionary<string, Lobby>();
            this.database = database;
        }

        public string CreateLobby(int hostID)
        {
            var host = this.database.Get(hostID);
            if (host != null && host.LobbyID == null)
            {
                string id = Guid.NewGuid().ToString();
                while (this.lobbies.ContainsKey(id))
                {
                    id = Guid.NewGuid().ToString();
                }

                this.lobbies.Add(id, new Lobby(id, Constants.LobbyManager.LobbyStateDefault, host.ID));
                this.JoinLobby(id, host.ID);
                this.lobbies[id].BeginTimer(this.DeleteLobby);

                return id;
            }

            return null;
        }

        public bool DeleteLobby(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                var lobby = this.lobbies[id];
                foreach (var member in lobby.Members)
                {
                    this.LeaveLobby(id, member.ID);
                }

                this.lobbies.Remove(id);
                return true;
            }

            return false;
        }

        public bool JoinLobby(string id, int userID)
        {
            var user = this.database.Get(userID);
            if (this.lobbies.ContainsKey(id) && user != null)
            {
                this.lobbies[id].AddMember(user);
                user.SetLobby(id);
                this.database.Update(user);
                return true;
            }

            return false;
        }

        public bool LeaveLobby(string id, int userID)
        {
            if (this.lobbies.ContainsKey(id) && this.lobbies[id].HasMember(userID))
            {
                this.lobbies[id].RemoveMember(userID);
                var user = this.database.Get(userID);
                user.Reset();
                this.database.Update(user);
                return true;
            }

            return false;
        }

        public Lobby GetLobby(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                return this.lobbies[id];
            }

            return null;
        }

        public bool SetTotalPayAmount(string id, float amount)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].SetTotalAmount(amount);
            }

            return false;
        }

        public bool SetUserPayAmount(string id, int userID, float amount)
        {
            if (this.lobbies.ContainsKey(id))
            {
                var member = this.lobbies[id].GetMember(userID);
                if (member != null)
                {
                    member.SetPayAmount(userID);
                    this.lobbies[id].UpdateMember(member);
                    return true;
                }
            }

            return false;
        }

        public bool SetReceiptUrl(string id, string url)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].SetReceiptUrl(url);
                return true;
            }

            return false;
        }

        public bool SetUserVerified(string id, int userID, bool verified)
        {
            if (this.lobbies.ContainsKey(id))
            {
                var member = this.lobbies[id].GetMember(userID);
                if (member != null)
                {
                    member.SetVerified(verified);
                    this.lobbies[id].UpdateMember(member);
                    return true;
                }
            }

            return false;
        }

        public bool BeginPayment(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].BeginPayment();
                return true;
            }

            return false;
        }
    }
}
