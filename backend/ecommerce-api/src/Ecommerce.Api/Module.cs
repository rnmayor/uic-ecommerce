namespace Ecommerce.Api
{
    internal record Module(string Value)
    {
        internal static readonly Module Tenancy = new Module("Tenancy");

        public static implicit operator string(Module module) => module.Value;
    }
}
