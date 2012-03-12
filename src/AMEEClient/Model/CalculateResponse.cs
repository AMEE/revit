using AMEEClient.Util;

namespace AMEEClient.Model
{
    public class CalculateResponse
    {
        public string apiVersion;
        public Pager pager;
        // #TODO profileCategories
        // public object[] profileCategories;
        public DataCategoryItem dataCategory;
        public Environment environment;
        public Amount totalAmount;
        public string path;
        public ProfileItem[] profileItems;
        public Profile profile;
    }
}
