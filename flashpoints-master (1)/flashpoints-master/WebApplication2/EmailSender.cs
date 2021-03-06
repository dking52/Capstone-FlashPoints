﻿using System;
using System.Net;
using System.Net.Mail;

namespace FlashPoints
{
    public static class EmailSender
    {
        public static void SendMail(string To, string Subject, string Body)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("flashpointsteam@gmail.com", "Flashpoints123!");

            // Footer for all emails
            Body = Body + Environment.NewLine + Environment.NewLine + Environment.NewLine +
                "This email was generated by the FlashPoints Team.";

            MailMessage mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.From = new MailAddress("flashpointsteam@gmail.com");
            mailMessage.To.Add(To);
            mailMessage.Body = Body;
            mailMessage.Subject = Subject;
            client.Send(mailMessage);
        }
    }
}