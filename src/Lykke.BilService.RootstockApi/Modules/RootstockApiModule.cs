using Autofac;
using JetBrains.Annotations;
using Lykke.BilService.RootstockApi.Settings;
using Lykke.Quintessence.Core.DependencyInjection;
using Lykke.Quintessence.DependencyInjection;
using Lykke.Quintessence.Settings;
using Lykke.SettingsReader;

namespace Lykke.BilService.RootstockApi.Modules
{
    [UsedImplicitly]
    public class RootstockApiModule : Module
    {
        private readonly IReloadingManager<AppSettings<RootstockApiSettings>> _appSettings;

        public RootstockApiModule(
            IReloadingManager<AppSettings<RootstockApiSettings>> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(
            ContainerBuilder builder)
        {
            builder
                .UseRootstock
                (
                    _appSettings.ConnectionString(x => x.Api.Db.DataConnString),
                    _appSettings.Nested(x => x.Api.ConfirmationLevel),
                    _appSettings.Nested(x => x.Api.GasPriceRange),
                    _appSettings.CurrentValue.Api.IsMainNet
                );
        }
    }
}