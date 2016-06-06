namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;

    using Filter;

    /// <summary>
    /// Reflect a match between a filter and spaces found by the filter
    /// </summary>
    public class GetSpacesReportByFilters
    {
        /// <summary>
        /// Filter information
        /// </summary>
        public SavedFilterInfo Filter { get; set; }

        /// <summary>
        /// Number of spaces per filter
        /// </summary>
        public Int32 SpacesCount { get; set; }
    }
}