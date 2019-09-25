using System;
using Microsoft.AspNetCore.Http;
using MultiTenancyServer.Models;

namespace MultiTenancyServer.AspNetCore.Extensions
{
    /// <summary>
    /// Extensions for HttpContext.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the tenant context of current context.
        /// </summary>
        /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
        /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
        /// <param name="httpContext">The httpcontext.</param>
        /// <returns>The context for the current tenant.</returns>
        public static ITenancyContext<TTenant, TKey> GetTenancyContext<TTenant, TKey>(this HttpContext httpContext)
            where TTenant : class, ITenanted<TKey>
            where TKey : IEquatable<TKey>
        {
            httpContext.Items.TryGetValue(GlobalConst.HttpContextTenancyContext, out var multiTenantContext);
            return (ITenancyContext<TTenant, TKey>) multiTenantContext;
        }
    }
}
