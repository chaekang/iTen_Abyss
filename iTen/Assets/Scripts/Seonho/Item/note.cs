using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class note : ItemObject
{
    private GameObject panel; 
    private TextMeshProUGUI textUI;

    private bool isPanelActive = false;
    public int noteNum;

    void Start()
    {
        
        panel = GameObject.Find("Panel"); 
        textUI = panel.transform.Find("NoteText").GetComponent<TextMeshProUGUI>();
       
        panel.SetActive(false);
        
    }

    public override void OnInteract()
    {
        
        Debug.Log("note interact");
        TogglePanel(true);
        UpdateText();
    }

    void Update()
    {
        if (isPanelActive && Input.GetKeyDown(KeyCode.Escape))
        {
            
            TogglePanel(false);
        }
    }

    private void TogglePanel(bool show)
    {
        isPanelActive = show;
        if (panel != null)
        {
            panel.SetActive(show);
        }
    }

    private void UpdateText()
    {
        if (textUI != null)
        {
           
            switch (noteNum)
            {
                case 1:
                    textUI.text = "This is note number 1.";
                    break;
                case 2:
                    textUI.text = "This is note number 2.";
                    break;
                case 3:
                    textUI.text = "This is note number 3.";
                    break;
                default:
                    textUI.text = "Default note content.";
                    break;
            }
        }
    }


}
