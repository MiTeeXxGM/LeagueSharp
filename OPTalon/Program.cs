﻿#region
using System;
using System.Collections;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Collections.Generic;
using System.Threading;
#endregion

namespace OPTalon
{


    internal class program
    {

        private const string Champion = "Talon";
        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q;
        private static Spell W;
        private static Spell E;
        private static Spell R;
        private static List<Spell> SpellList = new List<Spell>();
        private static Menu Config;
        private static Items.Item _tmt, _rah, _gbd;
        private static SpellSlot _igniteSlot;

        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }


        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;

        }


        private static void Game_OnGameLoad(EventArgs args)
        {

            if (Player.ChampionName != Champion) return;
            Q = new Spell(SpellSlot.Q, 250f);
            W = new Spell(SpellSlot.W, 600f);
            E = new Spell(SpellSlot.E, 700f);
            R = new Spell(SpellSlot.R, 500f);
            W.SetSkillshot(5f, 0f, 902f, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(5f, 650f, 650f, false, SkillshotType.SkillshotCircle);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);


            _igniteSlot = ObjectManager.Player.GetSpellSlot("SummonerDot");
            _tmt = new Items.Item(3077, 400f);
            _rah = new Items.Item(3074, 400f);
            _gbd = new Items.Item(3142, 0f);



            Config = new Menu("OPTalon", "OPT", true);


            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            //Combo Menu
            Config.AddSubMenu(new Menu("OPT-Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use Q")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWCombo", "Use W")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseECombo", "Use E")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRCombo", "Use R")).SetValue(true);
            Config.SubMenu("combo").AddItem(new MenuItem("MinR", "Min R Targets ?").SetValue(new Slider(1, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("Combo").AddItem(new MenuItem("ActiveCombo", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            //Harras Menu
            Config.AddSubMenu(new Menu("Harras", "harras"));
            Config.SubMenu("harras").AddItem(new MenuItem("QonPlayer", "Use Q").SetValue(true));
            Config.SubMenu("harras").AddItem(new MenuItem("WonPlayer", "Use W").SetValue(true));
            Config.SubMenu("harras").AddItem(new MenuItem("UseItems", "Use Items")).SetValue(true);
            Config.SubMenu("harras").AddItem(new MenuItem("EonPlayer", "Use E").SetValue(false));
            Config.SubMenu("harras").AddItem(new MenuItem("ManatoHarras", "> Mana Percent to UseSKill").SetValue(new Slider(30, 0, 100)));
            //LANECLEAR Menu
            Config.AddSubMenu(new Menu("Lane Clear", "laneclear"));
            Config.SubMenu("laneclear").AddItem(new MenuItem("QonCreep", "use Q").SetValue(true));
            Config.SubMenu("laneclear").AddItem(new MenuItem("WonCreep", "use W").SetValue(true));
            Config.SubMenu("laneclear").AddItem(new MenuItem("ManatoCreep", "> Mana Percent to LaneClear").SetValue(new Slider(30, 0, 100)));
            //Drawings Menu
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawEnable", "Enable Drawing"));            
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawW", "Draw W")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawE", "Draw E")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("DrawR", "Draw R")).SetValue(true);
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleQuality", "Circles Quality").SetValue(new Slider(100, 100, 10)));
            Config.SubMenu("Drawings").AddItem(new MenuItem("CircleThickness", "Circles Thickness").SetValue(new Slider(1, 10, 1)));

            Config.AddToMainMenu();

            Game.OnGameUpdate += OnGameUpdate;
            //Drawing.OnDraw += OnDraw;
            Game.PrintChat("Talon Assambly Loaded Successfully !!!!!");
        }

        private static void OnGameUpdate(EventArgs args)
        {
            if (Config.SubMenu("Combo").Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode.ToString() == "Mixed")
            {
                Mixed();
            }

           
        }

        private static void Combo()
        {


            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;


            if (E.IsReady() && (Config.SubMenu("Combo").Item("UseECombo").GetValue<bool>()))
            {
                E.CastOnUnit(target, false);
            }
            if (Config.SubMenu("Combo").Item("UseItems").GetValue<bool>() && _tmt.IsReady() && _rah.IsReady() && _gbd.IsReady())
            {
                if (Player.Distance(target) <= _gbd.Range)
                {
                    _gbd.Cast();
                }
                if (Player.Distance(target) <= _tmt.Range)
                {
                    _tmt.Cast();
                }
                if (Player.Distance(target) <= _rah.Range)
                {
                    _rah.Cast();
                }

            }
            if (Config.SubMenu("Combo").Item("UseRCombo").GetValue<bool>() && R.IsReady() && ObjectManager.Get<Obj_AI_Hero>().Count(hero => hero.IsValidTarget(R.Range)) >= Config.Item("MinR").GetValue<Slider>().Value)
            {
                R.CastOnUnit(target, false);
            }
        }

        private static void Mixed()
        {
            if (Player.Mana / Player.MaxMana * 100 < Config.Item("ManatoHarras").GetValue<Slider>().Value) return;

            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
            if (Config.SubMenu("harras").Item("WonPlayer").GetValue<bool>() && W.IsReady())
            {
                W.CastOnUnit(target, false);
            }
            if (Config.SubMenu("harras").Item("EonPlayer").GetValue<bool>() && E.IsReady())
            {
                E.Cast(target);
            }
            if (Config.SubMenu("harras").Item("QonPlayer").GetValue<bool>() && Q.IsReady())
            {
                Q.Cast(target);
            }
            if (Config.SubMenu("harras").Item("useItems").GetValue<bool>())
            {
                if (_tmt.IsReady())
                {
                    _tmt.Cast(target);
                }
                if (_rah.IsReady())
                {
                    _rah.Cast(target);
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {

            if (Config.SubMenu("Drawnings").Item("DrawEnable").GetValue<bool>())
            {
                if (Config.SubMenu("Drawnings").Item("DrawW").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, W.Range, Color.Blue,
                    Config.Item("CircleThickness").GetValue<Slider>().Value,
                    Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (Config.SubMenu("Drawnings").Item("DrawE").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, E.Range, Color.White,
                    Config.Item("CircleThickness").GetValue<Slider>().Value,
                    Config.Item("CircleQuality").GetValue<Slider>().Value);
                }
                if (Config.SubMenu("Drawnings").Item("DrawR").GetValue<bool>())
                {
                    Utility.DrawCircle(Player.Position, R.Range, Color.Red,
                    Config.Item("CircleThickness").GetValue<Slider>().Value,
                    Config.Item("CircleQuality").GetValue<Slider>().Value);
                }

            }

        }

        }


}