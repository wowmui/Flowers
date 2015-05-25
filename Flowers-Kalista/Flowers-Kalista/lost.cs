﻿/*
 * Thx
 * xcsoft-Sharpshooter
 * Hellsing-Kalista
 * OKTW-AAStyle
 * jQuery-ELKalista
 * Wiezerzz-Balista
 */
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using Item = LeagueSharp.Common.Items.Item;

namespace Flowers滑板鞋_重生_
{
    internal class lost
    {
        static Spell Q;
        static Spell W;
        static Spell E;
        static Spell R;
        internal static float getManaPer 
        { 
            get { return Player.Mana / Player.MaxMana * 100; } 
        }
        private static Obj_AI_Hero Player 
        { 
            get { return ObjectManager.Player; } 
        }
        private static float Hp百分比(Obj_AI_Hero Player)
        {
            return Player.Health * 100 / Player.MaxHealth;
        }
        private int InitTime 
        { 
            get; set;
        }
        private bool IsJumpPossible 
        { 
            get; set; 
        }
        private Vector3 FleePosition 
        { 
            get; set; 
        }
        public const string ChampionName = "Kalista";
        public static Orbwalking.Orbwalker Orbwalker;
        public static readonly Item 弯刀 = ItemData.Bilgewater_Cutlass.GetItem();
        public static readonly Item 破败 = ItemData.Blade_of_the_Ruined_King.GetItem();

        public static readonly Item 幽梦 = ItemData.Youmuus_Ghostblade.GetItem();
        public static void Game_OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != ChampionName)
            {
                return;
            }

            Notifications.AddNotification("Flowers Kalista by NightMoon", 1000);
            Notifications.AddNotification("`                  And  Lost`", 1000);
            Notifications.AddNotification("Version : 1.0.0.0", 1000);
            Game.PrintChat("Flowers-Kalista Loaded!~~~ Version : 1.0.0.0 Thanks for your use!");
            Game.PrintChat("What are the deficiencies please immediately feedback. Thank you! ");
            Q = new Spell(SpellSlot.Q, 1160f);
            W = new Spell(SpellSlot.W, 5000f);
            E = new Spell(SpellSlot.E, 1000f);
            R = new Spell(SpellSlot.R, 1500f);

            Q.SetSkillshot(0.25f, 35f, 1600f, true, SkillshotType.SkillshotLine);
//            R.SetSkillshot(0.50f, 1500, float.MaxValue, false, SkillshotType.SkillshotCircle);

            KalistaM.KalistaMenu();

