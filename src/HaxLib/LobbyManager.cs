namespace HaxLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;

    public sealed class LobbyManager : ILobbyManager
    {
        private Dictionary<string, Lobby> lobbies;
        private IDatabase database;

        public LobbyManager()
        {
            this.lobbies = new Dictionary<string, Lobby>();
            this.database = new Database();
        }

        public string CreateLobby(int hostID)
        {
            var host = this.database.Get(hostID);
            if (host != null && host.LobbyID == null)
            {
                string id = Guid.NewGuid().ToString().Substring(0,10);
                while (this.lobbies.ContainsKey(id))
                {
                    id = Guid.NewGuid().ToString().Substring(0, 10);
                }

                this.lobbies.Add(id, new Lobby(id, Constants.LobbyManager.LobbyStateDefault, host.ID, this.DeleteLobby));
                this.JoinLobby(id, host.ID);
                this.lobbies[id].BeginExpirationTimer(Constants.LobbyManager.LobbyLifetime);

                return id;
            }

            return null;
        }

        public bool DeleteLobby(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                var lobby = this.lobbies[id];
                var temp = lobby.Members.ToList();
                foreach (var member in temp)
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
            if (this.lobbies.ContainsKey(id) && user != null && user.LobbyID == null)
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
                return true;
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
                    member.SetPayAmount(amount);
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
                if (this.lobbies[id].HasMember(userID))
                {
                    this.lobbies[id].SetVerified(userID, verified);
                    return true;
                }
            }

            return false;
        }

        public bool SetHostConfirmed(string id, bool conf)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].SetConfirmed(conf);
                return true;
            }

            return false;
        }

        public bool SetMerchant(string id, int mid)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].SetMerchant(mid);
                return true;
            }

            return false;
        }

        public bool InitPayment(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                this.lobbies[id].InitPayment();
                return true;
            }

            return false;
        }

        public async Task<bool?> Pay(string id)
        {
            if (this.lobbies.ContainsKey(id))
            {
                Task<bool> task = Task.Run(() => this.ProcessPayment(id));
                bool res = await task;
                this.lobbies[id].BeginExpirationTimer(Constants.LobbyManager.LobbyLifetimeAfterPayment);
                return res;
            }
            else
            {
                return null;
            }
        }

        private bool ProcessPayment(string id)
        {
            var lobby = this.lobbies[id];
            if (lobby.AllVerified && lobby.HostConfirmed && lobby.Members.Sum(member => member.PayAmount) == lobby.TotalPayAmount)
            {
                foreach (var member in lobby.Members)
                {
                    member.SetBalance(member.Balance - (float)member.PayAmount);
                    this.database.Update(member);
                }

                return this.database.Pay((int)lobby.MerchantID, (float)lobby.TotalPayAmount);
            }

            return false;
        }
    }
}
