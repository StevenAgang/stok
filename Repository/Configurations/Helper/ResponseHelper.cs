using stok.Repository.ViewModel;

namespace stok.Repository.Configurations.Helper
{
    public class ResponseHelper
    {
        public object Status(int status, bool success, string message, object content)
        {
            return new ResponseHelperViewModel
            {
                Status = status,
                Success = success,
                Message = message,
                Content = content
            };
        }
    }
}