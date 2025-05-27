using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IottiMobileApp.Classes
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageWithViewModel<TPage, TViewModel>(this IServiceCollection services)
            where TPage : class
            where TViewModel : class
        {
            return services
                .AddTransient<TViewModel>()
                .AddTransient<TPage>();
        }
    }
}
