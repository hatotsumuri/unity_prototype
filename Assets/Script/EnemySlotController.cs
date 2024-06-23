using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class EnemySlotController : MonoBehaviour, EnemyObjectInterface, IPointerExitHandler
{
    private bool isEntered = false;
    private TextMeshProUGUI cardSlotText;
    private GameObject[,] enemyObjectList;
    private int line;
    private int field;
    public bool canDirect;
    public EnemyManager enemyManager;
    private MainManager mainManager;
    private SoundEffectManager soundEffectManager;


    void Awake()
    {
        this.canDirect = false;
        Transform cardSlotTextTransform = transform.Find("CardSlotText");
        if (cardSlotTextTransform != null)
        {
            GameObject cardSlotTextObject = cardSlotTextTransform.gameObject;
            this.cardSlotText = cardSlotTextObject.GetComponent<TextMeshProUGUI>();
            cardSlotText.text = "";
        }
        this.enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        this.mainManager = GameObject.FindAnyObjectByType<MainManager>();
        this.soundEffectManager = GameObject.FindObjectOfType<SoundEffectManager>();
    }

    public void ForcePointerEnter()
    {
        if (!this.isEntered)
        {
            this.isEntered = true;
            for (int i = 0; i < this.enemyObjectList.GetLength(0); i++)
            {
                GameObject enemyObject = this.enemyObjectList[i, this.field];
                EnemyObjectInterface enemyObjectInterface = enemyObject.GetComponent<EnemyObjectInterface>();
                if (enemyObjectInterface.GetObjectType() == "card")
                {
                    return;
                }
            }
            // 全てslotの場合直接攻撃
            this.cardSlotText.text = "Direct";
            this.canDirect = true;
        }
    }

    public void ForcePointerExit()
    {
        if (this.isEntered)
        {
            this.isEntered = false;
            this.cardSlotText.text = "";
        }
        if (this.canDirect)
        {
            this.canDirect = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isEntered)
        {
            isEntered = false;
            cardSlotText.text = "";
        }
    }

    void Update()
    {
        
    }

    public void SetEnemyObjectList(GameObject[,] enemyObjectList)
    {
        this.enemyObjectList = enemyObjectList;
    }

    public string GetObjectType ()
    {
        return "slot";//TODO 定数化
    }

    public void SetPoint(int l, int f)
    {
        this.line = l;
        this.field = f;
    }

    public void AttackStart()
    {

    }

    public void AttackEnd()
    {

    }

    public bool Attack(int power, int type, bool forceAttack)
    {
        if (this.canDirect)
        {
            this.soundEffectManager.PlayDirect01();
            this.enemyManager.DirectAttack(power);
            StartCoroutine(this.mainManager.Shake(0.36f, 20f));
            return true;
        }
        return false;
    }

    public int GetPower()
    {
        return 0;
    }

    public int GetLine()
    {
        return this.line;
    }

    public int GetField()
    {
        return this.field;
    }
}
