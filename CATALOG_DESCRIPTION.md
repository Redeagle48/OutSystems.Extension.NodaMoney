NodaMoney lets ODC applications perform type-safe money and currency operations with full ISO 4217 compliance.

Key capabilities:
- Create, add, subtract, multiply, and divide money with automatic currency validation
- Split money into equal parts without losing cents via fair allocation
- Convert between currencies with validated exchange rates
- Format and parse money strings with culture-aware locale support
- Look up any ISO 4217 currency for its symbol, name, and decimal digits
- Compare money values and check sign (zero, positive, negative)

All 26 actions use simple OutSystems types (Decimal, Text, Integer, Boolean) and wire directly into existing entities. Currency-mismatched arithmetic is rejected automatically. Input validation includes format-string whitelisting, split-count caps, and rounding-digit bounds.

Ideal for apps that need currency-safe money arithmetic, fair bill splitting, multi-currency conversion, or locale-specific formatting without external API calls.
