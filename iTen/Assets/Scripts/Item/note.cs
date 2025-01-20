using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Note : ItemObject
{
    public GameObject panel;
    public TextMeshProUGUI textUI;

    public int notePage = 0;

    private bool isItemPanelActive = false; // 아이템과 상호작용으로 열린 패널
    private bool isUiPanelActive = false;  // Tab 키로 열린 패널
    private List<string> viewedNotes = new List<string>(); // 유저가 열람한 노트 목록
    private int currentNoteIndex = 0;      // 탭으로 확인 중인 노트의 인덱스

    public int noteNum;

    void Start()
    {
        panel.SetActive(false);
        Debug.Log(viewedNotes.Count);
    }

    public override void OnInteract()
    {
        Debug.Log("note interact");
        panel.SetActive(true);
        isItemPanelActive = true;
        UpdateText();

        // 유저가 열람한 노트를 기록
        string noteContent = GetNoteContent(noteNum);
        if (!viewedNotes.Contains(noteContent))
        {
            viewedNotes.Add(noteContent);
        }

        Debug.Log("아이템 페이");
    }

    void Update()
    {
        // ESC로 패널 닫기
        if ((isItemPanelActive || isUiPanelActive) && Input.GetKeyDown(KeyCode.Escape))
        {
            panel.SetActive(false);
            isItemPanelActive = false;
            isUiPanelActive = false;
            notePage = 0;
        }

        // 탭 키로 열람한 노트 확인 (아이템 패널이 닫혀 있을 때만)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 아이템 패널이 열려 있을 때는 아무 작업도 하지 않음
            if (isItemPanelActive)
            {
                return; // 바로 반환
            }

            // UI 패널을 열거나 다음 페이지를 보여줌
            if (!isUiPanelActive)
            {
                panel.SetActive(true);
                isUiPanelActive = true;
                notePage = 0;
                Debug.Log("ui 페이지");
            }
            else
            {
                Debug.Log("다음 페이지로");
                ShowNextViewedNote(notePage);
                notePage++;
            }
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
                return "This is note number 1.";
            case 2:
                return "This is note number 2.";
            case 3:
                return "This is note number 3.";
            default:
                return "Default note content.";
        }
    }

    private void ShowNextViewedNote(int currentNoteIndex)
    {
        if (viewedNotes.Count == 0)
        {
            textUI.text = "No notes have been viewed yet.";
            return;
        }

        currentNoteIndex = (currentNoteIndex + 1) % viewedNotes.Count; 
        textUI.text = viewedNotes[currentNoteIndex];
    }
}
