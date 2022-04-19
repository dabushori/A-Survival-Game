
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotSetup : MonoBehaviour
{
    [SerializeField]
    protected TMPro.TMP_InputField amount;
    [SerializeField]
    protected Image icon;

    public void SetUp(int amount, Sprite icon)
    {
        this.amount.text = amount.ToString();
        this.icon.sprite = icon;
    }
}
