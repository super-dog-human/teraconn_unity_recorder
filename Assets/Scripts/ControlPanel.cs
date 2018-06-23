using UnityEngine;

public class ControlPanel : MonoBehaviour
{
	private GraphicSwitcher graphicSwitcher;

	void Start ()
	{
		graphicSwitcher = GameObject.Find("ScriptLoader").GetComponent<GraphicSwitcher>();
	}

	public void ButtonClick ()
	{
		switch (transform.name) {
		case "NextImageButton":
			graphicSwitcher.ChangeNextGraphic();
			break;
		case "PrevImageButton":
			graphicSwitcher.ChangePrevGraphic();
			break;
		default:
			break;
		}
	}
}
