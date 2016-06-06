namespace Weezlabs.Storgage.Model
{
    using Microsoft.AspNet.Identity;
    using System;

    /// <summary>
    /// User extension.
    /// </summary>
    public partial class User : IUser<Guid>
    {        
        /// <summary>
        /// Username.
        /// </summary>
        public string UserName
        {
            get { return Email ??"not used"; }

            set { Email = value ?? Email; }
        }              
    }
}
