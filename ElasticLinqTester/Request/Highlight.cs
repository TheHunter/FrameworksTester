using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ElasticSearch.Linq.Request
{
    /// <summary>
    /// Specifies highlighting of search results for one or more fields.
    /// </summary>
    public class Highlight
    {
        readonly List<string> fields = new List<string>();

        public void AddFields(params string[] newFields)
        {
            this.fields.AddRange(newFields);
        }

        /// <summary>
        /// Gets or sets the string to start the highlight of each word.
        /// </summary>
        /// <remarks>
        /// This is typically set to an opening HTML tag, hence the name.
        /// </remarks>
        public string PreTag { get; set; }

        /// <summary>
        /// Gets or sets the string to end the highlight of each word.
        /// </summary>
        /// <remarks>
        /// This is typically set to an closing HTML tag, hence the name.
        /// </remarks>
        public string PostTag { get; set; }

        /// <summary>
        /// Gets the fields highlighted by this request.
        /// </summary>
        public ReadOnlyCollection<string> Fields
        {
            get { return new ReadOnlyCollection<string>(this.fields); }
        }
    }
}
