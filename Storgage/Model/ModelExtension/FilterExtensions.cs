namespace Weezlabs.Storgage.Model.ModelExtension
{
    using System.Linq;
    using System.Data.Entity;

    public static class FilterExtensions
    {
        public static IQueryable<Filter> AttachIncludes(this IQueryable<Filter> filters)
        {
            return filters.Include(f => f.FilterRootDictionaries.Select(frd => frd.RootDictionary.SizeType)).
                Include(f => f.FilterRootDictionaries.Select(frd => frd.RootDictionary.SpaceAccessType)).
                Include(f => f.FilterRootDictionaries.Select(frd => frd.RootDictionary.SpaceType)).Include(f => f.Zip);
        }
    }
}