            Game.OnUpdate += 总菜单;
            Drawing.OnDraw += 范围显示;
            DamageIndicator.DamageToUnit = 显示E伤害;
        }

        private static float 显示E伤害(Obj_AI_Hero hero)
        {
            float damage = 0;

            if (E.IsReady() && KalistaM.菜单.Item("DrawEDamage").GetValue<bool>())
                damage += E.GetDamage(hero); 
            Color.FromArgb(255, 255, 255);

            return damage;
        }

        private static void 范围显示(EventArgs args)
        {
           if (Player.IsDead)
           {
               return;
           }

           var AA范围OKTWStyle = KalistaM.菜单.Item("drawingAA").GetValue<bool>();
           var AA目标OKTWStyle = KalistaM.菜单.Item("orb").GetValue<bool>();
           var Q范围 = KalistaM.菜单.Item("drawingQ").GetValue<Circle>();
           var W范围 = KalistaM.菜单.Item("drawingW").GetValue<Circle>();
           var E范围 = KalistaM.菜单.Item("drawingE").GetValue<Circle>();
           var R范围 = KalistaM.菜单.Item("drawingR").GetValue<Circle>();
           var 补刀小兵 = KalistaM.菜单.Item("bdxb").GetValue<Circle>();
           var 附近可击杀 = KalistaM.菜单.Item("fjkjs").GetValue<Circle>();

           if (AA范围OKTWStyle)
           {
               if (Hp百分比(Player) > 60)
                   Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), System.Drawing.Color.GreenYellow, 2);
               else if (Hp百分比(Player) > 30)
                   Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), System.Drawing.Color.Orange, 3);
               else
                   Render.Circle.DrawCircle(Player.Position, Orbwalking.GetRealAutoAttackRange(Player), System.Drawing.Color.Red, 4);
           }
           if (AA目标OKTWStyle)
           {
               var orbT = Orbwalker.GetTarget();

               if (orbT.IsValidTarget())
               {
                   if (orbT.Health > orbT.MaxHealth * 0.6)
                       Render.Circle.DrawCircle(orbT.Position, orbT.BoundingRadius + 15, System.Drawing.Color.GreenYellow, 5);
                   else if (orbT.Health > orbT.MaxHealth * 0.3)
                       Render.Circle.DrawCircle(orbT.Position, orbT.BoundingRadius + 15, System.Drawing.Color.Orange, 5);
                   else
                       Render.Circle.DrawCircle(orbT.Position, orbT.BoundingRadius + 15, System.Drawing.Color.Red, 5);
               }
           }
           if (Q.IsReady() && Q范围.Active)
               Render.Circle.DrawCircle(Player.Position, Q.Range, Q范围.Color);

           if (W.IsReady() && W范围.Active)
               Render.Circle.DrawCircle(Player.Position, W.Range, W范围.Color);

           if (E.IsReady() && E范围.Active)
               Render.Circle.DrawCircle(Player.Position, E.Range - 30, E范围.Color);

           if (R.IsReady() && R范围.Active)
               Render.Circle.DrawCircle(Player.Position, R.Range + 50, R范围.Color);

           if (补刀小兵.Active || 附近可击杀.Active)
           {
               var xMinions = MinionManager.GetMinions(Player.Position, Player.AttackRange + Player.BoundingRadius + 300, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

               foreach (var xMinion in xMinions)
               {
                   if (补刀小兵.Active && Player.GetAutoAttackDamage(xMinion) >= xMinion.Health)
                       Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, 补刀小兵.Color, 5);
                   else if (附近可击杀.Active && Player.GetAutoAttackDamage(xMinion) * 2 >= xMinion.Health)
                       Render.Circle.DrawCircle(xMinion.Position, xMinion.BoundingRadius, 附近可击杀.Color, 5);
               }
           }

           if (Game.MapId == (GameMapId)11 && KalistaM.菜单.Item("wushangdaye").GetValue<bool>())
           {
               const float circleRange = 100f;

               Render.Circle.DrawCircle(new Vector3(7461.018f, 3253.575f, 52.57141f), circleRange, System.Drawing.Color.Orange, 3); // blue team :red
               Render.Circle.DrawCircle(new Vector3(3511.601f, 8745.617f, 52.57141f), circleRange, System.Drawing.Color.Orange, 3); // blue team :blue
               Render.Circle.DrawCircle(new Vector3(7462.053f, 2489.813f, 52.57141f), circleRange, System.Drawing.Color.Orange, 3); // blue team :golems
               Render.Circle.DrawCircle(new Vector3(3144.897f, 7106.449f, 51.89026f), circleRange, System.Drawing.Color.Orange, 3); // blue team :wolfs
               Render.Circle.DrawCircle(new Vector3(7770.341f, 5061.238f, 49.26587f), circleRange, System.Drawing.Color.Orange, 3); // blue team :wariaths
               Render.Circle.DrawCircle(new Vector3(10930.93f, 5405.83f, -68.72192f), circleRange, System.Drawing.Color.Red, 3); // Dragon
               Render.Circle.DrawCircle(new Vector3(7326.056f, 11643.01f, 50.21985f), circleRange, System.Drawing.Color.Orange, 3); // red team :red
               Render.Circle.DrawCircle(new Vector3(11417.6f, 6216.028f, 51.00244f), circleRange, System.Drawing.Color.Orange, 3); // red team :blue
               Render.Circle.DrawCircle(new Vector3(7368.408f, 12488.37f, 56.47668f), circleRange, System.Drawing.Color.Orange, 3); // red team :golems
               Render.Circle.DrawCircle(new Vector3(10342.77f, 8896.083f, 51.72742f), circleRange, System.Drawing.Color.Orange, 3); // red team :wolfs
               Render.Circle.DrawCircle(new Vector3(7001.741f, 9915.717f, 54.02466f), circleRange, System.Drawing.Color.Orange, 3); // red team :wariaths                    
           }

            //balista biubiubiu fuckyou man 
       /*    if (KalistaM.菜单.Item("minBRange", true).GetValue<Circle>().Active)
               Render.Circle.DrawCircle(Player.Position, KalistaM.菜单.Item("minRange", true).GetValue<Slider>().Value, KalistaM.菜单.Item("minBRange", true).GetValue<Circle>().Color, 3);
           if (KalistaM.菜单.Item("maxBRange", true).GetValue<Circle>().Active)
               Render.Circle.DrawCircle(Player.Position, KalistaM.菜单.Item("maxRange", true).GetValue<Slider>().Value, KalistaM.菜单.Item("maxBRange", true).GetValue<Circle>().Color, 3);*/
        }

