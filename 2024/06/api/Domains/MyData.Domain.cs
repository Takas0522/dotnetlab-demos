namespace api.Domains;

public class MyDataDomain : IMyDataDomain {

    private List<MyData> myListData = new List<MyData>();

    public MyDataDomain()
    {
        myListData.Add(new MyData { Code = 1, Name = "Name1" });
        myListData.Add(new MyData { Code = 2, Name = "Name2" });
        myListData.Add(new MyData { Code = 3, Name = "Name3" });
        myListData.Add(new MyData { Code = 4, Name = "Name4" });
        myListData.Add(new MyData { Code = 5, Name = "Name5" });
    }

    public IEnumerable<MyData> GetMyListData()
    {
        return myListData;
    }

    public IEnumerable<MyData> GetMyListData(string searchWord)
    {
        return myListData.Where(x => x.Name.Contains(searchWord) || x.Code.ToString().Contains(searchWord));
    }
}