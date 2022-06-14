using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    TMP_Text nameText;
    public void SetNickName(string name)
    {
        nameText.text = name;
    }
}
