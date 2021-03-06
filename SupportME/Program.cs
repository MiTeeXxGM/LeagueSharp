﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System.Globalization;
using Color = System.Drawing.Color;
namespace SupportME
{
    class Program
    {
        public static SpeechSynthesizer sSynth = new SpeechSynthesizer();
        public static SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        public static Choices speechList = new Choices();
        public static Grammar gr;
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Menu Config;
        public static String time;
        public static Timer speechTimer;
        public static void Main(string[] args)
        {

            Config = new Menu("SupportMe", "SPM", true);
            Config.AddSubMenu(new Menu("Options", "Options"));
            Config.SubMenu("Options").AddItem(new MenuItem("TimeActivate", "Show me The Time").SetValue(true));
            Config.SubMenu("Options").AddItem(new MenuItem("SS", "Show me seconds")).SetValue(true);
            Config.SubMenu("Options").AddItem(new MenuItem("MS", "Show me Mana Suggestions")).SetValue(true);


            Config.AddToMainMenu();



            speechList.Add(new string[] { "show", "hide" });
            gr = new Grammar(new GrammarBuilder(speechList));
            speechTimer = new Timer(TimerCallBack, null, 0, 250);

            Game.PrintChat("Support Me loaded <font color='#00FFFF'>#MiTeeXxGM</font>");
            Drawing.OnDraw += Drawing_OnDraw;
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            #region TIME
            if (Config.Item("TimeActivate").GetValue<bool>())
            {
                if (Config.Item("SS").GetValue<bool>())
                {
                    time = DateTime.Now.ToString("hh:mm:ss tt", new CultureInfo("en-US"));
                }
                else
                {
                    time = DateTime.Now.ToString("hh:mm tt", new CultureInfo("en-US"));
                }
                Drawing.DrawText(Drawing.Width * 0.83f, Drawing.Height * 0.04f, System.Drawing.Color.White, "The Time is {0}", time);
            }
            else
            {
                Drawing.DrawText(Drawing.Width * 0.83f, Drawing.Height * 0.04f, System.Drawing.Color.White, "");
            }
            #endregion



            #region MANA
            if (Config.Item("MS").GetValue<bool>())
            {
                if (Player.Mana == Player.MaxMana)
                {
                    Drawing.DrawText(Drawing.Width * 0.85f, Drawing.Height * 0.08f, System.Drawing.Color.Chartreuse, "Your Champ Has FULL Mana");
                }

                if (Player.Mana < Player.MaxMana && Player.Mana > 200)
                {
                    Drawing.DrawText(Drawing.Width * 0.85f, Drawing.Height * 0.08f, System.Drawing.Color.Chartreuse, "Your Champ Has enough Mana !");
                }
                if (Player.Mana <= 190 && Player.Mana > 120)
                {
                    Drawing.DrawText(Drawing.Width * 0.85f, Drawing.Height * 0.08f, System.Drawing.Color.Yellow, "Your Champ Has Low Mana, Becareful !");
                }
                if (Player.Mana < 120)
                {
                    Drawing.DrawText(Drawing.Width * 0.82f, Drawing.Height * 0.08f, System.Drawing.Color.Red, "LOW MANA !! No way, You Have To RECALL !");
                }
            }
            #endregion

        }

        public static void TimerCallBack(object state)
        {

            sRecognize.RequestRecognizerUpdate();
            sRecognize.LoadGrammar(gr);
            sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
            sRecognize.SetInputToDefaultAudioDevice();
            sRecognize.RecognizeAsync(RecognizeMode.Multiple);
            sRecognize.Recognize();

        }
        public static void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "show":
                    doshow();
                    Game.PrintChat("Time Activated");
                    break;
                case "hide":
                    dounshow();
                    Game.PrintChat("Time DeActivated");
                    break;
                default:
                    Game.PrintChat(e.Result.Text);
                    break;
            }
        }

        public static void doshow()
        {
            Config.Item("TimeActivate").SetValue(true);
        }

        public static void dounshow()
        {
            Config.Item("TimeActivate").SetValue(!true);
        }



    }
}
