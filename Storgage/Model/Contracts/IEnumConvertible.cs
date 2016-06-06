namespace Weezlabs.Storgage.Model.Contracts
{
    /// <summary>
    /// Interface for entities can be converted to enumerations.
    /// </summary>
    /// <typeparam name="T">Enumeration type.</typeparam>
    public interface IEnumConvertible<T>
    {
        /// <summary>
        /// Convert entity to enumeration.
        /// </summary>
        /// <returns>Enum.</returns>
        T ToEnum();
    }
}
