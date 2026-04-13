using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.NodaMoney.Structures
{
    /// <summary>
    /// Represents currency information including code, symbol, name, and decimal digits.
    /// </summary>
    [OSStructure(Description = "Currency information including ISO 4217 code, symbol, English name, and decimal digit count.")]
    public struct CurrencyResult
    {
        [OSStructureField(Description = "ISO 4217 currency code (e.g., \"USD\", \"EUR\", \"JPY\").", IsMandatory = true)]
        public string Code;

        [OSStructureField(Description = "Currency symbol (e.g., \"$\", \"\u20ac\", \"\u00a5\").")]
        public string Symbol;

        [OSStructureField(Description = "English name of the currency (e.g., \"US Dollar\", \"Euro\").")]
        public string EnglishName;

        [OSStructureField(Description = "Number of decimal digits for the currency's minor unit (e.g., 2 for USD, 0 for JPY).")]
        public int DecimalDigits;

        /// <summary>
        /// Creates a new CurrencyResult with default values.
        /// </summary>
        public CurrencyResult()
        {
            Code = string.Empty;
            Symbol = string.Empty;
            EnglishName = string.Empty;
            DecimalDigits = 0;
        }

        /// <summary>
        /// Creates a new CurrencyResult from the specified values.
        /// </summary>
        /// <param name="code">The ISO 4217 currency code.</param>
        /// <param name="symbol">The currency symbol.</param>
        /// <param name="englishName">The English name of the currency.</param>
        /// <param name="decimalDigits">The number of decimal digits.</param>
        public CurrencyResult(string code, string symbol, string englishName, int decimalDigits)
        {
            Code = code;
            Symbol = symbol;
            EnglishName = englishName;
            DecimalDigits = decimalDigits;
        }
    }
}
