namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;

    public interface IDatabase
    {
        User GetUser(int id);
        Lobby GetLobby(int id);
        List<User> GetLobbyMembers(int id);
    }
}
