namespace HaxLib.Models
{
    public sealed class User
    {
        public User(int id, string name, string lobbyid) : this(id, name, false, lobbyid, null)
        {
        }

        public User(int id, string name, bool verified, string lobbyid, float? payamount)
        {
            this.ID = id;
            this.Name = name;
            this.Verified = verified;
            this.LobbyID = lobbyid;
            this.PayAmount = payamount;
        }

        public int ID { get; private set; }

        public string Name { get; private set; }

        public bool Verified { get; private set; }

        public string LobbyID { get; private set; }

        public float? PayAmount { get; private set; }

        public void Reset()
        {
            this.PayAmount = null;
            this.LobbyID = null;
            this.Verified = false;
        }

        public void SetLobby(string id)
        {
            this.LobbyID = id;
        }

        public void SetPayAmount(float amount)
        {
            this.PayAmount = amount;
        }

        public void SetVerified(bool verified)
        {
            this.Verified = verified;
        }
    }
}
