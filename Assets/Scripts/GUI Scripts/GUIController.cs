using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    // GUI
    [Header("Heads Up Display")]

    public GameObject healthBar;
    public RectTransform fuelBar;
    public RectTransform armourBar;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;

    [Header("Wave Information")]
    public TextMeshProUGUI waveNumber;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI timeText;

    [Header("Game State information")]
    public GameObject pauseMenu;
    public GameObject gameOverText;
    public GameObject winText;
    public GameObject backButton;

    [Header("Messages")]
    public TextMeshProUGUI pickupText;
    public float pickupMessageTime = 0.5f;

    GameManager game;
    PlayerController player;

    // gui variables
    //private float barWidth;
    private float healthBarHeight, fuelBarHeight, armourBarWidth;
    //float MAX_WIDTH;
    float MAX_HEALTH_HEIGHT, MAX_FUEL_HEIGHT, MAX_ARMOUR_WIDTH;

    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // initialize max bar heights
        MAX_HEALTH_HEIGHT = healthBar.GetComponent<RectTransform>().rect.height;
        MAX_FUEL_HEIGHT = fuelBar.rect.height;
        MAX_ARMOUR_WIDTH = armourBar.rect.width;
    }

    // Update is called once per frame
    void Update()
    {
        // Update HUD
        UpdateText();
        UpdateHealth();
        UpdateJetpackFuel();
        UpdateArmour();
    }

    #region public functions  
    public void ShowPickupMessage(string message)
    {
        pickupText.text = message;
        StartCoroutine(ShowPickupText());
    }

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
        if (percentHealth >= 60)
            healthImage.color = Color.green;
        else if (percentHealth >= 40 && percentHealth < 60)
            healthImage.color = Color.yellow;
        else if (percentHealth < 40)
            healthImage.color = Color.red;

    }

    private void UpdateArmour()
    {
        armourBarWidth = MAX_ARMOUR_WIDTH / player.maxArmour * player.Armour;
        armourBar.sizeDelta = new Vector2(armourBarWidth, armourBar.rect.height);
    }

    private void UpdateJetpackFuel()
    {
        fuelBarHeight = MAX_FUEL_HEIGHT / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(fuelBar.rect.width, fuelBarHeight);
    }
    #endregion

    IEnumerator ShowPickupText()
    {
        pickupText.gameObject.SetActive(true);

        yield return new WaitForSeconds(pickupMessageTime);
        pickupText.gameObject.SetActive(false);
    }

}