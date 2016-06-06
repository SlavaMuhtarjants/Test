namespace Weezlabs.Storgage.Model.Enums
{
    /// <summary>
    /// Contains infor about access type.
    /// </summary>
    public enum AbuseTypeDictionary
    {
        /// <summary>
        /// Abuse Type is Fraud.
        /// </summary>
        Fraud,

        /// <summary>
        /// Abuse Type is Abuse.
        /// </summary>
        Abuse,

        /// <summary>
        /// Abuse Type is IllegalContent.
        /// </summary>
        IllegalContent,

        /// <summary>
        /// Abuse Type is PropertyIssues.
        /// </summary>
        PropertyIssues,

        /// <summary>
        /// Abuse Type is Other.
        /// </summary>
        Other
    }

    /// <summary>
    /// AbuseTypeDictionary and ContactUsDictionary are stored in the same dictionary in the database actually and must be stored together 
    /// </summary>
    public enum ContactUsDictionary 
    {
        /// <summary>
        /// Contact us about bug
        /// </summary>
        ContactUsBugReport,
        /// <summary>
        /// Contact us about enhancement
        /// </summary>
        ContactUsEnhancement,
        /// <summary>
        /// Contact us about enhancement
        /// </summary>
        ContactUsOther
    }
}