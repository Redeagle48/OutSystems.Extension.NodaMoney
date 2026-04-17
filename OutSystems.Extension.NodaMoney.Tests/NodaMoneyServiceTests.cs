using OutSystems.Extension.NodaMoney;
using OutSystems.Extension.NodaMoney.Structures;
using global::NodaMoney;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OutSystems.Extension.NodaMoney.Tests
{
    /// <summary>
    /// Comprehensive unit tests for the NodaMoneyService implementation.
    /// Covers all 26 actions across money operations, currency operations,
    /// exchange rate operations, and formatting/parsing.
    /// </summary>
    [TestFixture]
    public class NodaMoneyServiceTests
    {
        private NodaMoneyService _sut = null!;

        [SetUp]
        public void SetUp()
        {
            _sut = new NodaMoneyService();
        }

        #region Helper Methods

        /// <summary>
        /// Helper to call MoneyCreate and return the result directly.
        /// </summary>
        private MoneyResult Create(decimal amount, string currencyCode)
        {
            _sut.MoneyCreate(amount, currencyCode, out var result);
            return result;
        }

        /// <summary>
        /// Helper to call MoneyAdd and return the result directly.
        /// </summary>
        private MoneyResult Add(decimal a1, string c1, decimal a2, string c2)
        {
            _sut.MoneyAdd(a1, c1, a2, c2, out var result);
            return result;
        }

        /// <summary>
        /// Helper to call MoneySubtract and return the result directly.
        /// </summary>
        private MoneyResult Subtract(decimal a1, string c1, decimal a2, string c2)
        {
            _sut.MoneySubtract(a1, c1, a2, c2, out var result);
            return result;
        }

        #endregion

        #region MoneyCreate Tests

        [TestCase(100.50, "USD", 100.50, "USD")]
        [TestCase(0, "EUR", 0, "EUR")]
        [TestCase(-50.75, "GBP", -50.75, "GBP")]
        [TestCase(1000, "JPY", 1000, "JPY")]
        [TestCase(99.999, "USD", 100.00, "USD")] // Rounds to 2 decimal places for USD
        public void MoneyCreate_ValidInput_ReturnsExpectedResult(
            decimal inputAmount, string inputCurrency, decimal expectedAmount, string expectedCurrency)
        {
            var result = Create(inputAmount, inputCurrency);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expectedAmount));
                Assert.That(result.CurrencyCode, Is.EqualTo(expectedCurrency));
                Assert.That(result.Formatted, Is.Not.Empty);
            });
        }

        [Test]
        public void MoneyCreate_NullCurrencyCode_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyCreate(100m, null!, out _));
        }

        [Test]
        public void MoneyCreate_EmptyCurrencyCode_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyCreate(100m, "", out _));
        }

        [Test]
        public void MoneyCreate_InvalidCurrencyCode_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyCreate(100m, "INVALID", out _));
        }

        [Test]
        public void MoneyCreate_CaseInsensitiveCurrency_Works()
        {
            var result = Create(100m, "usd");

            Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
        }

        [Test]
        public void MoneyCreate_CurrencyWithWhitespace_TrimsAndWorks()
        {
            var result = Create(100m, "  EUR  ");

            Assert.That(result.CurrencyCode, Is.EqualTo("EUR"));
        }

        #endregion

        #region MoneyAdd Tests

        [Test]
        public void MoneyAdd_SameCurrency_ReturnsSumCorrectly()
        {
            var result = Add(100.50m, "USD", 50.25m, "USD");

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(150.75m));
                Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
            });
        }

        [Test]
        public void MoneyAdd_NegativeAmounts_ReturnsCorrectSum()
        {
            var result = Add(-100m, "EUR", -50m, "EUR");

            Assert.That(result.Amount, Is.EqualTo(-150m));
        }

        [Test]
        public void MoneyAdd_ZeroAmount_ReturnsOtherAmount()
        {
            var result = Add(0m, "USD", 42.00m, "USD");

            Assert.That(result.Amount, Is.EqualTo(42.00m));
        }

        [Test]
        public void MoneyAdd_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyAdd(100m, "USD", 50m, "EUR", out _));
        }

        #endregion

        #region MoneySubtract Tests

        [Test]
        public void MoneySubtract_SameCurrency_ReturnsDifferenceCorrectly()
        {
            var result = Subtract(100.50m, "USD", 50.25m, "USD");

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(50.25m));
                Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
            });
        }

        [Test]
        public void MoneySubtract_ResultIsNegative_ReturnsNegativeAmount()
        {
            var result = Subtract(10m, "EUR", 50m, "EUR");

            Assert.That(result.Amount, Is.EqualTo(-40m));
        }

        [Test]
        public void MoneySubtract_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneySubtract(100m, "USD", 50m, "GBP", out _));
        }

        #endregion

        #region MoneyMultiply Tests

        [TestCase(100, "USD", 2.5, 250.00)]
        [TestCase(33.33, "EUR", 3, 99.99)]
        [TestCase(50, "GBP", 0, 0)]
        [TestCase(100, "USD", -1, -100.00)]
        public void MoneyMultiply_ValidInput_ReturnsExpectedProduct(
            decimal amount, string currency, decimal multiplier, decimal expected)
        {
            _sut.MoneyMultiply(amount, currency, multiplier, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expected));
                Assert.That(result.CurrencyCode, Is.EqualTo(currency));
            });
        }

        #endregion

        #region MoneyDivide Tests

        [TestCase(100, "USD", 3, 33.33)]
        [TestCase(100, "USD", 4, 25.00)]
        [TestCase(1000, "JPY", 3, 333)]
        public void MoneyDivide_ValidInput_ReturnsExpectedQuotient(
            decimal amount, string currency, decimal divisor, decimal expected)
        {
            _sut.MoneyDivide(amount, currency, divisor, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expected));
                Assert.That(result.CurrencyCode, Is.EqualTo(currency));
            });
        }

        [Test]
        public void MoneyDivide_ByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() =>
                _sut.MoneyDivide(100m, "USD", 0m, out _));
        }

        #endregion

        #region MoneyDivideByMoney Tests

        [Test]
        public void MoneyDivideByMoney_ValidInput_ReturnsCorrectRatio()
        {
            _sut.MoneyDivideByMoney(100m, "USD", 50m, "USD", out var ratio);

            Assert.That(ratio, Is.EqualTo(2m));
        }

        [Test]
        public void MoneyDivideByMoney_FractionalRatio_ReturnsCorrectValue()
        {
            _sut.MoneyDivideByMoney(100m, "EUR", 30m, "EUR", out var ratio);

            Assert.That(ratio, Is.GreaterThan(3m));
            Assert.That(ratio, Is.LessThan(4m));
        }

        [Test]
        public void MoneyDivideByMoney_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyDivideByMoney(100m, "USD", 50m, "EUR", out _));
        }

        [Test]
        public void MoneyDivideByMoney_ByZero_ThrowsDivideByZeroException()
        {
            Assert.Throws<DivideByZeroException>(() =>
                _sut.MoneyDivideByMoney(100m, "USD", 0m, "USD", out _));
        }

        #endregion

        #region MoneyAbs Tests

        [TestCase(-100.50, "USD", 100.50)]
        [TestCase(100.50, "USD", 100.50)]
        [TestCase(0, "EUR", 0)]
        public void MoneyAbs_ValidInput_ReturnsAbsoluteValue(
            decimal amount, string currency, decimal expectedAbs)
        {
            _sut.MoneyAbs(amount, currency, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expectedAbs));
                Assert.That(result.CurrencyCode, Is.EqualTo(currency));
            });
        }

        #endregion

        #region MoneyNegate Tests

        [TestCase(100, "USD", -100)]
        [TestCase(-50, "EUR", 50)]
        [TestCase(0, "GBP", 0)]
        public void MoneyNegate_ValidInput_ReturnsNegatedValue(
            decimal amount, string currency, decimal expectedNegated)
        {
            _sut.MoneyNegate(amount, currency, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expectedNegated));
                Assert.That(result.CurrencyCode, Is.EqualTo(currency));
            });
        }

        #endregion

        #region MoneyCompare Tests

        [TestCase(100, "USD", 50, "USD", 1)]
        [TestCase(50, "USD", 100, "USD", -1)]
        [TestCase(100, "EUR", 100, "EUR", 0)]
        public void MoneyCompare_ValidInput_ReturnsExpectedComparison(
            decimal a1, string c1, decimal a2, string c2, int expected)
        {
            _sut.MoneyCompare(a1, c1, a2, c2, out var result);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void MoneyCompare_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyCompare(100m, "USD", 100m, "EUR", out _));
        }

        #endregion

        #region MoneyIsZero Tests

        [Test]
        public void MoneyIsZero_ZeroAmount_ReturnsTrue()
        {
            _sut.MoneyIsZero(0m, "USD", out var isZero);

            Assert.That(isZero, Is.True);
        }

        [Test]
        public void MoneyIsZero_NonZeroAmount_ReturnsFalse()
        {
            _sut.MoneyIsZero(100m, "USD", out var isZero);

            Assert.That(isZero, Is.False);
        }

        [Test]
        public void MoneyIsZero_NegativeAmount_ReturnsFalse()
        {
            _sut.MoneyIsZero(-0.01m, "USD", out var isZero);

            Assert.That(isZero, Is.False);
        }

        #endregion

        #region MoneyIsPositive Tests

        [Test]
        public void MoneyIsPositive_PositiveAmount_ReturnsTrue()
        {
            _sut.MoneyIsPositive(100m, "USD", out var isPositive);

            Assert.That(isPositive, Is.True);
        }

        [Test]
        public void MoneyIsPositive_ZeroAmount_ReturnsFalse()
        {
            _sut.MoneyIsPositive(0m, "USD", out var isPositive);

            // NodaMoney considers zero as not positive
            Assert.That(isPositive, Is.False);
        }

        [Test]
        public void MoneyIsPositive_NegativeAmount_ReturnsFalse()
        {
            _sut.MoneyIsPositive(-100m, "USD", out var isPositive);

            Assert.That(isPositive, Is.False);
        }

        #endregion

        #region MoneyIsNegative Tests

        [Test]
        public void MoneyIsNegative_NegativeAmount_ReturnsTrue()
        {
            _sut.MoneyIsNegative(-100m, "USD", out var isNegative);

            Assert.That(isNegative, Is.True);
        }

        [Test]
        public void MoneyIsNegative_PositiveAmount_ReturnsFalse()
        {
            _sut.MoneyIsNegative(100m, "USD", out var isNegative);

            Assert.That(isNegative, Is.False);
        }

        [Test]
        public void MoneyIsNegative_ZeroAmount_ReturnsFalse()
        {
            _sut.MoneyIsNegative(0m, "USD", out var isNegative);

            Assert.That(isNegative, Is.False);
        }

        #endregion

        #region MoneyMin Tests

        [Test]
        public void MoneyMin_FirstIsSmaller_ReturnsFirst()
        {
            _sut.MoneyMin(50m, "USD", 100m, "USD", out var result);

            Assert.That(result.Amount, Is.EqualTo(50m));
        }

        [Test]
        public void MoneyMin_SecondIsSmaller_ReturnsSecond()
        {
            _sut.MoneyMin(100m, "EUR", 25m, "EUR", out var result);

            Assert.That(result.Amount, Is.EqualTo(25m));
        }

        [Test]
        public void MoneyMin_EqualAmounts_ReturnsEither()
        {
            _sut.MoneyMin(100m, "USD", 100m, "USD", out var result);

            Assert.That(result.Amount, Is.EqualTo(100m));
        }

        [Test]
        public void MoneyMin_WithNegativeValues_ReturnsSmaller()
        {
            _sut.MoneyMin(-100m, "USD", -50m, "USD", out var result);

            Assert.That(result.Amount, Is.EqualTo(-100m));
        }

        [Test]
        public void MoneyMin_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyMin(100m, "USD", 50m, "EUR", out _));
        }

        #endregion

        #region MoneyMax Tests

        [Test]
        public void MoneyMax_FirstIsLarger_ReturnsFirst()
        {
            _sut.MoneyMax(100m, "USD", 50m, "USD", out var result);

            Assert.That(result.Amount, Is.EqualTo(100m));
        }

        [Test]
        public void MoneyMax_SecondIsLarger_ReturnsSecond()
        {
            _sut.MoneyMax(25m, "EUR", 100m, "EUR", out var result);

            Assert.That(result.Amount, Is.EqualTo(100m));
        }

        [Test]
        public void MoneyMax_WithNegativeValues_ReturnsLarger()
        {
            _sut.MoneyMax(-100m, "USD", -50m, "USD", out var result);

            Assert.That(result.Amount, Is.EqualTo(-50m));
        }

        [Test]
        public void MoneyMax_DifferentCurrencies_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.MoneyMax(100m, "USD", 50m, "EUR", out _));
        }

        #endregion

        #region MoneySplit Tests

        [Test]
        public void MoneySplit_EvenSplit_ReturnsEqualParts()
        {
            _sut.MoneySplit(100m, "USD", 4, out var items);

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Count.EqualTo(4));
                Assert.That(items.All(i => i.Amount == 25.00m), Is.True);
                Assert.That(items.All(i => i.CurrencyCode == "USD"), Is.True);
            });
        }

        [Test]
        public void MoneySplit_UnevenSplit_DistributesRemainderFairly()
        {
            _sut.MoneySplit(100m, "USD", 3, out var items);

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Count.EqualTo(3));
                // Total must equal original amount
                var total = items.Sum(i => i.Amount);
                Assert.That(total, Is.EqualTo(100m));
                Assert.That(items.All(i => i.CurrencyCode == "USD"), Is.True);
            });
        }

        [Test]
        public void MoneySplit_SinglePart_ReturnsEntireAmount()
        {
            _sut.MoneySplit(100m, "USD", 1, out var items);

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Count.EqualTo(1));
                Assert.That(items[0].Amount, Is.EqualTo(100m));
                Assert.That(items[0].Index, Is.EqualTo(1));
            });
        }

        [Test]
        public void MoneySplit_IndicesAreOneBased()
        {
            _sut.MoneySplit(100m, "EUR", 3, out var items);

            var indices = items.Select(i => i.Index).ToList();
            Assert.That(indices, Is.EquivalentTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void MoneySplit_ZeroParts_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneySplit(100m, "USD", 0, out _));
        }

        [Test]
        public void MoneySplit_NegativeParts_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneySplit(100m, "USD", -1, out _));
        }

        [Test]
        public void MoneySplit_ExceedsUpperBound_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneySplit(100m, "USD", 10_001, out _));
        }

        [Test]
        public void MoneySplit_AtUpperBound_DoesNotThrow()
        {
            _sut.MoneySplit(100m, "USD", 10_000, out var items);

            Assert.That(items, Has.Count.EqualTo(10_000));
            Assert.That(items.Sum(i => i.Amount), Is.EqualTo(100m));
        }

        [Test]
        public void MoneySplit_SmallAmountManyParts_PreservesTotal()
        {
            _sut.MoneySplit(0.01m, "USD", 3, out var items);

            var total = items.Sum(i => i.Amount);
            Assert.That(total, Is.EqualTo(0.01m));
        }

        [Test]
        public void MoneySplit_JpyNoCents_SplitsCorrectly()
        {
            _sut.MoneySplit(100m, "JPY", 3, out var items);

            Assert.Multiple(() =>
            {
                Assert.That(items, Has.Count.EqualTo(3));
                var total = items.Sum(i => i.Amount);
                Assert.That(total, Is.EqualTo(100m));
            });
        }

        #endregion

        #region CurrencyGetInfo Tests

        [Test]
        public void CurrencyGetInfo_USD_ReturnsCorrectInfo()
        {
            _sut.CurrencyGetInfo("USD", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Code, Is.EqualTo("USD"));
                Assert.That(result.Symbol, Is.EqualTo("$"));
                Assert.That(result.EnglishName, Is.EqualTo("United States dollar"));
                Assert.That(result.DecimalDigits, Is.EqualTo(2));
            });
        }

        [Test]
        public void CurrencyGetInfo_EUR_ReturnsCorrectInfo()
        {
            _sut.CurrencyGetInfo("EUR", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Code, Is.EqualTo("EUR"));
                Assert.That(result.Symbol, Is.EqualTo("\u20ac"));
                Assert.That(result.EnglishName, Is.EqualTo("Euro"));
                Assert.That(result.DecimalDigits, Is.EqualTo(2));
            });
        }

        [Test]
        public void CurrencyGetInfo_JPY_ReturnsZeroDecimalDigits()
        {
            _sut.CurrencyGetInfo("JPY", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Code, Is.EqualTo("JPY"));
                Assert.That(result.DecimalDigits, Is.EqualTo(0));
            });
        }

        [Test]
        public void CurrencyGetInfo_InvalidCode_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.CurrencyGetInfo("XYZ123", out _));
        }

        [Test]
        public void CurrencyGetInfo_NullCode_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.CurrencyGetInfo(null!, out _));
        }

        [Test]
        public void CurrencyGetInfo_CaseInsensitive_Works()
        {
            _sut.CurrencyGetInfo("eur", out var result);

            Assert.That(result.Code, Is.EqualTo("EUR"));
        }

        #endregion

        #region CurrencyGetAll Tests

        [Test]
        public void CurrencyGetAll_ReturnsNonEmptyList()
        {
            _sut.CurrencyGetAll(out var currencies);

            Assert.That(currencies, Is.Not.Empty);
            Assert.That(currencies.Count, Is.GreaterThan(100));
        }

        [Test]
        public void CurrencyGetAll_ContainsUSD()
        {
            _sut.CurrencyGetAll(out var currencies);

            var usd = currencies.FirstOrDefault(c => c.Code == "USD");
            Assert.Multiple(() =>
            {
                Assert.That(usd.Code, Is.EqualTo("USD"));
                Assert.That(usd.Symbol, Is.EqualTo("$"));
            });
        }

        [Test]
        public void CurrencyGetAll_AllHaveNonEmptyCode()
        {
            _sut.CurrencyGetAll(out var currencies);

            Assert.That(currencies.All(c => !string.IsNullOrEmpty(c.Code)), Is.True);
        }

        #endregion

        #region CurrencyIsValid Tests

        [TestCase("USD", true)]
        [TestCase("EUR", true)]
        [TestCase("JPY", true)]
        [TestCase("GBP", true)]
        [TestCase("INVALID", false)]
        [TestCase("XYZ123", false)]
        [TestCase("", false)]
        [TestCase("   ", false)]
        public void CurrencyIsValid_VariousInputs_ReturnsExpected(string code, bool expected)
        {
            _sut.CurrencyIsValid(code, out var isValid);

            Assert.That(isValid, Is.EqualTo(expected));
        }

        [Test]
        public void CurrencyIsValid_NullInput_ReturnsFalse()
        {
            _sut.CurrencyIsValid(null!, out var isValid);

            Assert.That(isValid, Is.False);
        }

        [Test]
        public void CurrencyIsValid_CaseInsensitive_ReturnsTrue()
        {
            _sut.CurrencyIsValid("usd", out var isValid);

            Assert.That(isValid, Is.True);
        }

        #endregion

        #region ExchangeRateCreate Tests

        [Test]
        public void ExchangeRateCreate_ValidInput_ReturnsCorrectRate()
        {
            _sut.ExchangeRateCreate("USD", "EUR", 0.92m, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.BaseCurrencyCode, Is.EqualTo("USD"));
                Assert.That(result.QuoteCurrencyCode, Is.EqualTo("EUR"));
                Assert.That(result.Rate, Is.EqualTo(0.92m));
            });
        }

        [Test]
        public void ExchangeRateCreate_ZeroRate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.ExchangeRateCreate("USD", "EUR", 0m, out _));
        }

        [Test]
        public void ExchangeRateCreate_NegativeRate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.ExchangeRateCreate("USD", "EUR", -0.5m, out _));
        }

        [Test]
        public void ExchangeRateCreate_InvalidBaseCurrency_ThrowsInvalidCurrencyException()
        {
            Assert.Throws<InvalidCurrencyException>(() =>
                _sut.ExchangeRateCreate("INVALID", "EUR", 0.92m, out _));
        }

        [Test]
        public void ExchangeRateCreate_NullBaseCurrency_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.ExchangeRateCreate(null!, "EUR", 0.92m, out _));
        }

        #endregion

        #region ExchangeRateConvert Tests

        [Test]
        public void ExchangeRateConvert_UsdToEur_ReturnsConvertedAmount()
        {
            _sut.ExchangeRateConvert(100m, "USD", "EUR", 0.92m, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(92.00m));
                Assert.That(result.CurrencyCode, Is.EqualTo("EUR"));
            });
        }

        [Test]
        public void ExchangeRateConvert_LargeAmount_ReturnsCorrectResult()
        {
            _sut.ExchangeRateConvert(1000000m, "USD", "JPY", 150.5m, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(150500000m));
                Assert.That(result.CurrencyCode, Is.EqualTo("JPY"));
            });
        }

        [Test]
        public void ExchangeRateConvert_ZeroRate_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.ExchangeRateConvert(100m, "USD", "EUR", 0m, out _));
        }

        [Test]
        public void ExchangeRateConvert_NullFromCurrency_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.ExchangeRateConvert(100m, null!, "EUR", 0.92m, out _));
        }

        #endregion

        #region ExchangeRateParse Tests

        [Test]
        public void ExchangeRateParse_ValidString_ReturnsCorrectRate()
        {
            _sut.ExchangeRateParse("USD/EUR 0.92", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.BaseCurrencyCode, Is.EqualTo("USD"));
                Assert.That(result.QuoteCurrencyCode, Is.EqualTo("EUR"));
                Assert.That(result.Rate, Is.EqualTo(0.92m));
            });
        }

        [Test]
        public void ExchangeRateParse_NullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.ExchangeRateParse(null!, out _));
        }

        [Test]
        public void ExchangeRateParse_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.ExchangeRateParse("", out _));
        }

        [Test]
        public void ExchangeRateParse_InvalidFormat_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() =>
                _sut.ExchangeRateParse("not a rate", out _));
        }

        #endregion

        #region MoneyFormat Tests

        [Test]
        public void MoneyFormat_CurrencyFormat_ReturnsFormattedString()
        {
            _sut.MoneyFormat(1234.56m, "USD", "C", out var formatted);

            Assert.That(formatted, Is.Not.Empty);
            Assert.That(formatted, Does.Contain("1"));
        }

        [Test]
        public void MoneyFormat_NumericFormat_ReturnsNumericString()
        {
            _sut.MoneyFormat(1234.56m, "USD", "N", out var formatted);

            Assert.That(formatted, Is.Not.Empty);
        }

        [Test]
        public void MoneyFormat_NullFormat_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyFormat(100m, "USD", null!, out _));
        }

        [Test]
        public void MoneyFormat_EmptyFormat_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyFormat(100m, "USD", "", out _));
        }

        [TestCase("C")]
        [TestCase("N")]
        [TestCase("F")]
        [TestCase("G")]
        [TestCase("C2")]
        [TestCase("N0")]
        [TestCase("F3")]
        public void MoneyFormat_AllowedFormats_DoNotThrow(string format)
        {
            Assert.DoesNotThrow(() =>
                _sut.MoneyFormat(100m, "USD", format, out _));
        }

        [TestCase("####")]
        [TestCase("0.00%")]
        [TestCase("XXXX")]
        [TestCase("C99")]
        public void MoneyFormat_DisallowedFormats_ThrowArgumentException(string format)
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyFormat(100m, "USD", format, out _));
        }

        #endregion

        #region MoneyFormatWithCulture Tests

        [Test]
        public void MoneyFormatWithCulture_EnUs_ReturnsUsDollarFormat()
        {
            _sut.MoneyFormatWithCulture(1234.56m, "USD", "en-US", out var formatted);

            Assert.That(formatted, Does.Contain("$"));
            Assert.That(formatted, Does.Contain("1"));
        }

        [Test]
        public void MoneyFormatWithCulture_DeDe_ReturnsGermanFormat()
        {
            _sut.MoneyFormatWithCulture(1234.56m, "EUR", "de-DE", out var formatted);

            Assert.That(formatted, Is.Not.Empty);
        }

        [Test]
        public void MoneyFormatWithCulture_InvalidCulture_DoesNotThrowOnAllPlatforms()
        {
            // On macOS/Linux, CultureInfo.GetCultureInfo may not throw for unknown cultures.
            // This test verifies the call completes without unhandled exceptions.
            Assert.DoesNotThrow(() =>
                _sut.MoneyFormatWithCulture(100m, "USD", "xx-INVALID", out _));
        }

        [Test]
        public void MoneyFormatWithCulture_NullCulture_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyFormatWithCulture(100m, "USD", null!, out _));
        }

        #endregion

        #region MoneyParse Tests

        [Test]
        public void MoneyParse_DollarSign_ParsesCorrectly()
        {
            _sut.MoneyParse("$1,234.56", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(1234.56m));
                Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
            });
        }

        [Test]
        public void MoneyParse_NullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyParse(null!, out _));
        }

        [Test]
        public void MoneyParse_EmptyString_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyParse("", out _));
        }

        #endregion

        #region MoneyParseWithCulture Tests

        [Test]
        public void MoneyParseWithCulture_EnUs_ParsesCorrectly()
        {
            _sut.MoneyParseWithCulture("$1,234.56", "en-US", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(1234.56m));
                Assert.That(result.CurrencyCode, Is.EqualTo("USD"));
            });
        }

        [Test]
        public void MoneyParseWithCulture_NullMoneyString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyParseWithCulture(null!, "en-US", out _));
        }

        [Test]
        public void MoneyParseWithCulture_NullCulture_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyParseWithCulture("$100", null!, out _));
        }

        [Test]
        public void MoneyParseWithCulture_NumericOnlyDeDe_ReturnsEur()
        {
            // "1.234,56" has no currency symbol or ISO letters. Money.Parse would default to USD
            // regardless of culture. With the culture-honouring fix, the region currency (EUR) is used.
            _sut.MoneyParseWithCulture("1.234,56", "de-DE", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(1234.56m));
                Assert.That(result.CurrencyCode, Is.EqualTo("EUR"));
            });
        }

        [Test]
        public void MoneyParseWithCulture_NumericOnlyJaJp_ReturnsJpy()
        {
            _sut.MoneyParseWithCulture("100", "ja-JP", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(100m));
                Assert.That(result.CurrencyCode, Is.EqualTo("JPY"));
            });
        }

        [Test]
        public void MoneyParseWithCulture_NumericOnlyEnGb_ReturnsGbp()
        {
            _sut.MoneyParseWithCulture("1,234.56", "en-GB", out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(1234.56m));
                Assert.That(result.CurrencyCode, Is.EqualTo("GBP"));
            });
        }

        #endregion

        #region MoneyRound Tests

        [TestCase(10.456, "USD", 2, 10.46)]
        [TestCase(10.454, "USD", 2, 10.45)]
        [TestCase(10.5, "USD", 0, 10)]
        [TestCase(1234.5678, "BHD", 3, 1234.568)]
        public void MoneyRound_SpecificDigits_RoundsCorrectly(
            decimal amount, string currency, int digits, decimal expected)
        {
            _sut.MoneyRound(amount, currency, digits, out var result);

            Assert.Multiple(() =>
            {
                Assert.That(result.Amount, Is.EqualTo(expected));
                Assert.That(result.CurrencyCode, Is.EqualTo(currency));
            });
        }

        [Test]
        public void MoneyRound_DefaultDigits_UseCurrencyDefault()
        {
            // USD has 2 decimal digits
            _sut.MoneyRound(10.456m, "USD", -1, out var result);

            Assert.That(result.Amount, Is.EqualTo(10.46m));
        }

        [Test]
        public void MoneyRound_JpyDefault_RoundsToZeroDecimals()
        {
            // JPY has 0 decimal digits
            _sut.MoneyRound(1234.5m, "JPY", -1, out var result);

            Assert.That(result.Amount, Is.EqualTo(1234m));
        }

        [Test]
        public void MoneyRound_NullCurrency_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _sut.MoneyRound(100m, null!, 2, out _));
        }

        [Test]
        public void MoneyRound_InvalidDecimalDigits_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyRound(100m, "USD", -2, out _));
        }

        [Test]
        public void MoneyRound_ExceedsMaxDecimalDigits_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _sut.MoneyRound(100m, "USD", 29, out _));
        }

        [Test]
        public void MoneyRound_AtMaxDecimalDigits_DoesNotThrow()
        {
            Assert.DoesNotThrow(() =>
                _sut.MoneyRound(100m, "USD", 28, out _));
        }

        #endregion

        #region Edge Cases and Integration Tests

        [Test]
        public void MoneyCreate_MaxDecimal_DoesNotThrow()
        {
            // Very large amount should still work
            Assert.DoesNotThrow(() =>
                _sut.MoneyCreate(999999999999m, "USD", out _));
        }

        [Test]
        public void MoneyCreate_AmountAboveCap_ThrowsArgumentOutOfRange()
        {
            // MaxAmount is 10^15; anything above should be rejected.
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _sut.MoneyCreate(1_000_000_000_000_001m, "USD", out _));
        }

        [Test]
        public void MoneyCreate_AmountBelowNegativeCap_ThrowsArgumentOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _sut.MoneyCreate(-1_000_000_000_000_001m, "USD", out _));
        }

        [Test]
        public void MoneyParse_DecimalMaxValue_ThrowsArgumentOutOfRange()
        {
            // The amount bound applies post-parse: a 79-octillion-dollar string must not
            // leak into downstream flows.
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _sut.MoneyParse("$79228162514264337593543950335", out _));
        }

        [Test]
        public void MoneyMultiply_OverflowsCap_ThrowsArgumentOutOfRange()
        {
            // 9e14 * 10 = 9e15, which exceeds MaxAmount (1e15) — caught by output-side validation.
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                _sut.MoneyMultiply(900_000_000_000_000m, "USD", 10m, out _));
        }

        [Test]
        public void MoneyAdd_ThenSubtract_ReturnsOriginal()
        {
            var original = Create(100.50m, "USD");
            var added = Add(original.Amount, original.CurrencyCode, 50.25m, "USD");
            var result = Subtract(added.Amount, added.CurrencyCode, 50.25m, "USD");

            Assert.That(result.Amount, Is.EqualTo(original.Amount));
        }

        [Test]
        public void MoneyMultiply_ThenDivide_ReturnsOriginal()
        {
            _sut.MoneyMultiply(100m, "USD", 3m, out var multiplied);
            _sut.MoneyDivide(multiplied.Amount, multiplied.CurrencyCode, 3m, out var divided);

            Assert.That(divided.Amount, Is.EqualTo(100m));
        }

        [Test]
        public void MoneySplit_ThenSum_ReturnsOriginal()
        {
            decimal originalAmount = 999.99m;
            _sut.MoneySplit(originalAmount, "USD", 7, out var items);

            var total = items.Sum(i => i.Amount);
            Assert.That(total, Is.EqualTo(originalAmount));
        }

        [Test]
        public void ExchangeRateConvert_ThenReverse_ApproximatesOriginal()
        {
            decimal originalAmount = 100m;
            decimal rate = 0.92m;

            _sut.ExchangeRateConvert(originalAmount, "USD", "EUR", rate, out var converted);
            _sut.ExchangeRateConvert(converted.Amount, "EUR", "USD", 1m / rate, out var reversed);

            // Due to rounding, may not be exactly the same
            Assert.That(reversed.Amount, Is.EqualTo(originalAmount).Within(0.02m));
        }

        [Test]
        public void AllActions_HandleWhitespaceInCurrencyCode()
        {
            // Verify trimming works across all major actions
            Assert.DoesNotThrow(() =>
            {
                _sut.MoneyCreate(100m, "  USD  ", out _);
                _sut.MoneyAdd(100m, " USD", 50m, "USD ", out _);
                _sut.CurrencyGetInfo(" EUR ", out _);
                _sut.CurrencyIsValid("  GBP  ", out _);
                _sut.ExchangeRateCreate(" USD ", " EUR ", 0.92m, out _);
            });
        }

        #endregion
    }
}
