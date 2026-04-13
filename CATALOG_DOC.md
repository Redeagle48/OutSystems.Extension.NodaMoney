## What it does

NodaMoney gives ODC applications type-safe money handling with ISO 4217 compliance. It enforces same-currency arithmetic, rounds to each currency's minor unit, and splits money fairly so no cents are lost. Includes formatting, parsing, and exchange-rate conversion across 26 stateless actions.

## Actions

| Action | What it does | Inputs | Output |
|--------|-------------|--------|--------|
| MoneyCreate | Creates a validated money value | amount, currencyCode | moneyResult |
| MoneyAdd | Adds two same-currency values | amount1, ccy1, amount2, ccy2 | moneyResult |
| MoneySubtract | Subtracts second from first | amount1, ccy1, amount2, ccy2 | moneyResult |
| MoneyMultiply | Multiplies by a scalar | amount, currencyCode, multiplier | moneyResult |
| MoneyDivide | Divides by a scalar | amount, currencyCode, divisor | moneyResult |
| MoneyDivideByMoney | Ratio of two money values | amount1, ccy1, amount2, ccy2 | ratio |
| MoneyAbs | Absolute value | amount, currencyCode | moneyResult |
| MoneyNegate | Flips the sign | amount, currencyCode | moneyResult |
| MoneyCompare | Returns -1, 0, or 1 | amount1, ccy1, amount2, ccy2 | comparisonResult |
| MoneyIsZero | True if zero | amount, currencyCode | isZero |
| MoneyIsPositive | True if positive | amount, currencyCode | isPositive |
| MoneyIsNegative | True if negative | amount, currencyCode | isNegative |
| MoneyMin | Smaller of two values | amount1, ccy1, amount2, ccy2 | moneyResult |
| MoneyMax | Larger of two values | amount1, ccy1, amount2, ccy2 | moneyResult |
| MoneySplit | Fair split into N parts | amount, currencyCode, numberOfParts | allocationItems |
| CurrencyGetInfo | Details by ISO code | currencyCode | currencyResult |
| CurrencyGetAll | All ISO 4217 currencies | (none) | currencies |
| CurrencyIsValid | Validates a code | currencyCode | isValid |
| ExchangeRateCreate | Creates a validated rate | baseCcy, quoteCcy, rate | exchangeRateResult |
| ExchangeRateConvert | Converts between currencies | amount, fromCcy, toCcy, rate | moneyResult |
| ExchangeRateParse | Parses "USD/EUR 0.92" | exchangeRateString | exchangeRateResult |
| MoneyFormat | Formats with specifier | amount, currencyCode, format | formatted |
| MoneyFormatWithCulture | Formats for a locale | amount, currencyCode, cultureName | formatted |
| MoneyParse | Parses a money string | moneyString | moneyResult |
| MoneyParseWithCulture | Parses with a locale | moneyString, cultureName | moneyResult |
| MoneyRound | Rounds to N decimals | amount, currencyCode, decimalDigits | moneyResult |

All inputs use Decimal, Text, Integer, or Boolean. "ccy" abbreviates currencyCode.

## Structures

| Structure | What it represents | Key fields |
|-----------|--------------------|-----------|
| MoneyResult | Result of a money operation | Amount, CurrencyCode, Formatted |
| CurrencyResult | Currency registry entry | Code, Symbol, EnglishName, DecimalDigits |
| ExchangeRateResult | Rate between two currencies | BaseCurrencyCode, QuoteCurrencyCode, Rate |
| MoneyAllocationItem | One portion from a split | Amount, CurrencyCode, Index |

## How to use

- Add the NodaMoney dependency to your ODC module. All 26 actions appear under Server Actions.
- Pass Decimal amounts and Text currency codes. Actions return a MoneyResult with Amount, CurrencyCode, and Formatted.
- For multi-currency work, call ExchangeRateConvert with source, target, and rate.
- To split a bill, call MoneySplit then iterate the returned list.
- Use MoneyFormatWithCulture to display amounts in the end-user's locale.

## Constraints

- Add, Subtract, Compare, Min, Max, and DivideByMoney require matching currencies.
- MoneySplit accepts 1 to 10,000 parts.
- MoneyFormat allows specifiers C, N, F, G, L, R with optional digit 0-9.
- MoneyRound accepts 0-28, or -1 for currency default.
- Stateless: each call is independent.
