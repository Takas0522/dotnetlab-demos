public interface IMyDataDomain
{
    IEnumerable<MyData> GetMyListData();
    IEnumerable<MyData> GetMyListData(string searchWord);
}