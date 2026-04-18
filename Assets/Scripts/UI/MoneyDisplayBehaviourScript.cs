using TMPro;
using UnityEngine;

namespace UI
{
    public class MoneyDisplay : MonoBehaviour
    {
        public GameState gameState;
        public TextMeshProUGUI text;

        private void Start()
        {
            gameState.economy.OnMoneyChanged += UpdateMoney;
            UpdateMoney(gameState.economy.Money);
        }
    
        void UpdateMoney(int newAmount)
        {
            text.SetText(newAmount + "$");
        }

        public void DebugAddMoney()
        {
            gameState.economy.Money += 100;
            Debug.Log(gameState.economy.Money);
        }
        
        public void DebugRemoveMoney()
        {
            var newValue = gameState.economy.Money - 100;
            if (newValue > 0)
            {
                gameState.economy.Money = newValue;
            }
            else
            {
                gameState.economy.Money = 0;
            }
            Debug.Log(gameState.economy.Money);
        }
    }
}
