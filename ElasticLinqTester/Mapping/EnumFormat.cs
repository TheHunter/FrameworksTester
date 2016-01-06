namespace ElasticSearch.Linq.Mapping
{
    /// <summary>
    /// How an enum should be formatted in the JSON payload.
    /// </summary>
    public enum EnumFormat
    {
        /// <summary>
        /// Format enums as an integer using their ordinal.
        /// </summary>
        Integer,

        /// <summary>
        /// Format enums as a string using their name.
        /// </summary>
        String
    };
}
