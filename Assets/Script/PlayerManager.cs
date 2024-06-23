using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerCardPrefab;
    public Canvas canvas;
    public List<GameObject> playerCardList = new List<GameObject>();
    private int startHandSize = 6;
    private int cardIntervalX = 60;
    private List<CardProperties> cardDeck;
    private List<Vector2> targetCardPositions;
    private int life = 20;
    private TextMeshProUGUI playerLifeText;
    private MainManager mainManager;

    private void Awake()
    {
        this.targetCardPositions = new List<Vector2>();
        this.mainManager = GameObject.FindAnyObjectByType<MainManager>();
        this.cardDeck = new List<CardProperties>
        {
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            // TODO 捨て札から戻す実装のめんどいので一旦コピー
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            // TODO 捨て札から戻す実装のめんどいので一旦コピー
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            // TODO 捨て札から戻す実装のめんどいので一旦コピー
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            // TODO 捨て札から戻す実装のめんどいので一旦コピー
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_SINGLE, "Single Strike", 5),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_PIERCING, "Piercing Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 2),
            new CardProperties(CardTypeConstants.TYPE_LINE, "Line Strike", 3),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2),
            new CardProperties(CardTypeConstants.TYPE_BLAST, "Blast Attack", 2)
        };
        GameObject enemyLifeTextObject = GameObject.Find("PlayerLifeText");
        if (enemyLifeTextObject != null)
        {
            TextMeshProUGUI[] allTexts = enemyLifeTextObject.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in allTexts)
            {
                if (text.gameObject.name == "LifePointText")
                {
                    this.playerLifeText = text;
                    this.playerLifeText.text = this.life.ToString();
                    break;
                }
            }
        }
    }

    public void DirectAttack(int damage)
    {
        this.life -= damage;
        this.playerLifeText.text = life.ToString();
    }

    private List<CardProperties> DrawCards(int count)
    {
        List<CardProperties> drawnCards = new List<CardProperties>();
        System.Random rand = new System.Random();
        for (int i = 0; i < count; i++)
        {
            if (cardDeck.Count > 0)
            {
                int index = rand.Next(cardDeck.Count);
                drawnCards.Add(cardDeck[index]);
                cardDeck.RemoveAt(index);
            }
        }
        return drawnCards;
    }

    public void ResetPlayerHand()
    {
        for (int i = 0; i < playerCardList.Count; i++)
        {
            GameObject playerCard = playerCardList[i];
            Destroy(playerCard);
        }
        playerCardList.Clear();
        int startPx = -cardIntervalX * this.startHandSize;
        int px = startPx;
        List<CardProperties> drawnCards = DrawCards(this.startHandSize);
        foreach(CardProperties cardProperties in drawnCards)
        {
            GameObject playerCardObject = Instantiate(playerCardPrefab, this.canvas.transform);
            RectTransform rectTransform = playerCardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(px, -560);
            PlayerCardController playerCardController = playerCardObject.GetComponent<PlayerCardController>();
            playerCardController.SetInitPositionY(-560);
            playerCardController.SetCardProperties(cardProperties);
            playerCardController.SetCardText();
            playerCardList.Add(playerCardObject);
        }
        ArrangePlayerHand();
    }

    public void ArrangePlayerHand()
    {
        if (this.targetCardPositions != null)
        {
            this.targetCardPositions.Clear();
        }
        if (this.playerCardList.Count <= 1)
        {
            foreach (GameObject playerCardObject in this.playerCardList)
            {
                this.targetCardPositions.Add(new Vector2(0, -560));
            }
            return;
        }
        int startPx = -cardIntervalX * this.playerCardList.Count;
        int endPx = cardIntervalX * this.playerCardList.Count;
        int px = startPx;
        int intervalX = (endPx - startPx) / (this.playerCardList.Count - 1);
        foreach (GameObject playerCardObject in this.playerCardList)
        {
            this.targetCardPositions.Add(new Vector2(px, -560));
            px += intervalX;
        }
    }

    private void Update()
    {
        for (int i = 0; i < playerCardList.Count; i++)
        {
            GameObject playerCardObject = playerCardList[i];
            RectTransform rectTransform = playerCardObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, this.targetCardPositions[i], Time.deltaTime * 10);
        }
    }
}
