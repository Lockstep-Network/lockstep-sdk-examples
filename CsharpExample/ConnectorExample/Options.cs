using CommandLine;

namespace LockstepExamples
{
    public class Options
    {
        [Option('h', "hostname", Required = true, HelpText = "Hostname of the SFTP server")]
        public string Hostname { get; set; }

        [Option('u', "username", Required = false, HelpText = "Username to use when connecting to SFTP")]
        public string Username { get; set; }

        [Option('p', "password", Required = false, HelpText = "Password to use when connecting to SFTP")]
        public string Password { get; set; }

        [Option('n', "portnumber", Required = true, HelpText = "Port number of the SFTP server")]
        public int PortNumber { get; set; }

        [Option('f', "keyfile", Required = false,
            HelpText = "Full path of a file containing a PEM private key")]
        public string KeyFile { get; set; }

        [Option('s', "serverUrl", Required = true, HelpText = "The URL of the Lockstep Platform API server")]
        public string ServerUrl { get; set; }

        [Option('a', "apiKey", Required = true,
            HelpText = "The API key to use when contacting the Lockstep Platform API server")]
        public string ApiKey { get; set; }
    }
}