using OutSystems.ExternalLibraries.SDK;

namespace OutSystems.Extension.NodaMoney.Structures
{
    /// <summary>
    /// Represents a single item from a money split/allocation operation.
    /// </summary>
    [OSStructure(Description = "One item from a money split or allocation, with its position index.")]
    public struct MoneyAllocationItem
    {
        [OSStructureField(Description = "The allocated monetary amount for this portion.", IsMandatory = true)]
        public decimal Amount;

        [OSStructureField(Description = "ISO 4217 currency code (e.g., \"USD\").", IsMandatory = true)]
        public string CurrencyCode;

        [OSStructureField(Description = "1-based position index of this item in the allocation.", IsMandatory = true)]
        public int Index;

        /// <summary>
        /// Creates a new MoneyAllocationItem with default values.
        /// </summary>
        public MoneyAllocationItem()
        {
            Amount = 0m;
            CurrencyCode = string.Empty;
            Index = 0;
        }

        /// <summary>
        /// Creates a new MoneyAllocationItem from the specified values.
        /// </summary>
        /// <param name="amount">The allocated amount.</param>
        /// <param name="currencyCode">The ISO 4217 currency code.</param>
        /// <param name="index">The 1-based position index.</param>
        public MoneyAllocationItem(decimal amount, string currencyCode, int index)
        {
            Amount = amount;
            CurrencyCode = currencyCode;
            Index = index;
        }
    }
}
