﻿using Gallop;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using UmamusumeResponseAnalyzer;
using UmamusumeResponseAnalyzer.Handler;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Test
{
    internal class Program
    {
        static void Main()
        {
            Database.Initialize();
            var bytes = File.ReadAllBytes(@"C:\Users\Lipi\AppData\Local\UmamusumeResponseAnalyzer\packets\23-11-24 04-43-21-858R.msgpack");
            dynamic dyn = JObject.Parse(MessagePack.MessagePackSerializer.ConvertToJson(bytes)) ?? throw new Exception("反序列化失败");
            if (dyn.data.single_mode_load_common != null)
            {
                var data1 = dyn.data.single_mode_load_common;
                if (dyn.data.arc_data_set != null)
                {
                    data1.arc_data_set = dyn.data.arc_data_set;
                }
                if (dyn.data.venus_data_set != null)
                {
                    data1.venus_data_set = dyn.data.venus_data_set;
                }
                dyn.data = data1;
            }
            SingleModeCheckEventResponse obj = dyn.ToObject<SingleModeCheckEventResponse>();
            Handlers.ParseFriendSearchResponse(dyn.ToObject<FriendSearchResponse>());
            if (false)
            {
                obj.data.chara_info.card_id = 103102;
                obj.data.chara_info.skill_point = 0;
                obj.data.chara_info.proper_distance_short = 7;
                obj.data.chara_info.proper_distance_mile = 7;
                obj.data.chara_info.proper_distance_middle = 7;
                obj.data.chara_info.proper_distance_long = 7;
                obj.data.chara_info.proper_ground_turf = 7;
                obj.data.chara_info.proper_ground_dirt = 7;
                obj.data.chara_info.proper_running_style_nige = 7;
                obj.data.chara_info.proper_running_style_senko = 7;
                obj.data.chara_info.proper_running_style_sashi = 7;
                obj.data.chara_info.proper_running_style_oikomi = 7;
                obj.data.chara_info.skill_upgrade_info_array = new int[] { 6030101, 6030201, 6050101, 6050201 }.Select(x => new SkillUpgradeInfo
                {
                    condition_id = x,
                    current_count = 1,
                    total_count = 1
                }).ToArray();
                obj.data.chara_info.skill_tips_array = [];

                obj.data.chara_info.skill_point = 9999;
                obj.data.chara_info.skill_array =
                    [
                    new SkillData
                    {
                        skill_id = 201253,
                        level = 1,
                    }
                    ];
                obj.data.chara_info.skill_array = [];
            }
            //Handlers.ParseSkillTipsResponse(obj);
        }
    }
}