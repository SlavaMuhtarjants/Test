namespace Weezlabs.Storgage.RestApi
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Description;

    using Swashbuckle.Swagger;

    /// <summary>
    /// Filter for swagger
    /// </summary>
    public class SwaggerFilters : IOperationFilter
    {
        /// <summary>
        /// Apply filter to swagger
        /// </summary>
        /// <param name="operation">Operation.</param>
        /// <param name="schemaRegistry">SchemaRegistry.</param>
        /// <param name="apiDescription">ApiDescription.</param>
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            // Adding file upload for:
            // action UploadPhoto of SpacesController
            // action UploadAvatar of AccountController
            if (operation.operationId == "Spaces_UploadPhoto" || operation.operationId == "Account_UploadAvatar" ||
                operation.operationId == "Feedbacks_UploadLogs" ||
                operation.operationId == "Abuse_ContactUsUploadFile")
            {
                operation.consumes.Add("application/form-data");
                operation.parameters.Add(new Parameter
                {
                    name = "file",
                    @in = "formData",
                    required = true,
                    type = "file"
                });
            }
            // move api/account/refreshtoken to Auth tag
            if (operation.operationId == "Account_RefreshToken")
            {
                operation.tags = new List<String> { "Auth" };
                operation.consumes = new List<String>
                {
                    "application/x-www-form-urlencoded"
                };
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        type = "string",
                        name = "refresh_token",
                        required = true,
                        @in = "formData"
                    }
                };
            }
            // move api/account/signout to Auth tag
            if (operation.operationId == "Account_SignOut")
            {
                operation.tags = new List<String> { "Auth" };
                operation.consumes = new List<String>
                {
                    "application/x-www-form-urlencoded"
                };
                operation.parameters = new List<Parameter>
                {
                    new Parameter
                    {
                        type = "string",
                        name = "refresh_token",
                        required = true,
                        @in = "formData"
                    },
                    new Parameter
                    {
                        name = "Authorization",
                        @in = "header",
                        description = "access token",
                        required = true,
                        type = "string",
                        @default = "Bearer "
                    }
                };
            }
        }
    }
}