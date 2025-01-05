using UnityEngine;

[CreateAssetMenu(fileName = "New BatteryAction", menuName = "Inventory/Actions/BatteryAction")]
public class BatteryAction : ItemAction
{
    public override void Use(GameObject user)
    {
        FlashlightAction flashlight = FindObjectOfType<FlashlightAction>();
        flashlight.AddBattery(120f);

        Debug.Log("배터리가 사용되었습니다. 손전등에 2분 추가.");
    }
}