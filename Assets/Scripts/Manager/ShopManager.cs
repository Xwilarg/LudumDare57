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
                u.MoneyLabel.text = $"{u.Prices[u.Index]}";
                u.Button.onClick.AddListener(() =>
                {
                    PlayerManager.Instance.GainMoney(-u.Prices[u.Index]);
                    u.Index++;
                    if (u.Index == 3) u.Button.gameObject.SetActive(false);

                    UpdateShopUI(PlayerManager.Instance.Money);
                });
            }
        }

        public void UpdateShopUI(int totalMoney)
        {
            foreach (var u in _upgrades)
            {
                u.Button.interactable = totalMoney >= u.Prices[u.Index];
            }
        }

        public void UpgradeDrillSize()
        {
            PlayerManager.Instance.ForEach(x => x.UpgradeDrillSize());
        }

        public void UpgradeDrillCooldown()
        {
            PlayerManager.Instance.ForEach(x => x.UpgradeDrillCooldown());
        }

        public void GainHealth()
        {
            PlayerManager.Instance.ForEach(x => x.GainLife(1));
        }
    }

    [System.Serializable]
    public class ShopUpgrade
    {
        public Button Button;
        public int[] Prices;
        public TMP_Text MoneyLabel;

        [System.NonSerialized]
        public int Index;
    }
}