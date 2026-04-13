using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.NodaMoney.Structures
{
    /// <summary>
    /// Represents an exchange rate between two currencies.
    /// </summary>
    [OSStructure(Description = "Exchange rate information between a base currency and a quote currency.")]
    public struct ExchangeRateResult
    {
        [OSStructureField(Description = "ISO 4217 code of the base currency (the currency being converted from).", IsMandatory = true)]
        public string BaseCurrencyCode;

        [OSStructureField(Description = "ISO 4217 code of the quote currency (the currency being converted to).", IsMandatory = true)]
        public string QuoteCurrencyCode;

        [OSStructureField(Description = "The exchange rate value. Multiply the base amount by this to get the quote amount.", IsMandatory = true)]
        public decimal Rate;

        /// <summary>
        /// Creates a new ExchangeRateResult with default values.
        /// </summary>
        public ExchangeRateResult()
        {
            BaseCurrencyCode = string.Empty;
            QuoteCurrencyCode = string.Empty;
            Rate = 0m;
        }

        /// <summary>
        /// Creates a new ExchangeRateResult from the specified values.
        /// </summary>
        /// <param name="baseCurrencyCode">The base currency ISO 4217 code.</param>
        /// <param name="quoteCurrencyCode">The quote currency ISO 4217 code.</param>
        /// <param name="rate">The exchange rate value.</param>
        public ExchangeRateResult(string baseCurrencyCode, string quoteCurrencyCode, decimal rate)
        {
            BaseCurrencyCode = baseCurrencyCode;
            QuoteCurrencyCode = quoteCurrencyCode;
            Rate = rate;
        }
    }
}
