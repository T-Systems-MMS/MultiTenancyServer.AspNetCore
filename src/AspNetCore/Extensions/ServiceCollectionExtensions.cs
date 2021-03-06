﻿// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiTenancyServer;
using MultiTenancyServer.AspNetCore;
using MultiTenancyServer.Configuration.DependencyInjection;
using MultiTenancyServer.Http;
using MultiTenancyServer.Models;
using MultiTenancyServer.Options;
using MultiTenancyServer.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Contains extension methods to <see cref="IServiceCollection"/> for configuring multi-tenancy services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures the multi-tenancy system for the specified Tenant type.
        /// </summary>
        /// <typeparam name="TTenant">The type representing a Tenant in the system.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <returns>An <see cref="TenancyBuilder{TTenant, TKey}"/> for creating and configuring the multi-tenancy system.</returns>
        public static TenancyBuilder<TTenant, TKey> AddMultiTenancy<TTenant, TKey>(this IServiceCollection services)
            where TTenant : class, ITenanted<TKey>
            where TKey : IEquatable<TKey>
            => services.AddMultiTenancy<TTenant, TKey>(o => { });

        /// <summary>
        /// Adds and configures the multi-tenancy system for the specified Tenant type.
        /// </summary>
        /// <typeparam name="TTenant">The type representing a Tenant in the system.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
        /// <param name="services">The services available in the application.</param>
        /// <param name="setup">An action to configure the <see cref="TenancyOptions"/>.</param>
        /// <returns>An <see cref="TenancyBuilder{TTenant, TKey}"/> for creating and configuring the multi-tenancy system.</returns>
        public static TenancyBuilder<TTenant, TKey> AddMultiTenancy<TTenant, TKey>(this IServiceCollection services, Action<TenancyOptions> setup)
            where TTenant : class, ITenanted<TKey>
            where TKey : IEquatable<TKey>
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<ITenancyContextAccessor<TTenant, TKey>, TenancyContextAccessor<TTenant, TKey>>();
            services.TryAddScoped<ITenancyProvider<TTenant, TKey>, HttpTenancyProvider<TTenant, TKey>>();
            return services.AddMultiTenancyCore<TTenant, TKey>(setup);
        }
    }
}
