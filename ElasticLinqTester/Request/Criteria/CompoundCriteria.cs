// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ElasticSearch.Linq.Utility;

namespace ElasticSearch.Linq.Request.Criteria
{
    /// <summary>
    /// Base class for any criteria wanting to have criteria of its
    /// own such as AndCriteria and OrCriteria.
    /// </summary>
    public abstract class CompoundCriteria : ICriteria
    {
        readonly ReadOnlyCollection<ICriteria> criteria;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompoundCriteria"/> class. 
        /// Create a criteria that has subcriteria. The exact semantics of
        /// the subcriteria are controlled by subclasses of CompoundCriteria.
        /// </summary>
        /// <param name="criteria">
        /// </param>
        protected CompoundCriteria(IEnumerable<ICriteria> criteria)
        {
            Argument.CheckNotNull("criteria", criteria);
            this.criteria = new ReadOnlyCollection<ICriteria>(criteria.ToArray());
        }

        /// <inheritdoc/>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the criteria collection that is compounded by this criteria in some way (as determined by the subclass).
        /// </summary>
        public ReadOnlyCollection<ICriteria> Criteria
        {
            get { return this.criteria; }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Name, string.Join(", ", this.Criteria.Select(f => f.ToString()).ToArray()));
        }
    }
}