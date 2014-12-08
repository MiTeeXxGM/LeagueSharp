using System;
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
        static SpeechSynthesizer sSynth = new SpeechSynthesizer();
        static SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        static Choices speechList = new Choices();
        static Grammar gr;
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        static Menu Config;
        public static String time;


        static Timer speechTimer;
        static bool wantSpeech;
        



        static void Main(string[] args)
        {
            Game.PrintChat("Support Me loaded <font color='#00FFFF'>#MiTeeXxGM</font>");
            Config = new Menu("SupportMe", "SPM", true);
            Config.AddSubMenu(new Menu("Options", "Options"));
            Config.SubMenu("Options").AddItem(new MenuItem("TimeActivate", "Show me The Time").SetValue(true));
            Config.SubMenu("Options").AddItem(new MenuItem("SS", "Show me seconds")).SetValue(true);
            Config.SubMenu("Options").AddItem(new MenuItem("MS", "Show me Mana Suggestions")).SetValue(true);
            Config.AddSubMenu(new Menu("Enable", "enable"));
            Config.SubMenu("enable").AddItem(new MenuItem("speech", "enable").SetValue(true));
            Config.SubMenu("enable").AddItem(new MenuItem("speechinterval", "delay").SetValue(new Slider(1000, 250, 5000)));

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;


            speechList.Add(new string[] { "show", "hide" });
            gr = new Grammar(new GrammarBuilder(speechList));






            speechTimer = new Timer(TimerCallBack, null, 0, Config.Item("speechinterval").GetValue<Slider>().Value);

        }

        private static void TimerCallBack(object state)
        {
           
                    sRecognize.RequestRecognizerUpdate();
                    sRecognize.LoadGrammar(gr);
                    sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                    sRecognize.SetInputToDefaultAudioDevice();
                    sRecognize.RecognizeAsync(RecognizeMode.Multiple);
                    sRecognize.Recognize();
               
        }
        static void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "show":
                    doshow();
                    Game.PrintChat("Time Activated" );
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

        private static void doshow()
        {
            Config.Item("TimeActivate").SetValue(true);
        }

        private static void dounshow() 
        {
            Config.Item("TimeActivate").SetValue(!true);           
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

    }
}
