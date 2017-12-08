using UnityEngine;
using System.Collections;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
/*
 十年生死两茫茫，写程序，到天亮。
 千行代码，bug无处藏。
 纵使上线又何妨，朝令改，夕断肠。
 领导每天新想法，天天改，日日忙。
 相顾无言，唯有泪千行。
 每晚灯火阑珊处，加班时，工作狂。
*/

public class main : MonoBehaviour
{

    // Use this for initialization

    public Camera camera3D = null;
    public Camera camera2D = null;

    public GameObject target_1 = null;
    public GameObject target_2 = null;

    public GameObject plane = null;

    public UnityEngine.UI.Text textPath = null;

    public UnityEngine.UI.Image scanImg = null;
    public UnityEngine.UI.Image disImg = null;

    Dictionary<string, int> frame = new Dictionary<string, int>();

    public bool isScan = false;

    public bool scanOK = false;

    Bitmap tempBmp = null;

    byte[] tempByte = null;



    public int _baiseCol = 80;
    public int _cutCol = 40;


    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = UnityEngine.Color.red;   //设置字体颜色的
        bb.fontSize = 40;       //当然，这是字体大小
        if (GUI.Button(new Rect(10, 20, 150, 50), "重新扫描"))
        {
            scanOK = false;
            scanImg.sprite = null;
        }
        
        if (scanOK)
        {
            GUI.Label(new Rect(Screen.width / 2, 20, 250, 100), "识别成功", bb);
        }

        if (GUI.Button(new Rect(180, 20, 150, 50), "显示以识别图片"))
        {
            if (!System.IO.Directory.Exists(@Application.persistentDataPath + "/scan"))
            {
                textPath.text = "空空空空空空空空空";
                return;
            }

            var files = Directory.GetFiles(Application.persistentDataPath + "/scan", "*.png");

            string imgNmae = "";

            foreach (var _file in files)
            {
                Console.WriteLine(_file);
                imgNmae = _file;
                break;
            }

            string path = imgNmae;
            FileStream file = new FileStream(path, FileMode.Open);
            int len = (int)file.Length;
            byte[] byData = new byte[len];
            file.Read(byData, 0, len);
            string text = Encoding.UTF8.GetString(byData);
            file.Close();

            Texture2D mTexture = new Texture2D(10, 10);
            mTexture.LoadImage(byData);
            Sprite sprite = Sprite.Create(mTexture, new Rect(0, 0, mTexture.width, mTexture.height), new Vector2(0.5f, 0.5f));

            disImg.sprite = sprite;
        }

        
        if (GUI.Button(new Rect(10, 200, 50, 50), "+"))
        {
            _baiseCol++;
        }

        if (GUI.Button(new Rect(210, 200, 50, 50), "-"))
        {
            _baiseCol--;
        }
        GUI.Label(new Rect(110, 200, 50, 50), _baiseCol.ToString(), bb);
        if (GUI.Button(new Rect(10, 350, 50, 50), "+"))
        {
            _cutCol++;
        }

