using JetBrains.Annotations;
using Lykke.Quintessence.Domain.Services;

namespace Lykke.BilService.RootstockApi.Domain.Services
{
    [UsedImplicitly]
    public class RootstockAssetService : DefaultAssetServiceBase
    {
        public RootstockAssetService() : base(18, "", "RBTC", "RBTC")
        {
            
        }
    }
}