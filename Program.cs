// Program.cs
namespace Schedlify_tg
{
    public class Program
    {
        public static async Task Main()
        {
            Bot bot = new Bot();
            await bot.StartReceiving();
        }
    }
}