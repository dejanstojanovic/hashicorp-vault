namespace Vault.Sample.Extensions
{
    public class VaultConfiguration
    {
        public Uri Url { get; set; }
        public string Backend { get; set; }
        public string Secret { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
