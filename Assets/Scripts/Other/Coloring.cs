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
        // Изменение цвета у всех компонентов Image
        Image[] images = FindObjectsOfType<Image>(true); // true включает неактивные объекты
        foreach (Image image in images)
        {
            image.color = imageColor;
        }

        // Изменение цвета у всех компонентов TextMeshProUGUI
        TextMeshProUGUI[] textMeshPros = FindObjectsOfType<TextMeshProUGUI>(true); // true включает неактивные объекты
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
        // Найти скрипт в сцене
        Coloring colorChanger = FindObjectOfType<Coloring>();

        if (colorChanger == null)
        {
            Debug.LogWarning("Скрипт ChangeUITextColors не найден на сцене.");
            return;
        }

        // Вызвать метод изменения цветов
        colorChanger.ChangeUIColors();
    }
#endif
}