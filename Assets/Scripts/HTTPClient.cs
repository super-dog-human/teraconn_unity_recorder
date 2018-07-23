using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPClient : MonoBehaviour {
    public void postJson (string url, string jsonString) {
        StartCoroutine(Post(url, jsonString));
    }

    IEnumerator Post(string url, string jsonString) {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Result: " + request.downloadHandler.text);

        if(request.isHttpError || request.isNetworkError) {
            Debug.Log(request.error);
        }
    }
}
