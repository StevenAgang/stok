namespace stok.Repository.Configurations
{
    public class BadRequest(string? message = null, Exception? innerException = null) : Exception(message, innerException){}
}
