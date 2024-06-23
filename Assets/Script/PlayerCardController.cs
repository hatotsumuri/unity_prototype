using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class PlayerCardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public CardProperties cardProperties;
    private bool isHovered = false;
    private bool isDragging = false;
    private int hoverPositionY;
    private int initPositionY;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private Vector3 offsetPosition;
    private int originalSiblingIndex;
    private Camera uiCamera;
    private EnemyObjectInterface lastHoveredEnemyObject;
    private EnemyManager enemyManager;
    private PlayerManager playerManager;
    private MainManager mainManager;


    private void Awake()
    {
        this.mainManager = GameObject.FindAnyObjectByType<MainManager>();
        this.canvasGroup = gameObject.AddComponent<CanvasGroup>();
        this.rectTransform = GetComponent<RectTransform>();
        this.originalPosition = this.rectTransform.position;
        this.uiCamera = Camera.main;
        this.enemyManager = FindObjectOfType<EnemyManager>();
        this.playerManager = FindObjectOfType<PlayerManager>();
    }

    public void SetCardText()
    {
        Transform powerTextTransform = this.transform.Find("PowerText");
        if (powerTextTransform != null)
        {
            GameObject powerTextGameObject = powerTextTransform.gameObject;
            TextMeshProUGUI powerText = powerTextGameObject.GetComponent<TextMeshProUGUI>();
            if (powerText != null)
            {
                powerText.text = this.cardProperties.power.ToString();
            }
        }
        Transform nameTextTransform = this.transform.Find("NameText");
        if (nameTextTransform != null)
        {
            GameObject nameTextGameObject = nameTextTransform.gameObject;
            TextMeshProUGUI nameText = nameTextGameObject.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = this.cardProperties.name;
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        CheckHover(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.enemyManager == null || this.enemyManager.GetEnemyObjectList() == null)
        {
            return;
        }
        GameObject[,] enemyObjectList = this.enemyManager.GetEnemyObjectList();
        for (int l = 0; l < enemyObjectList.GetLength(0); l++)
        {
            for (int f = 0; f < enemyObjectList.GetLength(1); f++)
            {
                GameObject currentObject = enemyObjectList[l, f];
                EnemyObjectInterface enemyObjectInterface = currentObject.gameObject.GetComponent<EnemyObjectInterface>();
                if (enemyObjectInterface != null)
                {
                    enemyObjectInterface.AttackStart();
                }
                //Debug.Log("line:" + enemyObjectInterface.GetLine());
                //Debug.Log("field:" + enemyObjectInterface.GetField());
                //Debug.Log(enemyObjectInterface.GetObjectType());
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.lastHoveredEnemyObject != null)
        {
            if (this.lastHoveredEnemyObject.Attack(cardProperties.power, cardProperties.type, false))
            {
                this.playerManager.playerCardList.Remove(this.gameObject);
                Destroy(this.gameObject);
                this.playerManager.ArrangePlayerHand();
                this.lastHoveredEnemyObject = null;
                if (this.playerManager.playerCardList.Count == 0)
                {
                    int damage = this.enemyManager.GetDamage();
                    StartCoroutine(this.mainManager.Shake(0.36f, 20f));
                    this.playerManager.DirectAttack(damage);
                    this.mainManager.NextRound();
                }
            }
        }
        GameObject[,] enemyObjectList = enemyManager.GetEnemyObjectList();
        for (int l = 0; l < enemyObjectList.GetLength(0); l++)
        {
            for (int f = 0; f < enemyObjectList.GetLength(1); f++)
            {
                GameObject currentObject = enemyObjectList[l, f];
                EnemyObjectInterface enemyObject = currentObject.gameObject.GetComponent<EnemyObjectInterface>();
                if (enemyObject != null)
                {
                    enemyObject.AttackEnd();
                }
            }
        }  
    }

    internal void SetCardProperties(CardProperties cardProperties)
    {
        this.cardProperties = cardProperties;
    }

    private void CheckHover(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        EnemyObjectInterface currentHoveredEnemyObject = null;

        foreach (RaycastResult result in results)
        {
            EnemyObjectInterface enemyObject = result.gameObject.GetComponent<EnemyObjectInterface>();
            if (enemyObject != null)
            {
                currentHoveredEnemyObject = enemyObject;
                if (this.lastHoveredEnemyObject != enemyObject)
                {
                    enemyObject.ForcePointerEnter();
                }
                break;
            }
        }

        if (this.lastHoveredEnemyObject != null && this.lastHoveredEnemyObject != currentHoveredEnemyObject)
        {
            this.lastHoveredEnemyObject.ForcePointerExit();
        }
        this.lastHoveredEnemyObject = currentHoveredEnemyObject;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        this.isHovered = true;
        this.originalSiblingIndex = this.transform.GetSiblingIndex();
        this.transform.SetAsLastSibling();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.isHovered = false;
        this.transform.SetSiblingIndex(originalSiblingIndex);
    }

    public void SetInitPositionY(int initPositionY)
    {
        this.hoverPositionY = initPositionY + 60;
        this.initPositionY = initPositionY;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.originalPosition = this.rectTransform.position;
        Vector3 mouseWorldPosition = uiCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = originalPosition.z;
        offsetPosition = this.rectTransform.position - mouseWorldPosition;
        this.isDragging = true;
        this.canvasGroup.alpha = 0.5f; 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.isDragging = false;
        this.canvasGroup.alpha = 1f;
        this.rectTransform.position = this.originalPosition;
        this.transform.SetSiblingIndex(originalSiblingIndex);
    }

    private void Update()
    {
        if (this.isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = this.uiCamera.WorldToScreenPoint(this.originalPosition).z;
            Vector3 mouseWorldPosition = uiCamera.ScreenToWorldPoint(mousePos);
            this.rectTransform.position = mouseWorldPosition + offsetPosition;
        }
        if (this.isHovered)
        { 
            if (this.rectTransform.anchoredPosition.y <= hoverPositionY)
            {
                this.rectTransform.anchoredPosition += new Vector2(0, 10);
            }
        }
        else
        {
            if (this.rectTransform.anchoredPosition.y >= initPositionY)
            {
                this.rectTransform.anchoredPosition -= new Vector2(0, 10);
            }
        }
    }
}
