public class Money
{
    public int HasMoney { get; private set; }
    public void Add(int add)
    {
        HasMoney += add;
    }

    public void Sub(int sub)
    {
        if(HasMoney < sub)
        {
            HasMoney = 0;
            return;
        }

        HasMoney -= sub;
    }
}