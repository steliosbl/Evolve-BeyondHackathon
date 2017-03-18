namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;

    public sealed class Lobby
    {
        private Timer expirationTimer;

        public Lobby(string id, string state, int hostid, LobbyExpiredHandler expirationHandler) : this(id, state, hostid, null, null, new List<User>(), false, false, expirationHandler)
        {
        }

        public Lobby(string id, string state, int hostid, float? totalpayamount, string receipturl, List<User> members, bool hostconfirmed, bool paymentcomplete, LobbyExpiredHandler expirationHandler)
        {
            this.ID = id;
            this.State = state;
            this.HostID = hostid;
            this.TotalPayAmount = totalpayamount;
            this.ReceiptUrl = receipturl;
            this.Members = members;
            this.HostConfirmed = hostconfirmed;
            this.PaymentComplete = paymentcomplete;
            this.LobbyExpired += expirationHandler;
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

        public bool PaymentComplete { get; private set; }

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
            this.UpdateState();
        }

        public void RemoveMember(int id)
        {
            this.Members.RemoveAll(member => member.ID == id);
            this.UpdateState();
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
            this.Members.First(member => member.ID == id).SetVerified(verified);
            this.UpdateState();
        }

        public void UpdateMember(User member)
        {
            this.Members.RemoveAll(m => m.ID == member.ID);
            this.Members.Add(member);
            this.UpdateState();
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

        public async void BeginPayment()
        {
            this.State = "paying";
            Task<bool> payTask = Pay();
            this.PaymentComplete = await payTask;
            this.expirationTimer.Stop();
            this.BeginExpirationTimer(Constants.LobbyManager.LobbyLifetimeAfterPayment);
        }

        public void BeginExpirationTimer(int duration)
        {
            this.expirationTimer = new Timer((double)duration);
            this.expirationTimer.AutoReset = false;
            this.expirationTimer.Elapsed += (s, e) => { LobbyExpired(this.ID); };
            this.expirationTimer.Start();
        }

        public void SetConfirmed(bool conf)
        {
            this.HostConfirmed = conf;
            this.UpdateState();
        }

        private void UpdateState()
        {
            if (!this.HostConfirmed)
            {
                this.State = "waiting_host";
            }
            else if (!this.AllVerified)
            {
                this.State = "waiting_users_lock";
            }
            else
            {
                this.State = "ready";
            }
        }

        private async Task<bool> Pay()
        {
            Task<bool> task = Task.Run(() => this.ProcessPayment());
            bool res = await task;
            return res;
        }

        private bool ProcessPayment()
        {
            return true;
        }
    }
}
