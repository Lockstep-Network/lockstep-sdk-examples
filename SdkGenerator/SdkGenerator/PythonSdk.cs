﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwaggerDownload.Project;

namespace SwaggerDownload
{
    public static class PythonSdk
    {
        private static string FileHeader(ProjectSchema project)
        {
            return "#\n"
                   + $"# {project.ProjectName} for Python\n"
                   + "#\n"
                   + $"# (c) {project.ProjectStartYear}-{DateTime.UtcNow.Year} {project.CopyrightHolder}\n"
                   + "#\n"
                   + "# For the full copyright and license information, please view the LICENSE\n"
                   + "# file that was distributed with this source code.\n"
                   + "#\n"
                   + $"# @author     {project.AuthorName} <{project.AuthorEmail}>\n"
                   + $"# @copyright  {project.ProjectStartYear}-{DateTime.UtcNow.Year} {project.CopyrightHolder}\n"
                   + $"# @link       {project.Python.GithubUrl}\n"
                   + "#\n\n";
        }


        private static string FixupType(ApiSchema api, string typeName, bool isArray)
        {
            var s = typeName;
            if (api.IsEnum(typeName))
            {
                s = api.FindSchema(typeName).EnumType;
            }
            switch (s)
            {
                case "uuid":
                case "string":
                case "uri":
                case "email":
                case "date-time":
                case "date":
                case "Uri":
                case "tel":
                case "TestTimeoutException":
                    s = "str";
                    break;
                case "int32":
                case "integer":
                case "int64":
                    s = "int";
                    break;
                case "double":
                    case "float":
                    s = "float";
                    break;
                case "boolean":
                    s = "bool";
                    break;
                case "File": // A "File" object is an uploadable file
                    s = "str";
                    break;
                case "binary":
                case "byte[]":
                    s = "Response";
                    break;
            }
            if (isArray)
            {
                s = "list[" + s + "]";
            }
            if (s.EndsWith("FetchResult"))
            {
                s = $"FetchResult[{s[..^11]}]";
            }

            return s;
        }

        private static async Task ExportSchemas(ProjectSchema project, ApiSchema api)
        {
            foreach (var item in api.Schemas)
            {
                if (item.Fields != null)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(FileHeader(project));

                    // The "Future" import apparently must be the very first one in the file
                    if (item.Fields.Any(f => f.DataType == item.Name))
                    {
                        sb.AppendLine("from __future__ import annotations");
                    }

                    // Add in all the rest of the imports
                    sb.AppendLine("from dataclasses import dataclass");
                    foreach (var f in item.Fields.Where(f => f.DataTypeRef != null && f.DataType != item.Name))
                    {
                        sb.AppendLine(
                            $"from {project.Python.ModuleName}.models.{f.DataType.ToSnakeCase()} import {f.DataType}");
                    }

                    sb.AppendLine();
                    sb.AppendLine("@dataclass");
                    sb.AppendLine($"class {item.Name}:");
                    sb.Append(MakePythonDoc(api, item.DescriptionMarkdown, 4, null));
                    sb.AppendLine();
                    foreach (var f in item.Fields)
                    {
                        sb.AppendLine($"    {f.Name}: {FixupType(api, f.DataType, f.IsArray)} | None = None");
                    }

                    sb.AppendLine();

                    var modelPath = Path.Combine(project.Python.Folder, $"src\\{project.Python.ModuleName}\\models",
                        item.Name.ToSnakeCase() + ".py");
                    await File.WriteAllTextAsync(modelPath, sb.ToString());
                }
            }
        }

        private static async Task ExportEndpoints(ProjectSchema project, ApiSchema api)
        {
            // Gather a list of unique categories
            foreach (var cat in api.Categories)
            {
                var sb = new StringBuilder();

                // Let's see if we have to do any imports
                var imports = BuildImports(project, api, cat);

                // Construct header
                sb.Append(FileHeader(project));
                sb.AppendLine($"from {project.Python.ModuleName}.{project.Python.ResponseClass.ProperCaseToSnakeCase()} import {project.Python.ResponseClass}");
                sb.AppendLine($"from {project.Python.ModuleName}.error_result import ErrorResult");
                foreach (var import in imports.Distinct())
                {
                    sb.AppendLine(import);
                }
                sb.AppendLine();
                sb.AppendLine($"class {cat}Client:");
                sb.AppendLine("    \"\"\"");
                sb.AppendLine($"    API methods related to {cat}");
                sb.AppendLine("    \"\"\"");
                sb.AppendLine($"    from {project.Python.ModuleName}.{project.Python.ClassName.ProperCaseToSnakeCase()} import {project.Python.ClassName}");
                sb.AppendLine();
                sb.AppendLine($"    def __init__(self, client: {project.Python.ClassName}):");
                sb.AppendLine("        self.client = client");

                // Run through all APIs
                foreach (var endpoint in api.Endpoints)
                {
                    if (endpoint.Category == cat && !endpoint.Deprecated)
                    {
                        sb.AppendLine();

                        // Is this a file download API?
                        var isFileDownload = endpoint.ReturnDataType.DataType is "byte[]" or "binary" or "File";
                        var originalReturnDataType = FixupType(api, endpoint.ReturnDataType.DataType, endpoint.ReturnDataType.IsArray);
                        string returnDataType;
                        if (!isFileDownload)
                        {
                            returnDataType = $"{project.Python.ResponseClass}[{originalReturnDataType}]";
                        }
                        else
                        {
                            returnDataType = "Response";
                        }

                        // Figure out the parameter list
                        var hasBody = (from p in endpoint.Parameters where p.Location == "body" select p).Any();
                        var paramListStr = String.Join(", ", from p in endpoint.Parameters select $"{p.Name}: {FixupType(api, p.DataType, p.IsArray)}");
                        var bodyJson = String.Join(", ", from p in endpoint.Parameters where p.Location == "query" select $"\"{p.Name}\": {p.Name}");
                        var fileUploadParam = (from p in endpoint.Parameters where p.Location == "form" select p).FirstOrDefault();

                        // Write the method
                        sb.AppendLine($"    def {endpoint.Name.ToSnakeCase()}(self, {paramListStr}) -> {returnDataType}:");
                        sb.Append(MakePythonDoc(api, endpoint.DescriptionMarkdown, 8, endpoint.Parameters));
                        sb.AppendLine(endpoint.Path.Contains('{')
                            ? $"        path = f\"{endpoint.Path}\""
                            : $"        path = \"{endpoint.Path}\"");
                        sb.AppendLine($"        result = self.client.send_request(\"{endpoint.Method.ToUpper()}\", path, {(hasBody ? "body" : "None")}, {(string.IsNullOrWhiteSpace(paramListStr) ? "None" : "{" + bodyJson + "}")}, {(fileUploadParam == null ? "None" : fileUploadParam.Name)})");
                        if (isFileDownload)
                        {
                            sb.AppendLine("        return result");
                        }
                        else
                        {
                            sb.AppendLine("        if result.status_code >= 200 and result.status_code < 300:");
                            sb.AppendLine($"            return {project.Python.ResponseClass}(True, result.status_code, {originalReturnDataType}(**result.json()), None)");
                            sb.AppendLine("        else:");
                            sb.AppendLine($"            return {project.Python.ResponseClass}(False, result.status_code, None, ErrorResult(**result.json()))");
                        }
                    }
                }

                // Write this category to a file
                var classPath = Path.Combine(project.Python.Folder, "src", project.Python.Namespace, "clients",
                    cat.ToSnakeCase() + "_client.py");
                await File.WriteAllTextAsync(classPath, sb.ToString());
            }
        }

