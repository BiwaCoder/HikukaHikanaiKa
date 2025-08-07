using UnityEngine;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance { get; private set; }

    public long Money { get; private set; } = 300000000; // 初期値3億円

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 複製防止
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 永続化
    }

    public bool Spend(long amount)
    {
        if (Money >= amount)
        {
            Money -= amount;
            return true;
        }
        return false;
    }

    public void Add(long amount)
    {
        Money += amount;
    }
}
