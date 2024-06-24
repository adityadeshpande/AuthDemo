namespace AuthDemo.Dtos.Response
{
    public static partial class TokenEndpoint
    {
        public class TokenReponse
        {
            public string access_token { get; set; }
            public int expiration { get; set; }
            public string type { get; set; }
        }

    }
}
