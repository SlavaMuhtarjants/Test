namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Model.Exceptions;
    using IoC;    

    /// <summary>
    /// Enum extension for dictionaries
    /// </summary>
    public static class DictionaryEnumExtension
    {
        /// <summary>
        /// IDictionaryProvider
        /// </summary>
        private static IDictionaryProvider dictionaryProvider
        {
            get { return ContainerWrapper.Container.Resolve<IDictionaryProvider>(); }
        }

        /// <summary>
        /// Get id from dictionary by enum value
        /// </summary>
        /// <param name="enumValue">Enum value</param>
        /// <returns>Guid</returns>
        public static Guid GetDictionaryId(this Enum enumValue)
        {
            Type enumType = enumValue.GetType();
            List<PropertyInfo> dictionaryProperties = dictionaryProvider.GetType().GetProperties().ToList();

            // get IEmumerable properies from dictionaryProvider
            // note: we should get ToList() because Where returned WhereListIterator
            var dIEnumerableProperties =
                dictionaryProperties.Where(x => x.PropertyType.GetInterfaces().Any(y => y.Name == "IEnumerable"))
                    .ToList();

            // get all IEnumerable<T> where T is IEnumConvertible type:
            var dIEnumConvertibleProperties = dIEnumerableProperties.Where(
                x => x.PropertyType.GetGenericArguments()[0].GetInterfaces().Any(y => y.Name == "IEnumConvertible`1"))
                .ToList();

            foreach (PropertyInfo dIEnumConvertibleProperty in dIEnumConvertibleProperties)
            {
                // get type of T from IEnumerable<T> of dictionaryProvider property
                Type typeOfIEnumerableProperty = dIEnumConvertibleProperty.PropertyType.GetGenericArguments()[0];

                // checking for enumValue name of type equal name of type of IEnumerable property in dictionaryProvider
                // note: We have types of Enum with name equals to ef model
                if (enumType.Name != typeOfIEnumerableProperty.Name){ continue; }

                // get instance of IEnumerable<T> (where T is IEnumConvertible) property from dictionaryProvider
                Object instanceOfEnum =
                    dictionaryProvider.GetType()
                        .GetProperty(dIEnumConvertibleProperty.Name)
                        .GetValue(dictionaryProvider, null);

                // get converted instance of dictionary property
                // we actually know (check before): property is IEnumerable
                var convertedInstanceOfEnum = ((Object) instanceOfEnum) as IEnumerable<dynamic>;

                if (convertedInstanceOfEnum == null) { continue; }

                // for prevent multiple enumerable
                List<dynamic> dictionaryValues = convertedInstanceOfEnum.ToList();

                // check type of property (from ef):
                // get base type because getType return entity-proxy class
                if (dictionaryValues.First().GetType().BaseType != typeOfIEnumerableProperty) { continue; }

                Boolean hasPropertySynonym = dictionaryValues.First().GetType().GetProperty("Synonym") != null;
                Boolean hasPropertyId = dictionaryValues.First().GetType().GetProperty("Id") != null;

                // check type contains property Synonym and Id
                if (!(hasPropertySynonym && hasPropertyId)) { continue; }

                // get type of Id property
                dynamic typeOfId = dictionaryValues.First().GetType().GetProperty("Id").PropertyType;

                // check type of Id equals Guid
                if (!typeOfId.Equals(typeof (Guid))) { continue; }

                return dictionaryValues.Single(x => x.Synonym == enumValue.ToString()).Id;
            }

            throw new EnumToDictionaryException(String.Format("Cannot get database identifier for {0}.", enumValue.GetType().FullName));
        }
    }
}
