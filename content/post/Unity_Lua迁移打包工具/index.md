+++
title = "Unity中Lua迁移打包工具"
date = "2026-08-22T15:20:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "xLua", "Editor", "AssetBundle", "工具"]
+++

在 Unity 中使用 xLua + AB 包热更新时，Lua 脚本需要被当作 `TextAsset` 打进 AB 包。但 AB 包只能识别 `.txt` 后缀的文本文件，无法直接识别 `.lua` 后缀。本工具实现一键将 `.lua` 文件复制为 `.txt` 并自动打入 AB 包。

## 一、核心概念

### 1. 继承 Editor 类

继承 `Editor` 类，可以让其不被打包，只在编辑器中运行。

### 2. [MenuItem] 特性

```csharp
[MenuItem("XLua/自动生成txt后缀Lua")]
```

- 添加这个特性就可以在工具栏显示
- 参数就是栏目名称而已，子路径就是下拉框里面的子选项
- 点击菜单项即执行对应静态方法

---

## 二、完整脚本

```csharp
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
```

---

## 三、功能拆解

### 1. 获取文件并拼接路径

- 没有指定文件夹先生成，有指定文件夹就清空里面旧的 `.txt` 文件
- 获取指定文件夹中指定 `.lua` 后缀的文件
- `Directory.GetFiles(path, "*.lua")` 第二个参数过滤后缀，避免获取到 `.meta` 文件
- 文件路径拼接：`新路径 + 文件名 + .txt`

### 2. 生成后自动刷新文件夹方便查看

```csharp
AssetDatabase.Refresh();
```

- 调用 `AssetDatabase.Refresh()` 刷新资源数据库
- 刷新后 Project 窗口才能看到新生成的 `.txt` 文件

### 3. 自动把生成的 `.txt` 文件打印进 AB 包

```csharp
AssetImporter importer = AssetImporter.GetAtPath(相对路径);
importer.assetBundleName = "reqlua";
```

- **一定是刷新之后再打 AB 包**，不刷新打包就无效了
- `GetAtPath` 传入的路径必须是**相对 Assets 文件夹**的路径（`Assets/.../...`）
- 通过 `str.Substring(str.IndexOf("Assets"))` 截取相对路径
- 设置 `assetBundleName` 后，该文件会被打入指定名称的 AB 包

---

## 四、使用注意事项

1. **必须放在 Editor 文件夹下**
   - 这个脚本必须放在 `Editor` 文件夹下面，不然运行期参与编译报错

2. **打包报错可能是需要清空文件夹**
   - 打包如果报错，可能是需要清空文件夹
   - 脚本内部已实现自动清空旧 `.txt` 文件，但首次运行前需确保路径正确

3. **BOM 编码问题**
   - 报错显示：`LuaException: error loading module BasePanel from CustomLoader, BasePanel:1: unexpected symbol near '<\239>'`
   - 直接用 VSCode 将文本格式从 **UTF-8 BOM** 改为 **UTF-8**，不要做啥 BOM 剥离了
   - `luaEnv.AddLoader(CustomLoader);` 路径记得修改下，不然直接加载绝对路径的 Lua 文件而不是 AB 包里面的
   - 一直报错是之前生成的错误路径下 `.lua.txt` 文件并没有被删除，仍然有 BOM
