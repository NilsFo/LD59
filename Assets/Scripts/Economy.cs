using System;
using UnityEngine;

public class Economy : MonoBehaviour
{
    [SerializeField] private int money = 0;
    public event Action<int> OnMoneyChanged;

    public int Money
    {
        get => money;
        set
        {
            if (money == value) return;
            money = value;
            OnMoneyChanged?.Invoke(money);
        }
    }
}