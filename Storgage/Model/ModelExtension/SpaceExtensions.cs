namespace Weezlabs.Storgage.Model.ModelExtension
{
    using System.Linq;
    using System.Data.Entity;

    public static class SpaceExtensions
    {
        public static IQueryable<Space> AttachIncludes(this IQueryable<Space> spaces)
        {
            return spaces.Include(x => x.PhotoLibraries).Include(x => x.SizeType).
                Include(x => x.SpaceAccessType).Include(x => x.SpaceType).Include(x => x.User).Include(x => x.Zip).
                Include(x => x.User.EmailVerificationStatus).Include(x => x.User.PhoneVerificationStatus);
        }
    }
}
