using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class FlashlightManager : MonoBehaviour
{
    public static FlashlightManager Instance { get; private set; }

    public Light flashlight; // 손전등 불빛
    public TextMeshProUGUI timerText; // 배터리 타이머 UI
    public float flashlightDuration = 480f; // 배터리 지속 시간 (초)

    private float remainingTime; // 남은 배터리 시간
    private bool isTimerRunning = false;

    public bool CanUseFlashlight => remainingTime > 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        GameObject timerObject = GameObject.Find("FlashlightTimer");
        timerText = timerObject?.GetComponent<TextMeshProUGUI>();

        remainingTime = flashlightDuration;
        UpdateTimerUI();

        flashlight.enabled = false;
    }


    public void SetFlashlightState(bool isOn)
    {
        if (!CanUseFlashlight)
        {
            flashlight.enabled = false;
            return;
        }

        flashlight.enabled = isOn;

        if (isOn && !isTimerRunning)
        {
            StartCoroutine(RunBatteryTimer());
        }
    }

    private IEnumerator RunBatteryTimer()
    {
        isTimerRunning = true;

        while (remainingTime > 0 && flashlight.enabled)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            flashlight.enabled = false;
            Debug.Log("손전등 배터리가 다 떨어졌습니다.");
        }

        isTimerRunning = false;
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        if (timerText != null)
            timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void AddBatteryTime(float amount)
    {
        remainingTime += amount;
        UpdateTimerUI();
        Debug.Log($"현재 배터리 남은 시간: {Mathf.FloorToInt(remainingTime / 60)}분 {Mathf.FloorToInt(remainingTime % 60)}초");
    }
}