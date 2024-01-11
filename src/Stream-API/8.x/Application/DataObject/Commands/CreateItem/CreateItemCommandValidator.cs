using System.Runtime;
using Application.Interfaces;
using Azure.Core;
using FluentValidation;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Application.DataObject.Commands.CreateDataObject;

public class CreateItemCommandValidator : AbstractValidator<CreateDataObjectCommand>
{
    private readonly ICosmosSettings _settings;

    public CreateItemCommandValidator(ICosmosSettings settings)
    {
        _settings = settings;

        RuleFor(v => v.Data)
        .Custom((data, context) =>
        {
            var containerId = (context.InstanceToValidate)?.ContainerId;
            if (containerId == null)
            {
                context.AddFailure("ContainerId is required.");
                return;
            }

            if (!ValidateData(data, containerId, out IList<string> errorMessages))
            {
                foreach (var error in errorMessages)
                {
                    context.AddFailure(error);
                }
            }
        });
    }


    private bool ValidateData(string data, string containerId, out IList<string> errorMessages)
    {
        var container = _settings.Containers.FirstOrDefault(a => a.ContainerId == containerId)
            ?? throw new ArgumentException("ContainerId not found.");

        string jsonSchema = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", container.ValidationScheme));
        JSchema schema = JSchema.Parse(jsonSchema);
        JToken result = JToken.Parse(data);

        bool isValid = result.IsValid(schema, out errorMessages);

        return isValid;
    }
}
