namespace PlenkaAPI.Data
{
    public static class DbContextSingleton
    {
        private static MembraneContext _instance;

        private static readonly object SyncRoot = new();


        public static MembraneContext GetInstance()
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new MembraneContext();
                    }
                }
            }

            return _instance;
        }
    }
}