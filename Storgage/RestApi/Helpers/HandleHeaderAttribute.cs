namespace Weezlabs.Storgage.RestApi.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using Swashbuckle.Swagger;

    /// <summary>
    /// Custom header handler for swagger.
    /// </summary>
    public class HandleHeaderAttribute : IOperationFilter
    {
        /// <summary>
        /// Applies filter to swagger.
        /// </summary>
        /// <param name="operation">Operation</param>
        /// <param name="schemaRegistry">Swagger Schema</param>
        /// <param name="apiDescription">Rest Api Description</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var isAuthorized = apiDescription.ActionDescriptor.GetFilters().Any(x => x is AuthorizeAttribute);
            var isAllowAnonimous = apiDescription.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (isAuthorized && !isAllowAnonimous)
            {
                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                operation.parameters.Add(new Parameter
                {
                    name = "Authorization",
                    @in = "header",
                    description = "access token",
                    required = true,
                    type = "string",
                    @default = "Bearer "                   
                });
            }
        }
    }
}