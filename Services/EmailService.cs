using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace WashNGo.Services;

// ── Interface ─────────────────────────────────────────────
public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string toName, string resetLink);
    Task SendEmailVerificationAsync(string toEmail, string toName, string verifyLink);
    Task SendBookingConfirmationEmailAsync(string toEmail, string toName,
        string reservationId, string serviceName, string date, string slot);
}

// ── MailKit Implementation ────────────────────────────────
public class EmailService : IEmailService
{
    private readonly IConfiguration        _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ── Password Reset Email ──────────────────────────────
    public async Task SendPasswordResetEmailAsync(
        string toEmail, string toName, string resetLink)
    {
        var subject = "Reset Your WashNGo Password";
        var html    = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f1f5f9;margin:0;padding:20px;}}
  .box{{max-width:560px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#0ea5e9,#0284c7);padding:32px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:24px;font-weight:800;}}
  .hdr span{{color:#bae6fd;font-size:13px;}}
  .bdy{{padding:32px;}}
  .bdy p{{color:#475569;line-height:1.7;font-size:15px;}}
  .btn{{display:inline-block;background:#0ea5e9;color:#fff!important;padding:14px 36px;border-radius:10px;text-decoration:none;font-weight:700;font-size:15px;margin:20px 0;}}
  .warn{{background:#fef9c3;border-left:4px solid #f59e0b;padding:12px 16px;border-radius:6px;font-size:13px;color:#92400e;margin-top:16px;}}
  .ftr{{background:#f8fafc;padding:20px 32px;text-align:center;color:#94a3b8;font-size:12px;border-top:1px solid #e2e8f0;}}
  .link{{color:#64748b;font-size:12px;word-break:break-all;margin-top:12px;}}
</style>
</head>
<body>
<div class='box'>
  <div class='hdr'><h1>🚗 WashNGo</h1><span>Professional Vehicle Wash Service</span></div>
  <div class='bdy'>
    <h2 style='color:#0f172a;margin-top:0;'>Password Reset Request</h2>
    <p>Hi <strong>{toName}</strong>,</p>
    <p>We received a request to reset your WashNGo account password. Click the button below:</p>
    <div style='text-align:center;'>
      <a href='{resetLink}' class='btn'>Reset My Password</a>
    </div>
    <div class='warn'>
      ⏰ <strong>This link expires in 1 hour.</strong>
      If you didn't request this, ignore this email — your password won't change.
    </div>
    <p class='link'>Button not working? Copy this link:<br/>{resetLink}</p>
  </div>
  <div class='ftr'>&copy; {DateTime.Now.Year} WashNGo &nbsp;|&nbsp; Chittagong, Bangladesh<br/>Automated email — do not reply.</div>
</div>
</body></html>";

        await SendAsync(toEmail, toName, subject, html);
    }

    // ── Email Verification ────────────────────────────────
    public async Task SendEmailVerificationAsync(
        string toEmail, string toName, string verifyLink)
    {
        var subject = "Verify Your WashNGo Account";
        var html    = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f1f5f9;margin:0;padding:20px;}}
  .box{{max-width:560px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#0ea5e9,#0284c7);padding:32px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:24px;font-weight:800;}}
  .hdr span{{color:#bae6fd;font-size:13px;}}
  .bdy{{padding:32px;}}
  .bdy p{{color:#475569;line-height:1.7;font-size:15px;}}
  .btn{{display:inline-block;background:#22c55e;color:#fff!important;padding:14px 36px;border-radius:10px;text-decoration:none;font-weight:700;font-size:15px;margin:20px 0;}}
  .ftr{{background:#f8fafc;padding:20px 32px;text-align:center;color:#94a3b8;font-size:12px;border-top:1px solid #e2e8f0;}}
  .link{{color:#64748b;font-size:12px;word-break:break-all;margin-top:12px;}}
</style>
</head>
<body>
<div class='box'>
  <div class='hdr'><h1>🚗 WashNGo</h1><span>Professional Vehicle Wash Service</span></div>
  <div class='bdy'>
    <h2 style='color:#0f172a;margin-top:0;'>Welcome to WashNGo! 🎉</h2>
    <p>Hi <strong>{toName}</strong>,</p>
    <p>Thanks for creating an account. Please verify your email address to activate full access to your account:</p>
    <div style='text-align:center;'>
      <a href='{verifyLink}' class='btn'>Verify My Email</a>
    </div>
    <p class='link'>Button not working? Copy this link:<br/>{verifyLink}</p>
  </div>
  <div class='ftr'>&copy; {DateTime.Now.Year} WashNGo &nbsp;|&nbsp; Chittagong, Bangladesh<br/>Automated email — do not reply.</div>
</div>
</body></html>";

        await SendAsync(toEmail, toName, subject, html);
    }
    public async Task SendBookingConfirmationEmailAsync(
        string toEmail, string toName, string reservationId,
        string serviceName, string date, string slot)
    {
        var subject = $"Booking Confirmed – {reservationId}";
        var html    = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'/>
<style>
  body{{font-family:'Segoe UI',Arial,sans-serif;background:#f1f5f9;margin:0;padding:20px;}}
  .box{{max-width:560px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;box-shadow:0 4px 24px rgba(0,0,0,.08);}}
  .hdr{{background:linear-gradient(135deg,#0ea5e9,#0284c7);padding:32px;text-align:center;}}
  .hdr h1{{color:#fff;margin:0;font-size:24px;font-weight:800;}}
  .bdy{{padding:32px;}}
  .res{{font-size:28px;font-weight:800;color:#0ea5e9;font-family:monospace;letter-spacing:2px;text-align:center;margin:16px 0;}}
  table{{width:100%;border-collapse:collapse;margin:16px 0;}}
  td{{padding:10px 0;border-bottom:1px solid #e2e8f0;font-size:14px;color:#475569;}}
  td:last-child{{font-weight:600;color:#0f172a;text-align:right;}}
  .ftr{{background:#f8fafc;padding:20px 32px;text-align:center;color:#94a3b8;font-size:12px;border-top:1px solid #e2e8f0;}}
</style>
</head>
<body>
<div class='box'>
  <div class='hdr'><h1>✅ Booking Confirmed!</h1></div>
  <div class='bdy'>
    <p style='color:#475569;'>Hi <strong>{toName}</strong>, your wash appointment is confirmed.</p>
    <div class='res'>{reservationId}</div>
    <table>
      <tr><td>Service</td><td>{serviceName}</td></tr>
      <tr><td>Date</td><td>{date}</td></tr>
      <tr><td>Time Slot</td><td>{slot}</td></tr>
    </table>
    <p style='color:#475569;font-size:14px;'>Please arrive 5 minutes early and show your Reservation ID at the counter.</p>
  </div>
  <div class='ftr'>&copy; {DateTime.Now.Year} WashNGo &nbsp;|&nbsp; Chittagong, Bangladesh</div>
</div>
</body></html>";

        await SendAsync(toEmail, toName, subject, html);
    }

    // ── Core MailKit send ─────────────────────────────────
    private async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var smtpHost  = _config["Email:SmtpHost"]  ?? "smtp.gmail.com";
        var smtpPort  = int.Parse(_config["Email:SmtpPort"] ?? "587");
        var username  = _config["Email:Username"]  ?? "";
        var password  = _config["Email:Password"]  ?? "";
        var fromEmail = _config["Email:FromEmail"] ?? username;
        var fromName  = _config["Email:FromName"]  ?? "WashNGo";

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        try
        {
            using var client = new SmtpClient();

            // Connect using STARTTLS (port 587) — works with Gmail App Password
            await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}: {Message}", toEmail, ex.Message);
            throw;
        }
    }
}
