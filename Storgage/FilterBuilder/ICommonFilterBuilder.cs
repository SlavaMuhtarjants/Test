namespace Weezlabs.Storgage.FilterBuilder
{
    using DataTransferObjects.Filter;

    public interface ICommonFilterBuilder : IFilterBuilder
    {
        /// <summary>
        /// Set filter info
        /// </summary>
        /// <param name="filter">Filter object</param>
        void SetFilter(FilterInfo filter);
    }
}
