using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainManager: MonoBehaviour
{
    private int power;
    public RectTransform canvasRectTransform;
    public PlayerManager playerManager;
    public EnemyManager enemyManager;
    public BgmManager bgmManager;
    public GameObject board;
    public GameObject boardEffect;
    private RectTransform boardRectTransform;

    void Start()
    {
        Application.targetFrameRate = 60;
        this.bgmManager = GameObject.FindObjectOfType<BgmManager>();
        this.enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        this.playerManager = GameObject.FindObjectOfType<PlayerManager>();
        this.boardRectTransform = board.GetComponent<RectTransform>();
        this.bgmManager.PlayBttle01();
        this.ResetBoard();
    }

    public void ResetBoard()
    {
        this.playerManager.ResetPlayerHand();
        this.enemyManager.ResetEnemyObject();
    }

    public void NextRound()
    {
        this.playerManager.ResetPlayerHand();
        this.enemyManager.AddEnemyObject();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = boardRectTransform.anchoredPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            boardRectTransform.anchoredPosition = new Vector3(x, y, originalPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }
        float resetElapsed = 0.0f;
        const float resetDuration = 1.0f;
        while (resetElapsed < resetDuration)
        {
            boardRectTransform.anchoredPosition = Vector3.Lerp(boardRectTransform.anchoredPosition, originalPos, resetElapsed / resetDuration);
            resetElapsed += Time.deltaTime;
            if (resetElapsed > resetDuration) resetElapsed = resetDuration; 
            yield return null;
        }

        boardRectTransform.anchoredPosition = originalPos;
    }

    public IEnumerator Flash(float duration)
    {
        Image boardEffectImage = boardEffect.GetComponentInChildren<Image>();
        Color originalColor = boardEffectImage.color;
        boardEffectImage.color = Color.red;
        yield return new WaitForSeconds(duration);
        boardEffectImage.color = originalColor;
    }
}
