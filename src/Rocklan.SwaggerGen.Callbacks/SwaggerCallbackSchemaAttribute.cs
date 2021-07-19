using System;

namespace Rocklan.SwaggerGen.Callbacks
{
    /// <summary>
    /// Add this attribute to an enum's fields to indicate it is a callback
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SwaggerCallbackSchemaAttribute : Attribute
    {
        /// <summary>
        /// Unique type (eg, an enum's property) of this callback
        /// </summary>
        public Type SchemaType { get; }

        /// <summary>
        /// Short description of this callback
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Example URL to show on the swagger page
        /// </summary>
        public string ExampleAddress { get; }

        /// <summary>
        /// Creates a new SwaggerCallbackSchemaAttribute
        /// </summary>
        /// <param name="schemaType"></param>
        /// <param name="exampleAddress"></param>
        /// <param name="description"></param>
        public SwaggerCallbackSchemaAttribute(Type schemaType, string exampleAddress, string description)
        {
            SchemaType = schemaType;
            Description = description;
            ExampleAddress = exampleAddress;
        }
    }
}
