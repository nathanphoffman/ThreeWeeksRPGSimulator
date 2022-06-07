using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWeeksSimulator
{
    internal class Creature
    {
        public Creature(int hearts, int armor, int intelligence, int strength, int charisma, int dexterity)
        {
            this.hearts = hearts;
            this.armor = armor;
            this.intelligence = intelligence;
            this.strength = strength;
            this.charisma = charisma;
            this.dexterity = dexterity;
            this.attacks = new List<Attack>();
            SetHitpoints();
        }

        public int currentGroup;
        public int hearts;
        public int hitpoints;
        public int armor;
        public int intelligence;
        public int strength;
        public int charisma;
        public int dexterity;
        public List<Attack> attacks;

        public Creature SetCurrentGroup(int numGroups)
        {
            currentGroup = Dice.Random(numGroups);
            return this;
        }

        public Creature AddNWAttack(int damage = 0, int numAttacks = 1)
        {
            attacks.Add(new Attack { damage = damage, numAttacks = numAttacks, weaponType = Dice.enWeaponTypes.NW });
            return this;
        }

        public Creature AddOHAttack(int damage = 0, int numAttacks = 1)
        {
            attacks.Add(new Attack { damage = damage, numAttacks = numAttacks, weaponType = Dice.enWeaponTypes.OH });
            return this;
        }

        public Creature AddTHAttack(int damage = 0, int numAttacks = 1)
        {
            attacks.Add(new Attack { damage = damage, numAttacks = numAttacks, weaponType = Dice.enWeaponTypes.TH });
            return this;
        }

        private void SetHitpoints()
        {
            hitpoints = Dice.Roll2D6() * hearts;
        }

        public void LogCreatureStats()
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine($"Armor:{armor} Hearts:{hearts} Hitpoints:{hitpoints}");
            Console.WriteLine($"Stats: STR:{strength} INT:{intelligence} CHA:{charisma} DEX:{dexterity}");
            Console.WriteLine($"Creature is at position {currentGroup}");
            attacks.ForEach(attack => Console.WriteLine($"Attack:{attack.numAttacks}x{attack.weaponType.ToString()}+{attack.damage}"));
            Console.WriteLine("------------------------------");
        }

        public List<Attack> GetAttacksByType(Dice.enWeaponTypes weaponType, bool ranged)
        {
            return attacks.Where(x => x.weaponType == weaponType && x.ranged == ranged).ToList();
        }

        public Attack SelectAttack(bool mobile, bool ranged = false)
        {
            // two handed attacks always take priority since they deal more damage on average
            var THAttacks = GetAttacksByType(Dice.enWeaponTypes.TH, ranged);
            if (attacks.Count > 0 && !mobile) return Dice.Choose(THAttacks);

            var OHAttacks = GetAttacksByType(Dice.enWeaponTypes.OH, ranged);
            if (OHAttacks.Count > 0) return Dice.Choose(OHAttacks);

            var NWAttacks = GetAttacksByType(Dice.enWeaponTypes.NW, ranged);
            if (NWAttacks.Count > 0) return Dice.Choose(NWAttacks);

            // all creatures have a basic melee natural weapon, so if none are found return it
            if (!ranged) return new Attack { damage = 0, numAttacks = 1, ranged = false, weaponType = Dice.enWeaponTypes.NW };

            // if no weapon is found (asking for ranged but no ranged matches)
            return null;

        }

    }

    internal class Attack
    {
        public int damage;
        public int numAttacks;
        public bool ranged;
        public Dice.enWeaponTypes weaponType;
        public string MakeAttack(Creature attacker, Creature target)
        {
            int checkModifier = 0;
            if (weaponType == Dice.enWeaponTypes.NW || weaponType == Dice.enWeaponTypes.TH) checkModifier = attacker.strength;
            if (weaponType == Dice.enWeaponTypes.OH) checkModifier = attacker.dexterity;
            (int result, bool success) = Dice.MakeAttack(target.armor, weaponType, checkModifier, damage);
            if (success)
            {
                target.hitpoints -= result;
                return $"Creature dealt {result} damage.";
            }

            return "Creature swung and missed";
        }

    }

}
