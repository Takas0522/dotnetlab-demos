public interface INewMyDataDomain
{
    IEnumerable<NewMyData> GetMyListData();
    IEnumerable<NewMyData> GetMyListData(string searchWord);
}