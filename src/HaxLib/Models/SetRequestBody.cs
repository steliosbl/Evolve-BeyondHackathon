namespace HaxLib.Models
{
    public sealed class SetRequestBody
    {
        public string lobbyID;
        public int? userID;
        public float? totalPayAmount;
        public float? userPayAmount;
        public string receiptUrl;
        public bool? verified;
        public bool? hostConfirmed;
    }
}
