//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Weezlabs.Storgage.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Space
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Space()
        {
            this.Invoices = new HashSet<Invoice>();
            this.PhotoLibraries = new HashSet<PhotoLibrary>();
            this.SpaceBusies = new HashSet<SpaceBusy>();
            this.Chats = new HashSet<Chat>();
            this.AbuseSpaces = new HashSet<AbuseSpace>();
        }
    
        public System.Guid Id { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid SizeTypeId { get; set; }
        public string Title { get; set; }
        public string Decription { get; set; }
        public bool IsListed { get; set; }
        public System.Guid SpaceTypeId { get; set; }
        public System.Guid SpaceAccessTypeId { get; set; }
        public System.Data.Entity.Spatial.DbGeography Location { get; set; }
        public Nullable<System.Guid> DefaultPhotoID { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTimeOffset createdDate { get; set; }
        public decimal Rate { get; set; }
        public string FullAddress { get; set; }
        public string ShortAddress { get; set; }
        public Nullable<System.Guid> ZipId { get; set; }
        public Nullable<System.DateTimeOffset> AvailableSince { get; set; }
        public System.DateTime LastModified { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsOccupied { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhotoLibrary> PhotoLibraries { get; set; }
        public virtual SizeType SizeType { get; set; }
        public virtual SpaceType SpaceType { get; set; }
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SpaceBusy> SpaceBusies { get; set; }
        public virtual SpaceAccessType SpaceAccessType { get; set; }
        public virtual PhotoLibrary PhotoLibrary { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual Zip Zip { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AbuseSpace> AbuseSpaces { get; set; }
    }
}
