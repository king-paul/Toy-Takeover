using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    // GUI
    [Header("Heads Up Display")]
    public TextMeshProUGUI healthText;
    public RectTransform fuelBar;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;
    public TextMeshProUGUI armourText;    

    PlayerController player;
    // gui variables
    private float barWidth;
    float MAX_WIDTH;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Initialize jetpack fuel bar
        MAX_WIDTH = fuelBar.rect.width;
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        /** Update HUD **/
        healthText.text = player.Health.ToString();
        // update the jetpack fuel bar
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
        currentAmmoText.text = player.Ammo.ToString();
        maxAmmoText.text = player.MaxAmmo.ToString();
        armourText.text = player.Armour.ToString();
    }
}
