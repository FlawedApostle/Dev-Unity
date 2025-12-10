using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject pauseMenuCanvas;
    [SerializeField] private GameObject deathMenuCanvas;

    private bool isPaused = false;

    [SerializeField] private GameObject player; // assign in Inspector
    [SerializeField] private MonoBehaviour playerMovementScript; // or your input script

    // Camera FPS - To check if the game is active, if false (fame not running) stop cam movement , if true (game is running - active) then allow cam movements
    public static bool GameIsActive = false;

    /// Get reference to the camera (playerController)
    [SerializeField] private FirstPersonCamera fpsCamera;

    // Polling the players health - setting a float value ,
    // then in inspector - taking the actual health object from the player character controller - serialized field
    [SerializeField] private HealthSystem playerhealthsystem;

    private void Awake()
    {
        // check player health 
        Debug.Log("Player Health (start) - " + playerhealthsystem.getPlayerCurrentHealth());
        GetActiveScene();       // tell me what scene is loading in console
    }

    private void Start()
    {

        // Show main menu, hide all else
        mainMenuCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
        deathMenuCanvas.SetActive(false);
        
        // Freeze gameplay at start
        Time.timeScale = 0f;

        // Disable player input until game starts
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Mouse Cursor Visibility
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        /// Game hasn't started, nor has it been paused
        GameIsActive = false;
        isPaused = false;
    }


    private void Update()
    {
        // Only allow pausing if game is active
        if (GameIsActive && Input.GetKeyDown(KeyCode.Escape) && deathMenuCanvas.activeSelf == false)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        //if (playerhealthsystem.getPlayerCurrentHealth() <= 0)
        //{
        //    ShowDeathScreen();
        //    Debug.Log("Player Health is Zero ! we made it !" + playerhealthsystem.getPlayerCurrentHealth());

        //}

    }

    // Test
    public void OnClick()
    {
        Debug.Log("Button Clicked");
    }

    // --------------
    // START GAME
    // --------------
    public void StartGame()
    {
        mainMenuCanvas.SetActive(false);
        // Game hasn't started, nor has it been paused
        GameIsActive = true;    
        isPaused = false;

        /// Enable Player controls
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        /// Lock cursor for gameplay
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        /// Starts the game
        Time.timeScale = 1f;

        Debug.Log("Start Current Scene: " + SceneManager.GetActiveScene().name);
    }

    // --------------
    // Pause GAME
    // --------------
    public void PauseGame()
    {
        pauseMenuCanvas.SetActive(true);
        isPaused = true;
        
        Time.timeScale = 0f; // stop the game (time)

        /// Unlock cursor for the menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        /// Stop player Movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;
        
        /// stop camera
        if (fpsCamera != null)
            fpsCamera.CanLook = false;

        Debug.Log("Pause Current Scene: " + SceneManager.GetActiveScene().name);
    }
    // --------------
    // Resume GAME
    // --------------
    public void ResumeGame()
    {
        pauseMenuCanvas.SetActive(false);
        isPaused = false;

        Time.timeScale = 1f;    // Start Game
        /// (Re)-Lock the cursor (hide it)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
       
        /// (Re)-Enable the player movement
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
       
        /// allow camera again
        if (fpsCamera != null)
            fpsCamera.CanLook = true;

        Debug.Log("Resume Current Scene: " + SceneManager.GetActiveScene().name);
    }
    // --------------
    // Restart GAME
    // --------------
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Restart Current Scene: " + SceneManager.GetActiveScene().name);
    }
    // --------------
    // Quit GAME
    // --------------
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Current Scene: " + SceneManager.GetActiveScene().name);
    }
    // --------------
    // Quit GAME
    /// Called by player health script when health reaches 0
    // --------------
    public void ShowDeathScreen()
    {
        deathMenuCanvas.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 0f;
        GameIsActive = false;

        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        Debug.Log("Death - Current Scene: " + SceneManager.GetActiveScene().name);
    }


    // -------
    //  DEBUG: Get Active Current Scene Name
    // -------
    private void GetActiveScene()
    {
         Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);
    }
}
