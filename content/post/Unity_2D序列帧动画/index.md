+++
title = "Unity 2D序列帧动画实现"
date = "2026-05-03T10:40:00+08:00"
draft = false
categories = ["Unity"]
tags = ["笔记", "2D动画", "序列帧"]
+++

通过序列帧Sprite数组实现2D动画播放，适用于简单的2D游戏动画效果。

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation2D : MonoBehaviour
{
    //直接定义个序列帧Sprite数组
    public Sprite[] sprs;

    public SpriteRenderer sr;
    private float time;
    private int index = 0;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprs[index];
        // 打印开始信息
        Debug.Log("动画开始，当前帧：" + index);
    }

    void Update()
    {
        //每次增加帧间隔时间
        time += Time.deltaTime;
        //间隔时间到0.02f
        if (time >= 0.1f)
        {
            index++;
            //已经显示到最后一张图,从头显示
            if (index >= sprs.Length)
            {
                index = 0;
            }
            sr.sprite = sprs[index];
            //记得把间隔也重置下
            time = 0;

            // 打印当前播放的帧
            Debug.Log("正在播放第 " + index + " 帧");
        }
    }
}
```

## 实现原理

### 核心机制
- **Sprite数组**：`sprs[]` 存储所有序列帧图片
- **帧切换**：通过 `Time.deltaTime` 累加时间，达到间隔后切换帧
- **循环播放**：当播放到最后一张时，重置 `index = 0` 重新开始

### 关键参数
| 参数 | 说明 |
|------|------|
| `sprs[]` | 序列帧图片数组，在Inspector中拖入 |
| `time` | 时间累加器，记录帧间隔 |
| `index` | 当前播放帧索引 |
| `0.1f` | 每帧播放时长（秒） |

## 使用方法

### 1. 创建动画控制器脚本
将脚本挂载到包含 `SpriteRenderer` 组件的GameObject上

### 2. 准备序列帧图片
在Unity中选中多张序列帧图片，右键选择 **Create > Sprites > Square** 或直接拖入项目文件夹

### 3. 配置参数
- 将序列帧图片拖入 `Sprs` 数组
- 调整 `SpriteRenderer` 引用

### 4. 调整帧率
修改 `if (time >= 0.1f)` 中的数值：
- 减小数值（如0.05f）：播放更快
- 增大数值（如0.2f）：播放更慢

## 扩展功能

### 添加播放控制

```csharp
public class Animation2DExtended : MonoBehaviour
{
    public Sprite[] sprs;
    public SpriteRenderer sr;
    public float frameRate = 0.1f;  // 可调节帧率

    private float time;
    private int index = 0;
    public bool isPlaying = true;   // 播放控制
    public bool loop = true;        // 循环控制

    public int CurrentFrame => index;
    public int TotalFrames => sprs.Length;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sprs.Length > 0)
            sr.sprite = sprs[0];
    }

    void Update()
    {
        if (!isPlaying || sprs.Length == 0) return;

        time += Time.deltaTime;
        if (time >= frameRate)
        {
            index++;
            if (index >= sprs.Length)
            {
                if (loop)
                    index = 0;
                else
                {
                    index = sprs.Length - 1;
                    isPlaying = false;
                }
            }
            sr.sprite = sprs[index];
            time = 0;
        }
    }

    // 播放
    public void Play()
    {
        isPlaying = true;
    }

    // 暂停
    public void Pause()
    {
        isPlaying = false;
    }

    // 停止并重置
    public void Stop()
    {
        isPlaying = false;
        index = 0;
        if (sprs.Length > 0)
            sr.sprite = sprs[0];
    }

    // 跳转到指定帧
    public void GotoFrame(int frame)
    {
        index = Mathf.Clamp(frame, 0, sprs.Length - 1);
        sr.sprite = sprs[index];
    }
}
```

### 播放指定动画

```csharp
public void PlayAttackAnimation()
{
    StartCoroutine(PlaySequence(sprs_attack, 0.05f));
}

private IEnumerator PlaySequence(Sprite[] frames, float rate)
{
    foreach (var frame in frames)
    {
        sr.sprite = frame;
        yield return new WaitForSeconds(rate);
    }
}
```
