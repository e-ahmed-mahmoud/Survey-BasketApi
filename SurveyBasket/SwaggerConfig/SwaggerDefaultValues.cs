using System.Text.Json;
using System.Text.Json.Nodes; // Required for JsonNode
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SurveyBasket.SwaggerConfig;


public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        // 1. IsDeprecated is now a method/property check depending on version
        operation.Deprecated |= apiDescription.IsDeprecated;

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .FirstOrDefault(p => p.Name == parameter.Name);

            if (description == null) continue;

            // 1. Cast to concrete OpenApiParameter to set Description and Required
            if (parameter is OpenApiParameter concreteParameter)
            {
                concreteParameter.Description ??= description.ModelMetadata?.Description;
                concreteParameter.Required |= description.IsRequired;
            }

            // 2. Cast the Schema to concrete OpenApiSchema to set Default
            if (parameter.Schema is OpenApiSchema concreteSchema &&
                concreteSchema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue != DBNull.Value) // Add this check
            {
                try
                {
                    // Use the underlying type for Nullable types if possible
                    var type = description.ModelMetadata?.ModelType ?? description.DefaultValue.GetType();

                    var jsonString = JsonSerializer.Serialize(description.DefaultValue, type);
                    concreteSchema.Default = JsonNode.Parse(jsonString);
                }
                catch
                {
                    continue;
                }
            }

        }


    }
}
