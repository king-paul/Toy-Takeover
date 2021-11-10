using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    // GUI
    [Header("Heads Up Display")]
    //public TextMeshProUGUI healthText;
    //public RectTransform healthBar;
    //public GameObject[] healthMetre;
    public GameObject healthBar;
    public RectTransform fuelBar;
    public RectTransform armourBar;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;
    //public TextMeshProUGUI armourText;

    [Header("Wave Information")]
    public TextMeshProUGUI waveNumber;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI timeText;

    [Header("Game State information")]
    public GameObject pauseMenu;
    public GameObject gameOverText;
    public GameObject winText;
    public GameObject backButton;

    GameManager game;
    PlayerController player;

    // gui variables
    private float barWidth;
    private float healthBarHeight, fuelBarHeight;
    float MAX_WIDTH;
    float MAX_HEALTH_HEIGHT, MAX_FUEL_HEIGHT;

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // initialize max bar heights
        MAX_HEALTH_HEIGHT = healthBar.GetComponent<RectTransform>().rect.height;

        // Initialize jetpack fuel bar
        MAX_WIDTH = fuelBar.rect.width;
        MAX_FUEL_HEIGHT = fuelBar.rect.height;
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
    }

    // Update is called once per frame
    void Update()
    {
        /** Update HUD **/
        //healthText.text = player.Health.ToString();
        // update the jetpack fuel bar
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);
        currentAmmoText.text = player.Ammo.ToString();
        maxAmmoText.text = player.MaxAmmo.ToString();
        //armourText.text = player.Armour.ToString();

        UpdateHealth();
    }

    #region public functions   
    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);
    }
    public void ShowGameOver()
    {
        gameOverText.SetActive(true);
        backButton.SetActive(true);
    }
    public void ShowLevelComplete()
    {
        winText.SetActive(true);
        backButton.SetActive(true);
    }
    #endregion

    #region private functions
    private void UpdateText()
    {
        // info text
        waveNumber.text = game.WaveNumber.ToString();
        enemiesLeft.text = game.EnemiesLeft.ToString();
        timeText.text = ((int)Time.time).ToString();

        // ammo text
        currentAmmoText.text = player.Ammo.ToString();
        maxAmmoText.text = player.MaxAmmo.ToString();
    }

    // Gets the halthbar in the battery graphic to increase and decrease in size
    // as well as change colour based on what the player's health is
    private void UpdateHealth()
    {
        // get componenets
        RectTransform healthTransform = healthBar.GetComponent<RectTransform>();
        Image healthImage = healthBar.GetComponent<Image>();

        // Get health percentage
        float percentHealth = player.Health / player.maxHealth * 100;        

        healthBarHeight = MAX_HEALTH_HEIGHT / player.maxHealth * player.Health;
        healthTransform.sizeDelta = new Vector2(healthTransform.rect.width, healthBarHeight);

        // update the color of the bar based on the health percentage
        if (percentHealth >= 80)
            healthImage.color = Color.green;
        else if (percentHealth >= 40 && percentHealth < 80)
            healthImage.color = Color.yellow;
        else if (percentHealth < 40)
            healthImage.color = Color.red;

    }

    private void UpdateArmour()
    {

    }

    private void UpdateJetpackFuel()
    {
        fuelBarHeight = MAX_FUEL_HEIGHT / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(fuelBar.rect.width, fuelBarHeight);
    }
    #endregion
}