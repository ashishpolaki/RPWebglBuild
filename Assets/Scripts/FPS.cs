using UnityEngine;

public class FPS : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style = new GUIStyle();
    [SerializeField] private Rect rect;
    [SerializeField] private int fontSize = 20;
    [SerializeField] private Color normalFPSColor = Color.green;
    [SerializeField] private Color lowFPSColor = Color.red;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }
    void OnGUI()
    {
        rect.x = Screen.width / 2;
        float fps = 1.0f / deltaTime;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = fontSize;
        style.normal.textColor = fps < 30 ? lowFPSColor : normalFPSColor;
        string fpsText = fps.ToString("F2"); // Format FPS to 2 decimal places

        // Draw a box around the FPS label
        GUI.Box(rect, GUIContent.none);
        GUI.Label(rect, fpsText, style);
    }
}
