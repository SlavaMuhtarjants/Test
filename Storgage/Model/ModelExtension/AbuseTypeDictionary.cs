namespace Weezlabs.Storgage.Model
{
    using System;

    using Contracts;

    /// <summary>
    /// "Abuse Type" and "Contact Us Type" access type extension.
    /// </summary>
    public partial class AbuseTypeDictionary : IEnumConvertible<Enums.AbuseTypeDictionary>
    {
        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>AbuseTypeDictionary enum.</returns>
        public Enums.AbuseTypeDictionary ToEnum()
        {
            return (Enums.AbuseTypeDictionary)Enum.Parse(typeof(Enums.AbuseTypeDictionary), Synonym);
        }

        public Enums.AbuseTypeDictionary? ToEnumAbuse()
        {
            try
            {
                return (Enums.AbuseTypeDictionary)Enum.Parse(typeof(Enums.AbuseTypeDictionary), Synonym);
            }
            catch
            {
                return null;
            };
        }

        /// <summary>
        /// Converts model object to enum.
        /// </summary>
        /// <returns>AbuseTypeDictionary enum.</returns>
        public Enums.ContactUsDictionary? ToEnumContactUs()
        {
            try
            {
                return (Enums.ContactUsDictionary)Enum.Parse(typeof(Enums.ContactUsDictionary), Synonym);
            }
            catch
            {
                return null;
            };
            
        }

    }
}
