using JetBrains.Annotations;
using Lykke.BilService.RootstockApi.Modules;
using Lykke.BilService.RootstockApi.Settings;
using Lykke.Quintessence;
using Lykke.Sdk;

namespace Lykke.BilService.RootstockApi
{
    [UsedImplicitly]
    public class Startup : ApiStartupBase<RootstockApiSettings>
    {
        protected override string IntegrationName
            => "Rootstock";

        protected override void RegisterAdditionalModules(
            IModuleRegistration modules)
        {
            modules.RegisterModule<RootstockApiModule>();
            
            base.RegisterAdditionalModules(modules);
        }
    }
}