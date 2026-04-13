using System.Collections.Generic;
using OutSystems.Extension.NodaMoney.Structures;
using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.NodaMoney
{
    /// <summary>
    /// OutSystems ODC External Library providing type-safe money and currency operations
    /// powered by the NodaMoney library. Supports ISO 4217 currencies, arithmetic,
    /// formatting, parsing, exchange rate conversion, and fair allocation.
    /// </summary>
    [OSInterface(
        Name = "NodaMoney",
        Description = "Type-safe money and currency operations with ISO 4217 compliance. Provides arithmetic, formatting, parsing, exchange rate conversion, and fair money allocation powered by the NodaMoney library.",
        IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
    )]
    public interface INodaMoney
    {
        #region Money Operations

        /// <summary>
        /// Creates a money value, validates the currency, and rounds to the currency's minor units.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code (e.g., "USD", "EUR", "JPY").</param>
        /// <param name="moneyResult">The created money value with amount, currency code, and formatted representation.</param>
        [OSAction(
            Description = "Creates a money value, validates the currency code, and rounds to the currency's minor units.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyCreate(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code (e.g., \"USD\", \"EUR\", \"JPY\").")]
            string currencyCode,

            [OSParameter(Description = "The created money value with amount, currency code, and formatted representation.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Adds two money values. Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The first monetary amount.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the first amount.</param>
        /// <param name="amount2">The second monetary amount.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the second amount.</param>
        /// <param name="moneyResult">The sum as a money value.</param>
        [OSAction(
            Description = "Adds two money values. Both must use the same currency; throws an error if currencies differ.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyAdd(
            [OSParameter(Description = "The first monetary amount.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the first amount.")]
            string currencyCode1,

            [OSParameter(Description = "The second monetary amount.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the second amount.")]
            string currencyCode2,

            [OSParameter(Description = "The sum of the two money values.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Subtracts the second money value from the first. Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The monetary amount to subtract from.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the first amount.</param>
        /// <param name="amount2">The monetary amount to subtract.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the second amount.</param>
        /// <param name="moneyResult">The difference as a money value.</param>
        [OSAction(
            Description = "Subtracts the second money value from the first. Both must use the same currency; throws an error if currencies differ.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneySubtract(
            [OSParameter(Description = "The monetary amount to subtract from.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the first amount.")]
            string currencyCode1,

            [OSParameter(Description = "The monetary amount to subtract.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the second amount.")]
            string currencyCode2,

            [OSParameter(Description = "The difference of the two money values.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Multiplies a money value by a scalar multiplier.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="multiplier">The scalar multiplier.</param>
        /// <param name="moneyResult">The product as a money value.</param>
        [OSAction(
            Description = "Multiplies a money value by a scalar multiplier.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyMultiply(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The scalar multiplier.")]
            decimal multiplier,

            [OSParameter(Description = "The product of the money value and multiplier.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Divides a money value by a scalar divisor.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="divisor">The scalar divisor. Must not be zero.</param>
        /// <param name="moneyResult">The quotient as a money value.</param>
        [OSAction(
            Description = "Divides a money value by a scalar divisor. Throws an error if the divisor is zero.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyDivide(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The scalar divisor. Must not be zero.")]
            decimal divisor,

            [OSParameter(Description = "The quotient of the money value divided by the divisor.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Divides one money value by another, returning the ratio as a decimal.
        /// Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The dividend monetary amount.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the dividend.</param>
        /// <param name="amount2">The divisor monetary amount. Must not be zero.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the divisor.</param>
        /// <param name="ratio">The ratio of the first amount to the second (e.g., $100 / $50 = 2.0).</param>
        [OSAction(
            Description = "Divides one money value by another, returning the ratio as a decimal. Both must use the same currency.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyDivideByMoney(
            [OSParameter(Description = "The dividend monetary amount.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the dividend.")]
            string currencyCode1,

            [OSParameter(Description = "The divisor monetary amount. Must not be zero.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the divisor.")]
            string currencyCode2,

            [OSParameter(Description = "The ratio of the first amount to the second (e.g., $100 / $50 = 2.0).")]
            out decimal ratio
        );

        /// <summary>
        /// Returns the absolute value of a money amount.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="moneyResult">The absolute value as a money value.</param>
        [OSAction(
            Description = "Returns the absolute value of a money amount (removes any negative sign).",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyAbs(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The absolute value of the money amount.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Negates a money amount (positive becomes negative and vice versa).
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="moneyResult">The negated money value.</param>
        [OSAction(
            Description = "Negates a money amount (positive becomes negative and vice versa).",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyNegate(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The negated money value.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Compares two money values. Returns -1 if the first is less, 0 if equal, 1 if greater.
        /// Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The first monetary amount.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the first amount.</param>
        /// <param name="amount2">The second monetary amount.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the second amount.</param>
        /// <param name="comparisonResult">-1 if first is less than second, 0 if equal, 1 if first is greater than second.</param>
        [OSAction(
            Description = "Compares two money values. Returns -1 (less than), 0 (equal), or 1 (greater than). Both must use the same currency.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyCompare(
            [OSParameter(Description = "The first monetary amount.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the first amount.")]
            string currencyCode1,

            [OSParameter(Description = "The second monetary amount.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the second amount.")]
            string currencyCode2,

            [OSParameter(Description = "-1 if first is less than second, 0 if equal, 1 if first is greater than second.")]
            out int comparisonResult
        );

        /// <summary>
        /// Checks if a money value is zero.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="isZero">True if the money amount is zero.</param>
        [OSAction(
            Description = "Checks if a money value is zero.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyIsZero(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "True if the money amount is zero.")]
            out bool isZero
        );

        /// <summary>
        /// Checks if a money value is positive (greater than zero).
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="isPositive">True if the money amount is positive.</param>
        [OSAction(
            Description = "Checks if a money value is positive (greater than zero).",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyIsPositive(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "True if the money amount is positive.")]
            out bool isPositive
        );

        /// <summary>
        /// Checks if a money value is negative (less than zero).
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="isNegative">True if the money amount is negative.</param>
        [OSAction(
            Description = "Checks if a money value is negative (less than zero).",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyIsNegative(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "True if the money amount is negative.")]
            out bool isNegative
        );

        /// <summary>
        /// Returns the smaller of two money values. Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The first monetary amount.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the first amount.</param>
        /// <param name="amount2">The second monetary amount.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the second amount.</param>
        /// <param name="moneyResult">The smaller of the two money values.</param>
        [OSAction(
            Description = "Returns the smaller of two money values. Both must use the same currency.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyMin(
            [OSParameter(Description = "The first monetary amount.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the first amount.")]
            string currencyCode1,

            [OSParameter(Description = "The second monetary amount.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the second amount.")]
            string currencyCode2,

            [OSParameter(Description = "The smaller of the two money values.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Returns the larger of two money values. Both must use the same currency.
        /// </summary>
        /// <param name="amount1">The first monetary amount.</param>
        /// <param name="currencyCode1">ISO 4217 currency code for the first amount.</param>
        /// <param name="amount2">The second monetary amount.</param>
        /// <param name="currencyCode2">ISO 4217 currency code for the second amount.</param>
        /// <param name="moneyResult">The larger of the two money values.</param>
        [OSAction(
            Description = "Returns the larger of two money values. Both must use the same currency.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyMax(
            [OSParameter(Description = "The first monetary amount.")]
            decimal amount1,

            [OSParameter(Description = "ISO 4217 currency code for the first amount.")]
            string currencyCode1,

            [OSParameter(Description = "The second monetary amount.")]
            decimal amount2,

            [OSParameter(Description = "ISO 4217 currency code for the second amount.")]
            string currencyCode2,

            [OSParameter(Description = "The larger of the two money values.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Splits a money value into the specified number of equal parts without losing cents.
        /// Uses fair allocation (Bresenham-style) to distribute remainders.
        /// </summary>
        /// <param name="amount">The monetary amount to split.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="numberOfParts">The number of parts to split into. Must be greater than zero.</param>
        /// <param name="allocationItems">List of allocation items, each with its amount, currency code, and 1-based index.</param>
        [OSAction(
            Description = "Splits a money value into N equal parts without losing cents. Uses fair allocation to distribute any remainder across the parts.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneySplit(
            [OSParameter(Description = "The monetary amount to split.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The number of parts to split into. Must be greater than zero.")]
            int numberOfParts,

            [OSParameter(Description = "List of allocation items, each with its amount, currency code, and 1-based index.")]
            out List<MoneyAllocationItem> allocationItems
        );

        #endregion

        #region Currency Operations

        /// <summary>
        /// Gets currency information by ISO 4217 code.
        /// </summary>
        /// <param name="currencyCode">ISO 4217 currency code (e.g., "USD", "EUR", "JPY").</param>
        /// <param name="currencyResult">Currency information including code, symbol, English name, and decimal digits.</param>
        [OSAction(
            Description = "Gets currency information by ISO 4217 code, including symbol, English name, and decimal digit count.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void CurrencyGetInfo(
            [OSParameter(Description = "ISO 4217 currency code (e.g., \"USD\", \"EUR\", \"JPY\").")]
            string currencyCode,

            [OSParameter(Description = "Currency information including code, symbol, English name, and decimal digits.")]
            out CurrencyResult currencyResult
        );

        /// <summary>
        /// Lists all available ISO 4217 currencies in the NodaMoney registry.
        /// </summary>
        /// <param name="currencies">List of all available currencies with their information.</param>
        [OSAction(
            Description = "Lists all available ISO 4217 currencies in the NodaMoney registry.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void CurrencyGetAll(
            [OSParameter(Description = "List of all available currencies with their information.")]
            out List<CurrencyResult> currencies
        );

        /// <summary>
        /// Checks if a currency code is valid and registered in the NodaMoney currency registry.
        /// Returns false for null, empty, or whitespace input (does not throw).
        /// </summary>
        /// <param name="currencyCode">The currency code to validate.</param>
        /// <param name="isValid">True if the currency code is valid and registered.</param>
        [OSAction(
            Description = "Checks if a currency code is valid and registered in the ISO 4217 currency registry.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void CurrencyIsValid(
            [OSParameter(Description = "The currency code to validate.")]
            string currencyCode,

            [OSParameter(Description = "True if the currency code is valid and registered.")]
            out bool isValid
        );

        #endregion

        #region Exchange Rate Operations

        /// <summary>
        /// Creates and validates an exchange rate between two currencies.
        /// </summary>
        /// <param name="baseCurrencyCode">ISO 4217 code of the base currency.</param>
        /// <param name="quoteCurrencyCode">ISO 4217 code of the quote currency.</param>
        /// <param name="rate">The exchange rate value. Must be greater than zero.</param>
        /// <param name="exchangeRateResult">The validated exchange rate information.</param>
        [OSAction(
            Description = "Creates and validates an exchange rate between two currencies. The rate must be greater than zero.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void ExchangeRateCreate(
            [OSParameter(Description = "ISO 4217 code of the base currency (the currency being converted from).")]
            string baseCurrencyCode,

            [OSParameter(Description = "ISO 4217 code of the quote currency (the currency being converted to).")]
            string quoteCurrencyCode,

            [OSParameter(Description = "The exchange rate value. Must be greater than zero.")]
            decimal rate,

            [OSParameter(Description = "The validated exchange rate information.")]
            out ExchangeRateResult exchangeRateResult
        );

        /// <summary>
        /// Converts a money amount from one currency to another using the specified exchange rate.
        /// </summary>
        /// <param name="amount">The monetary amount to convert.</param>
        /// <param name="fromCurrencyCode">ISO 4217 code of the source currency.</param>
        /// <param name="toCurrencyCode">ISO 4217 code of the target currency.</param>
        /// <param name="rate">The exchange rate value (from -> to).</param>
        /// <param name="moneyResult">The converted money value in the target currency.</param>
        [OSAction(
            Description = "Converts a money amount from one currency to another using the specified exchange rate.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void ExchangeRateConvert(
            [OSParameter(Description = "The monetary amount to convert.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 code of the source currency.")]
            string fromCurrencyCode,

            [OSParameter(Description = "ISO 4217 code of the target currency.")]
            string toCurrencyCode,

            [OSParameter(Description = "The exchange rate value (from -> to).")]
            decimal rate,

            [OSParameter(Description = "The converted money value in the target currency.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Parses an exchange rate string in the format "BASE/QUOTE rate" (e.g., "USD/EUR 0.92").
        /// </summary>
        /// <param name="exchangeRateString">The exchange rate string to parse (e.g., "USD/EUR 0.92").</param>
        /// <param name="exchangeRateResult">The parsed exchange rate information.</param>
        [OSAction(
            Description = "Parses an exchange rate string in the format \"BASE/QUOTE rate\" (e.g., \"USD/EUR 0.92\").",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void ExchangeRateParse(
            [OSParameter(Description = "The exchange rate string to parse (e.g., \"USD/EUR 0.92\").")]
            string exchangeRateString,

            [OSParameter(Description = "The parsed exchange rate information.")]
            out ExchangeRateResult exchangeRateResult
        );

        #endregion

        #region Formatting & Parsing

        /// <summary>
        /// Formats a money value using the specified .NET format string.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="format">The .NET format string: "C" (currency), "N" (numeric), "F" (fixed-point), "G" (general).</param>
        /// <param name="formatted">The formatted money string.</param>
        [OSAction(
            Description = "Formats a money value using a .NET format string. Supported formats: \"C\" (currency), \"N\" (numeric), \"F\" (fixed-point), \"G\" (general).",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyFormat(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The .NET format string: \"C\" (currency), \"N\" (numeric), \"F\" (fixed-point), \"G\" (general).")]
            string format,

            [OSParameter(Description = "The formatted money string.")]
            out string formatted
        );

        /// <summary>
        /// Formats a money value using a specific culture (e.g., "en-US", "de-DE", "ja-JP").
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="cultureName">The culture name for formatting (e.g., "en-US", "de-DE", "ja-JP").</param>
        /// <param name="formatted">The culture-formatted money string.</param>
        [OSAction(
            Description = "Formats a money value using a specific culture (e.g., \"en-US\", \"de-DE\", \"ja-JP\").",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyFormatWithCulture(
            [OSParameter(Description = "The monetary amount.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "The culture name for formatting (e.g., \"en-US\", \"de-DE\", \"ja-JP\").")]
            string cultureName,

            [OSParameter(Description = "The culture-formatted money string.")]
            out string formatted
        );

        /// <summary>
        /// Parses a money string into a MoneyResult. Supports formats like "$1,234.56" or "EUR 1.234,56".
        /// </summary>
        /// <param name="moneyString">The money string to parse (e.g., "$1,234.56", "EUR 100.00").</param>
        /// <param name="moneyResult">The parsed money value.</param>
        [OSAction(
            Description = "Parses a money string into a MoneyResult. Supports formats like \"$1,234.56\" or \"EUR 100.00\".",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyParse(
            [OSParameter(Description = "The money string to parse (e.g., \"$1,234.56\", \"EUR 100.00\").")]
            string moneyString,

            [OSParameter(Description = "The parsed money value.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Parses a money string using a specific culture context.
        /// </summary>
        /// <param name="moneyString">The money string to parse.</param>
        /// <param name="cultureName">The culture name for parsing (e.g., "en-US", "de-DE").</param>
        /// <param name="moneyResult">The parsed money value.</param>
        [OSAction(
            Description = "Parses a money string using a specific culture context (e.g., \"en-US\", \"de-DE\").",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyParseWithCulture(
            [OSParameter(Description = "The money string to parse.")]
            string moneyString,

            [OSParameter(Description = "The culture name for parsing (e.g., \"en-US\", \"de-DE\").")]
            string cultureName,

            [OSParameter(Description = "The parsed money value.")]
            out MoneyResult moneyResult
        );

        /// <summary>
        /// Rounds a money amount to the specified number of decimal places, or to the currency's
        /// default decimal places if decimalDigits is -1.
        /// </summary>
        /// <param name="amount">The monetary amount to round.</param>
        /// <param name="currencyCode">ISO 4217 currency code.</param>
        /// <param name="decimalDigits">Number of decimal places to round to. Use -1 to round to the currency's default decimal digits.</param>
        /// <param name="moneyResult">The rounded money value.</param>
        [OSAction(
            Description = "Rounds a money amount to the specified decimal places. Use -1 to round to the currency's default decimal digits.",
            IconResourceName = "OutSystems.Extension.NodaMoney.resources.NodaMoney_icon.png"
        )]
        void MoneyRound(
            [OSParameter(Description = "The monetary amount to round.")]
            decimal amount,

            [OSParameter(Description = "ISO 4217 currency code.")]
            string currencyCode,

            [OSParameter(Description = "Number of decimal places to round to. Use -1 to round to the currency's default decimal digits.")]
            int decimalDigits,

            [OSParameter(Description = "The rounded money value.")]
            out MoneyResult moneyResult
        );

        #endregion
    }
}
