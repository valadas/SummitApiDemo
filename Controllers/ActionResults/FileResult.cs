// MIT License
// Copyright Eraware

using DotNetNuke.Services.FileSystem;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Eraware.Modules.SummitApiDemo.Controllers.ActionResults
{
    /// <summary>
    /// A result that returns a file as an attachment.
    /// </summary>
    public class FileResult : IHttpActionResult
    {
        private readonly IFileInfo fileInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileResult"/> class.
        /// </summary>
        /// <param name="fileInfo">The file to return (DNN file in this case).</param>
        public FileResult(IFileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
        }

        /// <inheritdoc/>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return new Task<HttpResponseMessage>(
                () =>
                {
                    var fileContent = FileManager.Instance.GetFileContent(this.fileInfo);
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(fileContent),
                    };
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(this.fileInfo.ContentType);
                    response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = this.fileInfo.FileName;
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue(this.fileInfo.ContentType.ToString());
                    response.Content.Headers.ContentLength = fileContent.Length;

                    return response;
                },
                cancellationToken);
        }
    }
}
