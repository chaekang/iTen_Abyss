using UnityEngine;

[CreateAssetMenu(fileName = "New FlashlightAction", menuName = "Inventory/Actions/FlashlightAction")]
public class FlashlightAction : ItemAction
{
    public float batteryTime = 120f;
    private float currentBatteryTime;

    private Light flashlight;

    private void OnEnable()
    {
        flashlight = GameObject.FindObjectOfType<Light>();
        currentBatteryTime = batteryTime;
    }

    public override void Use(GameObject user)
    {
        if (flashlight != null)
        {
            flashlight.enabled = true;
            currentBatteryTime -= Time.deltaTime;

            if (currentBatteryTime <= 20f)
            {
                flashlight.enabled = !flashlight.enabled;
            }

            if (currentBatteryTime <= 0f)
            {
                flashlight.enabled = false;
            }
        }
    }

    public void AddBattery(float amount)
    {
        currentBatteryTime += amount;
    }
}