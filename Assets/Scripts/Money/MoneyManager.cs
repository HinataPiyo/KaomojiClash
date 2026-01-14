using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager I { get; private set; }
    public Money Money { get; private set; }
    void Awake()
    {
        if(I == null)
        {
            I = this;
            Money = new Money();
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMoney(int add)
    {
        Money.Add(add);
        Debug.Log("所持金:" + Money.HasMoney);
    }
}