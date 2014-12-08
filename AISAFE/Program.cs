using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using AISAFE;

namespace AISAFE
{
   
    class Program
    {
        
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }

        static void Main(string[] args)
        {
           Drawing.OnDraw += Drawing_OnDraw;

            
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawText(Drawing.Width * 0.85f, Drawing.Height * 0.04f, System.Drawing.Color.White, "The Time is {0}", DateTime.Now.ToShortTimeString());  
            
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

       
    }
}
