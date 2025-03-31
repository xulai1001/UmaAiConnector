using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallop
{
    [MessagePackObject]
    public class ChoiceReward
    {
        [Key("select_index")]
        public int select_index; // 0x10
        [Key("gain_param_array")]
        public GainParam[] gain_param_array;
    }

    [MessagePackObject]
    public class GainParam
    {
        [Key("display_id")]
        public int display_id;
        [Key("effect_value_0")]
        public int effect_value_0;
        [Key("effect_value_1")]
        public int effect_value_1;
        [Key("effect_value_2")]
        public int effect_value_2;

        public string display()
        {
            return $"{display_id} ({effect_value_0}, {effect_value_1}, {effect_value_2})";
        }

        public int[] effects()
        {
            return new int[] { effect_value_0, effect_value_1, effect_value_2 };
        }
    }
}
