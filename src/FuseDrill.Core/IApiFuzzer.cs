namespace FuseDrill.Core;

public interface IApiFuzzer
{
    Task<FuzzerTests> TestWholeApi(Func<ApiCall, bool> filter = null);
}