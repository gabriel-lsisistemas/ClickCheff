namespace ApiClickCheff.Model
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; internal set; }
    }
}
