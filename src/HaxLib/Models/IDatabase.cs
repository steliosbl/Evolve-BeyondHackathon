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

        bool DeleteUser(int id);

        bool DeleteLobby(int id);

        int CountUsers();

        int CountLobbies();
    }
}
