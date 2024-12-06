using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerDetail : MonoBehaviour
{
    private TowerController towerController;

    public GameObject detailIcon;
    public GameObject detailTextObject;
    public TextMeshProUGUI detailText;
    public GameObject detailBackground;

    void Start()
    {
        towerController = transform.parent.parent.GetComponent<TowerController>();
        GetTowerDetail();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            HideTowerDetail();
        }
    }

    private void OnDisable()
    {
        HideTowerDetail();
    }

    public void GetTowerDetail()
    {
        string towerName = towerController.gameObject.name;
        string hp = towerController.hp.ToString();
        string dmg = towerController.damage.ToString();
        string AtkCD = towerController.attackCooldown.ToString();
        string range = towerController.range.ToString();
        string enegy = towerController.maintenanceEnergyPerSecond.ToString();
        detailText.text = "<b><size=0.7>" + towerName + "</size></b><br>" +
              "HP: " + hp + "<br>DMG: " + dmg + "<br>Attack CD: " + AtkCD + 
              "<br>Range: " + range + "<br>Souls used per 2s: " + enegy;
    }

    public void ShowTowerDetail()
    {
        detailTextObject.SetActive(true);
        detailBackground.SetActive(true);
    }

    public void HideTowerDetail() {
        detailTextObject.SetActive(false);
        detailBackground.SetActive(false);
    }
}
