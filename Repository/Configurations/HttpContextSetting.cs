namespace stok.Repository.Configurations
{
    public class HttpContextSetting
    {
        public bool InDevelopment { get; set; }
        public bool IsHttpOnly { get; set; }
        public bool IsSecure { get; set; }
        public string? SameSite { get; set; }
        public int ExpireInMinutes { get; set; }
    }

    public class HttpContextRefreshTokenSettings
    {
        public bool InDevelopment { get; set; }
        public bool IsHttpOnly { get; set; }
        public bool IsSecure { get; set; }
        public string? SameSite { get; set; }
        public int ExpireInDays { get; set; }
    }
}
