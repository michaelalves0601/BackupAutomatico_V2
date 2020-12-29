using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BackupNuvemSBuild
{
    class Email
    {
        public string SmtpServerString { get; set; }
        public string Origem { get; set; }
        public string Password { get; set; }
        public string[] Destinos { get; set; }
        public string Assunto { get; set; }
        public string CorpoEmail { get; set; }
        public Attachment[] Anexos { get; set; }

        Log log = new Log();


        public bool SendEmail()
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SmtpServerString);

                mail.From = new MailAddress(Origem);

                foreach (string destino in Destinos)
                    mail.To.Add(destino);

                mail.Subject = Assunto;
                mail.Body = CorpoEmail;

                foreach (Attachment anexo in Anexos)
                    mail.Attachments.Add(anexo);


                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(Origem, Password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                return true;
            }
            catch(Exception ex)
            {
                log.LogError("Falha no Envio do Email",
                                MethodBase.GetCurrentMethod().Name,
                                    MethodBase.GetCurrentMethod().ToString(),
                                        ex.Message);

                return false;
            }
        }
    }
}
