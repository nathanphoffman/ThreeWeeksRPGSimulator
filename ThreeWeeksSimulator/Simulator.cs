using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWeeksSimulator
{
    internal static class Simulator
    {

        public static void LogGroupPosition(int group, params List<Creature>[] teams)
        {
            var creaturesInGroup = teams.ToList().SelectMany(y => y.Where(a => a.currentGroup == group).Select(z => z.name)).ToList();
            if (creaturesInGroup.Count > 0) Console.WriteLine($"Group {group}: " + String.Join(',', creaturesInGroup));
            else Console.WriteLine($"Group {group}: none");
        }

        public static void LogGroupPositions(int numberofGroups, params List<Creature>[] teams)
        {
            Console.WriteLine("--------------------------");
            for (int i = 1; i <= numberofGroups; i++) LogGroupPosition(i, teams);
            Console.WriteLine("--------------------------");
        }

        public static void RunStatistics()
        {
            int[] results = new int[3];

            int i = 0;
            while (i++ < 100)
            {
                int result = Run(3);
                results[result] += 1;
            }

            Console.WriteLine($"Team1 wins: {results[1]} vs Team2 wins: {results[2]}");
        }

        public static int Run(int numberOfGroups = 3)
        {
            var creature1Ranged = new Creature("GoblinRanged", 3, 7, 1, 1, 1, 2).AddTHAttack(2, 1, true).SetCurrentGroup(numberOfGroups).AddDefaultWeapon();
            var creature1 = new Creature("Goblin", 3, 7, 1, 1, 1, 2).AddTHAttack().SetCurrentGroup(numberOfGroups).AddDefaultWeapon();
            var creature2Ranged = new Creature("OrcRanged", 3, 10, 1, 1, 1, 2).AddOHAttack(0, 1, true).SetCurrentGroup(numberOfGroups).AddDefaultWeapon();
            var creature2 = new Creature("Orc", 3, 10, 1, 1, 1, 2).AddOHAttack().SetCurrentGroup(numberOfGroups).AddDefaultWeapon();

            creature1.LogCreatureStats();
            creature1Ranged.LogCreatureStats();
            creature2.LogCreatureStats();
            creature2Ranged.LogCreatureStats();

            var team1 = new List<Creature> { creature1, creature1Ranged };
            var team2 = new List<Creature> { creature2, creature2Ranged };

            int stopper = 0;
            while (team1.Count > 0 && team2.Count > 0 && ++stopper < 100)
            {
                team1.ForEach(creature =>
                {
                    LogGroupPositions(numberOfGroups, team1, team2);
                    if(creature != null) Console.WriteLine(ProcessTurn(creature, team2, numberOfGroups));
                    RemoveDeadCreatures(team2);
                });
                team2.ForEach(creature =>
                {
                    LogGroupPositions(numberOfGroups, team1, team2);
                    if (creature != null) Console.WriteLine(ProcessTurn(creature, team1, numberOfGroups));
                    RemoveDeadCreatures(team1);
                });
            }

            Console.WriteLine("Battle has ended");
            if (team1.Count > 0) return 1;
            if (team2.Count > 0) return 2;
            else return 0;

        }

        public static void RemoveDeadCreatures(params List<Creature>[] teams)
        {
            foreach (var team in teams)
            {
                var removeCreatures = team.Where(creature => creature.hitpoints <= 0).ToList();
                removeCreatures.ForEach(creature => {
                    team.Remove(creature);
                    Console.WriteLine($"{creature.name} died");
                    });
            }
        }

        public static string ProcessTurn(Creature creature, List<Creature> opposingTeam, int numberOfGroups)
        {
            if (opposingTeam.Count == 0) return "Battle has ended";
            var atSpace = opposingTeam.Where(x => x.currentGroup == creature.currentGroup).ToList();
            var otherSpaces = opposingTeam.Where(x => x.currentGroup != creature.currentGroup).ToList();
            var noSpaces = Enumerable.Range(1, numberOfGroups).Where(x => otherSpaces.Where(y => y.currentGroup == x).Count() == 0).ToList();

            var atChoice = Dice.Choose(atSpace);
            var otherChoice = Dice.Choose(otherSpaces);

            { // 1. If it has two-handed melee and an enemy is on their space, swing
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.TH, false);
                if (attack != null && atSpace.Count > 0) return attack.MakeAttack(creature, atChoice);
            }
            { // 2. If it has two-handed ranged and an enemy is in an opposing space, shoot
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.TH, true);
                if (attack != null && otherSpaces.Count > 0) return attack.MakeAttack(creature, otherChoice);
            }
            { // 3. If it has one-handed melee and there is an enemy in a space it can reach, go to it and swing (go to the group with the fewest enemies)
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.OH, false);
                if (attack != null && otherSpaces.Count > 0)
                {
                    creature.currentGroup = otherChoice.currentGroup;
                    return attack.MakeAttack(creature, otherChoice);
                }
            }
            { // 4. If it has one-handed ranged shoot at a space if no enemy is on your space, otherwise move to an empty space and shoot if possible, if still not possible, shoot at a space even if your space is full
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.OH, true);
                if (attack != null && otherSpaces.Count > 0 && atSpace.Count == 0)
                {
                    return attack.MakeAttack(creature, otherChoice);
                }
                else if (attack != null && noSpaces.Count > 0)
                {
                    var combinedList = new List<Creature> { atChoice, otherChoice };
                    combinedList = combinedList.RemoveNulls().ToList();
                    creature.currentGroup = Dice.Choose(noSpaces);
                    return attack.MakeAttack(creature, Dice.Choose(combinedList));
                }
                else if (attack != null && otherSpaces.Count > 0) // cant move
                {
                    return attack.MakeAttack(creature, otherChoice);
                }
            }
            { // 5. If it has one-handed melee and there is an enemy on its space, swing
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.OH, false);
                if (attack != null && atSpace.Count > 0) return attack.MakeAttack(creature, atChoice);
            }
            { // 6. If it is natural-weapon attack a creature on its space
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.NW, false);
                if (attack != null && atSpace.Count > 0) return attack.MakeAttack(creature, atChoice);
            }
            { // 7. If it is natural-weapon move and attack the nearest creature with the fewest creatures on the space
                var attack = creature.GetAttackByType(Dice.enWeaponTypes.NW, false);
                if (attack != null && otherSpaces.Count > 0 && atSpace.Count == 0)
                {
                    creature.currentGroup = otherChoice.currentGroup;
                    return attack.MakeAttack(creature, otherChoice);
                }
            }

            return null;
        }
    }
}
