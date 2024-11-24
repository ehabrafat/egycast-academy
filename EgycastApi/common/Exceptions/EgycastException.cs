namespace EgycastApi;

public class EgycastException : Exception
{
    public int Status;
    
    public EgycastException(string msg, int status) : base(msg)
    {
        Status = status;
    }
}