        if (GUI.Button(new Rect(210, 350, 50, 50), "-"))
        {
            _cutCol--;
        }
        GUI.Label(new Rect(110, 350, 50, 50), _cutCol.ToString(), bb);
    }

    void Start()
    {

        frame.Add("starX", 0);
        frame.Add("starY", 0);
        frame.Add("overX", 0);
        frame.Add("overY", 0);

    }

    void Update()
    {

        if (isScan && !scanOK)
        {
            TestCapture();
        }
    }


    bool find_type = false;
    bool find_diy = false;
    public void Set_Scan(string name, bool temp)
    {
        if (name.Split('_')[0] == "type")
        {
            find_type = temp;
        }
        if (name.Split('_')[0] == "diy")
        {
            find_diy = temp;
        }

        if (find_type && find_diy)
        {
            isScan = true;
        }
        else
            isScan = false;
    }

    public void TestCapture()
    {
        Vector2 temp1 = camera3D.WorldToScreenPoint(target_1.transform.position);
        Vector2 temp2 = camera3D.WorldToScreenPoint(target_2.transform.position);
        if (temp1 == Vector2.zero) return;

        float dis = temp2.y - temp1.y;

        //Debug.Log(temp1 + "----" + temp2 + "----" + dis + "----" + Screen.width + "----" + Screen.height);

        if (dis > Screen.height / 3 && dis < Screen.height / 3 + Screen.height / 15)
        {
            if (temp1.x > Screen.width / 2 - Screen.height / 10 && temp1.x < Screen.width / 2 + Screen.height / 10 && temp2.x > Screen.width / 2 - Screen.height / 10 && temp2.x < Screen.width / 2 + Screen.height / 10)
            {
                StarCapture(temp1);
                scanOK = true;
            }
        }
    }

    string tempName = "";

    public void StarCapture(Vector2 pos)
    {

        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        tempName = Convert.ToInt64(ts.TotalMilliseconds).ToString();

        if (!System.IO.Directory.Exists(@Application.persistentDataPath + "/scan"))
        {
            System.IO.Directory.CreateDirectory(@Application.persistentDataPath + "/scan");//不存在就创建目录 
        }

        StartCoroutine(CaptureByRect(new Rect(pos.x - Screen.height / 5, pos.y - Screen.height / 5, Screen.height / 2.5f, Screen.height / 2.5f), Application.persistentDataPath + "/scan/" + tempName + ".png"));

        //Debug.LogError(Application.persistentDataPath);
    }
    int[,] point;
    Texture2D mTexture;
    int[] texSize = new int[2];
    public IEnumerator CaptureByRect(Rect mRect, string mFileName)
    {
        yield return new WaitForEndOfFrame();
        mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
        mTexture.ReadPixels(mRect, 0, 0);
        mTexture.Apply();

        //Debug.Log(mTexture.GetPixel(1, 100));

        //byte[] bytes1 = mTexture.EncodeToPNG();
        //System.IO.File.WriteAllBytes(Application.persistentDataPath + "/scan/" + tempName + "__.png", bytes1);

        frame["starX"] = 0;
        frame["starY"] = 0;
        frame["overX"] = 0;
        frame["overY"] = 0;

        FrameTexture(mTexture);//计算 边界

        Rect rec = new Rect(frame["starX"], frame["starY"], frame["overX"] - frame["starX"], frame["overY"] - frame["starY"]);

        Texture2D tex = new Texture2D((int)rec.width, (int)rec.height);


        //------------------------------------------------色替换
        /*
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                try
                {
                    var col = mTexture.GetPixel(frame["starX"] + i, frame["starY"] + j);
                    //if (col.r * 255 > 150 && col.g * 255 > 150 && col.b * 255 > 150)
                    //{
                    //    col.a = 0;
                    //    tex.SetPixel(i, j, col);
                    //}
                    //else
                        tex.SetPixel(i, j, col);
                }
                catch { }
            }
        }
         */
        //-----------------------------------------------9宫算
        texSize[0] = tex.width;
        texSize[1] = tex.height;
        point = new int[tex.width, tex.height];

        Judge(tex.width, tex.height);//计算线框外的所有点并赋值

        for (int i = 0; i < tex.width; i++) // 开始绘制  凡是 > 0 的点  都是线框外的点
        {
            for (int j = 0; j < tex.height; j++)
            {
                var col = mTexture.GetPixel(frame["starX"] + i, frame["starY"] + j);
                if (point[i, j] > 0)
                {                    
                    col.a = 0;
                    tex.SetPixel(i, j, col);
                }
                else
                    tex.SetPixel(i, j, col);
            }
        }
        //----------------------------------------
        byte[] bytes = tex.EncodeToPNG();

        tempByte = bytes;
        System.IO.File.WriteAllBytes(mFileName, bytes);

        var tempTex = new Texture2D((int)rec.width, (int)rec.height);
        tempTex.LoadImage(bytes);

        Sprite sprite = Sprite.Create(tempTex, new Rect(0, 0, tempTex.width, tempTex.height), new Vector2(0.5f, 0.5f));

        scanImg.sprite = sprite;

        //LockUnlockBitsExample();
    }

    int point_num = 1;

    void Judge(int width, int height) // 循环计算
    {
        Debug.Log(texSize[0]);
        for (int k = 0; k < texSize[0] * 2; k++)
        {
            if (k == 0) // 第一次左上角定位
            {
                point_num++;
                if (Judge9(0, 0))
                {
                    point[0, 0] = 1;
                }
            }
            else
            {//  循环查询 是否还有需要计算的
                point_num++; // 新点 ID  叠加
                bool find = false;
                int num = 0;
                for (int i = 0; i < texSize[0]; i++)
                {
                    for (int j = 0; j < texSize[1]; j++)
                    {
                        if (point[i, j] > 0) 
                        {
                            //Debug.Log(point[i, j] + "---------" + i + "**" + j);
                            if (point[i, j] == point_num - 1)  // 如果图片中还有匹配最新计算的点  就再计算
                            {
                                //Debug.Log(point[i, j] + "-//////////-" + i + "**" + j);
                                if (Judge9(i, j))  // true， 找到了新的空点，    false  全图搜索没有新点
                                {
                                    find = true;
                                }
                                num++;
                            }
                        }
                    }
                }
                if (num == 0)//全图搜索没有新点
                {
                    Debug.LogError(point_num + "---over--------" + k);
                    return;
                }
            }
        }
    }
    
    bool Judge9(int i, int j)// 9宫计算  周边8个点
    {
        bool find = false;

        for (int t = i - 1; t < i + 2; t++)
        {
            for (int y = j - 1; y < j + 2; y++)
            {
                if (t == i && y == j) // 自身点不计算
                {
                    //Debug.LogWarning(1);
                }
                else
                {
                    if (t >= 0 && y >= 0 && t < texSize[0] && y < texSize[1])  //边界外不计算
                    {
                        //Debug.LogWarning(t + "  " + y + "   " + point[t, y]);
                        if (JudgeColor(t, y))//判断颜色
                        {
                            find = true;
                        }
                    }
                }
            }
        }
        return find;
    }

    bool JudgeColor(int i, int j)
    {

        if (point[i, j] < 1) //若这个点没有被赋值和计算过  就开始判断
        {

            var col = mTexture.GetPixel(frame["starX"] + i, frame["starY"] + j); //up
            //Debug.Log(col + "-------------" + (col * 255));
            if (col.r * 255 > _baiseCol && col.g * 255 > _baiseCol && col.b * 255 > _baiseCol)
            {
                point[i, j] = point_num;   // 若颜色是白色   就给这个点赋值，作为线框外的点

                //Debug.LogError(i + "  " + j + "   " + point[i, j]);
                //Debug.LogError(point[i, j]);
                return true;
            }
        }
        return false;
    }

    //public void LockUnlockBitsExample()
    //{
    //    Bitmap bmp = new Bitmap(Application.persistentDataPath + "/scan_" + tempName + ".png");

    //    //var temp1213 = Bitmap2Byte(bmp);


    //    frame["starX"] = 0;
    //    frame["starY"] = 0;
    //    frame["overX"] = 0;
    //    frame["overY"] = 0;


    //    FrameBitMap(bmp);

    //    Bitmap bmpOut = new Bitmap(frame["overX"] - frame["starX"], frame["overY"] - frame["starY"], PixelFormat.Format24bppRgb);

    //    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmpOut);
    //    g.DrawImage(bmp, new Rectangle(0, 0, frame["overX"] - frame["starX"], frame["overY"] - frame["starY"]), new Rectangle(frame["starX"], frame["starY"], frame["overX"] - frame["starX"], frame["overY"] - frame["starY"]), GraphicsUnit.Pixel);
    //    g.Dispose();


    //    for (int i = 0; i < bmpOut.Width; i++)
    //    {
    //        for (int j = 0; j < bmpOut.Height; j++)
    //        {
    //            var temp = bmpOut.GetPixel(i, j);
    //            if (temp.R > 150 && temp.G > 150 && temp.B > 150)
    //            {
    //                bmpOut.SetPixel(i, j, System.Drawing.Color.White);
    //            }
    //        }
    //    }

    //    bmpOut.MakeTransparent(System.Drawing.Color.White);

    //    //bmpOut.Save(PathForFile(tempName + ".png"), ImageFormat.Png);

    //    bmpOut.Save(Application.persistentDataPath + "/bitmap_" + tempName + ".png", ImageFormat.Png);

    //    tempBmp = bmpOut;



    //    old_tempName = tempName;
    ////}
    //string old_tempName = "car_frame_1";
    //IEnumerator delay()
    //{
    //    yield return new WaitForSeconds(1);
    //    plane.transform.GetComponent<UnityEngine.UI.Image>().overrideSprite = Resources.Load("bitmap/" + old_tempName, typeof(Sprite)) as Sprite;

    //    Texture2D tex = (Texture2D)Resources.Load("bitmap/" + old_tempName) as Texture2D;

    //    Material mater = new Material(Shader.Find("Mobile/VertexLit"));
    //    mater.mainTexture = tex;

    //    plane.transform.GetComponent<Renderer>().sharedMaterial = mater;

    //    old_tempName = tempName;
    ////}

    //void FrameBitMap(Bitmap bmp)
    //{
    //    for (int i = 0; i < bmp.Width; i++)
    //    {
    //        for (int j = 0; j < bmp.Height; j++)
    //        {
    //            var temp = bmp.GetPixel(i, j);
    //            if (temp.R < 50 && temp.G < 50 && temp.B < 50)
    //            {
    //                //bmp.SetPixel(i, j, System.Drawing.Color.Transparent);
    //                if (frame["starX"] == 0)
    //                {
    //                    frame["starX"] = i;
    //                }
    //                else if (i > frame["starX"])
    //                {
    //                    if (frame["overX"] == 0)
    //                    {
    //                        frame["overX"] = i;
    //                    }
    //                    else if (i > frame["overX"])
    //                    {
    //                        frame["overX"] = i;
    //                    }
    //                }
    //                else if (i < frame["starX"])
    //                {
    //                    frame["starX"] = i;
    //                }
    //                //---------------------------
    //                if (frame["starY"] == 0)
    //                {
    //                    frame["starY"] = j;
    //                }
    //                else if (j > frame["starY"])
    //                {
    //                    if (frame["overY"] == 0)
    //                    {
    //                        frame["overY"] = j;
    //                    }
    //                    else if (j > frame["overY"])
    //                    {
    //                        frame["overY"] = j;
    //                    }
    //                }
    //                else if (j < frame["starY"])
    //                {
    //                    frame["starY"] = j;
    //                }
    //            }
    //        }
    //    }

    //    foreach (var key in frame)
    //    {
    //        Debug.LogError(key.Key + " = " + key.Value);
    //    }
    //    if (frame["starX"] > 5)
    //        frame["starX"] -= 5;

    //    if (frame["starY"] > 5)
    //        frame["starY"] -= 5;

    //    if (frame["overX"] < Screen.width - 5)
    //        frame["overX"] += 5;

    //    if (frame["overY"] < Screen.height - 5)
    //        frame["overY"] += 5;
    ////}

    void FrameTexture(Texture2D tex) // 计算线框边界
    {
        //Debug.Log(tex.width + "==" + tex.height);

        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                var temp = tex.GetPixel(i, j);
                if (temp.r * 255 < _cutCol && temp.g * 255 < _cutCol && temp.b * 255 < _cutCol)
                {
                    //bmp.SetPixel(i, j, System.Drawing.Color.Transparent);
                    if (frame["starX"] == 0)
                    {
                        frame["starX"] = i;
                    }
                    else if (i > frame["starX"])
                    {
                        if (frame["overX"] == 0)
                        {
                            frame["overX"] = i;
                        }
                        else if (i > frame["overX"])
                        {
                            frame["overX"] = i;
                        }
                    }
                    else if (i < frame["starX"])
                    {
                        frame["starX"] = i;
                    }
                    //---------------------------
                    if (frame["starY"] == 0)
                    {
                        frame["starY"] = j;
                    }
                    else if (j > frame["starY"])
                    {
                        if (frame["overY"] == 0)
                        {
                            frame["overY"] = j;
                        }
                        else if (j > frame["overY"])
                        {
                            frame["overY"] = j;
                        }
                    }
                    else if (j < frame["starY"])
                    {
                        frame["starY"] = j;
                    }
                }
            }
        }

        foreach (var key in frame)
        {
            //Debug.LogError(key.Key + " = " + key.Value);
        }
        if (frame["starX"] > 5)
            frame["starX"] -= 5;

        if (frame["starY"] > 5)
            frame["starY"] -= 5;

        if (frame["overX"] < Screen.width - 5)
            frame["overX"] += 5;

        if (frame["overY"] < Screen.height - 5)
            frame["overY"] += 5;
    }
}
