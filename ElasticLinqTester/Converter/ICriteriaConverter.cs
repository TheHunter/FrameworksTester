using ElasticSearch.Linq.Request.Criteria;
using Newtonsoft.Json.Linq;

namespace ElasticSearch.Linq.Converter
{
    public interface ICriteriaConverter
    {
        JObject MakeToken(ICriteria criteria);
    }
}
