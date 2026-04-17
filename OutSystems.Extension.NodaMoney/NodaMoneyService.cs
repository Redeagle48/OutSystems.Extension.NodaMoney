using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using global::NodaMoney;
using global::NodaMoney.Exchange;
using OutSystems.Extension.NodaMoney.Structures;
using Money = global::NodaMoney.Money;
using Currency = global::NodaMoney.Currency;
using CurrencyInfo = global::NodaMoney.CurrencyInfo;
using ExchangeRate = global::NodaMoney.Exchange.ExchangeRate;

namespace OutSystems.Extension.NodaMoney
{
    /// <summary>
    /// Implementation of <see cref="INodaMoney"/> providing type-safe money and currency
    /// operations powered by the NodaMoney library with ISO 4217 compliance.
    /// </summary>
    public class NodaMoneyService : INodaMoney
    {
        // Upper bound on |amount| for any money operation. 10^15 is ~10x global annual GDP;
        // anything larger is either a bug upstream or an attempt to smuggle decimal.MaxValue
        // into business logic. Rejecting early prevents silent misuse of 79-octillion-dollar values.
        private const decimal MaxAmount = 1_000_000_000_000_000m;

        // Cache of all ISO 4217 currencies. The NodaMoney registry is immutable for a
        // given process, so the list is computed once per Lambda cold start and reused.
        private static readonly Lazy<IReadOnlyList<CurrencyResult>> _allCurrenciesCache =
            new(() => CurrencyInfo.GetAllCurrencies()
                .Select(info => new CurrencyResult(info.Code, info.Symbol, info.EnglishName, info.DecimalDigits))
                .ToList()
                .AsReadOnly());

        #region Private Helpers

        /// <summary>
        /// Rejects amounts whose absolute value exceeds <see cref="MaxAmount"/>.
        /// </summary>
        private static void ValidateAmount(decimal amount, string paramName)
        {
            if (Math.Abs(amount) > MaxAmount)
                throw new ArgumentOutOfRangeException(
                    paramName,
                    $"Amount must be within ±{MaxAmount:N0}.");
        }

        /// <summary>
        /// Creates a NodaMoney.Money value from amount and currency code.
        /// Validates the currency code and throws a descriptive error if invalid.
        /// </summary>
        /// <param name="amount">The monetary amount.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <returns>A NodaMoney.Money instance.</returns>
        private static Money CreateMoney(decimal amount, string currencyCode)
        {
            ArgumentNullException.ThrowIfNull(currencyCode, nameof(currencyCode));

            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be empty or whitespace.", nameof(currencyCode));

            ValidateAmount(amount, nameof(amount));

            var currency = Currency.FromCode(currencyCode.Trim().ToUpperInvariant());
            return new Money(amount, currency);
        }

