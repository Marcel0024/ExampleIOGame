namespace IOGameServer.Application.Helpers
{
    public static class IdFactory
    {
        private const string _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_+-=\\|[];',./{}:\"<>?";
        private const int _idLength = 2;

        public static string GenerateUniqueId()
        {
            return GenerateRandomUniqueId().ToString();
        }

        private static ReadOnlySpan<char> GenerateRandomUniqueId()
        {
            return string.Create(_idLength, Random.Shared, (newId, random) =>
            {
                for (int i = 0; i < newId.Length; i++)
                {
                    var randomNumber = random.Next(_characters.Length);

                    newId[i] = _characters[randomNumber];
                }
            }).AsSpan();
        }
    }
}
