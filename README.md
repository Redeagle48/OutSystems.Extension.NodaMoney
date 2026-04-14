# NodaMoney -- OutSystems ODC External Library

> Type-safe money and currency operations with ISO 4217 compliance for OutSystems Developer Cloud, powered by the [NodaMoney](https://github.com/RemyDuijkeren/NodaMoney) library.

## Overview

This external library exposes 26 server actions that give ODC applications first-class money handling: arithmetic that never mixes currencies, loss-free splitting across N recipients, culture-aware formatting, exchange-rate conversion, and a full ISO 4217 currency registry. All inputs and outputs use simple OutSystems types (Decimal, Text, Integer, Boolean) so they wire directly to existing entities without conversion.

## Actions

### Money Operations (15)

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| `MoneyCreate` | Creates a money value, validates the currency, rounds to minor units | amount: Decimal, currencyCode: Text | moneyResult: MoneyResult |
| `MoneyAdd` | Adds two money values (same currency required) | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | moneyResult: MoneyResult |
| `MoneySubtract` | Subtracts second money from first (same currency required) | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | moneyResult: MoneyResult |
| `MoneyMultiply` | Multiplies a money value by a scalar | amount: Decimal, currencyCode: Text, multiplier: Decimal | moneyResult: MoneyResult |
| `MoneyDivide` | Divides a money value by a scalar (error if zero) | amount: Decimal, currencyCode: Text, divisor: Decimal | moneyResult: MoneyResult |
| `MoneyDivideByMoney` | Returns the ratio of two money values as a Decimal | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | ratio: Decimal |
| `MoneyAbs` | Returns the absolute value of a money amount | amount: Decimal, currencyCode: Text | moneyResult: MoneyResult |
| `MoneyNegate` | Flips the sign of a money amount | amount: Decimal, currencyCode: Text | moneyResult: MoneyResult |
| `MoneyCompare` | Compares two values: returns -1, 0, or 1 (same currency required) | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | comparisonResult: Integer |
| `MoneyIsZero` | Checks if a money value is zero | amount: Decimal, currencyCode: Text | isZero: Boolean |
| `MoneyIsPositive` | Checks if a money value is positive | amount: Decimal, currencyCode: Text | isPositive: Boolean |
| `MoneyIsNegative` | Checks if a money value is negative | amount: Decimal, currencyCode: Text | isNegative: Boolean |
| `MoneyMin` | Returns the smaller of two money values (same currency required) | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | moneyResult: MoneyResult |
| `MoneyMax` | Returns the larger of two money values (same currency required) | amount1: Decimal, currencyCode1: Text, amount2: Decimal, currencyCode2: Text | moneyResult: MoneyResult |
| `MoneySplit` | Splits money into N equal parts without losing cents (fair allocation) | amount: Decimal, currencyCode: Text, numberOfParts: Integer | allocationItems: List of MoneyAllocationItem |

### Currency Operations (3)

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| `CurrencyGetInfo` | Gets currency info by ISO 4217 code (symbol, name, decimal digits) | currencyCode: Text | currencyResult: CurrencyResult |
| `CurrencyGetAll` | Lists all available ISO 4217 currencies | _(none)_ | currencies: List of CurrencyResult |
| `CurrencyIsValid` | Checks if a currency code is valid and registered | currencyCode: Text | isValid: Boolean |

### Exchange Rate Operations (3)

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| `ExchangeRateCreate` | Creates and validates an exchange rate between two currencies | baseCurrencyCode: Text, quoteCurrencyCode: Text, rate: Decimal | exchangeRateResult: ExchangeRateResult |
| `ExchangeRateConvert` | Converts a money amount using a given exchange rate | amount: Decimal, fromCurrencyCode: Text, toCurrencyCode: Text, rate: Decimal | moneyResult: MoneyResult |
| `ExchangeRateParse` | Parses an exchange rate string (e.g., "USD/EUR 0.92") | exchangeRateString: Text | exchangeRateResult: ExchangeRateResult |

### Formatting and Parsing (5)

| Action | Description | Key Inputs | Key Outputs |
|--------|-------------|-----------|-------------|
| `MoneyFormat` | Formats a money value using a .NET format string | amount: Decimal, currencyCode: Text, format: Text | formatted: Text |
| `MoneyFormatWithCulture` | Formats a money value using a specific culture | amount: Decimal, currencyCode: Text, cultureName: Text | formatted: Text |
| `MoneyParse` | Parses a money string (e.g., "$1,234.56") | moneyString: Text | moneyResult: MoneyResult |
| `MoneyParseWithCulture` | Parses a money string using a specific culture | moneyString: Text, cultureName: Text | moneyResult: MoneyResult |
| `MoneyRound` | Rounds to specified decimal places (-1 for currency default) | amount: Decimal, currencyCode: Text, decimalDigits: Integer | moneyResult: MoneyResult |

## Structures

| Structure | Description | Fields |
|-----------|-------------|--------|
| `MoneyResult` | Result of a money operation | Amount (Decimal), CurrencyCode (Text), Formatted (Text) |
| `CurrencyResult` | Currency information | Code (Text), Symbol (Text), EnglishName (Text), DecimalDigits (Integer) |
| `ExchangeRateResult` | Exchange rate between two currencies | BaseCurrencyCode (Text), QuoteCurrencyCode (Text), Rate (Decimal) |
| `MoneyAllocationItem` | One item from a money split | Amount (Decimal), CurrencyCode (Text), Index (Integer) |

## OutSystems Data Type Mapping

| .NET Type | OutSystems Type | Usage |
|-----------|----------------|-------|
| `decimal` | Decimal | Money amounts, exchange rates, multipliers |
| `string` | Text | Currency codes, culture names, format strings, formatted output |
| `int` | Integer | Comparison results, split counts, decimal digit counts |
| `bool` | Boolean | Validation checks (IsZero, IsPositive, IsNegative, IsValid) |
| `List<T>` | Record List | Split allocation items, currency listings |

## Installation

1. Build the publish artifact:

```bash
dotnet publish OutSystems.Extension.NodaMoney/OutSystems.Extension.NodaMoney.csproj \
  -c Release -r linux-x64 --self-contained false -o ./publish
```

2. Create the ZIP package:

```bash
cd publish && zip -r ../OutSystems.Extension.NodaMoney.zip .
```

3. Upload `OutSystems.Extension.NodaMoney.zip` in ODC Portal under **External Libraries**.

## Local Development

```bash
dotnet build
dotnet test
```

140 unit tests cover all 26 actions, including edge cases and error paths.

## Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| NodaMoney | 2.7.0 | Core money and currency types with ISO 4217 registry |
| OutSystems.ExternalLibraries.SDK | 1.5.0 | OSInterface / OSAction / OSStructure attributes for ODC |

## Security Hardening

The following input-validation measures were applied after code review and penetration testing:

- **MoneyFormat format whitelist** -- Only standard .NET format specifiers are accepted: C, N, F, G, L, R, optionally followed by a single precision digit (0-9). Any other format string is rejected with an `ArgumentException`.
- **MoneySplit upper bound** -- The `numberOfParts` parameter is capped at 10,000 to prevent resource-exhaustion attacks. Values less than 1 or greater than 10,000 throw `ArgumentException`.
- **MoneyRound digit cap** -- The `decimalDigits` parameter must be -1 (currency default) or between 0 and 28 inclusive, matching the `decimal` type's maximum precision. Out-of-range values throw `ArgumentException`.
- **Currency code normalization** -- All currency code inputs are trimmed and uppercased before lookup. Null, empty, and whitespace-only strings are rejected.
- **Thread safety** -- The service class holds no mutable static state. Concurrent Lambda invocations are safe.

## Constraints

- **Stateless**: no in-memory caching between Lambda invocations. Each call is independent.
- **Same-currency requirement**: MoneyAdd, MoneySubtract, MoneyCompare, MoneyMin, MoneyMax, MoneyDivideByMoney all require both operands to share the same currency code. Use ExchangeRateConvert first if currencies differ.
- **Payload limit**: 5.5 MB for binary parameters (ODC platform limit).
- **Timeout**: configured in ODC Portal (default 30 seconds).
- **Runtime**: .NET 8.0, runs as AWS Lambda (linux-x64).

## CI/CD

Two GitHub Actions workflows automate testing and releasing.

### Test Workflow (`.github/workflows/test.yml`)

Triggers on every push and pull request to `main`/`master`, plus manual dispatch.

1. Restore dependencies and run all unit tests in Release configuration
2. Collect code coverage via `XPlat Code Coverage`
3. Generate a coverage summary badge and markdown report (posted to the GitHub Actions job summary)

### Release Workflow (`.github/workflows/release.yml`)

Triggers when a version tag matching `v*.*.*` is pushed (e.g., `v1.0.0`, `v2.1.0-beta`).

1. Run the full test suite as a gate -- packaging only proceeds if tests pass
2. Publish the project (`dotnet publish -c Release -r linux-x64 --self-contained false`)
3. Create a versioned ZIP named `NodaMoney_<tag>.zip`
4. Create a GitHub Release with the ZIP attached and auto-generated release notes

### Creating a Release

```bash
git tag v1.2.0
git push origin v1.2.0
```

This triggers the release workflow, which runs tests, builds the ODC-ready ZIP, and publishes a GitHub Release. Download the ZIP from the release page and upload it in **ODC Portal > External Libraries**.
