namespace Weezlabs.Storgage.RestApi.ActionFilters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Filters;

    /// <summary>
    /// Action filter to handle avoided parameters
    /// </summary>
    public class ValidateQueryParametersAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Overriding action excuting.
        /// </summary>
        /// <param name="actionContext">Http action context.</param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            Collection<HttpParameterDescriptor> parameters = actionContext.ActionDescriptor.GetParameters();
            IEnumerable<KeyValuePair<String, String>> queryParameters = actionContext.Request.GetQueryNameValuePairs();

            var errorsList = new List<String>();

            foreach (var param in queryParameters)
            {
                // try to find input Action parameter by name from query
                HttpParameterDescriptor inputParam = parameters.FirstOrDefault(x => x.ParameterName == param.Key);
                if (inputParam != null)
                {
                    if (inputParam.ParameterType.IsValueType)
                    {
                        if (!CanCovert(param.Value, (Type) inputParam.ParameterType))
                        {
                            errorsList.Add(String.Format(Resources.Messages.InvalidQueryParameterType,
                                param.Value, param.Key, inputParam.ParameterType.Name,
                                actionContext.ActionDescriptor.ActionName));
                        }
                    }
                    else if (inputParam.ParameterType.BaseType == typeof (Array))
                    {
                        if (!CanCovert(param.Value, inputParam.ParameterType.GetElementType()))
                        {
                            errorsList.Add(String.Format(Resources.Messages.InvalidQueryParameterType,
                                param.Value, param.Key, inputParam.ParameterType.GetElementType(),
                                actionContext.ActionDescriptor.ActionName));
                        }
                    }
                }
                else
                {
                    errorsList.Add(String.Format(Resources.Messages.MissedQueryParameter, param.Key,
                        actionContext.ActionDescriptor.ActionName));
                }
            }
            if (errorsList.Any())
            {
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, String.Join("\n", errorsList));
            }
        }

        /// <summary>
        /// Cheking for convert to target type
        /// </summary>
        /// <param name="value">Converted value</param>
        /// <param name="type">Checked Type</param>
        /// <returns>true if we can convert value to target type</returns>
        private Boolean CanCovert(String value, Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.IsValid(value);
        }
    }
}