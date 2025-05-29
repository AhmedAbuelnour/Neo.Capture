namespace Neo.Capture.Domain.Operation
{
    public class EndpointResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class EndpointResult<TValue> : EndpointResult
    {
        public TValue Value { get; set; }
    }
}
