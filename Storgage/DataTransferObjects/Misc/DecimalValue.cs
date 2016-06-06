namespace Weezlabs.Storgage.DataTransferObjects.Misc
{
    using System;

    public class DecimalValue
    {
        public Decimal Value { get; set; }

        public DecimalValue (Decimal value)
        {
            Value = value;
        }       
    }
}
