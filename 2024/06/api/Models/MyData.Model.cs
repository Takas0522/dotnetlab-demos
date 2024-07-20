public abstract class MyDataBase
{
    public abstract int Code { get; set; }
    public abstract string Name { get; set; }
}

public class MyData : MyDataBase
{
    public override int Code { get; set; }
    public override string Name { get; set; }
}


public class NewMyData: MyDataBase
{
    public override int Code { get; set; }
    public override string Name { get; set; }
    public string NewData { get; set; } = "";
}