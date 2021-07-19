# Rocklan.SwaggerGen.Callbacks
Adds support for displaying callbacks (webhooks) info in swagger via swashbuckle

## Basic Usage

Add the library to your aspnet core project and then add the following code to your startup class:


```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        ...
        services.AddSwaggerGen(options =>
        {
            ...
            
            options.OperationFilter<CallbacksOperationFilter>();
            options.DocumentFilter<AdditionalSchemaDocumentFilter<CallBackPayloadDataDTO>>();
        }
    }
}
```

notice that we have also added a reference to the `CallBackPayloadDataDTO` class, which I'll describe in a minute.

Once you have the startup code added, you can add a new Api endpoint to your application:

```csharp
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
```

This endpoint should accept subscription (webhook) requests and allows api callers to register their webhooks. We need to define what data they have to passthrough:

```csharp
public class SubscribeDTO
{
    [EnumDataType(typeof(WebhookEventType))]
    public WebhookEventType EventType { get; set; }
    public string CallbackUrl { get; set; }
}
```

So this means they have to pass through the callback URL that we will call, and also the specific webhook that they want to trap. Let's go ahead and define one:

```csharp
public enum WebhookEventType
{
    [EnumMember(Value = "My first and only callback method")]
    [SwaggerCallbackSchema(
        typeof(CallBackPayloadDataDTO),
        "https://example.com",
        "This is a webhook that occurs")]
    OneTypeOfCallback
}
```

We also need to define what payload we will pass to the user when calling their webhook:

```csharp
public class CallBackPayloadDataDTO
{
    public string Name { get; set; }
}
```

That should be enough to get you started. If you have more than one webhook (callback) then you can add it to the `WebhookEventType` enum.


## Display enum names instead of integers

There is a small problem with the simple example above. When viewing the subscribe POST method in swagger, only a dropdown with numbers are available for the 
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
