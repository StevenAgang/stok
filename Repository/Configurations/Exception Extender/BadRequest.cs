namespace stok.Repository.Configurations
{
    public class BadRequest(string message, Exception? innerException = null) : Exception(message, innerException){}
}
