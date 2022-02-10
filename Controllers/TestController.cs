// MIT License
// Copyright Eraware

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using Eraware.Modules.SummitApiDemo.Controllers.ActionResults;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Eraware.Modules.SummitApiDemo.Controllers
{
    /// <summary>
    /// Demo for DNN Summit presentation about webapi.
    /// </summary>
    public class TestController : DnnApiController
    {
        /// <summary>
        /// Gets nothing, could be used to delete a record or some other operation that does not return anything back.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public void GetVoid()
        {
            // Do something that does not need to return...
            // We have no control over the actual response message, the framework decides.
            return;
        }

        /// <summary>
        /// Gets a string.
        /// </summary>
        /// <returns>A string.</returns>
        [HttpGet]
        [AllowAnonymous]
        public string GetString()
        {
            // We can return primitive types too...
            return "Hello in a string.";
        }

        /// <summary>
        /// Gets a list of strings.
        /// </summary>
        /// <returns>A list of strings.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<string> GetListOfString()
        {
            // We can also return any complex object or enumerable, anything that is serializable basically.
            // With complex objects that refer to other objects.
            // Be careful of a reference loop which will fail
            // (like a product that relates to a category but the category relates to multiple products including this one).
            return new List<string>
            {
                "Hello",
                "as",
                "a",
                "list",
            };
        }

        /// <summary>
        /// Gets a message.
        /// </summary>
        /// <returns>The actual HttpResponseMessage.</returns>
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetMessage()
        {
            // This is the most commonly seen way, full control over the actual HttpMessage.
            // Allows us to set headers, cookies, use custom serializers, etc.
            return this.Request.CreateResponse(HttpStatusCode.OK, "Hello the good old way.");
        }

        /// <summary>
        /// Gets a test message.
        /// </summary>
        /// <returns>A test message.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetSync()
        {
            // Using IHttpActionResults requires .Net 4.5 and up
            // - Easier to use than HttpResponseMessage, has many response constructors.
            // - Allows easier tests because we can mock the response
            // - Required if you want to use async
            // - You can create your own response contructors in their own class.
            Thread.Sleep(2000);
            return this.Ok(new { message = "Hello sync." });

            // return this.BadRequest();
            // return this.Conflict();
            // return this.Content(HttpStatusCode.OK, "some object");
            // return this.Created("url to the created resource", "any object you want");
            // return this.InternalServerError(new System.Exception("Exception message"));
            // return this.Redirect("url to redirect to (302 response)");
            // return this.StatusCode(HttpStatusCode.NoContent); // <= Request processed but nothing found to return...
            // etc.

            // We can also create our own cutom return types,
            // this helps isolate the response creation logic
            // into it's own reusable class, see GetLogo method for an example.
        }

        /// <summary>
        /// Gets a test message asynchronously.
        /// </summary>
        /// <returns>An awaitable task with a message for result.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetAsync()
        {
            await Task.Delay(2000);
            return this.Ok(new { message = "Hello from async." });
        }

        /// <summary>
        /// Gets the current site logo as a download.
        /// </summary>
        /// <returns>A browser download of the logo.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetLogo()
        {
            var logoFile = FileManager.Instance.GetFile(
                this.PortalSettings.PortalId,
                this.PortalSettings.LogoFile);
            return new FileResult(logoFile);
        }

        /// <summary>
        /// Returns a sanitied string that used to contain XSS scripts.
        /// </summary>
        /// <returns>Sanitized string.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetSanitized()
        {
            var badString = "I am bad <script type=\"text/javascript\">alert(\"Hacked!\");</script>";
            var sanitized = HttpUtility.HtmlEncode(badString);

            return this.Ok(sanitized);
        }

        /// <summary>
        /// Gets a random string array very slowly to simulate heavy processing.
        /// </summary>
        /// <returns>Probably a slow response.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetSlow()
        {
            var result = await Task.Run(() => this.GetBigSlowArray(20, 20));
            return this.Ok(result);
        }

        /// <summary>
        /// Gets a random string array very slowly but uses datacache if available.
        /// </summary>
        /// <returns>Probably a slow response.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IHttpActionResult> GetSlowWithDataCache()
        {
            return await Task.Run(() =>
            {
                // Scoping by IP but here you could use the userId or anything else that makes sense.
                var ip = this.Request.GetIPAddress();
                var cacheKey = $"MyBigArray_{ip}";

                var cached = DataCache.GetCache<IEnumerable<string>>(cacheKey);
                if (cached != null)
                {
                    return this.Ok(cached);
                }

                var result = this.GetBigSlowArray(20, 20);

                /* Basic usage
                DataCache.SetCache(cacheKey, result);
                */

                /* This way it expires exactly in that amount of time no matter what.
                DataCache.SetCache(cacheKey, result, DateTime.Now.AddSeconds(30));
                */

                // This way it has a sliding expiration
                // so will only expire if not accessed within that time window
                DataCache.SetCache(cacheKey, result, TimeSpan.FromSeconds(30));

                return this.Ok(result);
            });
        }

        /// <summary>
        /// Gets a random string array very slowly but uses output cache which caches the whole response.
        /// </summary>
        /// <returns>Probably a slow response.</returns>
        [CacheWebApi]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetSlowWithOutputCache()
        {
            var result = this.GetBigSlowArray(20, 20);
            var response = this.Request.CreateResponse(HttpStatusCode.OK, result);

            /*
            //response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            //{
            //    Public = true,
            //    MaxAge = TimeSpan.FromMinutes(20),
            //};
            */

            /*
            HttpContext.Current.Response.AppendHeader("Cache-Control", "public, max-age=3600");
            */

            return response;
        }

        /// <summary>
        /// Never do this.
        /// </summary>
        /// <param name="displayName">The user display name to filter by.</param>
        /// <returns>A list of display names.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult SqlInjection(string displayName)
        {
            using (var ctx = DataContext.Instance())
            {
                var users = ctx.ExecuteQuery<UserDto>(
                    System.Data.CommandType.Text,
                    $"SELECT Email FROM Users WHERE DisplayName = '{displayName}'");

                return this.Ok(new { users });
            }
        }

        /// <summary>
        /// This is better.
        /// </summary>
        /// <param name="displayName">The display name to search for.</param>
        /// <returns>A list of emails.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult NoSqlInjection(string displayName)
        {
            try
            {
                using (var ctx = DataContext.Instance())
                {
                    var users = ctx.ExecuteQuery<UserDto>(
                        System.Data.CommandType.Text,
                        $"SELECT Email FROM Users WHERE DisplayName = @displayName",
                        new { displayName });

                    return this.Ok(new { users });
                }
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw;
            }
        }

        private IEnumerable<string> GetBigSlowArray(int lenght, int size)
        {
            var result = new List<string>();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < size; i++)
            {
                // if we were not trying to make this bad, we should use a StringBuilder here.
                var currentString = string.Empty;

                for (int j = 0; j < lenght; j++)
                {
                    // We are trying to make things bad here but
                    // should never intantiate a new random in the inner loop
                    // disposable objects should always be disposed or in a using block.
                    var random = new Random();

                    Thread.Sleep(10); // making it even worst by introducing a 10ms delay

                    var charIndex = random.Next(chars.Length - 1);
                    currentString += chars[charIndex];
                }

                result.Add(currentString);
            }

            return result;
        }

        /// <summary>
        /// A sample user Dto.
        /// </summary>
        public class UserDto
        {
            /// <summary>
            /// Gets or sets the user email.
            /// </summary>
            public string Email { get; set; }
        }
    }
}
