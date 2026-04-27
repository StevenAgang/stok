using stok.Repository.Configurations.Exception_Extender;

namespace stok.Repository.Configurations.Validation
{
    public static class RefreshTokenValidation
    {
        public static void ValidateRefreshToken(string refreshToken)
        {
                if (string.IsNullOrEmpty(refreshToken))
                {
                    throw new Forbidden("Forbidden request");
                }
        }
    }
}
