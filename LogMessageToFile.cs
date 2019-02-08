/***********************************************************************************************************************************
*                                                 GOD First                                                                        *
* Author: Dustin Ledbetter                                                                                                         *
* Release Date: 11-19-2018                                                                                                         *
* Last Edited:  01-29-2019                                                                                                         *
* Version: 1.0                                                                                                                     *
* Purpose: Called when a logging event has been setup in other methods for actions occurring while the code is running             *
************************************************************************************************************************************/

using System;
using System.IO;

namespace SFTP_Process
{
    class LogMessageToFile
    {

        #region  |--This Method is used to write all of our logs to a txt file--|

        public void LogMessagesToFile(string logPath, string msg)
        {
            // Get the Date and time stamps as desired
            string currentLogDate = DateTime.Now.ToString("MMddyyyy");
            string currentLogTimeInsertMain = DateTime.Now.ToString("HH:mm:ss tt");

            // Setup Message to display in .txt file 
            msg = string.Format("Time: {0:G}:  Message: {1}{2}", currentLogTimeInsertMain, msg, Environment.NewLine);

            // Add message to the file 
            File.AppendAllText(logPath + "SFTP_Process_Logs_" + currentLogDate + ".txt", msg);
        }

        #endregion

    } // End of the class: LogMessageToFile

} // End of file
