using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LudumDare57.Manager
{
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { private set; get; }

        [SerializeField]
        private ShopUpgrade[] _upgrades;

        private void Awake()
        {
            Instance = this;
            foreach (var u in _upgrades)
            {
                u.MoneyLabel.text = $"{u.Price}";
                u.Button.onClick.AddListener(() =>
                {
                    PlayerManager.Instance.GainMoney(-u.Price);
                    u.Button.gameObject.SetActive(false);
                });
            }
        }

        public void UpdateShopUI(int totalMoney)
        {
            foreach (var u in _upgrades)
            {
                u.Button.interactable = totalMoney >= u.Price;
            }
        }

        public void UpgradeDrill()
        {
            PlayerManager.Instance.ForEach(x => x.UpgradeDrill());
        }
    }

    [System.Serializable]
    public class ShopUpgrade
    {
        public Button Button;
        public int Price;
        public TMP_Text MoneyLabel;
    }
}