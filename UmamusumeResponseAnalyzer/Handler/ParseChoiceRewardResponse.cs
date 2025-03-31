using Gallop;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Frozen;
using CsvHelper;

namespace UmamusumeResponseAnalyzer.Handler
{
    public static partial class Handlers
    {
        public static void ParseChoiceRewardResponse(List<ChoiceReward> @rewards)
        {
            AnsiConsole.WriteLine($"> 当前事件效果: ");
            foreach (var reward in @rewards)
            {
                //var branches = rewards.FindAll(x => x.select_index == reward.select_index).Count();
                var line = string.Join(", ", reward.gain_param_array.Select(x => explainGainParam(x)));
                AnsiConsole.MarkupLine($"选项 [yellow]{reward.select_index}[/]: {line}");
            }
            AnsiConsole.WriteLine("-------");
        }

        public static string explainGainParam(GainParam param)
        {
            if (GainParamInfos.TryGetValue(param.display_id, out var info)) {
                var effects = param.effects();
                var effect_text = new List<String>();
                try
                {
                    for (var i = 0; i < 3; ++i)
                    {
                        switch (info.paramTypes[i])
                        {
                            case (int)GainParamType.Number:
                                effect_text.Add(effects[i].ToString()); break;
                            case (int)GainParamType.Entry:
                                if (EntryNames.TryGetValue(effects[i], out var t1))
                                    effect_text.Add(t1);
                                else
                                    effect_text.Add($"属性 #{effects[i]}");
                                break;
                            case (int)GainParamType.Buff:
                                if (BuffNames.TryGetValue(effects[i], out var t2))
                                    effect_text.Add(t2);
                                else
                                    effect_text.Add($"Buff #{effects[i]}");
                                break;
                            case (int)GainParamType.Chara:
                                effect_text.Add(Database.Names.GetCharacter(effects[i]).Name);
                                break;
                            case (int)GainParamType.Card:
                                effect_text.Add(Database.Names.GetSupportCard(effects[i]).Nickname);
                                break;
                            case (int)GainParamType.Skill:
                                var skill = SkillManagerGenerator.Default[effects[i]];
                                if (skill != null)
                                    effect_text.Add(skill.Name);
                                else
                                    effect_text.Add($"技能 #{effects[i]}");
                                break;
                        }   // switch
                    }
                    while (effect_text.Count < 3) effect_text.Add("");
                    return string.Format(info.text, effect_text[0].Replace("[", "「").Replace("]", "」"), effect_text[1].Replace("[", "「").Replace("]", "」"));
                }
                catch
                {
                    return $"(Error: {param.display()})";
                }
            }
            else
            {
                return param.display();
            }
        }

        /// <summary>
        /// 对应 text_data category=394的内容，后续可以改成合适的处理方式
        /// </summary>
        public static readonly FrozenDictionary<int, GainParamInfo> GainParamInfos = new Dictionary<int, GainParamInfo>
        {
            { 1, new GainParamInfo("{0}[cyan]+{1}[/]", 2, 1, 0) },
            { 2, new GainParamInfo("{0}[red]-{1}[/]", 2, 1, 0) },
            { 3, new GainParamInfo("粉丝数[cyan]+{0}[/]", 1, 0, 0) },
            { 4, new GainParamInfo("[yellow]{0}[/]的羁绊[cyan]+{1}[/]", 4, 1, 0) },
            { 5, new GainParamInfo("[yellow]{0}[/]的羁绊[cyan]+{1}[/](乱入无效)", 4, 1, 0) },
            { 6, new GainParamInfo("[yellow]「{0}」[/]的Hint[cyan]+{1}[/]", 6, 1, 0) },
            { 8, new GainParamInfo("概率获得随机Hint", 0, 0, 0) },
            { 9, new GainParamInfo("获得[green]{0}[/]", 3, 0, 0) },
            { 10, new GainParamInfo("治疗[blue]{0}[/]", 3, 0, 0) },
            { 11, new GainParamInfo("可以和[yellow]{0}[/]外出了", 4, 0, 0) },
            { 12, new GainParamInfo("[red]连续事件中断[/]", 0, 0, 0) },
            { 13, new GainParamInfo("全属性[cyan]+{0}[/]", 1, 0, 0) },
            { 14, new GainParamInfo("随机{0}种属性[cyan]+{1}[/]", 1, 1, 0) },
            { 15, new GainParamInfo("治疗全部负面状态", 0, 0, 0) },
            { 16, new GainParamInfo("治疗{0}个负面状态", 1, 0, 0) },
            { 17, new GainParamInfo("[yellow]{0}[/][red]锁训练[/]", 4, 0, 0) },
            { 18, new GainParamInfo("[yellow]{0}[/][red]锁比赛[/]", 4, 0, 0) },
            { 19, new GainParamInfo("之前训练的属性[cyan]+{0}[/]", 1, 0, 0) },
            { 20, new GainParamInfo("之前训练的属性[red]-{0}[/]", 1, 0, 0) },
            { 21, new GainParamInfo("获得赛后属性", 0, 0, 0) },
            { 22, new GainParamInfo("根据比赛名次获得属性", 0, 0, 0) },
            { 24, new GainParamInfo("获得碎片 #{0}", 1, 0, 0) },
            { 25, new GainParamInfo("所有蔬菜[green]+{0}[/]", 1, 0, 0) },
            { 26, new GainParamInfo("所有训练等级[green]+{0}[/]", 1, 0, 0) },
            { 27, new GainParamInfo("羁绊最低的[cyan]{1}[/]张卡羁绊[cyan]+{2}[/](除[yellow]{0}[/])", 5, 1, 1) },
            { 28, new GainParamInfo("羁绊最低的[cyan]{0}[/]张卡羁绊[cyan]+{1}[/]", 1, 1, 0) },
            { 29, new GainParamInfo("羁绊最低的卡羁绊[cyan]+{1}[/], 不包含[yellow]{0}[/]", 5, 1, 0) },
            { 30, new GainParamInfo("羁绊最低的卡羁绊[cyan]+{0}[/]", 1, 0, 0) },
            { 31, new GainParamInfo("[yellow]连续事件{0}[/]将获得以下技能", 1, 0, 0) },
            { 32, new GainParamInfo("[yellow]连续事件{0}[/]将获得以下技能", 1, 0, 0) },
            { 33, new GainParamInfo("[yellow]{0}[/]的指南槽+{1}", 4, 1, 0) },
            { 34, new GainParamInfo("随机{0}种属性[red]-{1}[/]", 1, 1, 0) },
            { 35, new GainParamInfo("[yellow]{0}[/]的羁绊[red]-{1}[/]", 4, 1, 0) },
            { 36, new GainParamInfo("[yellow]{0}[/]的羁绊[red]-{1}[/](乱入无效)", 4, 1, 0) },
            { 37, new GainParamInfo("获得[blue]{0}[/]", 3, 0, 0) },
            { 38, new GainParamInfo("{0}[red]-{1}[/](由于[green]{2}[/]的效果无效)", 2, 1, 3) },
            { 39, new GainParamInfo("{0}[cyan]+{1}[/](由于[blue]{2}[/]的效果无效)", 2, 1, 3) },
            { 40, new GainParamInfo("由于[blue]{1}[/]的效果, {0}[red]无效[/]", 3, 3, 0) },
            { 41, new GainParamInfo("{0}[red]-{1}[/](由于[cyan]超绝好调[/]的效果无效)", 2, 1, 0) },
            { 42, new GainParamInfo("[blue]{0}[/]由于[cyan]超绝好调[/]的效果无效", 3, 0, 0) },
            { 43, new GainParamInfo("[blue]{0}[/]由于[green]挑战领域[/]的效果无效", 3, 0, 0) },
            { 44, new GainParamInfo("全员羁绊槽[cyan]+{0}[/]", 1, 0, 0) },
        }.ToFrozenDictionary();

