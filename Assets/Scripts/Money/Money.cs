public static class Money
{
    public static int HasMoney { get; private set; }  = 1000;
    public static void Add(int add)
    {
        HasMoney += add;
    }

    public static void Sub(int sub)
    {
        if(HasMoney < sub)
        {
            HasMoney = 0;
            return;
        }

        HasMoney -= sub;
    }
}