using UnityEngine;
using TMPro;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyCardPrefab;
    public GameObject cardSlotPrefab;
    public GameObject board;
    private int line = 3;
    private int field = 4;
    private int startPxX = -380;
    private int endPxX = 360;
    private int startPxY = 860;
    private int endPxY = 140;
    private int intervalX;
    private int intervalY;
    private GameObject[,] enemyObjectList = new GameObject[3, 4];
    private float[,] enemyCardProbabilities = new float[,]{
            { 0.68f, 0.82f, 0.82f, 0.68f },
            { 0.76f, 0.86f, 0.86f, 0.76f },
            { 0.86f, 0.92f, 0.92f, 0.86f },
        };
    private float[,] addEnemyCardProbabilities = new float[,]{
            { 0.40f, 0.60f, 0.60f, 0.40f }
        };
    private int life = 20;
    private TextMeshProUGUI enemyLifeText;

    void Awake()
    {
        this.intervalY = (this.endPxY - this.startPxY) / (line-1);
        this.intervalX = (this.endPxX - this.startPxX) / (field-1);
        this.enemyObjectList = new GameObject[line,field];
        GameObject enemyLifeTextObject = GameObject.Find("EnemyLifeText");
        if (enemyLifeTextObject != null)
        {
            TextMeshProUGUI[] allTexts = enemyLifeTextObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (text.gameObject.name == "LifePointText")
                {
                    this.enemyLifeText = text;
                    this.enemyLifeText.text = this.life.ToString();
                    break;
                }
            }
        }
    }

    public void DirectAttack (int damage)
    {
        this.life -= damage;
        this.enemyLifeText.text = life.ToString();
    }

    public int GetDamage()
    {
        int damage = 0;
        for (int f = 0; f < enemyObjectList.GetLength(1); f++)
        {
            GameObject targetEnemyObject = enemyObjectList[2, f];
            EnemyObjectInterface enemyObjectInterface = targetEnemyObject.GetComponent<EnemyObjectInterface>();
            if (enemyObjectInterface.GetObjectType() == "card")
            {
                damage += enemyObjectInterface.GetPower();
            }
        }
        return damage;
    }

    public GameObject[,] GetEnemyObjectList()
    {
        return this.enemyObjectList;
    }

    public void AddEnemyObject()
    {
        // 一番手前の行を削除
        for (int f = 0; f < enemyObjectList.GetLength(1); f++)
        {
            Destroy(enemyObjectList[2, f]);
        }

        // そのほかのオブジェクトを一つ前の行に移動
        for (int l = enemyObjectList.GetLength(0) - 2; l >= 0; l--)
        {
            for (int f = 0; f < enemyObjectList.GetLength(1); f++)
            {
                enemyObjectList[l + 1, f] = enemyObjectList[l, f];
                EnemyObjectInterface enemyObjectInterface = enemyObjectList[l + 1, f]?.GetComponent<EnemyObjectInterface>();
                enemyObjectInterface.SetPoint(l + 1, f);
                RectTransform rectTransform = enemyObjectList[l + 1, f]?.GetComponent<RectTransform>();
                bool lastObjectFlg = false;
                if (l == 0 && f == enemyObjectList.GetLength(1)-1)
                {
                    lastObjectFlg = true;
                }
                StartCoroutine(MoveAndAddEnamyObject(rectTransform, rectTransform.anchoredPosition.y + this.intervalY, 0.25f, lastObjectFlg));
            }
        }
    }

    private IEnumerator MoveAndAddEnamyObject(RectTransform rectTransform, float targetY, float duration, bool lastObjectFlg)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x, targetY);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (rectTransform == null) yield break;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = endPos;
        }
        if (lastObjectFlg)
        {
            // 一番後ろの行に新しいオブジェクトを追加
            for (int f = 0; f < enemyObjectList.GetLength(1); f++)
            {
                float probability = this.addEnemyCardProbabilities[0, f];
                if (Random.value > probability)
                {
                    CreateEnemySlot(0, f, this.startPxX + this.intervalX * f, this.startPxY);
                }
                else
                {
                    CreateEnemyCard(0, f, this.startPxX + this.intervalX * f, this.startPxY);
                }
            }
        }
    }

    public void ResetEnemyObject()
    {
        foreach(GameObject enemyCoin in this.enemyObjectList)
        {
            Destroy(enemyCoin);
        }
        this.enemyObjectList = new GameObject[line, field];

        for (int l = 0; l < enemyObjectList.GetLength(0); l++)
        {
            for (int f = 0; f < enemyObjectList.GetLength(1); f++)
            {
                float probability = this.enemyCardProbabilities[l, f];
                if (Random.value > probability)
                {
                    CreateEnemySlot(l, f, this.startPxX + this.intervalX * f, this.startPxY + this.intervalY * l);
                }
                else
                {
                    CreateEnemyCard(l, f, this.startPxX + this.intervalX * f, this.startPxY + this.intervalY * l);
                }
            }
        }
    }

    private void CreateEnemyCard(int l, int f, float positionX, float positionY)
    {
        GameObject enemyCardObject = Instantiate(enemyCardPrefab, board.transform);
        RectTransform rectTransform = enemyCardObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(positionX, positionY);
        EnemyObjectInterface enemyObjectInterface = enemyCardObject.GetComponent<EnemyObjectInterface>();
        if (enemyObjectInterface != null)
        {
            enemyObjectInterface.SetEnemyObjectList(this.enemyObjectList);
            enemyObjectInterface.SetPoint(l, f);
        }
        enemyObjectList[l, f] = enemyCardObject;
    }

    public void CreateEnemySlot(int l, int f, float positionX, float positionY)
    {
        GameObject cardSlotObject = Instantiate(cardSlotPrefab, board.transform);
        RectTransform rectTransform = cardSlotObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(positionX, positionY);
        EnemyObjectInterface enemyObjectInterface = cardSlotObject.GetComponent<EnemyObjectInterface>();
        if (enemyObjectInterface != null)
        {
            enemyObjectInterface.SetEnemyObjectList(this.enemyObjectList);
            enemyObjectInterface.SetPoint(l, f);
        }
        enemyObjectList[l, f] = cardSlotObject;
    }
}
