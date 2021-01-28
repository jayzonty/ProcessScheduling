
namespace ProcessScheduling
{
    /// <summary>
    /// Class containing data regarding a condition
    /// </summary>
    [System.Serializable]
    public class Condition
    {
        /// <summary>
        /// Enum describing the possible comparison operations
        /// </summary>
        public enum ComparisonOperation
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanEqual,
            GreaterThan,
            GreaterThanEqual
        }

        /// <summary>
        /// Name of the attribute to check the value of
        /// </summary>
        public string targetAttributeName;

        /// <summary>
        /// Value to compare the attribute with
        /// </summary>
        public int comparisonValue;

        /// <summary>
        /// Comparison operation
        /// </summary>
        public ComparisonOperation operation;
    }
}
