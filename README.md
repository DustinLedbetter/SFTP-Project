# SFTP-Project; Start Date: 1-24-2019
A project that sets up  SFTP for files from a local location to a remote location with added timestamp, checks if successful, and then backs up original file to a backup location and removes from upload folder upon success.

--------------------
Technologies Used:
--------------------
Visual Studio utilizing C#, WinSCP, SFTP file transfers

--------------
Description:
--------------
I learned how SFTP works and created a project to achieve this with a file on my desktop. I first succeeded in getting the file to SFTP to the remote location. I then got checking in place to make sure the file was in fact transferred correctly. After that, I set up the code to then on success move the original file over to a back up folder as well. Once I had the core parts working I built in the logging feature and email upon error feature for future debugging and errors.

-----------------
What I learned:
-----------------
-I learned how SFTP works and how to accomplish this.
<br>
-I learned how to setup file checking and more about working with remote files

--------------------------
Complications or Issues:
--------------------------

--Complication: Only complication I cam into was in renaming the files during transfer to remote location. I was trying to add the date to the files and had an issue with not being able to get this done at the right time to prevent the file from overriding another file already on the remote server or backup folder
<br>
<br>
--Solution: I simply save the file name before I begin the transfer, change the name of the file, then transfer it as needed and set the name back to what it was if transfer fails.
