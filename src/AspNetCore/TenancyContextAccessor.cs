using System;
using Microsoft.AspNetCore.Http;
using MultiTenancyServer.AspNetCore.Extensions;
using MultiTenancyServer.Models;
using MultiTenancyServer.Services;

namespace MultiTenancyServer.AspNetCore
{
    /// <summary>
    /// The TenancyContextAccessor to access the current Tenant context.
    /// </summary>
    /// <typeparam name="TTenant">The type representing a tenant.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for a tenant.</typeparam>
    public class TenancyContextAccessor<TTenant, TKey> : ITenancyContextAccessor<TTenant, TKey>
        where TTenant : class, ITenanted<TKey>
        where TKey : IEquatable<TKey>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenancyContextAccessor{TTenant,TKey}"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HttpContextAccessor.</param>
        public TenancyContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        /// <inheritdoc />
        public ITenancyContext<TTenant, TKey> TenancyContext => httpContextAccessor.HttpContext?.GetTenancyContext<TTenant, TKey>();

        /// <inheritdoc />
        public TKey GetTenantIdOrDefault()
        {
            return this.TenancyContext?.Tenant != null ? this.TenancyContext.Tenant.Id : default;
        }
    }
}
