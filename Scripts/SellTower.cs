using TMPro;
using UnityEngine;

public class SellTower : MonoBehaviour
{
    private TowerController towerController;

    public LayerMask layerMask;
    public GameObject sellIcon;
    public GameObject sellCostTextObject;
    public TextMeshProUGUI sellCostText;
    public bool isManipulating = false;


    void Awake()
    {
        towerController = transform.parent.parent.GetComponent<TowerController>();
    }

    void Start()
    {
        layerMask = LayerMask.GetMask("Tower Selection");
        sellCostText.text = Mathf.RoundToInt((towerController.price * 0.8f)).ToString();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.collider.gameObject.name == "Sell Selection")
            {
                if (!isManipulating) isManipulating = true;
                else
                {
                    Sell();
                }
            }
        }
    }

    public void DisplaySellCost()
    {
        sellIcon.SetActive(false);
        sellCostTextObject.SetActive(true);
    }

    private void OnDisable()
    {
        isManipulating = false;

        sellIcon.SetActive(true);
        sellCostTextObject.SetActive(false);
    }

    private void OnEnable()
    {
        sellCostText.text = (towerController.price * 0.8).ToString();
    }

    public void Sell()
    {
        TowerManager.Instance.DeselectTower();
        SpotManager.Instance.SellTower(towerController);
        BaseManagement.Instance.coins += Mathf.RoundToInt(towerController.price * 0.8f);
    }
}
