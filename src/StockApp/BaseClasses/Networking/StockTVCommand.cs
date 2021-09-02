using System;
using System.Collections.Generic;
using System.Linq;

namespace StockApp.BaseClasses
{

    public class StockTVCommand
    {
        readonly List<KeyValuePair<CommandTopics, string>> commandList;
        public readonly Action<byte[]> CallBackAction;

        public string CommandString()
        {
            var commands = commandList.Select(x => String.Format("{0}={1}", x.Key, x.Value));
            var returnValue = String.Join(";", commands);
            return returnValue;
        }

        #region Konstruktor

        private StockTVCommand(CommandTopics topic, string value, Action<byte[]> callBackAction) : this(topic, value)
        {
            CallBackAction = callBackAction;
        }

        private StockTVCommand(CommandTopics topic, string value) : this()
        {
            commandList.Add(new KeyValuePair<CommandTopics, string>(topic, value));
        }

        private StockTVCommand()
        {
            commandList = new();
        }

        #endregion



        public static StockTVCommand SendSettingsCommand(StockTVSettings tVSettings)
        {
            StockTVCommand x = new();
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.GameModus, tVSettings.GameModus.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.ColorModus, tVSettings.ColorModus.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.TurnsPerGame, tVSettings.TurnsPerGame.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.PointsPerTurn, tVSettings.PointsPerTurn.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.NextBahnModus, tVSettings.NextBahnModus.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.Bahn, tVSettings.Bahn.ToString()));
            x.commandList.Add(new KeyValuePair<CommandTopics, string>(CommandTopics.Spielgruppe, tVSettings.Spielgruppe.ToString()));
            return x;
        }

        public static StockTVCommand SpielModusCommand(GameModis modus) { return new StockTVCommand(CommandTopics.GameModus, modus.ToString()); }
        public static StockTVCommand ColorModusCommand(ColorModis modus) { return new StockTVCommand(CommandTopics.ColorModus, modus.ToString()); }
        public static StockTVCommand AnzahlKehrenProSpielCommand(int anzahl) { return new StockTVCommand(CommandTopics.TurnsPerGame, anzahl.ToString()); }
        public static StockTVCommand AnzahlPunkteProKehreCommand(int anzahl) { return new StockTVCommand(CommandTopics.PointsPerTurn, anzahl.ToString()); }
        public static StockTVCommand NextBahnModusCommand(NextBahnModis modus) { return new StockTVCommand(CommandTopics.NextBahnModus, modus.ToString()); }


        public static StockTVCommand ResetCommand() { return new StockTVCommand(CommandTopics.Reset, "true"); }
        public static StockTVCommand GetSettingsCommand(Action<byte[]> action) { return new StockTVCommand(CommandTopics.Get, GetCommandValues.Settings.ToString(), action); }
        public static StockTVCommand GetResultCommand(Action<byte[]> action) { return new StockTVCommand(CommandTopics.Get, GetCommandValues.Result.ToString(), action); }


        public static StockTVCommand SendBegegnungenCommand(IEnumerable<StockTVBegegnung> begegnungen)
        {
            string s = string.Empty;
            foreach (var b in begegnungen)
            {
                s += $"{b.SpielNummer}:{b.TeamNameA}:{b.TeamNameB};";
            }
            return new StockTVCommand(CommandTopics.SetBegegnungen, s);
        }



    }

   
}
