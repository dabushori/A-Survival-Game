
using UnityEngine;
using UnityEngine.UI;

/*
 * A class to set up the parameters of an item slot in a recipe
 */
public class SlotSetup : MonoBehaviour
{
    [SerializeField]
    protected TMPro.TMP_InputField amount;
    [SerializeField]
    protected Image icon;

    /*
     * Set up the parameters of the slot
     */
    public void SetUp(int amount, Sprite icon)
    {
        this.amount.text = amount.ToString();
        this.icon.sprite = icon;
    }
}
