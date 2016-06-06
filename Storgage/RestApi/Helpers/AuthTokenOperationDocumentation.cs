namespace Weezlabs.Storgage.RestApi.Helpers
{    
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Description;

    using Swashbuckle.Swagger;

    /// <summary>
    /// Class for adding custom documentation.
    /// </summary>
    public class AuthTokenOperationDocumentation : IDocumentFilter
    {
        /// <summary>
        /// Token end point.
        /// </summary>
        public const String TokenEndpoint = "/api/account/signin";

        /// <summary>
        /// Makes documentation for OAuth token endpoint.
        /// </summary>
        /// <param name="swaggerDoc">Swagger document.</param>
        /// <param name="schemaRegistry">Schema.</param>
        /// <param name="apiExplorer">Web Api explorer.</param>
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            swaggerDoc.paths.Add(TokenEndpoint, new PathItem
            {
                post = new Operation
                {
                    tags = new List<String> { "Auth" },
                    consumes = new List<String>
                    {
                        "application/x-www-form-urlencoded"
                    },
                    parameters = new List<Parameter> 
                    {
                        new Parameter
                        {
                            type = "string",
                            name = "grant_type",
                            required = true,
                            @in = "formData",
                            @default = "password"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "username",
                            required = true,
                            @in = "formData"
                        },
                        new Parameter
                        {
                            type = "string",
                            name = "password",
                            required = true,
                            @in = "formData"
                        },                   
                    },
                    responses = new Dictionary<String, Response> 
                    {
                        { "200", new Response { description = Resources.Messages.AuthSuccess } },
                        { "400", new Response { description = Resources.Messages.InvalidUserCredentials }  }
                    }
                }
            });
        }
    }
}