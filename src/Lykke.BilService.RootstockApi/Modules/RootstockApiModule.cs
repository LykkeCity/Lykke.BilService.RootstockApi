using Autofac;
using JetBrains.Annotations;
using Lykke.BilService.RootstockApi.Settings;
using Lykke.Quintessence.Settings;
using Lykke.Quintessence.Utils;

namespace Lykke.BilService.RootstockApi.Modules
{
    [UsedImplicitly]
    public class RootstockApiModule : Module
    {
        private readonly AppSettings<RootstockApiSettings> _appSettings;

        public RootstockApiModule(
            AppSettings<RootstockApiSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(
            ContainerBuilder builder)
        {
            builder
                .RegisterRootstock(_appSettings.Api.IsMainNet);
        }
    }
}