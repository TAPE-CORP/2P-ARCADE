using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text moneyText;
    public GameObject coinIconPrefab; // 튀는 아이콘 프리팹
    public RectTransform coinSpawnTarget; // TMP 기준 위치

    [Header("설정")]
    public int unit = 1; // 몇 단위당 하나 튀게 할건지
    public float popDelay = 0.05f; // 한 개씩 튈 때 간격

    private int currentMoney = 0;

    public void AddMoney(int amount)
    {
        int pops = amount / unit;
        currentMoney += amount;
        UpdateMoneyText();
        StartCoroutine(SpawnCoins(pops));
    }

    private void UpdateMoneyText()
    {
        moneyText.text = $"{currentMoney}";
    }

    private IEnumerator SpawnCoins(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(coinIconPrefab, coinSpawnTarget.position, Quaternion.identity, coinSpawnTarget.parent);
            CoinPopEffect effect = coin.GetComponent<CoinPopEffect>();
            effect.StartPop();

            yield return new WaitForSeconds(popDelay);
        }
    }
}
