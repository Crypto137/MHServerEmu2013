namespace MHServerEmu
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = $"MHServerEmu2013 ({ServerApp.VersionInfo})";
            ServerApp.Instance.Run();
        }
    }
}
