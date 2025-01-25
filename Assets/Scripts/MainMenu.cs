using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public VisualElement ui;
    public Button playButton;
    public Button howToPlayButton;
    public Button quitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        playButton = ui.Q<Button>("Button-PlayGame");
        playButton.clicked += OnPlayButtonClicked;
        howToPlayButton = ui.Q<Button>("Button-HowToPlay");
        howToPlayButton.clicked += OnHowToPlayButtonClicked;
        quitButton = ui.Q<Button>("Button-ExitGame");
        quitButton.clicked += OnQuitButtonClicked;
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

    private void OnHowToPlayButtonClicked()
    {
        Debug.Log("HowToPlay!");
    }

    private void OnPlayButtonClicked()
    {
        gameObject.SetActive(false);
    }
}