        /// <summary>
        /// text_data 142
        /// </summary>
        public static readonly FrozenDictionary<int, string> BuffNames = new Dictionary<int, string>
        {
            { 1, "熬夜" },
            { 2, "懒惰成性" },
            { 3, "皮肤粗糙" },
            { 4, "变胖" },
            { 5, "偏头痛" },
            { 6, "不擅长练习" },
            { 7, "能人" },
            { 8, "惹人怜爱○" },
            { 9, "潜力股" },
            { 10, "擅长练习○" },
            { 11, "擅长练习◎" },
            { 12, "微小的破绽" },
            { 13, "夺目的光辉" },
            { 14, "与粉丝的约定・北海道" },
            { 15, "与粉丝的约定・东北" },
            { 16, "与粉丝的约定・中山" },
            { 17, "与粉丝的约定・关西" },
            { 18, "与粉丝的约定・小仓" },
            { 19, "仍在准备中" },
            { 20, "玻璃般的双脚" },
            { 21, "怪云行天" },
            { 22, "与粉丝的约定・川崎" },
            { 23, "英雄的光辉" },
            { 24, "待春之蕾" },
            { 25, "正向思考" },
            { 26, "幸运体质" },
            { 27, "热血誓言・短距离" },
            { 28, "不可动摇的热血誓言・短距离" },
            { 29, "热血誓言・英里" },
            { 30, "不可动摇的热血誓言・英里" },
            { 31, "铁心的挑战者" },
            { 32, "休息的心得" },
            { 34, "面向未来的开发合作" },
            { 35, "船橋の誇り" },
            { 36, "凍りついた翼" },
            { 37, "前程万哩" },
            { 100, "热情领域：＜天狼星＞队" },
            { 101, "热情领域：聚集于宝座的人们" },
            { 102, "热情领域：作为先祖的引导者们" },
            { 103, "热情领域：留下印记的人们" },
            { 104, "热情领域：伝説の体現者" }
        }.ToFrozenDictionary();

        public static readonly FrozenDictionary<int, string> EntryNames = new Dictionary<int, string>
        {
            { 1, "速度" },
            { 2, "耐力" },
            { 3, "力量" },
            { 4, "根性" },
            { 5, "智力" },
            { 10, "体力" },
            { 20, "干劲" },
            { 30, "技能点数" }
        }.ToFrozenDictionary();
    }

    public class GainParamInfo
    {
        public string text;
        public int[] paramTypes = { 0, 0, 0 };

        public GainParamInfo(string text, int arg1, int arg2, int arg3)
        {
            this.text = text;
            this.paramTypes = [ arg1, arg2, arg3 ];
        }
    }   
    
    public enum GainParamType
    {
        /// <summary>
        ///  没有用到
        /// </summary>
        None = 0, 
        /// <summary>
        /// 数值
        /// </summary>
        Number = 1, 
        /// <summary>
        ///  词条，如力量
        /// </summary>
        Entry = 2,
        /// <summary>
        /// Buff，如爱娇
        /// </summary>
        Buff = 3,
        /// <summary>
        /// 角色ID
        /// </summary>
        Chara = 4,
        /// <summary>
        /// 支援卡ID
        /// </summary>
        Card = 5,
        /// <summary>
        /// 技能ID
        /// </summary>
        Skill = 6
    }
}
