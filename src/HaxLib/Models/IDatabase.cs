namespace HaxLib.Models
{
    using System;
    using System.Collections.Generic;

    public interface IDatabase
    {
        User Get(int id);

        User Get(string name);

        List<User> GetLobbyMembers(int id);

        bool Add(User user);

        bool Update(User user);

        bool Delete(int id);

        int Count();

        bool Pay(int id, float amount);
    }
}
