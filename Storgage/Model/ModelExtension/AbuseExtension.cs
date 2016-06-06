namespace Weezlabs.Storgage.Model
{
    using System.Linq;
    using System.Data.Entity;

    public static class AbuseExtension
    {
        public static IQueryable<Abuse> AttachIncludes(this IQueryable<Abuse> abuses)
        {
            return abuses.Include(a => a.AbuseTypes.Select(at => at.AbuseTypeDictionary));
        }
    }
}
