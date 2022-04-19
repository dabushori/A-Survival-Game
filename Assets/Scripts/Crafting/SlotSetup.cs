
using UnityEngine;
using UnityEngine.UI;

public class SlotSetup : MonoBehaviour
{
    [SerializeField]
    TMPro.TMP_InputField amount;
    [SerializeField]
    Image icon;

   public void SetUp(int amount, Sprite icon)
    {
        this.amount.text = amount.ToString();
        this.icon.sprite = icon;
    }
}
