using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.NodaMoney.Structures
{
    /// <summary>
    /// Represents the result of a money operation, containing the amount,
    /// currency code, and a culture-formatted string representation.
    /// </summary>
    [OSStructure(Description = "Result of a money operation containing amount, currency code, and formatted representation.")]
    public struct MoneyResult
    {
        [OSStructureField(Description = "The monetary amount.", IsMandatory = true)]
        public decimal Amount;

        [OSStructureField(Description = "ISO 4217 currency code (e.g., \"USD\", \"EUR\").", IsMandatory = true)]
        public string CurrencyCode;

        [OSStructureField(Description = "Culture-formatted string representation of the money value (e.g., \"$100.50\").")]
        public string Formatted;

        /// <summary>
        /// Creates a new MoneyResult with default values.
        /// </summary>
        public MoneyResult()
        {
            Amount = 0m;
            CurrencyCode = string.Empty;
            Formatted = string.Empty;
        }

        /// <summary>
        /// Creates a new MoneyResult from the specified values.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <param name="formatted">The formatted string representation.</param>
        public MoneyResult(decimal amount, string currencyCode, string formatted)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
            Formatted = formatted;
        }
    }
}