        /// <summary>
        /// True when the string contains only digits, whitespace, sign, separators, and parentheses —
        /// i.e. no currency symbol or ISO letters for NodaMoney to key on.
        /// </summary>
        private static bool IsNumericOnly(string s)
        {
            foreach (var c in s)
            {
                if (!char.IsDigit(c) && !char.IsWhiteSpace(c) &&
                    c != '.' && c != ',' && c != '-' && c != '+' &&
                    c != '(' && c != ')')
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Parses a money string using a specific culture to resolve ambiguous currency symbols.
        /// Relies on <see cref="NumberStyles.Currency"/> to strip the culture's currency symbol,
        /// honour thousands/decimal separators, and accept parenthesised negatives.
        /// The resulting ISO currency code is derived from the culture's <see cref="RegionInfo"/>.
        /// </summary>
        private static Money ParseMoneyWithCulture(string moneyString, CultureInfo culture)
        {
            var region = new RegionInfo(culture.Name);
            var amount = decimal.Parse(moneyString.Trim(), NumberStyles.Currency, culture);
            return new Money(amount, region.ISOCurrencySymbol);
        }

        /// <summary>
        /// Converts a NodaMoney.Money instance to a MoneyResult structure.
        /// Uses NodaMoney's "L" (localised) format so the currency symbol matches the
        /// currency's ISO code (e.g. "$100.50" for USD) rather than the invariant "¤" placeholder.
        /// Also enforces <see cref="MaxAmount"/> so that arithmetic that overflows into
        /// unreasonable magnitudes fails loudly rather than polluting downstream flows.
        /// </summary>
        private static MoneyResult ToMoneyResult(Money money)
        {
            ValidateAmount(money.Amount, "amount");
            return new MoneyResult(
                money.Amount,
                money.Currency.Code,
                money.ToString("L")
            );
        }

        /// <summary>
        /// Validates that two currency codes are the same, throwing a descriptive error if not.
        /// </summary>
        /// <param name="code1">The first currency code.</param>
        /// <param name="code2">The second currency code.</param>
        private static void ValidateSameCurrency(string code1, string code2)
        {
            var normalized1 = code1?.Trim().ToUpperInvariant() ?? string.Empty;
            var normalized2 = code2?.Trim().ToUpperInvariant() ?? string.Empty;

            if (normalized1 != normalized2)
                throw new InvalidCurrencyException(
                    $"Currency mismatch: both values must use the same currency, " +
                    $"but received '{normalized1}' and '{normalized2}'. " +
                    $"Use ExchangeRateConvert to work with different currencies.");
        }

        /// <summary>
        /// Validates that a format string is in the allowed whitelist.
        /// Permits standard format specifiers (C, N, F, G) optionally followed by
        /// a single precision digit (0-9).
        /// </summary>
        private static bool IsAllowedFormat(string format)
        {
            if (format.Length < 1 || format.Length > 2)
                return false;

            char specifier = char.ToUpperInvariant(format[0]);
            if (specifier is not ('C' or 'N' or 'F' or 'G'))
                return false;

            if (format.Length == 2 && !char.IsAsciiDigit(format[1]))
                return false;

            return true;
        }

        #endregion

        #region Money Operations

        /// <inheritdoc />
        public void MoneyCreate(decimal amount, string currencyCode, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            var money = CreateMoney(amount, currencyCode);
            moneyResult = ToMoneyResult(money);
        }

        /// <inheritdoc />
        public void MoneyAdd(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ValidateSameCurrency(currencyCode1, currencyCode2);
            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);
            var result = m1 + m2;
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneySubtract(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ValidateSameCurrency(currencyCode1, currencyCode2);
            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);
            var result = m1 - m2;
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneyMultiply(decimal amount, string currencyCode, decimal multiplier, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            var money = CreateMoney(amount, currencyCode);
            var result = money * multiplier;
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneyDivide(decimal amount, string currencyCode, decimal divisor, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            if (divisor == 0m)
                throw new DivideByZeroException("Cannot divide money by zero.");

            var money = CreateMoney(amount, currencyCode);
            var result = money / divisor;
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneyDivideByMoney(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out decimal ratio)
        {
            ratio = 0m;

            ValidateSameCurrency(currencyCode1, currencyCode2);

            if (amount2 == 0m)
                throw new DivideByZeroException("Cannot divide by a zero money value.");

            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);
            ratio = m1 / m2;
        }

        /// <inheritdoc />
        public void MoneyAbs(decimal amount, string currencyCode, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            var money = CreateMoney(amount, currencyCode);
            var result = Money.Abs(money);
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneyNegate(decimal amount, string currencyCode, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            var money = CreateMoney(amount, currencyCode);
            var result = -money;
            moneyResult = ToMoneyResult(result);
        }

        /// <inheritdoc />
        public void MoneyCompare(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out int comparisonResult)
        {
            comparisonResult = 0;

            ValidateSameCurrency(currencyCode1, currencyCode2);
            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);
            comparisonResult = m1.CompareTo(m2);
        }

        /// <inheritdoc />
        public void MoneyIsZero(decimal amount, string currencyCode, out bool isZero)
        {
            isZero = false;

            var money = CreateMoney(amount, currencyCode);
            isZero = Money.IsZero(money);
        }

        /// <inheritdoc />
        public void MoneyIsPositive(decimal amount, string currencyCode, out bool isPositive)
        {
            isPositive = false;

            var money = CreateMoney(amount, currencyCode);
            isPositive = money.Amount > 0m;
        }

        /// <inheritdoc />
        public void MoneyIsNegative(decimal amount, string currencyCode, out bool isNegative)
        {
            isNegative = false;

            var money = CreateMoney(amount, currencyCode);
            isNegative = money.Amount < 0m;
        }

        /// <inheritdoc />
        public void MoneyMin(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ValidateSameCurrency(currencyCode1, currencyCode2);
            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);

            moneyResult = ToMoneyResult(m1.CompareTo(m2) <= 0 ? m1 : m2);
        }

        /// <inheritdoc />
        public void MoneyMax(decimal amount1, string currencyCode1, decimal amount2, string currencyCode2, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ValidateSameCurrency(currencyCode1, currencyCode2);
            var m1 = CreateMoney(amount1, currencyCode1);
            var m2 = CreateMoney(amount2, currencyCode2);

            if (m1.CompareTo(m2) >= 0)
                moneyResult = ToMoneyResult(m1);
            else
                moneyResult = ToMoneyResult(m2);
        }

        /// <inheritdoc />
        public void MoneySplit(decimal amount, string currencyCode, int numberOfParts, out List<MoneyAllocationItem> allocationItems)
        {
            allocationItems = new List<MoneyAllocationItem>();

            if (numberOfParts <= 0)
                throw new ArgumentException("Number of parts must be greater than zero.", nameof(numberOfParts));

            if (numberOfParts > 10_000)
                throw new ArgumentException("Number of parts cannot exceed 10,000.", nameof(numberOfParts));

            var money = CreateMoney(amount, currencyCode);

            // NodaMoney's Split requires >= 2 shares; handle 1 as special case
            if (numberOfParts == 1)
            {
                allocationItems.Add(new MoneyAllocationItem(money.Amount, money.Currency.Code, 1));
                return;
            }

            var parts = MoneyExtensions.Split(money, numberOfParts).ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                allocationItems.Add(new MoneyAllocationItem(
                    parts[i].Amount,
                    parts[i].Currency.Code,
                    i + 1
                ));
            }
        }

        #endregion

        #region Currency Operations

        /// <inheritdoc />
        public void CurrencyGetInfo(string currencyCode, out CurrencyResult currencyResult)
        {
            currencyResult = new CurrencyResult();

            ArgumentNullException.ThrowIfNull(currencyCode, nameof(currencyCode));
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be empty or whitespace.", nameof(currencyCode));

            var info = CurrencyInfo.FromCode(currencyCode.Trim().ToUpperInvariant());
            currencyResult = new CurrencyResult(
                info.Code,
                info.Symbol,
                info.EnglishName,
                info.DecimalDigits
            );
        }

        /// <inheritdoc />
        public void CurrencyGetAll(out List<CurrencyResult> currencies)
        {
            currencies = new List<CurrencyResult>(_allCurrenciesCache.Value);
        }

        /// <inheritdoc />
        public void CurrencyIsValid(string currencyCode, out bool isValid)
        {
            isValid = false;

            if (string.IsNullOrWhiteSpace(currencyCode))
                return;

            isValid = CurrencyInfo.TryFromCode(currencyCode.Trim().ToUpperInvariant(), out _);
        }

        #endregion

        #region Exchange Rate Operations

        /// <inheritdoc />
        public void ExchangeRateCreate(string baseCurrencyCode, string quoteCurrencyCode, decimal rate, out ExchangeRateResult exchangeRateResult)
        {
            exchangeRateResult = new ExchangeRateResult();

            ArgumentNullException.ThrowIfNull(baseCurrencyCode, nameof(baseCurrencyCode));
            ArgumentNullException.ThrowIfNull(quoteCurrencyCode, nameof(quoteCurrencyCode));

            if (string.IsNullOrWhiteSpace(baseCurrencyCode))
                throw new ArgumentException("Base currency code cannot be empty or whitespace.", nameof(baseCurrencyCode));
            if (string.IsNullOrWhiteSpace(quoteCurrencyCode))
                throw new ArgumentException("Quote currency code cannot be empty or whitespace.", nameof(quoteCurrencyCode));
            if (rate <= 0m)
                throw new ArgumentException("Exchange rate must be greater than zero.", nameof(rate));

            var baseCurrency = Currency.FromCode(baseCurrencyCode.Trim().ToUpperInvariant());
            var quoteCurrency = Currency.FromCode(quoteCurrencyCode.Trim().ToUpperInvariant());
            var exchangeRate = new ExchangeRate(baseCurrency, quoteCurrency, rate);

            exchangeRateResult = new ExchangeRateResult(
                exchangeRate.BaseCurrency.Code,
                exchangeRate.QuoteCurrency.Code,
                exchangeRate.Value
            );
        }

        /// <inheritdoc />
        public void ExchangeRateConvert(decimal amount, string fromCurrencyCode, string toCurrencyCode, decimal rate, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ArgumentNullException.ThrowIfNull(fromCurrencyCode, nameof(fromCurrencyCode));
            ArgumentNullException.ThrowIfNull(toCurrencyCode, nameof(toCurrencyCode));

            if (rate <= 0m)
                throw new ArgumentException("Exchange rate must be greater than zero.", nameof(rate));

            var fromCurrency = Currency.FromCode(fromCurrencyCode.Trim().ToUpperInvariant());
            var toCurrency = Currency.FromCode(toCurrencyCode.Trim().ToUpperInvariant());
            var exchangeRate = new ExchangeRate(fromCurrency, toCurrency, rate);
            var sourceMoney = new Money(amount, fromCurrency);
            var converted = exchangeRate.Convert(sourceMoney);

            moneyResult = ToMoneyResult(converted);
        }

        /// <inheritdoc />
        public void ExchangeRateParse(string exchangeRateString, out ExchangeRateResult exchangeRateResult)
        {
            exchangeRateResult = new ExchangeRateResult();

            ArgumentNullException.ThrowIfNull(exchangeRateString, nameof(exchangeRateString));
            if (string.IsNullOrWhiteSpace(exchangeRateString))
                throw new ArgumentException("Exchange rate string cannot be empty or whitespace.", nameof(exchangeRateString));

            var parsed = ExchangeRate.Parse(exchangeRateString);

            exchangeRateResult = new ExchangeRateResult(
                parsed.BaseCurrency.Code,
                parsed.QuoteCurrency.Code,
                parsed.Value
            );
        }

        #endregion

        #region Formatting & Parsing

        /// <inheritdoc />
        public void MoneyFormat(decimal amount, string currencyCode, string format, out string formatted)
        {
            formatted = string.Empty;

            ArgumentNullException.ThrowIfNull(format, nameof(format));
            if (string.IsNullOrWhiteSpace(format))
                throw new ArgumentException("Format string cannot be empty or whitespace.", nameof(format));

            var trimmed = format.Trim();
            if (!IsAllowedFormat(trimmed))
                throw new ArgumentException(
                    "Format must be one of: C, N, F, G (optionally followed by a precision digit 0-9).",
                    nameof(format));

            var money = CreateMoney(amount, currencyCode);
            formatted = money.ToString(trimmed, CultureInfo.InvariantCulture);
        }

        /// <inheritdoc />
        public void MoneyFormatWithCulture(decimal amount, string currencyCode, string cultureName, out string formatted)
        {
            formatted = string.Empty;

            ArgumentNullException.ThrowIfNull(cultureName, nameof(cultureName));
            if (string.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentException("Culture name cannot be empty or whitespace.", nameof(cultureName));

            var money = CreateMoney(amount, currencyCode);
            var culture = CultureInfo.GetCultureInfo(cultureName.Trim());
            formatted = money.ToString("C", culture);
        }

        /// <inheritdoc />
        public void MoneyParse(string moneyString, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ArgumentNullException.ThrowIfNull(moneyString, nameof(moneyString));
            if (string.IsNullOrWhiteSpace(moneyString))
                throw new ArgumentException("Money string cannot be empty or whitespace.", nameof(moneyString));

            Money parsed;
            try
            {
                parsed = Money.Parse(moneyString);
            }
            catch (FormatException ex) when (ex.Message.Contains("multiple currencies"))
            {
                // "$" alone is ambiguous (USD, CAD, AUD, ...). Default to USD only for a bare "$"
                // with no letter prefix — rules out "A$", "CA$", "HK$", "S$", "NZ$", etc., which
                // would silently collapse to USD in the previous implementation.
                var trimmed = moneyString.TrimStart();
                if (trimmed.StartsWith('$') && !moneyString.Any(char.IsLetter))
                {
                    parsed = ParseMoneyWithCulture(moneyString, CultureInfo.GetCultureInfo("en-US"));
                }
                else
                {
                    throw new FormatException(
                        $"Cannot determine currency from '{moneyString}': the symbol is ambiguous across multiple currencies. " +
                        "Use MoneyParseWithCulture with an explicit culture name (e.g. \"en-AU\", \"en-CA\") to disambiguate.",
                        ex);
                }
            }
            moneyResult = ToMoneyResult(parsed);
        }

        /// <inheritdoc />
        public void MoneyParseWithCulture(string moneyString, string cultureName, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ArgumentNullException.ThrowIfNull(moneyString, nameof(moneyString));
            ArgumentNullException.ThrowIfNull(cultureName, nameof(cultureName));
            if (string.IsNullOrWhiteSpace(moneyString))
                throw new ArgumentException("Money string cannot be empty or whitespace.", nameof(moneyString));
            if (string.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentException("Culture name cannot be empty or whitespace.", nameof(cultureName));

            var culture = CultureInfo.GetCultureInfo(cultureName.Trim());
            Money parsed;

            // When the string has no symbol or ISO letters, NodaMoney's Money.Parse ignores the
            // supplied culture for currency resolution and falls back to a process default (USD).
            // Honour the caller's culture by deriving the currency from its RegionInfo directly.
            if (IsNumericOnly(moneyString))
            {
                parsed = ParseMoneyWithCulture(moneyString, culture);
            }
            else
            {
                try
                {
                    parsed = Money.Parse(moneyString, culture);
                }
                catch (FormatException ex) when (ex.Message.Contains("multiple currencies"))
                {
                    parsed = ParseMoneyWithCulture(moneyString, culture);
                }
            }
            moneyResult = ToMoneyResult(parsed);
        }

        /// <inheritdoc />
        public void MoneyRound(decimal amount, string currencyCode, int decimalDigits, out MoneyResult moneyResult)
        {
            moneyResult = new MoneyResult();

            ArgumentNullException.ThrowIfNull(currencyCode, nameof(currencyCode));
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code cannot be empty or whitespace.", nameof(currencyCode));

            var normalizedCode = currencyCode.Trim().ToUpperInvariant();
            var currencyInfo = CurrencyInfo.FromCode(normalizedCode);
            var currency = Currency.FromCode(normalizedCode);

            int digits = decimalDigits == -1 ? currencyInfo.DecimalDigits : decimalDigits;

            if (digits < 0 || digits > 28)
                throw new ArgumentException("Decimal digits must be -1 (for currency default) or between 0 and 28.", nameof(decimalDigits));

            var rounded = Math.Round(amount, digits, MidpointRounding.ToEven);
            var money = new Money(rounded, currency);
            moneyResult = ToMoneyResult(money);
        }

        #endregion
    }
}
