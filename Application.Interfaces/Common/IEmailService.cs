using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Common;


public interface IEmailService
{
    /// <summary>
    /// Send a simple HTML/plain email.
    /// </summary>
    Task SendMailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send an email with attachments provided as IFormFile (typical for controller file uploads).
    /// Attachments can be null or empty.
    /// </summary>
    Task SendMailWithAttachmentAsync(string to, string subject, string htmlBody, IEnumerable<IFormFile>? attachments, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send an email with attachments provided as streams (filename + stream + contentType).
    /// </summary>
    Task SendMailWithAttachmentAsync(string to, string subject, string htmlBody, IEnumerable<(string FileName, Stream Content, string ContentType)>? attachments, CancellationToken cancellationToken = default);
}
