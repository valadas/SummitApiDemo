// MIT License
// Copyright Eraware

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;

namespace Eraware.Modules.SummitApiDemo.Controllers
{
    /// <summary>
    /// Caches web api results on the client side.
    /// </summary>
    public class CacheWebApiAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheWebApiAttribute"/> class.
        /// </summary>
        public CacheWebApiAttribute()
        {
            this.Duration = TimeSpan.FromMinutes(20);
        }

        /// <summary>
        /// Gets or sets the time to cache on the client.
        /// </summary>
        public TimeSpan Duration { get; set; }

        /// <inheritdoc/>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
            {
                MaxAge = this.Duration,
                Private = true,
            };
        }

        /// <inheritdoc/>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            await base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
            actionExecutedContext.Response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue()
            {
                MaxAge = this.Duration,
                Private = true,
            };
        }
    }
}
