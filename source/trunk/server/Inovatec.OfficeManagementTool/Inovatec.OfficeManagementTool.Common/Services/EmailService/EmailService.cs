using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace Inovatec.OfficeManagementTool.Common.Services.EmailService
{
    public class EmailService: IEmailService
    {
        public async Task<bool> SendEmail(string destination, string subject, string style, string body)
        {
            string from = ConfigProvider.Email;
            string password = ConfigProvider.EmailPassword;
            string smtpUrl = ConfigProvider.SmtpUrl;
            int smtpPort = ConfigProvider.SmtpPort;

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(from);
                mail.To.Add(destination);
                mail.Subject = subject;
                mail.Body = GetBody(style, body);
                mail.IsBodyHtml = true;
                
                var attachment = new Attachment(Path.Join(ConfigProvider.PathToStaticFiles, "assets", "inovatecLogo.png"), new ContentType("image/png"));
                attachment.ContentId = "Logo.png";
                attachment.Name = "Logo.png";

                mail.Attachments.Add(attachment);

                using (SmtpClient smtp = new SmtpClient(smtpUrl, smtpPort))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(from, password);
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return true;
        }

		private string GetBody(string style, string body)
		{
            return @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"">
								<html lang=""en"" xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
								<head>
									<meta charset=""UTF-8"">
									<meta name=""viewport"" content=""width=device-width,initial-scale=1"">
									<meta name=""x-apple-disable-message-reformatting"">
									<title></title>
									<!--[if mso]>
									<noscript>
										<xml>
											<o:OfficeDocumentSettings>
												<o:PixelsPerInch>96</o:PixelsPerInch>
											</o:OfficeDocumentSettings>
										</xml>
									</noscript>
									<![endif]-->
									<style> 
										table, td, div, h1, p {font-family: Arial, sans-serif;}  
	#order {
			font-family: Arial, Helvetica, sans-serif;
			border-collapse: collapse;
			width: 90%;
		}

		#order td,
		#order th {
			border: 1px solid #ddd;
			padding: 8px;
		}

		#order tr:nth-child(even) {
			background-color: #d0e4e2;
		}

		#order tr:hover {
			background-color: #efd16463;
		}

		#order th {
			padding-top: 12px;
			padding-bottom: 12px;
			text-align: left;
			background-color: #489890;
			color: white;
		}

		.absolute-center > * {
			margin:auto;
		}

		button {
			margin-top: 20px;
			font-family: inherit;
			font-size: 20px;
			background: royalblue;
			color: white;
			padding: 0.7em 1em;
			padding-left: 0.9em;
			display: flex;
			align-items: center;
			border: none;
			border-radius: 16px;
			overflow: hidden;
			transition: all 0.2s;
		}

		button span  {
			display: block;
			margin-left: 0.3em;
			transition: all 0.3s ease-in-out;
		}

        a.absolute-center {
            flex: 1;
			text-decoration: none;
		}
" + style + @"
									</style>
								</head>
								<body style=""margin:0;padding:0;"">
									<table role=""presentation"" style=""width:100%;border-collapse:collapse;border:0;border-spacing:0;background:#ffffff;"">
										<tr>
											<td align=""center"" style=""padding:0;"">
												<table role=""presentation"" style=""width:602px;border-collapse:collapse;border:1px solid #cccccc;border-spacing:0;text-align:left;"">
													<tr>
														<td align=""center"" style=""padding:40px 0 30px 0;background:#e7e7e7;"">
															<img src=""cid:Logo.png"" alt="""" width=""300"" style=""height:auto;display:block;"" />
														</td>
													</tr>
												
													" + body + @"
													<tr>
														<td style=""padding:30px;background:#333333;"">
															<table role=""presentation"" style=""width:100%;border-collapse:collapse;border:0;border-spacing:0;font-size:9px;font-family:Arial,sans-serif;"">
																<tr>
																	<td style=""padding:0;width:100%; text-align:center;"">
																		<p style=""margin:0;font-size:14px;line-height:16px;font-family:Arial,sans-serif;color:#ffffff;"">
																			©&nbsp;2023 - Inovatec | All Rights Reserved | <a href=""https://www.inovatec.com/privacy-policy/"" style=""color:#ffffff;"">Privacy</a>
																		</p>
																	</td>
																</tr>
															</table>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</body>
								</html>";
        }
    }
}