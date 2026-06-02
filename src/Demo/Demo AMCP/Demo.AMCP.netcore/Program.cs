namespace StarDust.Demo.AMCP.netcore;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await Executor.RunAsync(args);
    }
}