        private static List<string> BuildImports(ProjectSchema project, ApiSchema api, string cat)
        {
            var imports = new List<string>();
            foreach (var endpoint in api.Endpoints)
            {
                if (endpoint.Category == cat && !endpoint.Deprecated)
                {
                    foreach (var p in endpoint.Parameters)
                    {
                        if (p.DataTypeRef != null)
                        {
                            AddImport(project, api, imports, p.DataType);
                        }
                    }

                    // The return type of a file download has special rules
                    if (endpoint.ReturnDataType.DataType is "File" or "byte[]" or "binary")
                    {
                        imports.Add($"from requests.models import Response");
                    }
                    else
                    {
                        AddImport(project, api, imports, endpoint.ReturnDataType.DataType);
                    }
                }
            }

            imports.Sort();
            return imports.Distinct().ToList();
        }

        private static void AddImport(ProjectSchema project, ApiSchema api, List<string> imports, string dataType)
        {
            if (api.IsEnum(dataType) || dataType is null or "TestTimeoutException" or "File" or "byte[]" or "binary")
            {
                return;
            }

            if (dataType is "ActionResultModel")
            {
                imports.Add($"from {project.Python.Namespace}.action_result_model import ActionResultModel");
            }
            else
            {
                if (dataType.EndsWith("FetchResult"))
                {
                    imports.Add($"from {project.Python.Namespace}.fetch_result import FetchResult");
                    dataType = dataType[..^11];
                }

                imports.Add($"from {project.Python.Namespace}.models.{dataType.ToSnakeCase()} import {dataType}");
            }
        }

        private static string MakePythonDoc(ApiSchema api, string description, int indent, List<ParameterField> parameters)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return "";
            }

            var sb = new StringBuilder();
            var prefix = "".PadLeft(indent);

            // According to some documentation, python "summary" lines can be on the line following the """.
            // Let's see if that works.
            sb.AppendLine($"{prefix}\"\"\"");

            // Remove the auto-generated text
            var pos = description.IndexOf("### ", StringComparison.Ordinal);
            if (pos > 0)
            {
                description = description[..pos];
            }

            // Wrap at 72 column width maximum
            sb.AppendLine(description.WrapMarkdown(72, prefix));

            // Add documentation for parameters
            if (parameters != null)
            {
                sb.AppendLine();
                sb.AppendLine($"{prefix}Parameters");
                sb.AppendLine($"{prefix}----------");
                foreach (var p in parameters)
                {
                    sb.AppendLine($"{prefix}{p.Name} : {FixupType(api, p.DataType, p.IsArray)}");
                    sb.AppendLine(p.DescriptionMarkdown.WrapMarkdown(72, $"{prefix}    "));
                }
            }
            sb.AppendLine($"{prefix}\"\"\"");

            return sb.ToString();
        }

        public static async Task Export(ProjectSchema project, ApiSchema api)
        {
            await ExportSchemas(project, api);
            await ExportEndpoints(project, api);

            // Let's try using Scriban to populate these files
            await ScribanFunctions.ExecuteTemplate(
                Path.Combine(".", "templates", "python", "ApiClient.py.scriban"),
                project, api,
                Path.Combine(project.Python.Folder, "src", project.Python.Namespace, project.Python.ClassName.ProperCaseToSnakeCase() + ".py"));
            await ScribanFunctions.ExecuteTemplate(
                Path.Combine(".", "templates", "python", "__init__.py.scriban"),
                project, api,
                Path.Combine(project.Python.Folder, "src", project.Python.Namespace, "__init__.py"));
            await StringExtensions.PatchFile(Path.Combine(project.Python.Folder, "setup.cfg"), "version = [\\d\\.]+",
                $"version = {api.Semver3}");
        }
    }
}