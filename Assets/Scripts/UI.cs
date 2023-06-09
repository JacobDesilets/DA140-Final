using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private Label label;
    private VisualElement frame;
    private Label contextualText;
    private Label errorText;

    private int playerCount;

    public VisualTreeAsset playerListEntryTemplate;

    private VisualElement[] playerLabels;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var rootVisualElement = uiDocument.rootVisualElement;

        frame = rootVisualElement.Q<VisualElement>("Frame").Q<VisualElement>("TopBar");
        contextualText = rootVisualElement.Q<VisualElement>("BottomBox").Q<Label>("ContextualText");
        errorText = rootVisualElement.Q<VisualElement>("BottomBox").Q<Label>("ErrorText");

    }

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        playerCount = GameManager.Instance.playerCount;
        playerLabels = new VisualElement[playerCount];

        for(int i = 1; i <= playerCount; i++)
        {
            VisualElement newLabel = playerListEntryTemplate.Instantiate();
            newLabel.name = $"Player{i}Label";
            playerLabels[i - 1] = newLabel;
            newLabel.Q<Label>("PlayerInfo").text = $"Player {i}: {GameManager.Instance.getScore(i)} points";
            frame.Add(newLabel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        int currentPlayer = GameManager.Instance.turn;
        //foreach(VisualElement l in playerLabels)
        //{
        //    l.Q<Label>("PlayerInfo").text = $"Player {i}: {GameManager.Instance.getScore(i)} points";
        //    if (l.name == $"Player{currentPlayer}Label") { l.style.opacity = 1f; }
        //    else { l.style.opacity = 0.5f; }
        //}

        for (int i = 1; i <= playerCount; i++)
        {
            var l = playerLabels[i-1];
            if (l.name == $"Player{currentPlayer}Label") { l.style.opacity = 1f; }
            else { l.style.opacity = 0.5f; }
            l.Q<Label>("PlayerInfo").text = $"Player {i}: {GameManager.Instance.getScore(i)} points";
        }

        if (GameManager.Instance.pc.claimingStage)
        {
            contextualText.text = "Press 1-4 to claim edge features (clockwise). Press 5 to skip";
        } else
        {
            contextualText.text = "Place the tile!";
        }

        errorText.text = GameManager.Instance.errorText;
        

    }
}
