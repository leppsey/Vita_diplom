namespace Isomerization.Domain.Data
{
    public static class DbContextSingleton
    {
        private static IsomerizationContext _instance;

        private static readonly object SyncRoot = new();


        public static IsomerizationContext GetInstance()
        {
            if (_instance == null)
            {
                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new IsomerizationContext();
                    }
                }
            }

            return _instance;
        }
    }
}