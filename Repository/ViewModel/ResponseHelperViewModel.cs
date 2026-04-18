using System.Text.Json.Serialization;

namespace stok.Repository.ViewModel
{
    public class ResponseHelperViewModel
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Content { get; set; }
    }
}
