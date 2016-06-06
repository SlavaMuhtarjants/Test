namespace Weezlabs.Storgage.FilterBuilder
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Interface for filter builder
    /// </summary>
    public interface IFilterBuilder
    {
        /// <summary>
        /// Return expression tree
        /// </summary>
        /// <typeparam name="T">Type of filtered class</typeparam>
        /// <returns>Expression tree</returns>
        Expression<Func<T, Boolean>> BuildFilter<T>();
    }
}
