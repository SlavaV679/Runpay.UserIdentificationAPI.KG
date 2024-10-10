namespace Runpay.UserIdentification.Domain.Models
{
    public class ResponseToClient
    {
        public string RequestId { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorText { get; set; }
    }
}
