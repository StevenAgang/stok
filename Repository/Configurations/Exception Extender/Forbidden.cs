namespace stok.Repository.Configurations.Exception_Extender
{
    public class Forbidden(string? message = null, Exception? innerException = null) : Exception(message,innerException)
    {
    }
}
