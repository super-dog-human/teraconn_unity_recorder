using UnityEngine;

public class FPSIndicator : MonoBehaviour {
    [SerializeField]
    const float m_updateInterval = 0.5f;
    float m_accum;
    int m_frames;
    float m_timeleft;
    float m_fps;

    void Update() {
        m_timeleft -= Time.deltaTime;
        m_accum += Time.timeScale / Time.deltaTime;
        m_frames++;

        if ( 0 < m_timeleft ) return;

        m_fps = m_accum / m_frames;
        m_timeleft = m_updateInterval;
        m_accum = 0;
        m_frames = 0;
    }

    void OnGUI() {
        GUIStyleState styleState = new GUIStyleState();
        styleState.textColor = Color.black;

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal = styleState;

        GUILayout.Label("FPS: " + m_fps.ToString( "f2" ), guiStyle);
    }
}