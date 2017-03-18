namespace HaxLib.Models
{
    using System;

    public interface ILobbyManager
    {
        bool CreateLobby(int hostID);

        bool DeleteLobby(string ID);

        bool JoinLobby(string ID, int userID);

        bool LeaveLobby(string ID, int userID);

        Lobby GetLobby(string ID);

        bool SetTotalPayAmount(string ID, float amount);

        bool SetUserPayAmount(string id, int userID, float amount);

        bool SetReceiptUrl(string id, string ID);

        bool SetUserVerified(string id, int userID, bool verified);

        bool BeginPayment(string id);
    }
}
