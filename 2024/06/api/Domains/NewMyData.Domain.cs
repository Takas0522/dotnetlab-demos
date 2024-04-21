namespace api.Domains;

public class NewMyDataDomain : INewMyDataDomain {

    private List<NewMyData> myListData = new List<NewMyData>();

    public NewMyDataDomain()
    {
        myListData.Add(new NewMyData { Code = 1, Name = "Name1" , NewData = "NewData1"});
        myListData.Add(new NewMyData { Code = 2, Name = "Name2" , NewData = "NewData2"});
        myListData.Add(new NewMyData { Code = 3, Name = "Name3" , NewData = "NewData3"});
        myListData.Add(new NewMyData { Code = 4, Name = "Name4" , NewData = "NewData4"});
        myListData.Add(new NewMyData { Code = 5, Name = "Name5" , NewData = "NewData5"});
    }

    public IEnumerable<NewMyData> GetMyListData()
    {
        return myListData;
    }

    public IEnumerable<NewMyData> GetMyListData(string searchWord)
    {
        return myListData.Where(x => x.Name.Contains(searchWord) || x.Code.ToString().Contains(searchWord));
    }
}