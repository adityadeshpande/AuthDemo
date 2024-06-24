namespace AuthDemo.Dtos.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SigningKey { get; set; }
        public string PrivateKey { get; set; }
        public int ExpirationSeconds { get; set; }
    }
}
