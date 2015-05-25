﻿using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Flowers滑板鞋_重生_
{
    class DamageIndicator //by xSalice Modify the By xcsoft
    {
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;

        public static Color Color = Color.Lime;
        public static Color FillColor = Color.Goldenrod;
        public static bool Fill = true;

        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;
        private static Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }
        public static void Initialize(Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
        {
            Enabled = false;
            Drawing.OnDraw += Drawing_OnDraw;
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (Fill)
                {
                    float differenceInHP = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (int i = 0; i < differenceInHP; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
            foreach (var unit in HeroManager.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
            {
                var damage = damageToUnit(unit);

                if (damage <= 0)
                    continue;

                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                var currentHealthPercentage = unit.Health / unit.MaxHealth;

            }
        }
    }
}