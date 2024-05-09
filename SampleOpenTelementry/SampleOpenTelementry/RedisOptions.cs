namespace SampleOpenTelementry
{
    public class RedisOptions
    {
        public bool Ssl { get; set; }

        public bool AllowAdmin { get; set; }

        public string? Password { get; set; }

        public int ConnectRetry { get; set; }

        public int ConnectTimeout { get; set; }

        public int DefaultDatabase { get; set; }

        public required List<string> EndPoints { get; set; }
    }
}
