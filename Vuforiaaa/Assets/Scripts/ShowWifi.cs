using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Assets;
using WifiConn;
using System.Collections.Generic;


public class ShowWifi : MonoBehaviour,IConnectionCallback {

    public GameObject[] aps;
    public TextMesh[] texts;
    public TextMesh connecting;
    public TextMesh fetchingData;

    public Material WepMaterial;
    public Material WpaMaterial;
    public Material Wpa2Material;
    public Material NoneMaterial;
    public Material AutoMaterial;

    public bool IsVisible { get; private set; }

    public readonly static Queue<Action> ExecuteOnMainThread = new Queue<Action>();

    // Use this for initialization
    void Start () {

        foreach(GameObject ap in aps)
        {
            ap.SetActive(false);
        }
        foreach(TextMesh text in texts)
        {
            text.gameObject.SetActive(false);
        }

        connecting.gameObject.SetActive(false);
        fetchingData.gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        while (ExecuteOnMainThread.Count > 0)
        {
            ExecuteOnMainThread.Dequeue().Invoke();
        }
    }

    private void OnBecameVisible()
    {
        this.IsVisible = true;
        WifiConn.WifiConn.Init();
        WifiConn.WifiConn.TryConnect("Arduino-Uno-WiFi-11B7B7", null, WifiConn.WifiConn.NONE, this);
        connecting.gameObject.SetActive(true);
        //WifiConn.WifiConn.TryConnect("ADAMO-D3B4", "6U5WX5LCCDTFMQ", WifiConn.WifiConn.WPA, this);
        if (Application.platform != RuntimePlatform.Android)
        {
            StartCoroutine(GetWifi());
        }

            //WifiConn.WifiConn.ConnectTo("Arduino-Uno-WiFi-11B7B7", null, WifiConn.WifiConn.NONE);
            //StartCoroutine(GetWifi());
        }


    public void Connect()
    {
        ExecuteOnMainThread.Enqueue(() => { StartCoroutine(DoPeticions()); });
    }

    public void Disconnect()
    {
        WifiConn.WifiConn.StopConnection();
    }

    private void OnBecameInvisible()
    {
        this.IsVisible = false;
        foreach (GameObject ap in aps)
        {
            ap.SetActive(false);
        }
        foreach (TextMesh text in texts)
        {
            text.gameObject.SetActive(false);
        }
        //WifiConn.WifiConn.StopConnection();
    }

    IEnumerator DoPeticions()
    {
        yield return GetWifi();
        while (this.IsVisible)
        {
            Debug.Log(this.gameObject.activeInHierarchy);
            yield return UpdateWifi();
            yield return new WaitForSeconds(3);
        }
    }

    IEnumerator UpdateWifi()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.240.1/wifi/scan");
        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            ResultClass result = JsonConvert.DeserializeObject<ResultClass>(www.downloadHandler.text);
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            Array.Sort(result.result.Aps);

            for (int x = 0; x < 4 && x < result.result.Aps.Length; x++)
            {
                float size = 0;
                if (Single.TryParse(result.result.Aps[x].rssi, out size))
                {
                    if (size > -20) continue;
                    size = (size + 100f) * 1.2f;
                    size /= 100f;
                    texts[x].text = result.result.Aps[x].essid;
                    aps[x].SetActive(true);
                    texts[x].gameObject.SetActive(true);

                    switch (result.result.Aps[x].enc)
                    {
                        case "WEP":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = WepMaterial;
                            break;

                        case "WPA":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = WpaMaterial;
                            break;

                        case "WPA2":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = Wpa2Material;
                            break;

                        case "None":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = NoneMaterial;
                            break;

                        case "Auto":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = AutoMaterial;
                            break;
                    }
                }
                aps[x].transform.localScale = new Vector3(size, 1f, 1f);
            }
        }
    }



    IEnumerator GetWifi()
    {
        connecting.gameObject.SetActive(false);
        fetchingData.gameObject.SetActive(true);
        UnityWebRequest www = UnityWebRequest.Get("http://192.168.240.1/wifi/scan");
        //yield return new WaitForSeconds(3);
        yield return www.SendWebRequest();
        www.Abort();
        www = UnityWebRequest.Get("http://192.168.240.1/wifi/scan");
        yield return www.SendWebRequest();
        

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            ResultClass result = JsonConvert.DeserializeObject<ResultClass>(www.downloadHandler.text);
            // Show results as text
            Debug.Log(www.downloadHandler.text);
            Array.Sort(result.result.Aps);

            for (int x = 0; x < 4 && x < result.result.Aps.Length; x++)
            {
                float size = 0;
                if (Single.TryParse(result.result.Aps[x].rssi, out size))
                {
                    if (size > -20) continue;
                    size = (size + 100f) * 1.2f;
                    size /= 100f;
                    texts[x].text = result.result.Aps[x].essid;
                    aps[x].SetActive(true);
                    texts[x].gameObject.SetActive(true);

                    switch (result.result.Aps[x].enc)
                    {
                        case "WEP":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = WepMaterial;
                            break;

                        case "WPA":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = WpaMaterial;
                            break;

                        case "WPA2":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = Wpa2Material;
                            break;

                        case "None":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = NoneMaterial;
                            break;

                        case "Auto":
                            aps[x].transform.GetChild(0).GetComponent<Renderer>().material = AutoMaterial;
                            break;
                    }
                }
                aps[x].transform.localScale = new Vector3(size, 1f, 1f);
            }
        }
        fetchingData.gameObject.SetActive(false);
    }
}
