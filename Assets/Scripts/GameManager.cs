using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { Init, Running, Paused, Win, Loss};

public class GameManager : MonoBehaviour
{
    // GUI
    [Header("Heads Up Display")]
    public TextMeshProUGUI healthText;
    public RectTransform fuelBar;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;
    public TextMeshProUGUI armourText;

    [Header("Pause and Game Over")]
    public GameObject gameOverText;
    public GameObject pauseText;

    //public GameObject[] walls;

    GameState state;
    PlayerController player;

    // gui variables
    private float barWidth;
    float MAX_WIDTH;

    // properties
    public GameState State { get => state; set => state = value; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Initialize jetpack fuel bar
        MAX_WIDTH = fuelBar.rect.width;
        barWidth = MAX_WIDTH / player.maxFuel * player.Fuel;
        fuelBar.sizeDelta = new Vector2(barWidth, fuelBar.rect.height);

        state = GameState.Running;
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

        // get input
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (state == GameState.Running)
            {
                pauseText.SetActive(true);
                Time.timeScale = 0;
                state = GameState.Paused;                
            }
            else if (state == GameState.Paused)
            {
                pauseText.SetActive(false);
                Time.timeScale = 1;
                state = GameState.Running;
            }
        }

    }

    public void Die()
    {
        gameOverText.gameObject.SetActive(true);
        state = GameState.Loss;
        Time.timeScale = 0;
    }

}
