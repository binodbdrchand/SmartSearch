namespace SmartSearch.Core.Domain.Exceptions
{
    public class RecordNotFoundException : Exception
    {
        public RecordNotFoundException() { }
        public RecordNotFoundException(string message) : base(message) { }
        public RecordNotFoundException(Exception ex) : base(ex.Message) { }
    }
}
