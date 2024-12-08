namespace FuseDrill.Core;

public class ApiCall
{
    public required string MethodName { get; set; }
    public int Order { get; set; } // From 1 to MethodName count;
    public required object Request { get; set; }
    public required object Response { get; set; }
    public string HttpMethod { get; internal set; }
}
