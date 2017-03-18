namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;

    public sealed class Lobby
    {
        private Timer timer;

        public Lobby(string id, string state, int hostid) : this(id, state, hostid, null, null, new List<User>(), false)
        {
        }

        public Lobby(string id, string state, int hostid, float? totalpayamount, string receipturl, List<User> members, bool hostconfirmed)
        {
            this.ID = id;
            this.State = state;
            this.HostID = hostid;
            this.TotalPayAmount = totalpayamount;
            this.ReceiptUrl = receipturl;
            this.Members = members;
            this.HostConfirmed = hostconfirmed;
        }

        public delegate bool LobbyExpiredHandler(string id);

        public event LobbyExpiredHandler LobbyExpired;

        public string ID { get; private set; }

        public string State { get; private set; }

        public int HostID { get; private set; }

        public float? TotalPayAmount { get; private set; }

        public string ReceiptUrl { get; private set; }

        public List<User> Members { get; private set; }

        public bool HostConfirmed { get; private set; }

        public bool AllVerified
        {
            get
            {
                return this.Members.Count(member => !member.Verified) == 0;
            }
        }

        public void AddMember(User member)
        {
            this.Members.Add(member);
        }

        public void RemoveMember(int id)
        {
            this.Members.RemoveAll(member => member.ID == id);
        }

        public void SetReceiptUrl(string url)
        {
            this.ReceiptUrl = url;
        }

        public void SetTotalAmount(float amount)
        {
            this.TotalPayAmount = amount;
        }

        public void SetVerified(int id, bool verified)
        {
            this.Members[id].SetVerified(verified);
        }

        public void UpdateMember(User member)
        {
            this.Members.RemoveAll(m => m.ID == member.ID);
            this.Members.Add(member);
        }

        public User GetMember(int id)
        {
            if (this.HasMember(id))
            {
                return this.Members.First(member => member.ID == id); 
            }

            return null;
        }

        public bool HasMember(int id)
        {
            return this.Members.Count(member => member.ID == id) == 1;
        }

        public void BeginPayment()
        {
            this.State = "paying";
        }

        public void BeginTimer(LobbyExpiredHandler handler)
        {
            this.LobbyExpired += handler;
            this.timer = new Timer((double)Constants.LobbyManager.LobbyLifetime);
            this.timer.AutoReset = false;
            this.timer.Elapsed += (s, e) => { LobbyExpired(this.ID); };
            this.timer.Start();
        }

        public void SetConfirmed(bool conf)
        {
            this.HostConfirmed = conf;
        }

        private void UpdateState()
        {
            if (this.State == Constants.LobbyManager.LobbyStateDefault)
            {

            }
        }
    }
}