/*       private static void 物品使用()//引用  JustMaokai THX
        {

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null || !target.IsValidTarget())
                return;

            var botrk = ItemData.Blade_of_the_Ruined_King.GetItem();
            var Ghost = ItemData.Youmuus_Ghostblade.GetItem();
            var cutlass = ItemData.Bilgewater_Cutlass.GetItem();

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target) && target.HealthPercent <= KalistaM.菜单.Item("eL").GetValue<Slider>().Value && KalistaM.菜单.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (botrk.IsReady() && botrk.IsOwned(Player) && botrk.IsInRange(target) && target.HealthPercent <= KalistaM.菜单.Item("oL").GetValue<Slider>().Value && KalistaM.菜单.Item("UseBOTRK").GetValue<bool>())

                botrk.Cast(target);

            if (cutlass.IsReady() && cutlass.IsOwned(Player) && cutlass.IsInRange(target) && target.HealthPercent <= KalistaM.菜单.Item("HLe").GetValue<Slider>().Value && KalistaM.菜单.Item("UseBilge").GetValue<bool>())

                cutlass.Cast(target);

            if (Ghost.IsReady() && Ghost.IsOwned(Player) && target.IsValidTarget(E.Range) && KalistaM.菜单.Item("useGhostblade").GetValue<bool>())

                Ghost.Cast();

        }*/
        public static bool UseBotrk(Obj_AI_Hero target)
        {
            var 使用弯刀 = KalistaM.菜单.Item("UseBC").GetValue<bool>();
            var 使用破败 = KalistaM.菜单.Item("UseBRK").GetValue<bool>();

            if (使用破败 && 破败.IsReady() && target.IsValidTarget(破败.Range) &&
                Player.Health + Player.GetItemDamage(target, Damage.DamageItems.Botrk) < Player.MaxHealth)
            {
                return 破败.Cast(target);
            }
            else if (使用弯刀 && 弯刀.IsReady() && target.IsValidTarget(弯刀.Range))
            {
                return 弯刀.Cast(target);
            }
            return false;
        }

        public static bool UseYoumuu(Obj_AI_Base target)
        {
            var 使用幽梦 = KalistaM.菜单.Item("USEYUU").GetValue<bool>();

            if (使用幽梦 && 幽梦.IsReady() && target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player) + 50))
            {
                return 幽梦.Cast();
            }
            return false;
        }
        private static void 总菜单(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    连招();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    骚扰();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    清线();
                    清野();
                    break;
            }

            if (KalistaM.菜单.Item("AutoQ").GetValue<KeyBind>().Active)
            {
                骚扰();
            }

            连招E击杀();
            E抢野怪();
            自动E();
           /* Balista();*/
            逃跑();
        }

        private static void 连招()
        {
            if (!(getManaPer > KalistaM.菜单.Item("lzmp").GetValue<Slider>().Value))
                return;

            if (KalistaM.菜单.Item("lzp").GetValue<bool>())
            {
                var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);

                if (Q.CanCast(Qtarget) && Q.GetPrediction(Qtarget).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())
                    Q.Cast(Qtarget);
            }

            var Minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy).Where(x => x.Health <= E.GetDamage(x)).OrderBy(x => x.Health).FirstOrDefault();
            var Target = HeroManager.Enemies.Where(x => E.CanCast(x) && E.GetDamage(x) >= 1 && !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield)).OrderByDescending(x => E.GetDamage(x)).FirstOrDefault();
            var target = Target;
            if (KalistaM.菜单.Item("lze").GetValue<bool>() && (E.Instance.State == SpellState.Ready || E.Instance.State == SpellState.Surpressed) && target.HasBuffOfType(BuffType.SpellShield))
            {
                if (Player.Distance(target, true) > Math.Pow(Orbwalking.GetRealAutoAttackRange(target), 2))
                {
                    var minions = ObjectManager.Get<Obj_AI_Minion>().Where(m => m.IsValidTarget(Orbwalking.GetRealAutoAttackRange(m)));
                    if (minions.Any(m => m.IsRendKillable()))
                    {
                        E.Cast(true);
                    }
                    else
                    {
                        var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy).Find(m => m.Health > Player.GetAutoAttackDamage(m) && m.Health < Player.GetAutoAttackDamage(m) + E.GetDamage(m, (m.HasBuffOfType(BuffType.SpellShield) ? m.GetRendBuff().Count + 1 : 1)));
                        if (minion != null)
                        {
                            Orbwalker.ForceTarget(minion);
                        }
                    }
                }
                else if (E.IsInRange(target))
                {
                    if (target.IsRendKillable())
                    {
                        E.Cast(true);
                    }
                    else if (target.GetRendBuff().Count >= KalistaM.菜单.Item("lzeeeeee").GetValue<Slider>().Value)
                    {
                        if (target.ServerPosition.Distance(Player.ServerPosition, true) > Math.Pow(E.Range * 0.8, 2) ||
                            target.GetRendBuff().EndTime - Game.Time < 0.3)
                        {
                            E.Cast(true);
                        }
                    }
                }
            }
            if (Target.Health <= E.GetDamage(Target) || (E.CanCast(Minion) && E.CanCast(Target)))
                E.Cast();
        }

        private static void 骚扰()
        {
            if (!(getManaPer > KalistaM.菜单.Item("srmp").GetValue<Slider>().Value))
                return;

            if (KalistaM.菜单.Item("srq").GetValue<bool>())
            {
                var Qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical, true);

                if (Q.CanCast(Qtarget) && Q.GetPrediction(Qtarget).Hitchance >= HitChance.VeryHigh && !Player.IsWindingUp && !Player.IsDashing())
                    Q.Cast(Qtarget);
            }
        }
        private static void 清野()
        {
            if (!Orbwalking.CanMove(1) || !(getManaPer > KalistaM.菜单.Item("qxmp").GetValue<Slider>().Value))
                return;

            var Mobs = MinionManager.GetMinions(Player.ServerPosition, Orbwalking.GetRealAutoAttackRange(Player) + 100, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (Mobs.Count <= 0)
                return;

            if (KalistaM.菜单.Item("qyq").GetValue<bool>() && Q.CanCast(Mobs[0]))
                Q.Cast(Mobs[0]);

            if (KalistaM.菜单.Item("qye").GetValue<bool>() && E.CanCast(Mobs[0]))
            {
                if (Mobs[0].Health + (Mobs[0].HPRegenRate / 2) <= E.GetDamage(Mobs[0]))
                    E.Cast();
            }
        }

        private static void 清线()
        {

            if (!Orbwalking.CanMove(1) || !(getManaPer > KalistaM.菜单.Item("qxmp").GetValue<Slider>().Value))
                return;

            var Minions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy);

            if (Minions.Count <= 0)
                return;

            if (KalistaM.菜单.Item("qxq").GetValue<bool>() && Q.IsReady())
            {
                foreach (var minion in Minions.Where(x => x.Health <= Q.GetDamage(x)))
                {
                    var killcount = 0;

                    foreach (var colminion in Q_GetCollisionMinions(Player, Player.ServerPosition.Extend(minion.ServerPosition, Q.Range)))
                    {
                        if (colminion.Health <= Q.GetDamage(colminion))
                            killcount++;
                        else
                            break;
                    }

                    if (killcount >= KalistaM.菜单.Item("qxqqq").GetValue<Slider>().Value)
                    {
                        if (!Player.IsWindingUp && !Player.IsDashing())
                        {
                            Q.Cast(minion);
                            break;
                        }
                    }
                    if (KalistaM.菜单.Item("qxe").GetValue<bool>() && E.IsReady())
                    {
                        var minionkillcount = 0;
                        foreach (var Minion in Minions.Where(x => E.CanCast(x) && x.Health <= E.GetDamage(x))) { minionkillcount++; }
                        if (minionkillcount >= KalistaM.菜单.Item("qxeee").GetValue<Slider>().Value)
                            E.Cast();
                    }
                }
            }
        }

        private static List<Obj_AI_Base> Q_GetCollisionMinions(Obj_AI_Hero source, Vector3 targetposition)
        {
            var input = new PredictionInput
            {
                Unit = source,
                Radius = Q.Width,
                Delay = Q.Delay,
                Speed = Q.Speed,
            };

            input.CollisionObjects[0] = CollisionableObjects.Minions;

            return LeagueSharp.Common.Collision.GetCollision(new List<Vector3> { targetposition }, input).OrderBy(obj => obj.Distance(source, false)).ToList();
        }

        private static void 连招E击杀()
        {
            if (!KalistaM.菜单.Item("lzeee").GetValue<bool>() || !E.IsReady())
            {
                return;
            }

            var target = HeroManager.Enemies.FirstOrDefault(x => !x.HasBuffOfType(BuffType.Invulnerability) && !x.HasBuffOfType(BuffType.SpellShield) && E.CanCast(x) && (x.Health + (x.HPRegenRate / 2)) <= E.GetDamage(x));

            if (E.CanCast(target))
            {
                E.Cast();
            }
        }

        private static void E抢野怪()
        {
            if (!KalistaM.菜单.Item("eqiangyeguai").GetValue<bool>() || !E.IsReady())
            {
                return;
            }

            var Mob = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.Health + (x.HPRegenRate / 2) <= E.GetDamage(x));

            if (E.CanCast(Mob))
            {
                E.Cast();
            }

            var Minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth).FirstOrDefault(x => x.Health <= E.GetDamage(x) && (x.SkinName.ToLower().Contains("siege") || x.SkinName.ToLower().Contains("super")));

            if (E.CanCast(Minion))
            {
                E.Cast();
            }
        }

        private static void 自动E()
        {
            var 敌人 = HeroManager.Enemies.Where(o => o.HasBuffOfType(BuffType.SpellShield)).OrderBy(o => o.Distance(Player, true)).FirstOrDefault();

            if (!KalistaM.菜单.Item("DamageExxx").GetValue<bool>() || !E.IsReady())
            {
                if (敌人 != null)
                {
                    if (敌人.Distance(Player, true) < Math.Pow(E.Range + 200, 2))
                    {
                        if (ObjectManager.Get<Obj_AI_Minion>().Any(o => o.IsRendKillable() && E.IsInRange(o)))
                        {
                            E.Cast();
                        }
                        return;
                    }
                }
            }
        }

    /*    private static void Balista()
        {

            if (Player.IsDead || !KalistaM.菜单.Item("useToggle").GetValue<bool>() &&
                !KalistaM.菜单.Item("useOnComboKey").GetValue<KeyBind>().Active || !R.IsReady()) return;

            var blitzfriend =
                ObjectManager.Get<Obj_AI_Hero>()
                    .SingleOrDefault(
                        x =>
                            x.IsAlly && Player.Distance(x.ServerPosition) < KalistaM.菜单.Item("maxRange", true).GetValue<Slider>().Value &&
                            Player.Distance(x.ServerPosition) >= KalistaM.菜单.Item("minRange", true).GetValue<Slider>().Value &&
                            x.ChampionName == "Blitzcrank");

            if (blitzfriend == null)
                return;


            foreach (Obj_AI_Hero enem in ObjectManager.Get<Obj_AI_Hero>().Where(enem => enem.IsValid && enem.IsEnemy && enem.Distance(Player,true) <= 2450f)) //+950f is blitz Q range.
            {
                if (KalistaM.菜单.Item("target" + enem.ChampionName).GetValue<bool>() && enem.Health > 200)
                {
                    if (enem.Buffs != null)
                    {
                        for (int i = 0; i < enem.Buffs.Count(); i++)
                        {
                            if (enem.Buffs[i].Name == "rocketgrab2" && enem.Buffs[i].IsActive)
                            {
                                if (R.IsReady())
                                {
                                    R.Cast(KalistaM.菜单.Item("usePackets").GetValue<bool>());
                                }
                            }
                        }
                    }
                }
            }
        }*/
        private static void 逃跑()
        {
            var 逃跑 = KalistaM.菜单.Item("Flee").GetValue<KeyBind>().Active;
            var Target = Vector3.Zero;
            var FleePosition = Vector3.Zero;
            var InitTime = 0;

            if (Target != Vector3.Zero)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Target);

                if (Environment.TickCount - InitTime > 500)
                {
                    Target = Vector3.Zero;
                    InitTime = 0;
                }
                else
                {
                    return;
                }
            }

            if (逃跑)
            {
                var dashObjects = Flee.GetDashObjects();
                Orbwalking.Orbwalk(dashObjects.Count > 0 ? dashObjects[0] : null, Game.CursorPos);
            }
            if (逃跑)
            {
                var wallCheck = Flee.GetFirstWallPoint(Player.Position, Game.CursorPos);

                if (wallCheck != null)
                {
                    wallCheck = Flee.GetFirstWallPoint((Vector3)wallCheck, Game.CursorPos, 5);
                }

                Vector3 movePosition = wallCheck != null ? (Vector3)wallCheck : Game.CursorPos;

                var tempGrid = NavMesh.WorldToGrid(movePosition.X, movePosition.Y);
                FleePosition = NavMesh.GridToWorld((short)tempGrid.X, (short)tempGrid.Y);

                Obj_AI_Base target = null;

                if (逃跑)
                {
                    var dashObjects = Flee.GetDashObjects();
                    if (dashObjects.Count > 0)
                    {
                        target = dashObjects[0];
                    }
                }

                if (Q.IsReady() && wallCheck != null)
                {
                    Vector3 wallPosition = movePosition;

                    Vector2 direction = (Game.CursorPos.To2D() - wallPosition.To2D()).Normalized();
                    float maxAngle = 80;
                    float step = maxAngle / 20;
                    float currentAngle = 0;
                    float currentStep = 0;
                    bool jumpTriggered = false;
                    while (true)
                    {
                        if (currentStep > maxAngle && currentAngle < 0)
                        {
                            break;
                        }

                        if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                        {
                            currentAngle = (currentStep) * (float)Math.PI / 180;
                            currentStep += step;
                        }
                        else if (currentAngle > 0)
                        {
                            currentAngle = -currentAngle;
                        }

                        Vector3 checkPoint;

                        if (currentStep == 0)
                        {
                            currentStep = step;
                            checkPoint = wallPosition + 300 * direction.To3D();
                        }
                        else
                        {
                            checkPoint = wallPosition + 300 * direction.Rotated(currentAngle).To3D();
                        }

                        if (!checkPoint.IsWall())
                        {
                            wallCheck = Flee.GetFirstWallPoint(checkPoint, wallPosition);
                            if (wallCheck != null)
                            {
                                Vector3 wallPositionOpposite = (Vector3)Flee.GetFirstWallPoint((Vector3)wallCheck, wallPosition, 5);

                                if (Player.GetPath(wallPositionOpposite).ToList().To2D().PathLength() - Player.Distance(wallPositionOpposite) > 200)
                                {
                                    if (Player.Distance(wallPositionOpposite, true) < Math.Pow(300 - Player.BoundingRadius / 2, 2))
                                    {
                                        InitTime = Environment.TickCount;
                                        Target = wallPositionOpposite;
                                        Q.Cast(wallPositionOpposite);
                                        jumpTriggered = true;
                                        break;
                                    }
                                    else
                                    {

                                    }
                                }
                            }
                        }
                    }

                    if (!jumpTriggered)
                    {
                        Orbwalking.Orbwalk(target, movePosition, 90f, 0f, false, false);
                    }
                }
                else
                {
                    Orbwalking.Orbwalk(target, movePosition, 90f, 0f, false, false);
                }
            }
        }
    }
}
