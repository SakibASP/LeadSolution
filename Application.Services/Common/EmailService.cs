using Application.Interfaces.Common;
using Common.Utils.Helper;
using Core.Models.Common;
using Infrastructure.Interfaces.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Serilog;

using Encryptor = EncryptionHelper.EncryptionHelper;

namespace Application.Services.Common;

/// <summary>
/// Author: Md. Sakibur Rahman
/// Created Date: 11 Sep, 2025
/// </summary>

public class EmailService(IOptions<SmtpSettings> options,
    IGenericRepo<EmailSendHistory> genericRepo,
    IHttpContextAccessor httpContext) : IEmailService
{
    private readonly SmtpSettings _settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
    private readonly IHttpContextAccessor _httpContext = httpContext;
    private readonly IGenericRepo<EmailSendHistory> _iGenericRepo = genericRepo;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    #region - send mails -
    public async Task SendMailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = CreateBaseMessage(to, subject, htmlBody);
        await SendAsync(message, cancellationToken).ConfigureAwait(false);
    }

    public async Task SendMailWithAttachmentAsync(string to, string subject, string htmlBody, IEnumerable<IFormFile>? attachments, CancellationToken cancellationToken = default)
    {
        var mimeAttachments = new List<(string FileName, Stream Content, string ContentType)>();
        if (attachments != null)
        {
            foreach (var file in attachments)
            {
                if (file == null || file.Length == 0) continue;
                var memory = new MemoryStream();
                await file.CopyToAsync(memory, cancellationToken).ConfigureAwait(false);
                memory.Position = 0;
                mimeAttachments.Add((file.FileName, memory, file.ContentType ?? "application/octet-stream"));
            }
        }

        await SendMailWithAttachmentAsync(to, subject, htmlBody, mimeAttachments, cancellationToken).ConfigureAwait(false);

        // dispose the memory streams we created
        foreach (var (_, stream, _) in mimeAttachments)
            stream.Dispose();
    }

    public async Task SendMailWithAttachmentAsync(string to, string subject, string htmlBody, IEnumerable<(string FileName, Stream Content, string ContentType)>? attachments, CancellationToken cancellationToken = default)
    {
        var message = CreateBaseMessage(to, subject, htmlBody);

        if (attachments != null)
        {
            var multipart = new Multipart("mixed");

            // body (alternative: plain + html)
            var body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            multipart.Add(body);

            foreach (var (fileName, contentStream, contentType) in attachments)
            {
                if (contentStream == null) continue;
                try
                {
                    // copy into a MemoryStream if the provided stream is not seekable
                    Stream attachStream;
                    if (!contentStream.CanSeek)
                    {
                        var ms = new MemoryStream();
                        await contentStream.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
                        ms.Position = 0;
                        attachStream = ms;
                    }
                    else
                    {
                        attachStream = contentStream;
                        attachStream.Position = 0;
                    }

                    var attachment = new MimePart(contentType)
                    {
                        Content = new MimeContent(attachStream, ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = fileName
                    };

                    multipart.Add(attachment);
                }
                catch (Exception ex)
                {
                    Log
                        .ForContext("UserName", CurrentUser)
                        .ForContext("Path", RequestPath)
                        .Error(ex, "Failed adding attachment {FileName}", fileName);
                    // swallow attachment errors to allow sending the mail without that attachment (optional)
                }
            }

            message.Body = multipart;
        }

        await SendAsync(message, cancellationToken).ConfigureAwait(false);
    }
    #endregion

    #region - private helpers -
    private MimeMessage CreateBaseMessage(string to, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject ?? string.Empty;

        // prefer HTML; add a fallback plain text version
        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody,
            TextBody = StripHtmlToPlainText(htmlBody)
        };

        message.Body = builder.ToMessageBody();
        return message;
    }

    private static string StripHtmlToPlainText(string? html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        // very simple fallback — if you need better conversion use HtmlAgilityPack
        var plain = System.Text.RegularExpressions.Regex.Replace(html, "<[^>]+>", string.Empty);
        plain = System.Net.WebUtility.HtmlDecode(plain);
        return plain;
    }

    private async Task SendAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        using var client = new SmtpClient();
        try
        {
            // connect
            var secureSocket = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
            await client.ConnectAsync(_settings.Host, _settings.Port, secureSocket, cancellationToken).ConfigureAwait(false);

            // authenticate if username provided
            if (!string.IsNullOrEmpty(_settings.UserName))
            {
                await client.AuthenticateAsync(_settings.UserName, Encryptor.Decrypt(_settings.Password), cancellationToken).ConfigureAwait(false);
            }

            await client.SendAsync(message, cancellationToken).ConfigureAwait(false);
            await _iGenericRepo.AddAsync(new EmailSendHistory
            {
                To = string.Join(",", message.To),
                From = string.Join(",", message.From),
                Subject = message.Subject,
                Body = message.HtmlBody ?? message.TextBody
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log
                .ForContext("UserName", CurrentUser)
                .ForContext("Path", RequestPath)
                .Error(ex, "Failed to send email to {To}", string.Join(",", message.To));
            throw; // bubble up so caller can handle retry / logging / fallback
        }
        finally
        {
            try
            {
                await client.DisconnectAsync(true, cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // ignore
            }
        }
    }
    #endregion
}

