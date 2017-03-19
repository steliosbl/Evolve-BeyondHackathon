namespace HaxLib.Models
{
    public sealed class PostRequestBody
    {
        public string lobbyID;
        public int? userID;
        public float? totalPayAmount;
        public float? userPayAmount;
        public string receiptUrl;
        public bool? verified;
        public bool? hostConfirmed;
        public int? merchantID;
    }
}
