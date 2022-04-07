using System;
using System.IO;
using CommandLine;
using Newtonsoft.Json;
using SwaggerDownload.Project;

namespace SwaggerDownload
{
    internal static class Program
    {
        private class Options
        {
            [Option('p', "Project", Required = true, HelpText = "Specify a project file")]
            public string ProjectFile { get; set; }
        }

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async o =>
                {
                    // Retrieve project
                    if (!File.Exists(o.ProjectFile))
                    {
                        Console.WriteLine($"Unable to read SdkGenerator project file: {o.ProjectFile}");
                        return;
                    }
                    var text = await File.ReadAllTextAsync(o.ProjectFile);
                    var project = JsonConvert.DeserializeObject<ProjectSchema>(text);
                    
                    // Fetch the environment and version number
                    var api = await DownloadFile.GenerateApi(project);
                    Console.WriteLine($"Retrieving swagger file from {project?.SwaggerUrl} (version {api.Semver2})...");

                    // Let's do some software development kits!
                    await TypescriptSdk.Export(project, api);
                    await CSharpSdk.Export(project, api);
                    await JavaSdk.Export(project, api);
                    await RubySdk.Export(project, api);
                    await PythonSdk.Export(project, api);
                    Console.WriteLine("Exported SDKs.");

                    // Do we want to upload data models to readme?
                    if (project?.Readme != null)
                    {
                        Console.WriteLine("Uploading data models to Readme...");
                        await ReadmeUpload.UploadSchemas(api, project?.Readme?.ApiKey, "list");
                        Console.WriteLine("Uploaded data models to Readme.");
                    }
                }).Wait();
        }
    }
}