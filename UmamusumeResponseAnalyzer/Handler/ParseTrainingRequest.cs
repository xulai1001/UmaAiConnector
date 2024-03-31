using Gallop;
using MathNet.Numerics.RootFinding;
using Spectre.Console;
using System.Text.RegularExpressions;
using UmamusumeResponseAnalyzer.Entities;
using UmamusumeResponseAnalyzer.Game;

namespace UmamusumeResponseAnalyzer.Handler
{
    public static partial class Handlers
    {
        public static void ParseTrainingRequest(Gallop.SingleModeExecCommandRequest @event)
        {
            int turn = @event.current_turn;
            if(GameStats.currentTurn!=0 && turn != GameStats.currentTurn) return;
            int trainingId = GameGlobal.ToTrainId[@event.command_id];
            if(GameStats.stats[turn]!=null)
                GameStats.stats[turn].playerChoice=trainingId;
        }

        public static void ParseNonTrainingRequest(bool isNonTraining, bool isRace, int turn, int id)
        {
            if (GameStats.currentTurn != 0 && turn != GameStats.currentTurn) return;
            if (GameStats.stats[turn] != null)
            {
                if (isNonTraining)
                {
                    GameStats.stats[turn].isNonTraining = true;
                    GameStats.stats[turn].nonTrainingCommandType = id;

                    // 这部分之后应该常量化/I18n
                    var action = id switch
                    {
                        3 => "出行",
                        7 => "休息",
                        _ => $"非训练回合 #{id}"
                    };
                    AnsiConsole.MarkupLine($"[cyan]{action}[/]");
                }
                if (isRace)
                {
                    GameStats.stats[turn].isRace = true;
                    GameStats.stats[turn].raceProgramId = id;
                    AnsiConsole.MarkupLine($"[cyan]出赛[/]");
                }
            }
        }
    }
}

