using Application.Interfaces;
using FluentValidation;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Application.DataObject.Commands.CreateDataObject;

public class CreateItemCommandValidator : AbstractValidator<CreateDataObjectCommand>
{
    private readonly ICosmosSettings _settings;

    public CreateItemCommandValidator(ICosmosSettings settings)
    {
        _settings = settings;

        RuleFor(v => v.Item)
        .Custom((item, context) =>
        {
            var containerId = (context.InstanceToValidate)?.ContainerId;
            if (containerId == null)
            {
                context.AddFailure("ContainerId is required.");
                return;
            }

            if (!ValidateData(item, containerId, out IList<string> errorMessages))
            {
                foreach (var error in errorMessages)
                {
                    context.AddFailure(error);
                }
            }
        });
    }


    private bool ValidateData(string item, string containerId, out IList<string> errorMessages)
    {
        var container = _settings.Containers.FirstOrDefault(a => a.ContainerId == containerId)
            ?? throw new ArgumentException("ContainerId not found.");

        string jsonSchema = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings", container.ValidationScheme));
        JSchema schema = JSchema.Parse(jsonSchema);
        JToken result = JToken.Parse(item);

        bool isValid = result.IsValid(schema, out errorMessages);

        return isValid;
    }
}
