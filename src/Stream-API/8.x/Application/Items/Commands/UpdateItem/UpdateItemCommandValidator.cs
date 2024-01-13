using FluentValidation;
using Newtonsoft.Json.Linq;

namespace Application.Items.Commands.UpdateItem;

public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
{
    public UpdateItemCommandValidator()
    {
        RuleFor(v => v.Item)
        .Custom((item, context) =>
        {
            var id = context.InstanceToValidate?.Id;

            if (!ValidateId(item, id, out IList<string> errorMessages))
            {
                foreach (var error in errorMessages)
                {
                    context.AddFailure(error);
                }
            }
        });
    }

    private static bool ValidateId(string item, string id, out IList<string> errorMessages)
    {
        JObject jsonObj = JObject.Parse(item);
        var itemId = jsonObj["id"]!.Value<string>();

        errorMessages = new List<string>
        {
            "Item id does not match parameter id."
        };

        return itemId == id;
    }
}
