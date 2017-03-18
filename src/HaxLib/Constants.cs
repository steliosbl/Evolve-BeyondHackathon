namespace HaxLib
{
    public static class Constants
    {
        public static class Database
        {
            public const string ConfigFilename = "DB.cfg";
            public const string UserTableName = "users";
            public const string LobbyTableName = "lobbies";
        }

        public static class LobbyManager
        {
            public const int LobbyLifetime = 1800000;
            public const string LobbyStateDefault = "waiting_host";
        }
    }
}
