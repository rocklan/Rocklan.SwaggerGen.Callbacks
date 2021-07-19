using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

using Microsoft.OpenApi.Expressions;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Rocklan.SwaggerGen.Callbacks
{
    /// <summary>
    /// Adds support for callbacks to the swagger doc
    /// </summary>
    public class CallbacksOperationFilter : IOperationFilter
    {
        private readonly OpenApiResponses _defaultCallbackResponses = new OpenApiResponses()
        {
            {"200", new OpenApiResponse(){
                Description = "Your server implementation should return this HTTP status " +
                "code if the data was received successfully"}},
            {"4xx", new OpenApiResponse(){
                Description = "If your server returns an HTTP status code indicating " +
                "it does not understand the format of the payload the delivery will be treated as a failure. No retries are attempted."}},
            {"5xx", new OpenApiResponse(){
                Description = "If your server returns an HTTP status code indicating " +
                "a server-side error the delivery will be treated as a failure. No retries are attempted."}},
        };

        private class Callback
        {
            public string Name { get; set; }

            public OpenApiCallback CallbackObj { get; set; }
        }

        /// <summary>
        /// Modifies the operations section of the swagger page to add callbacks
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Search the parameters of this method for any properties that may have the SwaggerCallbackSchemaAttribute applied
            // assign that field name to the callback section
            foreach (var parameterInfo in context.MethodInfo.GetParameters())
            {
                foreach (var callback in FindCallbacks(parameterInfo))
                {
                    operation.Callbacks.Add(callback.Name, callback.CallbackObj);
                }
            }
        }

        private IEnumerable<Callback> FindCallbacks(ParameterInfo parameterInfo)
        {
            // Iterate through all of the Properties on the parameter and see if any of their types contain the attribute
            foreach (var propInfo in parameterInfo.ParameterType.GetProperties())
            {
                if (!propInfo.PropertyType.IsEnum)
                    continue;

                var enumType = propInfo.PropertyType;
                foreach (var pmInfo in enumType.GetMembers(BindingFlags.Public | BindingFlags.Static))
                {
                    var callbackAttrib = pmInfo.GetCustomAttribute<SwaggerCallbackSchemaAttribute>();
                    if (callbackAttrib == null)
                        continue;

                    var obsoleteAttrib = pmInfo.GetCustomAttribute<ObsoleteAttribute>();
                    var enumMemberAttrib = pmInfo.GetCustomAttribute<EnumMemberAttribute>();
                    var enumValue = enumMemberAttrib != null && !string.IsNullOrEmpty(enumMemberAttrib.Value) ? enumMemberAttrib.Value : pmInfo.Name;

                    var callbackObjRequest = new OpenApiRequestBody();
                    callbackObjRequest.Content.Add("application/json", new OpenApiMediaType()
                    {
                        Schema = new OpenApiSchema()
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = callbackAttrib.SchemaType.Name,
                                Type = ReferenceType.Schema
                            }
                        }
                    }
                    );

                    var callbackObj = new OpenApiPathItem();
                    callbackObj.Operations.Add(
                        OperationType.Post, new OpenApiOperation()
                        {
                            Summary = callbackAttrib.Description,
                            RequestBody = callbackObjRequest,
                            Deprecated = obsoleteAttrib != null,
                            Responses = _defaultCallbackResponses
                        }
                    );

                    yield return new Callback()
                    {
                        Name = enumValue,
                        CallbackObj = new OpenApiCallback()
                        {
                            PathItems = new Dictionary<RuntimeExpression, OpenApiPathItem>()
                            {
                                {
                                    RuntimeExpression.Build(callbackAttrib.ExampleAddress), callbackObj
                                }
                            }
                        }
                    };
                }
            }
        }
    }
}
