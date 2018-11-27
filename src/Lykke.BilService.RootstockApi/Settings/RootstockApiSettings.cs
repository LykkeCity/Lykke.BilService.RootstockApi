using JetBrains.Annotations;
using Lykke.Quintessence.Settings;

namespace Lykke.BilService.RootstockApi.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RootstockApiSettings : ApiSettings
    {
        public bool IsMainNet { get; set; }
    }
}