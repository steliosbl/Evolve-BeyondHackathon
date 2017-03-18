namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;

    public interface IDatabase
    {
        User GetUser(int id);

        Lobby GetLobby(int id);

        List<User> GetLobbyMembers(int id);

        bool AddUser(User user);

        bool AddLobby(Lobby lobby);

        bool UpdateUser(User user);

        bool UpdateLobby(Lobby lobby);

        bool DeleteUser(User user);

        bool DeleteLobby(Lobby lobby);

        int CountUsers();

        int CountLobbies();
    }
}
