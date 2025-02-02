using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Note : ItemObject
{
    public GameObject panel;
    public TextMeshProUGUI textUI;


    private bool isItemPanelActive = false; // 아이템과 상호작용으로 열린 패널
    
    public int noteNum;

    void Start()
    {
        panel.SetActive(false);
        
    }

    public override void OnInteract()
    {
        Debug.Log("note interact");
        panel.SetActive(true);
        isItemPanelActive = true;
        UpdateText();

        
    }

    void Update()
    {
        // ESC로 패널 닫기
        if (isItemPanelActive && Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(false);
            isItemPanelActive = false;
            
        }

        
        
    }

    private void UpdateText()
    {
        if (textUI != null)
        {
            textUI.text = GetNoteContent(noteNum);
        }
    }

    private string GetNoteContent(int noteNumber)
    {
        switch (noteNumber)
        {
            case 1:
                return "**Day 1**\n\nI’m trapped in this FUKING building.\n\nI have no idea why I’m here, and who am I. \n\nFeels like I’m the only one here, but every now and then, I hear these bone-chilling screams echoing from far away.\n\nWhatever this place is, I need to get out this goddamn place.\n\nMaybe then I’ll figure out why I’m here... or even who the hell I am.";
            case 2:
                return "**Day 2**\n\n FUCKING MONSTER IS HERE!!!!\n\nI sneezed because my nose was itchy, and suddenly, this ungodly roar came from somewhere far away.\n\nBefore I knew it... this enormous, grotesque thing—something I’ve never seen before—came charging straight at me.\n\nWhat the hell is this place?!";
            case 3:
                return "**Day 4**\n\nI found one of the engines.\n\nIt had three empty battery slots. Lucky for me, I’d picked up some batteries earlier, so I shoved them in, and the damn thing roared to life.\n\n**Warning:** Once you turn on an engine, you better pray.";
            default:
                return "Default note content.";
        }
    }

    
}
