using System.Collections.Generic;
using Contrib.KubeClient.CustomResources;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Contrib.IdentityServer4.KubernetesStore
{
    /// <summary>
    /// Extension methods to add Kubernetes support to IdentityServer.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// Remember to call <see cref="Contrib.KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder, IEnumerable<IdentityResource> identityResources = null)
            => builder.AddKubernetesConfigurationStore(new ConfigurationBuilder().Build(), identityResources);

        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// Remember to call <see cref="Contrib.KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="configuration">The configuration section to be bound to <see cref="KubernetesConfigurationStoreOptions"/>.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IIdentityServerBuilder AddKubernetesConfigurationStore(this IIdentityServerBuilder builder, IConfiguration configuration, IEnumerable<IdentityResource> identityResources = null)
        {
            builder.Services.AddIdentityKubernetesConfigurationStore(configuration, identityResources);
            return builder;
        }

        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// Remember to call <see cref="KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="services">The service collection to register the services in.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IServiceCollection AddIdentityKubernetesConfigurationStore(this IServiceCollection services, IEnumerable<IdentityResource> identityResources = null)
            => services.AddIdentityKubernetesConfigurationStore(new ConfigurationBuilder().Build(), identityResources);

        /// <summary>
        /// Configures Kubernetes Custom Resource Definition implementation of
        /// <see cref="IClientStore"/>, <see cref="IResourceStore"/>, and <see cref="ICorsPolicyService"/> with IdentityServer.
        /// Remember to call <see cref="KubeClient.CustomResources.DependencyInjectionExtensions.UseCustomResourceWatchers"/> during startup.
        /// </summary>
        /// <param name="services">The service collection to register the services in.</param>
        /// <param name="configuration">The configuration section to be bound to <see cref="KubernetesConfigurationStoreOptions"/>.</param>
        /// <param name="identityResources"><see cref="IdentityResource"/> to be used. Default is an empty list.</param>
        public static IServiceCollection AddIdentityKubernetesConfigurationStore(this IServiceCollection services, IConfiguration configuration, IEnumerable<IdentityResource> identityResources = null)
        {
            services.Configure<KubernetesConfigurationStoreOptions>(configuration)
                    .AddKubernetesClient();
            services.AddCustomResourceWatcher(ClientResource.Definition)
                    .AddCustomResourceWatcher(ApiResourceResource.Definition);
            services.AddSingleton<IClientStore, KubernetesClientStore>()
                    .AddSingleton<IResourceStore, KubernetesResourceStore>()
                    .AddSingleton<ICorsPolicyService, KubernetesCorsPolicyService>()
                    .TryAddSingleton(identityResources ?? new List<IdentityResource>());
            return services;
        }
    }
}