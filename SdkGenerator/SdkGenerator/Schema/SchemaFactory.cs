﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SdkGenerator.Schema;

public static class SchemaFactory
{
    public static SchemaItem MakeSchema(JsonProperty jsonSchema)
    {
        if (jsonSchema.Value.TryGetProperty("properties", out var schemaPropertiesElement))
        {
            // Basic schema
            var item = new SchemaItem
            {
                Name = jsonSchema.Name,
                // Handle fields
                Fields = new List<SchemaField>()
            };

            // The fetch result generic objects don't have their own description
            if (!item.Name.EndsWith("FetchResult"))
            {
                item.DescriptionMarkdown = SafeGetPropString(jsonSchema.Value, "description");
            }

            foreach (var prop in schemaPropertiesElement.EnumerateObject())
            {
                var field = new SchemaField
                {
                    Name = prop.Name
                };

                // Let's parse and cleanse the data type in more detail
                var typeRef = GetTypeRef(prop);
                field.DataType = typeRef.DataType;
                field.DataTypeRef = typeRef.DataTypeRef;
                field.IsArray = typeRef.IsArray;

                // Attributes about the field
                field.Nullable = GetBooleanElement(prop.Value, "nullable");
                field.Deprecated = GetBooleanElement(prop.Value, "deprecated");
                field.ReadOnly = GetBooleanElement(prop.Value, "readOnly");
                field.MinLength = GetIntElement(prop.Value, "minLength");
                field.MaxLength = GetIntElement(prop.Value, "maxLength");
                field.DescriptionMarkdown = SafeGetPropString(prop.Value, "description");
                item.Fields.Add(field);
            }

            if (IsValidModel(item))
            {
                return item;
            }
        }
        else if (jsonSchema.Value.TryGetProperty("enum", out var enumPropertiesElement))
        {
            // Basic schema
            var item = new SchemaItem
            {
                Name = jsonSchema.Name,
                Fields = null,
                Enums = new List<int>()
            };
            item.DescriptionMarkdown = SafeGetPropString(jsonSchema.Value, "description");
            item.EnumType = SafeGetPropString(jsonSchema.Value, "type");
            foreach (var value in enumPropertiesElement.EnumerateArray())
            {
                if (value.ValueKind == JsonValueKind.Number)
                {
                    item.Enums.Add(value.GetInt32());
                }
            }

            if (IsValidModel(item) && !string.IsNullOrWhiteSpace(item.DescriptionMarkdown))
            {
                return item;
            }
        }

        return null;
    }

