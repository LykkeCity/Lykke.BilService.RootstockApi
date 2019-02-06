using System.Threading.Tasks;
using Lykke.BilService.RootstockApi.Settings;
using Lykke.Quintessence;

namespace Lykke.BilService.RootstockApi
{
    internal static class Program
    {
        #if DEBUG
        
        private const bool IsDebug = true;
        
        #else

        private const bool IsDebug = false;

        #endif
        
        private static Task Main()
        {
            return ApiStarter
                .StartAsync<Startup, RootstockApiSettings>(IsDebug);
        }
    }
}