// MIT License
// Copyright Eraware

using DotNetNuke.DependencyInjection;
using DotNetNuke.Services.Localization;
using Eraware.Modules.SummitApiDemo.Data;
using Eraware.Modules.SummitApiDemo.Data.Entities;
using Eraware.Modules.SummitApiDemo.Data.Repositories;
using Eraware.Modules.SummitApiDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Eraware.Modules.SummitApiDemo
{
    /// <summary>
    /// Implements the IDnnStartup interface to run at application start.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup : IDnnStartup
    {
        /// <summary>
        /// Registers the dependencies for injection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ModuleDbContext, ModuleDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IItemService>(provider => new ItemService(provider.GetService<IRepository<Item>>()));
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<ILocalizationService, LocalizationService>();
        }
    }
}