using ElasticSearch.Linq.Request.Facets;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    public interface IFacetConverter
    {
        JObject MakeToken(IFacet facet, ICriteriaConverter criteriaConverter = null, int? defaultSize = 10);
    }
}
