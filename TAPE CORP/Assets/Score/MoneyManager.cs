using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    [Header("UI ����")]
    public TMP_Text moneyText;
    public GameObject coinIconPrefab; // Ƣ�� ������ ������
    public RectTransform coinSpawnTarget; // TMP ���� ��ġ

    [Header("����")]
    public int unit = 1; // �� ������ �ϳ� Ƣ�� �Ұ���
    public float popDelay = 0.05f; // �� ���� ƥ �� ����

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
