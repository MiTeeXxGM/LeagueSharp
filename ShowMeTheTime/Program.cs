using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace TheTime
{
    class Program
    {
        public static Menu Config;

        static void Main(string[] args)
        {

            Config = new Menu("ShowTheTime", "ShowTheTime", true);
            Config.AddSubMenu(new Menu("Time", "Time"));
            Config.SubMenu("Time").AddItem(new MenuItem("TimeActivate", "Time").SetValue(new KeyBind(88, KeyBindType.Press)));
            Config.AddToMainMenu();
            Game.OnGameUpdate += Game_OnGameUpdate;
            Game.PrintChat("Time Assembly Loaded Successfuly... // Developed  By <font color='#00FFFF'> MiTeeXxGM </font>");

        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (Config.Item("TimeActivate").GetValue<KeyBind>().Active)
            {

                Game.PrintChat("The Time is : <font color='#00FFFF'>{0}</font>", DateTime.Now.ToShortTimeString());

            }
        }
    }
}
