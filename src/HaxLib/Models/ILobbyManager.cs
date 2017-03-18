namespace HaxLib.Models
{
    using System;

    public interface ILobbyManager
    {
        string CreateLobby(int hostid);

        bool DeleteLobby(string id);

        bool JoinLobby(string id, int userid);

        bool LeaveLobby(string id, int userid);

        Lobby GetLobby(string id);

        bool SetTotalPayAmount(string id, float amount);

        bool SetUserPayAmount(string id, int userid, float amount);

        bool SetReceiptUrl(string id, string url);

        bool SetUserVerified(string id, int userid, bool verified);

        bool BeginPayment(string id);
    }
}
