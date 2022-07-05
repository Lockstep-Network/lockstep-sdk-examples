﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdkGenerator.Schema;

namespace SdkGenerator.Sdks;

public static class JavaSdk
{
    private static string FileHeader(ProjectSchema project)
    {
        return "\n/**\n"
               + $" * {project.ProjectName} for Java\n"
               + " *\n"
               + $" * (c) 2021-{DateTime.UtcNow.Year} {project.CopyrightHolder}\n"
               + " *\n"
               + " * For the full copyright and license information, please view the LICENSE\n"
               + " * file that was distributed with this source code.\n"
               + " *\n"
               + $" * @author     {project.AuthorName} <{project.AuthorEmail}>\n"
               + $" * @copyright  2021-{DateTime.UtcNow.Year} {project.CopyrightHolder}\n"
               + $" * @link       {project.Java.GithubUrl}\n"
               + " */\n\n";
    }

    private static string JavaTypeName(ApiSchema api, string typeName, bool isArray)
    {
        var s = typeName;
        if (api.IsEnum(typeName))
        {
            s = api.FindSchema(typeName).EnumType;
        }

        switch (s)
        {
            case "string":
            case "uuid":
            case "date-time":
            case "date":
            case "uri":
            case "Uri":
            case "tel":
            case "email":
                s = "String";
                break;
            case "int32":
            case "integer":
                s = "Integer";
                break;
            case "double":
                s = "Double";
                break;
            case "int64":
                s = "Long";
                break;
            case "float":
                s = "Float";
                break;
            case "object":
                s = "Object";
                break;
            case "boolean":
                s = "Boolean";
                break;
            case "File":
            case "binary":
                s = "byte[]";
                break;
            case "TestTimeoutException":
                s = "ErrorResult";
                break;
        }

        if (isArray)
        {
            s += "[]";
        }

        if (s.EndsWith("FetchResult"))
        {
            s = $"FetchResult<{s[..^11]}>";
        }

        return s;
    }

    private static string FixupType(ApiSchema api, string typeName, bool isArray, bool nullable)
    {
        var s = JavaTypeName(api, typeName, isArray);
        return nullable ? $"@Nullable {s}" : $"@NotNull {s}";
    }

    private static async Task ExportSchemas(ProjectSchema project, ApiSchema api)
    {
        var modelsDir = Path.Combine(project.Java.Folder, "src", "main", "java",
            project.Java.Namespace.Replace('.', Path.DirectorySeparatorChar), "models");
        Directory.CreateDirectory(modelsDir);
        foreach (var modelFile in Directory.EnumerateFiles(modelsDir, "*.java"))
        {
            File.Delete(modelFile);
        }

        foreach (var item in api.Schemas)
        {
            if (item.Fields != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(FileHeader(project));
                sb.AppendLine($"package {project.Java.Namespace}.models;");
                sb.AppendLine();
                foreach (var import in GetImports(project, api, item))
                {
                    if (!string.IsNullOrWhiteSpace(import) &&
                        !import.StartsWith($"import {project.Java.Namespace}.models."))
                    {
                        sb.AppendLine(import);
                    }
                }

                sb.AppendLine("import org.jetbrains.annotations.NotNull;");
                sb.AppendLine("import org.jetbrains.annotations.Nullable;");

                // Add class and header
                sb.AppendLine();
                sb.Append(item.DescriptionMarkdown.ToJavaDoc(0));
                sb.AppendLine($"public class {item.Name}");
                sb.AppendLine("{");

                // First do the fields
                foreach (var field in item.Fields)
                {
                    if (!field.Deprecated)
                    {
                        // This is the field; we collect all fields
                        sb.AppendLine(
                            $"    private {FixupType(api, field.DataType, field.IsArray, field.Nullable)} {field.Name.ToCamelCase()};");
                    }
                }

                // Next all the getters/setters
                sb.AppendLine();
                foreach (var field in item.Fields)
                {
                    if (!field.Deprecated)
                    {
                        // For whatever reason, Java wants the field description to be the "return" value of the getter
                        sb.Append(field.DescriptionMarkdown.ToJavaDoc(4, "The field " + field.Name));
                        sb.AppendLine(
                            $"    public {FixupType(api, field.DataType, field.IsArray, field.Nullable)} get{field.Name.ToProperCase()}() {{ return this.{field.Name.ToCamelCase()}; }}");

                        // For whatever reason, Java wants the field description to be the "value" param of the setter
                        var pf = new ParameterField
                        {
                            Name = "value",
                            DescriptionMarkdown = "The new value for " + field.Name
                        };
                        sb.Append(field.DescriptionMarkdown.ToJavaDoc(4, null, new List<ParameterField> { pf }));
                        sb.AppendLine(
                            $"    public void set{field.Name.ToProperCase()}({FixupType(api, field.DataType, field.IsArray, field.Nullable)} value) {{ this.{field.Name.ToCamelCase()} = value; }}");
                    }
                }

                sb.AppendLine("};");
                var classPath = Path.Combine(modelsDir, item.Name + ".java");
                await File.WriteAllTextAsync(classPath, sb.ToString());
            }
        }
    }

