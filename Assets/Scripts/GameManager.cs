using UnityEngine;

// Counts the collected orbs and displays the score and win message.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int totalOrbs;
    private int collected;
    private GUIStyle scoreStyle;
    private GUIStyle winStyle;

    private void Awake()
    {
        // Keep one manager and reset the score each time Play starts.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        collected = 0;
        totalOrbs = GameObject.FindGameObjectsWithTag("Orb").Length;
    }

    public void Collect()
    {
        collected++;
        Debug.Log("[GameManager] Collected " + collected + "/" + totalOrbs, this);
    }

    private void OnGUI()
    {
        // Cache the styles so OnGUI does not create them every frame.
        if (scoreStyle == null)
        {
            scoreStyle = new GUIStyle(GUI.skin.label);
            scoreStyle.fontSize = 20;
            winStyle = new GUIStyle(GUI.skin.label);
            winStyle.fontSize = 52;
            winStyle.fontStyle = FontStyle.Bold;
            winStyle.alignment = TextAnchor.MiddleCenter;
            winStyle.normal.textColor = Color.yellow;
        }

        GUI.Label(new Rect(12f, 10f, 400f, 30f),
            "Orbs collected: " + collected + " / " + totalOrbs, scoreStyle);

        if (totalOrbs > 0 && collected >= totalOrbs)
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "YOU WIN!", winStyle);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
