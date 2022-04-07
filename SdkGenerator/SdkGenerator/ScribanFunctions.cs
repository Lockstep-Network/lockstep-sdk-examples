using System;
using System.IO;
using System.Threading.Tasks;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;
using SwaggerDownload.Project;

namespace SwaggerDownload
{
    public static class ScribanFunctions
    {
        public static async Task ExecuteTemplate(string templateName, ProjectSchema project, ApiSchema api, string outputFile)
        {
            try
            {
                var templateText = await File.ReadAllTextAsync(templateName);
                var template = Template.Parse(templateText);
                var scriptObject1 = new ScriptObject();
                scriptObject1.Import(typeof(StringExtensions));
                var context = new TemplateContext();
                context.PushGlobal(scriptObject1);
                context.SetValue(new ScriptVariableGlobal("api"), api);
                context.SetValue(new ScriptVariableGlobal("project"), project);
                var result = await template.RenderAsync(context);
                await File.WriteAllTextAsync(outputFile, result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}