using System;
using System.Collections.Generic;
using System.IO;
using Renci.SshNet;

namespace LockstepExamples
{

    public class FileDownloader
    {
        private readonly ConnectionInfo _connection;

        /// <summary>
        /// A connection to a remote site via SFTP
        /// </summary>
        /// <param name="hostname">The DNS name of the SFTP host</param>
        /// <param name="user">The username to use when connecting</param>
        /// <param name="pw">The password to use when connecting</param>
        /// <param name="port">The port number to use</param>
        /// <param name="keyFile">The full path of a file to use </param>
        public FileDownloader(string hostname, string user, string pw, int port, string keyFile)
        {
            if (!File.Exists(keyFile))
            {
                throw new Exception($"Unable to find keyfile: {keyFile}");
            }
            
            // Set up the connection
            var key = new PrivateKeyFile(keyFile);
            var methods = new List<AuthenticationMethod>
            {
                new PasswordAuthenticationMethod(user, pw),
                new PrivateKeyAuthenticationMethod(user, new[] { key })
            };
            this._connection = new ConnectionInfo(hostname, port, user, methods.ToArray());
        }

        /// <summary>
        /// Connects to the SFTPServer and downloads all XML files
        /// </summary>
        public List<string> RetrieveFiles()
        {
            var files = new List<string>();
            using (var client = new SftpClient(this._connection))
            {
                client.Connect();

                // Look for all files within the SFTP server
                foreach (var file in client.ListDirectory("/"))
                {
                    if (file.Name.EndsWith(".xml"))
                    {
                        var tempFileName = Path.GetTempFileName();
                        using (var s = File.Open(tempFileName, FileMode.OpenOrCreate))
                        {
                            client.DownloadFile(file.FullName, s);
                        }

                        files.Add(tempFileName);
                    }
                }

                client.Disconnect();
            }

            return files;
        }
    }
}