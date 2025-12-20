using UnityEngine;

public class Context : MonoBehaviour
{
    public static Context I { get; private set; }

    [SerializeField] GameObject player;
    [SerializeField] GameObject enemy;
    public GameObject Player => player;
    public GameObject Enemy => enemy;

    void Awake()
    {
        if(I == null)
        {
            I = this;
        }
    }
    
}