using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Shiny;
using Shiny.Prism;

namespace Covid19Radar.Shiny
{
    public class ShinyAppStartup : PrismStartup
    {
        public ShinyAppStartup() : base(PrismContainerExtension.Current)
        {

        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            //Enable your shiny services here
        }
    }
}