    private static int? GetIntElement(JsonElement propValue, string propertyName)
    {
        if (propValue.TryGetProperty(propertyName, out var element))
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                return element.GetInt32();
            }
        }

        return null;
    }

    private static bool GetBooleanElement(JsonElement propValue, string propertyName)
    {
        if (propValue.TryGetProperty(propertyName, out var element))
        {
            return element.GetBoolean();
        }

        return false;
    }

    private static string SafeGetPropString(JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out var prop))
        {
            return prop.GetString() ?? "";
        }

        Console.WriteLine($"Missing {name} on element: {element}");
        return "";
    }

    private static SchemaRef GetTypeRef(JsonProperty prop)
    {
        // Is this a core type?
        if (prop.Value.TryGetProperty("type", out var typeElement))
        {
            var rawType = typeElement.GetString();
            if (string.Equals(rawType, "array"))
            {
                foreach (var innerType in prop.Value.EnumerateObject())
                {
                    if (innerType.NameEquals("items"))
                    {
                        var innerSchemaRef = GetTypeRef(innerType);
                        innerSchemaRef.IsArray = true;
                        return innerSchemaRef;
                    }
                }

                return null;
            }

            prop.Value.TryGetProperty("format", out var formatProp);
            if (formatProp.ValueKind == JsonValueKind.String)
            {
                rawType = formatProp.GetString();
            }

            return new SchemaRef
            {
                DataType = rawType
            };
        }

        if (prop.Value.TryGetProperty("$ref", out var refElement))
        {
            var refType = refElement.GetString();
            if (refType != null)
            {
                return MakeClassRef(refType, false);
            }
        }

        // Is this an "All Of" combined element?  If so, look through all children for a $ref
        if (prop.Value.TryGetProperty("allOf", out var allOfElement))
        {
            foreach (var subProp in allOfElement.EnumerateArray())
            {
                if (subProp.TryGetProperty("$ref", out var subRefElement))
                {
                    var refType = subRefElement.GetString();
                    if (refType != null)
                    {
                        return MakeClassRef(refType, false);
                    }
                }
            }
        }

        Console.WriteLine($"Missing type: {prop}");
        return new SchemaRef
        {
            DataType = "object",
        };
    }

    private static SchemaRef MakeClassRef(string refType, bool isArray)
    {
        var classname = refType.Substring(refType.LastIndexOf("/", StringComparison.Ordinal) + 1);
        return new SchemaRef
        {
            DataType = classname,
            DataTypeRef = $"/docs/{classname.ToLower()}",
            IsArray = isArray,
        };
    }

    public static List<EndpointItem> MakeEndpoint(JsonProperty prop)
    {
        var items = new List<EndpointItem>();
        var path = prop.Name;
        foreach (var endpointProp in prop.Value.EnumerateObject())
        {
            var item = new EndpointItem
            {
                Parameters = new List<ParameterField>(),
                Path = path,
                Method = endpointProp.Name,
                Name = SafeGetPropString(endpointProp.Value, "summary"),
                DescriptionMarkdown = SafeGetPropString(endpointProp.Value, "description")
            };
            items.Add(item);

            // Determine category
            endpointProp.Value.TryGetProperty("tags", out var tags);
            item.Category = tags.ValueKind == JsonValueKind.Array
                ? tags.EnumerateArray().FirstOrDefault().GetString()
                : "Utility";

            // Determine if deprecated
            endpointProp.Value.TryGetProperty("deprecated", out var deprecatedProp);
            item.Deprecated = deprecatedProp.ValueKind == JsonValueKind.True;

            // Parse parameters
            endpointProp.Value.TryGetProperty("parameters", out var parameterListProp);
            if (parameterListProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var paramProp in parameterListProp.EnumerateArray())
                {
                    var p = new ParameterField();
                    item.Parameters.Add(p);
                    p.Name = SafeGetPropString(paramProp, "name");
                    p.Location = SafeGetPropString(paramProp, "in");
                    p.DescriptionMarkdown = SafeGetPropString(paramProp, "description");

                    // Parse the field's required status
                    paramProp.TryGetProperty("required", out var requiredProp);
                    p.Required = requiredProp.ValueKind == JsonValueKind.True;

                    // Parse the field's schema
                    foreach (var paramSchemaProp in paramProp.EnumerateObject())
                    {
                        if (paramSchemaProp.NameEquals("schema"))
                        {
                            var schemaRef = GetTypeRef(paramSchemaProp);
                            p.DataType = schemaRef.DataType;
                            p.DataTypeRef = schemaRef.DataTypeRef;
                            p.IsArray = schemaRef.IsArray;
                        }
                    }
                }
            }

            // Parse the request body parameter
            endpointProp.Value.TryGetProperty("requestBody", out var requestBodyProp);
            if (requestBodyProp.ValueKind == JsonValueKind.Object)
            {
                requestBodyProp.TryGetProperty("content", out var requestBodyContentProp);
                foreach (var encodingProp in requestBodyContentProp.EnumerateObject())
                {
                    if (encodingProp.Name == "application/json")
                    {
                        var p = new ParameterField
                        {
                            Name = "body",
                            Location = "body",
                            DescriptionMarkdown = SafeGetPropString(requestBodyProp, "description"),
                            Required = true,
                        };
                        item.Parameters.Add(p);
                        foreach (var innerSchemaProp in encodingProp.Value.EnumerateObject())
                        {
                            if (innerSchemaProp.NameEquals("schema"))
                            {
                                var typeRef = GetTypeRef(innerSchemaProp);
                                p.DataType = typeRef.DataType;
                                p.DataTypeRef = typeRef.DataTypeRef;
                                p.IsArray = typeRef.IsArray;
                            }
                        }
                    }
                    else if (encodingProp.Name == "multipart/form-data")
                    {
                        item.Parameters.Add(new ParameterField
                        {
                            Name = "filename",
                            Location = "form",
                            DescriptionMarkdown = "The full path of a file to upload to the API",
                            Required = true,
                            DataType = "File",
                            DataTypeRef = "File",
                            IsArray = false,
                        });
                    }
                }
            }

            // Parse the "success" response type
            endpointProp.Value.TryGetProperty("responses", out var responsesProp);
            foreach (var response in responsesProp.EnumerateObject())
            {
                if (response.Name.StartsWith("2"))
                {
                    response.Value.TryGetProperty("content", out var contentProp);
                    contentProp.TryGetProperty("application/json", out var appJsonProp);
                    foreach (var responseSchemaProp in appJsonProp.EnumerateObject())
                    {
                        item.ReturnDataType = GetTypeRef(responseSchemaProp);

                        break;
                    }
                }
            }
        }

        return items;
    }

    private static bool IsValidModel(SchemaItem item)
    {
        var name = item.Name;
        return !name.EndsWith("Argument")
               && !name.EndsWith("Attribute")
               && !name.EndsWith("Base")
               && !name.EndsWith("Exception")
               && !name.EndsWith("FetchResult")
               && !name.EndsWith("Handle")
               && !name.EndsWith("Info")
               && !string.Equals(name, "ActionResultModel")
               && !string.Equals(name, "Assembly")
               && !string.Equals(name, "CustomAttributeData")
               && !string.Equals(name, "Module")
               && !string.Equals(name, "MemberBase")
               && !string.Equals(name, "MethodBase")
               && !string.Equals(name, "ProblemDetails")
               && !string.Equals(name, "Type");
    }
}