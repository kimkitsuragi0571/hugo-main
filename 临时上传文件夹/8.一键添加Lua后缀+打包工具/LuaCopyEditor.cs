using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//继承Editor类,可以让其不被打包,只在编辑器中运行
public class LuaCopyEditor : Editor
{
   [MenuItem("XLua/自动生成txt后缀Lua")]
   public static void CopyLuaToText()
   {
      Debug.Log("哔哔哔,开始自动执行Lua到Txt的转化");
      //首先找到所有的Lua文件(固定在Lua子文件夹下)
      string path = Application.dataPath + "/_Script/Lua/";
      //要是没有文件夹直接就返回得啦
      if (!Directory.Exists(path))
         return;
      //得到每个Lua文件的路径,才能进行迁移拷贝
      //第二个参数指定是Lua文件,不然.meta也获取了
      string[] strs = Directory.GetFiles(path,"*.lua");
      //然后把Lua文件拷贝到新的文件夹里面,确定一个新的路径
      string newPath = Application.dataPath + "/_Script/LuaTxt/";
      
      //文件夹不存在就先创建文件夹
      if (!Directory.Exists(newPath))
         Directory.CreateDirectory(newPath);
      else
      {
         //如果已经存在文件夹,需要先把旧的.txt文件都清空
         string[] oldFileStrs = Directory.GetFiles(newPath, "*.txt");
         foreach(string str in oldFileStrs)
         {
            File.Delete(str);
         }
      }
      
   //存储拼接后的文件路径
   List<string> newFileNames = new List<string>();
      //文件名方便拼接
   string fileName;
   foreach(string str in strs)
   {
      fileName = newPath + str.Substring(str.LastIndexOf('/') + 1) + ".txt";
      //每次都把新的文件路径压入List
      newFileNames.Add(fileName);
      File.Copy(str,fileName);
   }
   //创建后记得刷新下才看得见文件夹
   //嫌麻烦这里直接用API刷新一下
   AssetDatabase.Refresh();
   Debug.Log("哔哔哔,已经生成.txt文件");
   
   //一定是刷新之后再打AB包,不刷新打包就无效了
   foreach (string str in newFileNames)
   {
      Debug.Log("哔哔哔,开始将.txt文件打入AB包");
      //该API传入路径必须是相对Assets文件夹 即 Assets/.../...
      AssetImporter importer = AssetImporter.GetAtPath(str.Substring(str.IndexOf("Assets")));
      //如果包不为空,我们就打入reqlua包里面
      if (importer != null)
         importer.assetBundleName = "reqlua";
   }
   }
}
