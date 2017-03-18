namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class Lobby
    {
        public Lobby(int id, string state, int hostid) : this(id, state, hostid, null, null, new List<User>())
        {
        }

        public Lobby(int id, string state, int hostid, decimal? totalpayamount, string receipturl, List<User> members)
        {
            this.ID = id;
            this.State = state;
            this.HostID = hostid;
            this.TotalPayAmount = totalpayamount;
            this.ReceiptUrl = receipturl;
            this.Members = members;
        }

        public int ID { get; private set; }

        public string State { get; private set; }

        public int HostID { get; private set; }

        public decimal? TotalPayAmount { get; private set; }

        public string ReceiptUrl { get; private set; }

        public List<User> Members { get; private set; }
    }
}