    private static async Task ExportEndpoints(ProjectSchema project, ApiSchema api)
    {
        var clientsDir = Path.Combine(project.Java.Folder, "src", "main", "java",
            project.Java.Namespace.Replace('.', Path.DirectorySeparatorChar), "clients");
        Directory.CreateDirectory(clientsDir);
        foreach (var clientsFile in Directory.EnumerateFiles(clientsDir, "*.java"))
        {
            File.Delete(clientsFile);
        }

        // Gather a list of unique categories
        foreach (var cat in api.Categories)
        {
            var sb = new StringBuilder();

            // Construct header
            sb.AppendLine(FileHeader(project));
            sb.AppendLine($"package {project.Java.Namespace}.clients;");
            sb.AppendLine();
            sb.AppendLine($"import {project.Java.Namespace}.{project.Java.ClassName};");
            sb.AppendLine($"import {project.Java.Namespace}.RestRequest;");
            sb.AppendLine($"import {project.Java.Namespace}.{project.Java.ResponseClass};");
            sb.AppendLine("import org.jetbrains.annotations.NotNull;");
            sb.AppendLine("import org.jetbrains.annotations.Nullable;");
            foreach (var import in GetImports(project, api, cat))
            {
                sb.AppendLine(import);
            }

            sb.AppendLine();
            sb.AppendLine("/**");
            sb.AppendLine($" * Contains all methods related to {cat}");
            sb.AppendLine(" */");
            sb.AppendLine($"public class {cat}Client");
            sb.AppendLine("{");
            sb.AppendLine($"    private {project.Java.ClassName} client;");
            sb.AppendLine();
            sb.AppendLine("    /**");
            sb.AppendLine($"     * Constructor for the {cat} API collection");
            sb.AppendLine("     *");
            sb.AppendLine(
                $"     * @param client A {{@link {project.Java.Namespace}.{project.Java.ClassName}}} platform client");
            sb.AppendLine("     */");
            sb.AppendLine($"    public {cat}Client(@NotNull {project.Java.ClassName} client) {{");
            sb.AppendLine("        super();");
            sb.AppendLine("        this.client = client;");
            sb.AppendLine("    }");

            // Run through all APIs
            foreach (var endpoint in api.Endpoints)
            {
                if (endpoint.Category == cat && !endpoint.Deprecated)
                {
                    sb.AppendLine();
                    sb.Append(endpoint.DescriptionMarkdown.ToJavaDoc(4,
                        $"A {{@link {project.Java.Namespace}.models.{project.Java.ResponseClass}}} containing the results",
                        endpoint.Parameters));

                    // Figure out the parameter list
                    var paramListStr = string.Join(", ", from p in endpoint.Parameters
                        select $"{FixupType(api, p.DataType, p.IsArray, !p.Required)} {p.Name}");

                    // What is our return type?
                    var returnType = JavaTypeName(api, endpoint.ReturnDataType.DataType,
                        endpoint.ReturnDataType.IsArray);
                    var requestType = returnType == "byte[]" ? "BlobRequest" : $"RestRequest<{returnType}>";

                    // Write the method
                    sb.AppendLine(
                        $"    public @NotNull {project.Java.ResponseClass}<{returnType}> {endpoint.Name.ToCamelCase()}({paramListStr})");
                    sb.AppendLine("    {");
                    sb.AppendLine(
                        $"        {requestType} r = new {requestType}(this.client, \"{endpoint.Method.ToUpper()}\", \"{endpoint.Path}\");");

                    // Add parameters options
                    foreach (var o in endpoint.Parameters)
                    {
                        switch (o.Location)
                        {
                            case "body":
                                sb.AppendLine("        r.AddBody(body);");
                                break;
                            case "query":
                                sb.AppendLine($"        r.AddQuery(\"{o.Name}\", {o.Name}.toString());");
                                break;
                            case "path":
                                sb.AppendLine($"        r.AddPath(\"{{{o.Name}}}\", {o.Name}.toString());");
                                break;
                            case "form":
                                break;
                            default:
                                throw new Exception("Unknown location " + o.Location);
                        }
                    }

                    // Execute the request
                    if (returnType.Contains("FetchResult"))
                    {
                        sb.AppendLine($"        return r.Call(new TypeToken<{returnType}>() {{}}.getType());");
                    }
                    else if (returnType == "byte[]")
                    {
                        sb.AppendLine("        return r.Call();");
                    }
                    else
                    {
                        sb.AppendLine($"        return r.Call({returnType}.class);");
                    }

                    sb.AppendLine("    }");
                }
            }

            // Close out the namespace
            sb.AppendLine("}");

            // Write this category to a file
            var classPath = Path.Combine(clientsDir, $"{cat}Client.java");
            await File.WriteAllTextAsync(classPath, sb.ToString());
        }
    }

