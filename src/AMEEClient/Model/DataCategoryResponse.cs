namespace AMEEClient.Model
{
    public class DataCategoryResponse
    {
        public string apiVersion { get; set; }

        public DataCategoryItem dataCategory { get; set; }
        //choices
        // selections
        public string path { get; set; }
        
        //itemDefinition
        public DataItems children { get; set; }
    }
}