using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class EnemyCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, EnemyObjectInterface
{
    private bool isEntered = false;
    private int power;
    private TextMeshProUGUI powerText;
    private Vector3 originalScale;
    private float scaleFactor = 1.2f;
    private GameObject[,] enemyObjectList;
    private int line;
    private int field;
    public bool canAttack;
    private CanvasGroup canvasGroup;
    public EnemyManager enemyManager;
    private SoundEffectManager soundEffectManager;

    void Awake()
    {
        this.power =　UnityEngine.Random.Range(1, 10);
        Transform powerTextTransform = this.transform.Find("PowerText");
        if (powerTextTransform != null)
        {
            GameObject powerTextGameObject = powerTextTransform.gameObject;
            this.powerText = powerTextGameObject.GetComponent<TextMeshProUGUI>();
            if (this.powerText != null)
            {
                this.powerText.text = this.power.ToString();
            }
        }
        this.originalScale = transform.localScale;
        this.canvasGroup = gameObject.AddComponent<CanvasGroup>();
        this.enemyManager = GameObject.FindObjectOfType<EnemyManager>();
        this.soundEffectManager = GameObject.FindObjectOfType<SoundEffectManager>();
    }

    public void ForcePointerEnter()
    {
        if (!canAttack)
        {
            return;
        }
        if (!isEntered)
        {
            isEntered = true;
            transform.localScale = originalScale * scaleFactor;
        }
    }

    public void ForcePointerExit()
    {
        if (isEntered)
        {
            isEntered = false;
            transform.localScale = originalScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        this.isEntered = true;
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.isEntered = false;
        transform.localScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.isEntered = true;
        transform.localScale = originalScale * scaleFactor;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.isEntered = false;
        transform.localScale = originalScale;
    }
 
    public void SetEnemyObjectList(GameObject[,] enemyObjectList)
    {
        this.enemyObjectList = enemyObjectList;
    }

    public string GetObjectType()
    {
        return "card";//TODO 定数化
    }

    public void SetPoint(int l, int f)
    {
        this.line = l;
        this.field = f;
    }

    public void AttackStart()
    {
        this.canAttack = true;
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.canvasGroup.alpha = 1.0f;
        for (int l = this.line + 1; l < this.enemyObjectList.GetLength(0); l++)
        {
            GameObject enemyObject = this.enemyObjectList[l, this.field];
            EnemyObjectInterface enemyObjectInterface = enemyObject.GetComponent<EnemyObjectInterface>();
            if (enemyObjectInterface.GetObjectType() == "card")
            {
                this.canvasGroup.alpha = 0.6f;
                this.canAttack = false;
            }
        }
    }

    public void AttackEnd()
    {
        this.canvasGroup.alpha = 1f;
        this.canAttack = false;
    }

    public bool Attack(int power, int type, bool forceAttack)
    {
        if(this.canAttack == false && forceAttack == false) {
            return false;
        }
        this.power -= power;

        //　貫通攻撃
        if (type == CardTypeConstants.TYPE_PIERCING)
        {
            for (int l = 0; l < 3; l++)
            {
                if (l == this.line)
                {
                    continue;
                }
                GameObject targetEnemyObject = enemyObjectList[l, this.field];
                EnemyObjectInterface targetEnemyObjectInterface = targetEnemyObject.GetComponent<EnemyObjectInterface>();
                if(targetEnemyObjectInterface.GetObjectType() == "card")
                {
                    targetEnemyObjectInterface.Attack(power, CardTypeConstants.TYPE_SINGLE, true);
                }
            }
        }
        //　列攻撃
        if (type == CardTypeConstants.TYPE_LINE)
        {
            for (int f = 0; f < 4; f++)
            {
                if (f == this.field)
                {
                    continue;
                }
                GameObject targetEnemyObject = enemyObjectList[this.line, f];
                EnemyObjectInterface targetEnemyObjectInterface = targetEnemyObject.GetComponent<EnemyObjectInterface>();
                if (targetEnemyObjectInterface.GetObjectType() == "card")
                {
                    targetEnemyObjectInterface.Attack(power, CardTypeConstants.TYPE_SINGLE, true);
                }
            }
        }
        //　破裂攻撃
        if (type == CardTypeConstants.TYPE_BLAST)
        {
            int[] dl = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] df  = { -1, 0, 1, -1, 1, -1, 0, 1 };
            for (int i = 0; i < 8; i++)
            {
                int nl = this.line + dl[i];
                int nf = this.field + df[i];

                if (nl >= 0 && nl < 3 && nf >= 0 && nf < 4)
                {
                    GameObject targetEnemyObject = this.enemyObjectList[nl, nf];
                    EnemyObjectInterface targetEnemyObjectInterface = targetEnemyObject.GetComponent<EnemyObjectInterface>();
                    if (targetEnemyObjectInterface.GetObjectType() == "card")
                    {
                        targetEnemyObjectInterface.Attack(power, CardTypeConstants.TYPE_SINGLE, true);
                    }
                }
            }
        }

        if (this.power > 0)
        {
            this.powerText.text = this.power.ToString();
            StartCoroutine(AttackAnimation());
        }
        else
        {
            StartCoroutine(DirectWithAnimation());
        }
        return true;
    }

    IEnumerator AttackAnimation()
    {
        this.soundEffectManager.PlayAttack1();
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 5;
            float angle = Mathf.Lerp(-20, 20, Mathf.PingPong(Time.time * 16, 1));
            this.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        this.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    IEnumerator DirectWithAnimation()
    {
        this.soundEffectManager.PlayBreak01();
        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * 5; 
            float scale = Mathf.Lerp(1, 2, timer);
            this.transform.localScale = new Vector3(scale, scale, scale);
　          Color color = this.GetComponent<SpriteRenderer>().color;
            color.a = Mathf.Lerp(1.0f, 0, timer);
            this.GetComponent<SpriteRenderer>().color = color;
            float angle = Mathf.Lerp(-20, 20, Mathf.PingPong(Time.time * 16, 1));
            this.transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        Vector2 position = rectTransform.anchoredPosition;
        enemyObjectList[this.line, this.field] = null;
        Destroy(this.gameObject);
        this.enemyManager.CreateEnemySlot(this.line, this.field, position.x, position.y);
    }

    public int GetPower ()
    {
        return this.power;
    }

    void Update()
    {

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