    private static void AddImport(ApiSchema api, string name, HashSet<string> list)
    {
        if (name.EndsWith("FetchResult"))
        {
            list.Add("FetchResult");
            list.Add("type-token");

            var innerType = name[..^11];
            AddImport(api, innerType, list);
        }
        else if (name.Equals("TestTimeoutException"))
        {
            list.Add("ErrorResult");
        }
        else if (!api.IsEnum(name))
        {
            list.Add(name);
        }
    }

    private static List<string> GetImports(ProjectSchema project, ApiSchema api, string category)
    {
        var types = new HashSet<string>();
        foreach (var endpoint in api.Endpoints)
        {
            if (endpoint.Category == category && !endpoint.Deprecated)
            {
                AddImport(api, endpoint.ReturnDataType.DataType, types);
                foreach (var p in endpoint.Parameters)
                {
                    AddImport(api, p.DataType, types);
                }
            }
        }

        // Deduplicate the list and generate import statements
        return (from t in types select GetImportForType(project, t)).Distinct().ToList();
    }

    private static List<string> GetImports(ProjectSchema project, ApiSchema api, SchemaItem schema)
    {
        var types = new HashSet<string>();
        foreach (var field in schema.Fields)
        {
            if (!field.Deprecated)
            {
                AddImport(api, field.DataType, types);
            }
        }

        // Deduplicate the list and generate import statements
        return (from t in types select GetImportForType(project, t)).Distinct().ToList();
    }

    private static string GetImportForType(ProjectSchema project, string type)
    {
        switch (type)
        {
            case "string":
            case "uuid":
            case "object":
            case "int32":
            case "boolean":
            case "double":
            case "array":
            case "email":
            case "uri":
            case "float":
            case "date":
            case "date-time":
                return null;
            case "type-token":
                return "import com.google.gson.reflect.TypeToken;";
            case "binary":
            case "File":
            case "byte[]":
                return $"import {project.Java.Namespace}.BlobRequest;";
            case "FetchResult":
                return $"import {project.Java.Namespace}.{type};";
            default:
                return $"import {project.Java.Namespace}.models.{type};";
        }
    }

    public static async Task Export(ProjectSchema project, ApiSchema api)
    {
        if (project.Java == null)
        {
            return;
        }

        await ExportSchemas(project, api);
        await ExportEndpoints(project, api);

        // Let's try using Scriban to populate these files
        await ScribanFunctions.ExecuteTemplate(
            Path.Combine(".", "templates", "java", "ApiClient.java.scriban"),
            project, api,
            Path.Combine(project.Java.Folder, "src", "main", "java",
                project.Java.Namespace.Replace('.', Path.DirectorySeparatorChar),
                project.Java.ClassName + ".java"));
        await Extensions.PatchFile(Path.Combine(project.Java.Folder, "pom.xml"),
            $"<artifactId>{project.Java.ModuleName.ToLower()}<\\/artifactId>\\s+<version>[\\d\\.]+<\\/version>",
            $"<artifactId>{project.Java.ModuleName.ToLower()}</artifactId>\r\n    <version>{api.Semver4}</version>");
        await Extensions.PatchFile(
            Path.Combine(project.Java.Folder, "src", "main", "java",
                project.Java.Namespace.Replace('.', Path.DirectorySeparatorChar), "RestRequest.java"),
            "request.addHeader\\(\"SdkVersion\", \"[\\d\\.]+\"\\);",
            $"request.addHeader(\"SdkVersion\", \"{api.Semver4}\");");
    }
}