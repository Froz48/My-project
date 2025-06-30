using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Coloring : MonoBehaviour
{
    public Color imageColor;
    public Color textMPColor;

    [ContextMenu("Change UI Colors")]
    public void ChangeUIColors()
    {
        Image[] images = FindObjectsOfType<Image>(true); 
        foreach (Image image in images)
        {
            image.color = imageColor;
        }

        TextMeshProUGUI[] textMeshPros = FindObjectsOfType<TextMeshProUGUI>(true); 
        foreach (TextMeshProUGUI textMeshPro in textMeshPros)
        {
            textMeshPro.color = textMPColor;
        }

        Debug.Log($"Цвет изменен для {images.Length} Image и {textMeshPros.Length} TextMeshProUGUI.");
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Change UI Colors")]
    public static void ChangeUIColorsMenu()
    {
        Coloring colorChanger = FindObjectOfType<Coloring>();

        if (colorChanger == null)
        {
            Debug.LogWarning("Скрипт ChangeUITextColors не найден на сцене.");
            return;
        }

        colorChanger.ChangeUIColors();
    }
#endif
}