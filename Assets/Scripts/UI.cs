using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private Label label;
    private VisualElement frame;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var rootVisualElement = uiDocument.rootVisualElement;

        frame = rootVisualElement.Q<VisualElement>("Frame").Q<VisualElement>("TopBar");
        label = frame.Q<Label>("Label");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(label.text);
        label.text = GameManager.Instance.getCurrentPlayerText();
    }
}
