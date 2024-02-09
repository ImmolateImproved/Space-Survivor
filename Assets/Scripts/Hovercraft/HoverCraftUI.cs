using TMPro;
using UnityEngine;

public class HoverCraftUI : MonoBehaviour
{
    [SerializeField] private HoverCraft hoverCraft;
    [SerializeField] private TextMeshProUGUI speedText;

    private void Update()
    {
        if (speedText)
        {
            speedText.text = ((int)hoverCraft.Speed).ToString();
        }
    }
}