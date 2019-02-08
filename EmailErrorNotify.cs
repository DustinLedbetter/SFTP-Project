/***********************************************************************************************************************************
*                                                 GOD First                                                                        *
* Author: Dustin Ledbetter                                                                                                         *
* Release Date: 11-19-2018                                                                                                         *
* Last Edited: 1-29-2019                                                                                                           *
* Version: 1.0                                                                                                                     *
* Purpose: Used to send email out to developer responsible for the site when an error occurs                                       *
************************************************************************************************************************************/

using System.Net;
using System.Net.Mail;

namespace SFTP_Process
{
    class EmailErrorNotify
    {

        // This flag is used to test which part of the try catch is being called and stored in the logs of the main storefront 
        public static string checkFlag;

        // This method is called to send an email when an exception error occurs in the main extension
        public static void CreateMessage(string esubject, string ebody)
        {
			
            // Create a message and set up the recipients.
            MailMessage message = new MailMessage
            (
               "nulled for security",
               "nulled for security",
               esubject,
               ebody
            );

            // Set it so that our html newline will register when the email is created
            message.IsBodyHtml = true;

            #region |--This section is used to create the client without using the webconfig file on the storefronts--|

            // Setup and prepare to send the email
            SmtpClient client = new SmtpClient();
            try
            {
                client.Host = "nulled for security";
                //client.Host = "nulled for security";
                NetworkCredential NetworkCred = new NetworkCredential();
                NetworkCred.UserName = "nulled for security";
                NetworkCred.Password = "nulled for security";
                client.UseDefaultCredentials = nulled for security;
                client.Credentials = nulled for security;
                client.Port = nulled for security;
                client.Send(message);
                // Set flag to Yes so we can know the try succeeded
                checkFlag = "Yes";
            }
            catch
            {
                // Our email send failed; clean up the pieces
                message.Dispose();
                client = null;
                // Set flag to No so we can know the try Failed
                checkFlag = "No";
            }

            #endregion

        } // End of CreateMessage Method

    } // End of the class: EmailErrorNotify

} // End of file
