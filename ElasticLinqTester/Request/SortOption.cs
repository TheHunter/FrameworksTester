using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request
{
    /// <summary>
    /// Specifies the options desired for sorting by an individual field.
    /// </summary>
    public class SortOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortOption"/> class. 
        /// Create a new SortOption for the given name, order and ignore.
        /// </summary>
        /// <param name="name">
        /// Name of the field to sort by.
        /// </param>
        /// <param name="ascending">
        /// True if this field should be in ascending order; false otherwise.
        /// </param>
        /// <param name="ignoreUnmapped">
        /// Whether unmapped fields should be ignored or not.
        /// </param>
        public SortOption(string name, bool ascending, bool ignoreUnmapped = false)
        {
            Argument.CheckNotEmpty("name", name);
            this.Name = name;
            this.Ascending = ascending;
            this.IgnoreUnmapped = ignoreUnmapped;
        }

        /// <summary>
        /// Gets the name of the field to be sorted.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this field should be sorted in ascending order or not.
        /// </summary>
        public bool Ascending { get; private set; }

        /// <summary>
        /// Gets a value indicating whether documents with unmapped fields should be ignored or not.
        /// </summary>
        public bool IgnoreUnmapped { get; private set; }
    }
}
