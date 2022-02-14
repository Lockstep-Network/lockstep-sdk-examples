using System;
using System.Collections.Generic;
using System.IO;
using LockstepSDK;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace LockstepExamples
{

    public class FileDownloader
    {
        private string _hostname;
        private string _username;
        private string _password;
        private int _port;
        private string _keyFile;
        private ConnectionInfo _connection;

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
            this._hostname = hostname;
            this._username = user;
            this._password = pw;
            this._port = port;
            this._keyFile = keyFile;
            if (!File.Exists(keyFile))
            {
                throw new Exception($"Unable to find keyfile: {keyFile}");
            }
            
            // Set up the connection
            var kfile = new PrivateKeyFile(this._keyFile);
            var methods = new List<AuthenticationMethod>
            {
                new PasswordAuthenticationMethod(this._username, this._password),
                new PrivateKeyAuthenticationMethod(user, new[] { kfile })
            };
            this._connection = new ConnectionInfo(this._hostname, this._port, this._username, methods.ToArray());
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