using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LessonMaterial : MonoBehaviour
{
	public bool isLoadCompleted = false;
	public List<Sprite> graphics;

	private string materialPath;
	private string lessonId;
	private const string jsonFileName = "lesson_material.json";
	private Material material;

	void Start()
	{
		lessonId     = Application.absoluteURL.Split("?"[0])[1];
		materialPath = Path.Combine(Application.persistentDataPath, lessonId);

		string materialUrl = Application.absoluteURL.Replace("?", "") + "/materials.zip";
		StartCoroutine("DownloadMaterial", materialUrl);
	}

	private IEnumerator DownloadMaterial(string url)
	{
		using (WWW www = new WWW(url))
		{
			yield return www;

			if (www.error.Length > 0)
			{
				isLoadCompleted = true;
				yield break;
			}

			System.IO.MemoryStream zipStream = new System.IO.MemoryStream(www.bytes);
			ZipUtility.UnzipFromStream(zipStream, materialPath);

			string jsonPath = Path.Combine(materialPath, "materials", jsonFileName);
			string jsonText = System.IO.File.ReadAllText(jsonPath);
			material = JsonUtility.FromJson<Material>(jsonText);

			LoadGraphics();
			isLoadCompleted = true;

			Debug.Log("load completed.");
		}
	}

	private void LoadGraphics()
	{
		foreach (GraphicMaterial graphic in material.graphics)
		{
			string filePath = Path.Combine(materialPath, "materials", graphic.filename);
			byte[] bytes = System.IO.File.ReadAllBytes(filePath);

			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(bytes);
			graphics.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 800.0f));
		}
	}
}

[System.Serializable]
public class Material
{
	public GraphicMaterial[] graphics;
}

[System.Serializable]
public class GraphicMaterial
{
	public string id;
	public string filename;
	public int width;
	public int height;
}