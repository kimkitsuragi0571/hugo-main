using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    public static void Main(string[] args)
    {
        var serv = new ServiceCollection();

        // 开启日志系统+注册日志级别和输出对象(直接在注册列表中注册对日志的要求)
        // 需要在DI容器创建之前开启，因为其他类创建时需要立即用到日志系统
        serv.AddLogging((ILoggingBuilder logBuilder) =>
        {
            // 告诉日志系统把记录的文字打印到Console窗口内
            logBuilder.AddConsole();
            // 设定打印日志的最低级别，低于Debug级别的不打印
            logBuilder.SetMinimumLevel(LogLevel.Debug);
        });

        // 注册服务
        serv.AddScoped<IAtk, ReAtk>();
        serv.AddScoped<Player>();

        // 构建容器并获取服务
        using (var prov = serv.BuildServiceProvider())
        {
            var player = prov.GetRequiredService<Player>();
            // 执行带日志的攻击方法
            player.AtkLogger(skillName: "超级重击", powerValue: "999");
        }
    }
}

public class Player
{
    private readonly IAtk _atk;

    // ILogger<T>只是决定日志的标签而已，建议用当前Player类名便于找问题
    private readonly ILogger<Player> _logger;

    // 容器创建Player时，会自动注入 IAtk 和 ILogger<Player>
    public Player(IAtk atk, ILogger<Player> logger)
    {
        _atk = atk;
        _logger = logger;
    }

    // 设个带日志的Atk方法用于演示
    public void AtkLogger(string skillName, string powerValue)
    {
        // 实际开发中推荐用占位符形式，字符串拼接会丧失结构化字段信息
        // _logger.LogInformation("技能名:{SkillName},能量值:{Power}", skillName, powerValue);
        
        _logger.LogInformation("技能名" + skillName);
        _logger.LogInformation("能量值" + powerValue);

        // 测试：通过_logger工具打印不同级别的Log
        try
        {
            int power = int.Parse(powerValue);

            if (power > 100)
            {
                _logger.LogWarning("能量值过高: {Power}", power);
            }

            _atk.Atk();
            _logger.LogDebug("技能执行完毕");
        }
        catch (FormatException ex)
        {
            _logger.LogError(ex, message: "能量值解析失败");
        }
    }
}

public interface IAtk
{
    void Atk();
}
 
public class ReAtk:IAtk
{
    public void Atk()
    {
        Console.WriteLine("重攻击");
    }
}

public class LightAtk:IAtk
{
    public void Atk()
    {
        Console.WriteLine("轻攻击");
    }
}
