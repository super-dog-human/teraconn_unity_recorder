﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class LessonMaterial : MonoBehaviour {
    public List<Sprite> graphics;
    public List<string> graphicIds;

    string materialPath;
    string lessonID;
    string materialURL;
    const string jsonFileName = "lesson_material.json";
    Material material;
    Subject<Unit> loadSubject = new Subject<Unit>();

    public IObservable<Unit> OnLoadCompleted {
        get { return loadSubject; }
    }

    public void SetMaterialURL (string url) {
        materialURL = url;
    }

    void Start () {
        if (Debug.isDebugBuild) return;

        lessonID = Application.absoluteURL.Split('?')[1];// TODO change "?" to "/"
//        materialURL =  Application.absoluteURL.Replace("?", "") + "/materials.zip";;
        materialURL = $"https://storage.googleapis.com/demo_teraconnect/{lessonID}/materials.zip"; // FIXME
        materialPath = Path.Combine(Application.persistentDataPath, lessonID);

        StartCoroutine("DownloadMaterial");
    }

    IEnumerator DownloadMaterial () {
        using (WWW www = new WWW(materialURL)) {
            yield return www;

            if (www.error.Length > 0) {
                yield break;
            }

            UnzipFromBytes(www.bytes);
            LoadGraphics();

            loadSubject.OnNext(Unit.Default);
            loadSubject.OnCompleted();
        }
    }

    void UnzipFromBytes (byte[] bytes) {
        System.IO.MemoryStream zipStream = new System.IO.MemoryStream(bytes);
        ZipUtility.UnzipFromStream(zipStream, materialPath);
        string jsonPath = Path.Combine(materialPath, "materials/" + jsonFileName);
        string jsonText = System.IO.File.ReadAllText(jsonPath);
        material = JsonUtility.FromJson<Material>(jsonText);
    }

    void LoadGraphics () {
        foreach (GraphicMaterial graphic in material.graphics) {
            string filePath = Path.Combine(materialPath, "materials/" + graphic.filename);
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            graphics.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 800.0f));
            graphicIds.Add(graphic.id);
        }
    }
}

[System.Serializable]
public class Material {
    public GraphicMaterial[] graphics;
}

[System.Serializable]
public class GraphicMaterial {
    public string id;
    public string filename;
    public int width;
    public int height;
}