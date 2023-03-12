using Microsoft.AspNetCore.Mvc.ModelBinding;

using MongoDB.Bson;

namespace SimpleTraveling.CostService.Binders;

public class ObjectIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context) =>
        context.Metadata.ModelType == typeof(ObjectId) ? new ObjectIdModelBinder() : (IModelBinder?)null;
}

public class ObjectIdModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
        if (value == null)
        {
            return Task.CompletedTask;
        }

        if (!ObjectId.TryParse(value, out var objectId))
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid ObjectId");
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(objectId);
        return Task.CompletedTask;
    }
}
