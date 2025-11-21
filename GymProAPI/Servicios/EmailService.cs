using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace GymProAPI.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void EnviarTicket(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var smtp = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]))
                {
                    Credentials = new NetworkCredential(_config["Smtp:User"], _config["Smtp:Pass"]),
                    EnableSsl = true
                };

                var mensaje = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:From"] ?? _config["Smtp:User"]),
                    Subject = asunto,
                    Body = cuerpoHtml,
                    IsBodyHtml = true
                };

                mensaje.To.Add(destinatario);

                smtp.Send(mensaje);
            }
            catch (SmtpException smtpEx)
            {
                throw new Exception($"SMTP error: {smtpEx.StatusCode} - {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"General error: {ex.Message}", ex);
            }
        }
    }
 }