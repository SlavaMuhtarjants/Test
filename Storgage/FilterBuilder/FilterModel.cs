namespace Weezlabs.Storgage.FilterBuilder
{
    using System;

    using Weezlabs.Storgage.FilterBuilder.Enums;

    /// <summary>
    /// Model for creating lambda expressions
    /// </summary>
    public class FilterModel
    {
        /// <summary>
        /// Name of filtered property
        /// </summary>
        public String PropertyName { get; set; }

        /// <summary>
        /// Type of lambda operation
        /// </summary>
        public OperationsEnum Operation { get; set; }

        /// <summary>
        /// Constant value for lambda
        /// </summary>
        public Object Value { get; set; }
    }
}
