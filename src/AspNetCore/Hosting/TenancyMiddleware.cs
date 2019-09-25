// Copyright (c) Kris Penner. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Threading.Tasks;
using KodeAid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using MultiTenancyServer.AspNetCore;
using MultiTenancyServer.Models;
using MultiTenancyServer.Stores;

namespace MultiTenancyServer.Hosting
{
    internal class TenancyMiddleware<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        public TenancyMiddleware(RequestDelegate next, ILogger<TenancyMiddleware<TTenant, TKey>> logger)
        {
            ArgCheck.NotNull(nameof(next), next);
            ArgCheck.NotNull(nameof(logger), logger);
            _next = next;
            _logger = logger;
        }

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public async Task InvokeAsync(
            HttpContext httpContext, ITenancyContext<TTenant, TKey> tenancyContext, ITenancyProvider<TTenant, TKey> tenancyProvider, ITenantStore<TTenant, TKey> tenantStore)
        {
            var tenant = await tenancyProvider.GetCurrentTenantAsync(httpContext.RequestAborted).ConfigureAwait(false);

            if (_logger.IsEnabled(LogLevel.Information))
            {
                if (tenant != null)
                {
                    var tenantId = await tenantStore.GetTenantIdAsync(tenant, httpContext.RequestAborted).ConfigureAwait(false);
                    var canonicalName = await tenantStore.GetCanonicalNameAsync(tenant, httpContext.RequestAborted).ConfigureAwait(false);

                    _logger.LogInformation("Tenant {TenantId} with canonical name {CanonicalName} was found for request {RequestUrl}.",
                        tenantId, canonicalName, httpContext.Request.GetDisplayUrl());
                }
                else
                {
                    _logger.LogInformation("No tenant was found for request {RequestUrl}.", httpContext.Request.GetDisplayUrl());
                }
            }
            
            if (!httpContext.Items.ContainsKey(GlobalConst.HttpContextTenancyContext))
            {
                tenancyContext.Tenant = tenant;
                httpContext.Items.Add(GlobalConst.HttpContextTenancyContext, tenancyContext);
            }
            else
            {
                httpContext.Items.TryGetValue(GlobalConst.HttpContextTenancyContext, out var currentTenancyContext);
                if (currentTenancyContext is ITenancyContext<TTenant, TKey> iTenancyContext)
                {
                    iTenancyContext.Tenant = tenant;
                }
            }

            await _next(httpContext).ConfigureAwait(false);
        }
    }
}
