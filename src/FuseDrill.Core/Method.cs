using OneOf;


namespace FuseDrill.Core;

public class Method
{
    public required string MethodName { get; set; }
    public string HttpMethod { get; set; }
    public required List<string> MethodParameterTypeNames { get; set; }
    public List<OneOf<Type, VoidEmptyType>> MethodParameterTypes { get; set; }
}
