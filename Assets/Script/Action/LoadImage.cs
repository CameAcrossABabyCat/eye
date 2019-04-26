// ==================================================================
// 作    者：A.R.I.P.风暴洋-宋杨
// 説明する：加载头像类
// 作成時間：2018-07-30
// 類を作る：LoadImage.cs
// 版    本：v 1.0
// 会    社：大连仟源科技
// QQと微信：731483140
// ==================================================================

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadImage : MonoBehaviour
{

    public static LoadImage GetLoadIamge;
    void Awake()
    {
        GetLoadIamge = this;

    }


    void Start()
    {
        // current2D = tex;
    }
    private Dictionary<string, Texture2D> LoadedIamge = new Dictionary<string, Texture2D>();



    public void LoadNoSave(string url, RawImage[] image = null, bool IsSize = false)
    {
        StartCoroutine(GetMessage(url, image));
    }


    public void Load(string url, RawImage[] image = null, bool IsSize = false)
    {
        //  Debug.Log(url);
        Texture2D cuuretimage = null;
        bool IsGet = LoadedIamge.TryGetValue(url, out cuuretimage);
        if (IsGet)
        {
            if (image != null)
            {
                foreach (var item in image)
                {
                    if (item != null)
                    {
                        //if(IsSize)
                        //item.GetComponent<RectTransform>().sizeDelta = new Vector2(cuuretimage.width, cuuretimage.height);
                        item.texture = cuuretimage;
                    }
                }
            }
        }
        else
            StartCoroutine(GetMessage(url, image));
    }


    public void LoadTextute(string url, List<Texture> GroupTexture)
    {
        StartCoroutine(GetGroup(url, GroupTexture));
    }


    private IEnumerator GetGroup(string url, List<Texture> GroupTexture)
    {
        MessageManager._Instantiate.AddLockNub();
        WWW www = new WWW(url);
        yield return www;
        MessageManager._Instantiate.DisLockNub();
        if (www.error == null)
        {
            GroupTexture.Add(www.texture);
            if (!LoadedIamge.ContainsKey(url))
                LoadedIamge.Add(url, www.texture);
        }
        else
        {
            MessageManager._Instantiate.Show("获取头像失败");
        }
    }




    private IEnumerator GetMessage(string url, RawImage[] image, bool IsSzie = false)
    {
        MessageManager._Instantiate.AddLockNub();

        WWW www = new WWW(url);
        yield return www;
        MessageManager._Instantiate.DisLockNub();
        if (www.error == null)
        {
            foreach (var item in image)
            {
                if (item != null)
                {
                    Debug.Log(www.texture.width + "----IMG-----" + www.texture.height);
                    //if (IsSzie)
                    //    item.GetComponent<RectTransform>().sizeDelta = new Vector2(www.texture.width, www.texture.height);
                    item.texture = www.texture;
                }
                else
                    Debug.Log("目标RawImage已被摧毁");
            }
            if (!LoadedIamge.ContainsKey(url))
                LoadedIamge.Add(url, www.texture);
        }
        else
        {
            MessageManager._Instantiate.Show("获取头像失败");
        }
    }


    public Texture2D texture2DTexture(Texture2D tex, int swidth, int sheght)
    {
        Texture2D res = new Texture2D(swidth, sheght, TextureFormat.ARGB32, false);
        for (int i = 0; i < res.height; i++)
        {
            for (int j = 0; j < res.width; j++)
            {
                Color newcolor = tex.GetPixelBilinear((float)j / (float)res.width, (float)i / (float)res.height);
                res.SetPixel(j, i, newcolor);
            }
        }
        res.Apply();
        return res;
    }
    [SerializeField]
    Texture2D current2D;

    public void SendImage(Texture2D img)
    {

        current2D = img;
        IMGTT.texture = img;
        Up();
    }


    public void Up()
    {
        float X = 0;
        float Y = 0;
        if (current2D.width > current2D.height)
        {
            X = 1024;
            Y = ((float)(current2D.height) / (float)current2D.width) * 1024;
        }
        else
        {
            Y = 1024;
            X = ((float)(current2D.width) / (float)current2D.height) * 1024;
        }
        // Texture2D newtext = texture2DTexture(current2D, System.Convert.ToInt32(X), System.Convert.ToInt32(Y));
        Texture2D newtext = texture2DTexture(current2D, 256, 256);
        string base64String = System.Convert.ToBase64String(newtext.EncodeToJPG());
        // MessageManager._Instantiate.Show("base转换完成");
        StartCoroutine(UploadTexture(base64String));
    }

    public void SendImage(byte[] img)
    {
        string base64String = System.Convert.ToBase64String(img);
        // MessageManager._Instantiate.Show("base转换完成");
        StartCoroutine(UploadTexture(base64String));
    }

    public RawImage IMGTT;
    public Text MSG;
    public Texture2D tex;
    IEnumerator UploadTexture(string url, string GetTex)
    {

        WWWForm form = new WWWForm();
        //form.AddField ("imgData", "pic1");
        //form.AddBinaryData ("imgData", GetTex);
        Debug.Log(url);

        WWW www = new WWW(url, form);
        yield return www;

        if (www.error != null)
            print(www.error);
        else
        {

            MSG.text = www.text;
            Debug.Log(www.text);
        }
    }

    IEnumerator UploadTexture(string GetTex)
    {
        //MessageManager._Instantiate.Show("上传开始");
        string url = Static.Instance.URL + "ajax_up_img.php";
        WWWForm form = new WWWForm();
        form.AddField("huiyuan_id", Static.Instance.GetValue("huiyuan_id"));
        form.AddField("img_url", GetTex);
        DtaMD5 data = EncryptDecipherTool.UserMd5Obj();
        form.AddField("token", data.token);
        form.AddField("time", data.time);
        Debug.Log(url);
        MessageManager._Instantiate.AddLockNub();
        WWW www = new WWW(url, form);
        yield return www;
        //MSG.text = string.Empty;
        MessageManager._Instantiate.DisLockNub();
        if (www.error != null)
        {
            // MSG.text = www.error;
            //MessageManager._Instantiate.Show("图片上传失败");
        }
        else
        {
            //  MSG.text = www.text;
            // Debug.Log(www.text);
            MessageManager._Instantiate.WindowShowMessage("图片上传成功");

            //JsonData jd = JsonMapper.ToObject(www.text);
            HttpModel my = GameManager.GetGameManager.http_body.GetTValue("Http_My");
            my.Get();
        }
    }

    public void Laodtext()
    {
        float X = 0;
        float Y = 0;
        if (tex.width > tex.height)
        {
            X = 256.0f;
            Y = ((float)(tex.height) / (float)tex.width) * 256f;
        }
        else
        {
            Y = 256.0f;
            X = ((float)(tex.width) / (float)tex.height) * 256f;
        }
        //Color[] AA=tex.GetPixels();
        //tex.Resize(tex.width/4,tex.height/4,TextureFormat.ARGB32,false);
        //tex.SetPixels(AA);
        //tex.Apply();
        Texture2D newtext = texture2DTexture(tex, System.Convert.ToInt32(X), System.Convert.ToInt32(Y));
        Debug.Log(tex.width + "--" + tex.height);
        string base64String = System.Convert.ToBase64String(newtext.EncodeToPNG());
        StartCoroutine(UploadTexture(base64String));
    }

}
