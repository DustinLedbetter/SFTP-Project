/***************************************************************************************************************************************************
*                                                 GOD First                                                                                        *
* Authors: Dustin Ledbetter                                                                                                                        *
* Release Date: 1-29-2019                                                                                                                          *
* Version: 1.0                                                                                                                                     *
* Purpose: This is an application that will be used to ftp a file from my desktop to a server location, check if it made it,                       *
*          and delete and backup the files in the original folder.                                                                                             *
***************************************************************************************************************************************************/

/*
    This is added From NuGet Package Management:
    1. WINSCP.NET

    References added:
    1. System.Configuration
*/

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using WinSCP;

namespace SFTP_Process
{
    class Program
    {
        static void Main(string[] args)
        {
            // Variables to be passed from config file

            // Session Options For winSCP Application
            string hostName = ConfigurationManager.AppSettings["HostName"];
            string userName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string sshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"];

            // Paths For Moving The Files Between Folders
            string localPath = ConfigurationManager.AppSettings["LocalPath"];
            string backUpPath = ConfigurationManager.AppSettings["BackUpPath"];

            // Path for logging to a text file
            string logPath = ConfigurationManager.AppSettings["LogPath"];

            // Create instance for using the LogMessageToFile class methods
            LogMessageToFile LMTF = new LogMessageToFile();

            // Safety Net
            try
            {
                // Setup session options -- SshHostKeyFingerprint is found in winSCP when connected to the server under "Session" >>> "Server/protocol Information"
                SessionOptions sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Sftp,
                    HostName = hostName,
                    UserName = userName,
                    Password = password,
                    SshHostKeyFingerprint = sshHostKeyFingerprint,
                };

                // Start a session to move files
                using (Session session = new Session())
                {
                    // Setup timestamp to add to files during upload
                    string datetimestamp = DateTime.Now.ToString("_MM-dd-yyyy_HH.mm.sstt", CultureInfo.InvariantCulture);

                    // Setup directory and get list of files that are to be transferred so we can add the timestamp to them
                    DirectoryInfo d = new DirectoryInfo(localPath);
                    FileInfo[] Files = d.GetFiles("*"); // Getting files

                    if (Files.Length != 0)
                    {
                        // Setup our log file header for this upload
                        string stamp = DateTime.Now.ToString("MM-dd-yyyy HH:mm tt", CultureInfo.InvariantCulture);

                        // Log messages to the text file for future debugging
                        LMTF.LogMessagesToFile(logPath, "//----------------------------------------------------------------");
                        LMTF.LogMessagesToFile(logPath, "// File Uploads for: " + stamp);
                        LMTF.LogMessagesToFile(logPath, "//----------------------------------------------------------------");

                        // Connect
                        session.Open(sessionOptions);

                        // Log messages to the text file for future debugging
                        LMTF.LogMessagesToFile(logPath, "Connection to sftp session sucessful");
                        LMTF.LogMessagesToFile(logPath, " ");
  
                        // Upload file setup options
                        TransferOptions transferOptions = new TransferOptions();
                        transferOptions.TransferMode = TransferMode.Binary; // default mode: Based on file type

                        // Setup an index counter to show which file we are working on
                        int i = 1;

                        // Loop through each file in the folder: Add timestamp to filename, upload the new file, move file from upload folder to a backup folder
                        foreach (FileInfo file in Files)
                        {
                            // Set up timestamp addition to files that are to be transferred
                            string oldFileName = file.ToString();
                            LMTF.LogMessagesToFile(logPath, "[" + i + "] " + "Uploading file: " + oldFileName);   // Log messages to the text file for future debugging
                            string splitname = oldFileName.Split('.')[0];
                            string newFileName = splitname + datetimestamp + "." + oldFileName.Split('.')[1];
                            LMTF.LogMessagesToFile(logPath, "   1) Timestamp added to original file name:  " + newFileName);   // Log messages to the text file for future debugging
                           
                            // Saftey Net to keep program going if a particular file upload has an issue
                            try
                            {
                                // Rename the file in the upload folder to have the timestamp added (Used File.Move as it is the easiest way to accomplish this task)
                                File.Move(localPath + oldFileName, localPath + newFileName);

                                // Run process to upload the current file to the remote server
                                TransferOperationResult transferResult;
                                transferResult = session.PutFiles(localPath + newFileName, "/", false, transferOptions);

                                // Throw on any error for upload
                                transferResult.Check();

                                // Check if the file was added to the new location: If yes, move original to backup location
                                if (session.FileExists(newFileName))
                                {
                                    // First inform that the file was transferred correctly
                                    LMTF.LogMessagesToFile(logPath, "   2) File has been successfully transferred: " + Convert.ToString(session.FileExists(newFileName)).ToUpper()); // Log messages to the text file for future debugging
                                   
                                    // Setup path for backup destination folder
                                    string backupDestinationPath = backUpPath + newFileName;

                                    // Move original to back up location and remove from upload folder
                                    File.Move(localPath + newFileName, backupDestinationPath);

                                    // Inform the the file has been backed up and removed correctly 

                                    // Log messages to the text file for future debugging
                                    LMTF.LogMessagesToFile(logPath, "   3) File has been successfully backed up and removed from upload folder");
                                    LMTF.LogMessagesToFile(logPath, " ");

                                }
                                // If no, inform that the file was not transferred
                                else
                                {
                                    // Log messages to the text file for future debugging
                                    LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                                    LMTF.LogMessagesToFile(logPath, "File " + oldFileName + " was not correctly transferred");

                                    // Setup email to send from else when file is not transferred properly

                                    //Setup our date and time for error
                                    string currentLogDate = DateTime.Now.ToString("MMddyyyy");
                                    string currentLogTimeInsertMain = DateTime.Now.ToString("HH:mm:ss tt");
                                    string ErrorDate = string.Format("Date: {0}  Time: {1:G} <br>", currentLogDate, currentLogTimeInsertMain);

                                    // Setup our email body and message
                                    string subjectstring = "SFTP File Transfer Error: File " + oldFileName + " was not correctly transferred";
                                    string bodystring = "SFTP File Transfer Error: File " + oldFileName + " was not correctly transferred <br>" +
                                                        ErrorDate +
                                                        "Error Message: <br>" +
                                                        "File not correctly transferred. Check the upload folder to see if the files to upload have the timestamp added already" + "<br>";

                                    // Call method to send our error as an email to developers maintaining sites
                                    EmailErrorNotify.CreateMessage(subjectstring, bodystring);

                                    // Log that the email method has been called
                                    LMTF.LogMessagesToFile(logPath, $"SFTP Noncorrectly Transferred Error send email method called");

                                    // This logs Email sent successfully flag response 
                                    LMTF.LogMessagesToFile(logPath, $"Email sent successfully: {EmailErrorNotify.checkFlag}");

                                    LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                                    LMTF.LogMessagesToFile(logPath, " ");
                                }

                            }// Close outer try block
                            catch (Exception e)
                            {
                                // Log messages to the text file for future debugging
                                LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                                LMTF.LogMessagesToFile(logPath, "Failed to upload file: " + oldFileName);
                                LMTF.LogMessagesToFile(logPath, "Error: " + e);

                                // Setup email to send from catch block

                                //Setup our date and time for error
                                string currentLogDate = DateTime.Now.ToString("MMddyyyy");
                                string currentLogTimeInsertMain = DateTime.Now.ToString("HH:mm:ss tt");
                                string ErrorDate = string.Format("Date: {0}  Time: {1:G} <br>", currentLogDate, currentLogTimeInsertMain);

                                // Setup our email body and message
                                string subjectstring = "SFTP Process Error: Failed to upload file: " + oldFileName;
                                string bodystring = "SFTP Process Error: Failed to upload file: " + oldFileName + "<br>" +
                                                    ErrorDate +
                                                    "Error Message: <br>" +
                                                    e + "<br>";

                                // Call method to send our error as an email to developers maintaining sites
                                EmailErrorNotify.CreateMessage(subjectstring, bodystring);

                                // Log that the email method has been called
                                LMTF.LogMessagesToFile(logPath, $"SFTP Failed to Upload File Error send email method called");

                                // This logs Email sent successfully flag response 
                                LMTF.LogMessagesToFile(logPath, $"Email sent successfully: {EmailErrorNotify.checkFlag}");

                                LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                                LMTF.LogMessagesToFile(logPath, " ");

                            }

                            // Increase counter to inform we are working on the next file in the Files list
                            i++;

                        } // Close foreach loop

                    } // Close IF statement 

                } // Close using block

                return;

            } // Close try block
            catch (Exception e)
            {
                // Log messages to the text file for future debugging
                LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                LMTF.LogMessagesToFile(logPath, "Process Failed: ");
                LMTF.LogMessagesToFile(logPath, "Error: " + e);

                // Setup email to send from catch block

                //Setup our date and time for error
                string currentLogDate = DateTime.Now.ToString("MMddyyyy");
                string currentLogTimeInsertMain = DateTime.Now.ToString("HH:mm:ss tt");
                string ErrorDate = string.Format("Date: {0}  Time: {1:G} <br>", currentLogDate, currentLogTimeInsertMain);

                // Setup our email body and message
                string subjectstring = "SFTP Process Error: Process Failed";
                string bodystring = "SFTP Process Error: Process Failed <br>" +
                                    ErrorDate +
                                    "Error Message: <br>" +
                                    e + "<br>";

                // Call method to send our error as an email to developers maintaining sites
                EmailErrorNotify.CreateMessage(subjectstring, bodystring);

                // Log that the email method has been called
                LMTF.LogMessagesToFile(logPath, $"SFTP Process Failed Error send email method called");
                
                // This logs Email sent successfully flag response 
                LMTF.LogMessagesToFile(logPath, $"Email sent successfully: {EmailErrorNotify.checkFlag}");

                LMTF.LogMessagesToFile(logPath, "//////////////////////////////////////////////////////////////////////////////");
                LMTF.LogMessagesToFile(logPath, " ");

                return;
            }

        } // Close Main

    } // Close Program

} // Close Namespace
