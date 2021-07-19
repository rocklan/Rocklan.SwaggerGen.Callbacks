# Rocklan.SwaggerGen.Callbacks
Adds support for displaying callbacks (webhooks) info in swagger via swashbuckle

## Basic Usage

Add the library to your aspnet core project and then add the following code:


```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddSwaggerGen(options =>
        {
            ...
            
            options.OperationFilter<Rocklan.SwaggerGen.Callbacks.CallbacksOperationFilter>();
            options.DocumentFilter<Rocklan.SwaggerGen.Callbacks.AdditionalSchemaDocumentFilter<SubscribeDTO>>();
        }
    }
}

public class SubscribeController : ApiController
{
    /// <summary>
    /// Subscribes to a webhook
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPost()]
    public IActionResult Subscribe(SubscribeDTO data)
    {
        //TODO: put subscription data in a database somewhere
        return Ok("ok");
    }
}

public class SubscribeDTO
{
    [EnumDataType(typeof(WebhookEventType))]
    public WebhookEventType EventType { get; set; }
    public string CallbackUrl { get; set; }
}

public enum WebhookEventType
{
    [EnumMember(Value = "My first and only callback method")]
    [SwaggerCallbackSchema(
        typeof(CallBackPayloadDataDTO),
        "https://example.com",
        "This is a webhook that occurs")]
    OneTypeOfCallback
}

public class CallBackPayloadDataDTO
{
    public string Name { get; set; }
}
```

That should be enough to get you started. If you have more than one webhook (callback) then you can add it to the `WebhookEventType` enum.


## Display enum names instead of integers

There is a problem with the simple example above. When viewing the subscribe POST method in swagger, only a dropdown with numbers are available for the 
Webhook Event Type. It would be much easier to use and understand if it was an array of strings. If you would like to do this you can do the following:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddControllers().AddJsonOptions(opt =>
        {
            opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}
```

and then add the `StringEnumConverter` to the WebhookEventType enum:

```csharp
[JsonConverter(typeof(StringEnumConverter))]
public enum WebhookEventType
{
    ...
} 
```
