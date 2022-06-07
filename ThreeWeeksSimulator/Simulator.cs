using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWeeksSimulator
{
    internal static class Simulator
    {
        public static void Run(int numberOfGroups = 3) {

            var creature1 = new Creature(1, 7, 1, 1, 1, 2).AddOHAttack().SetCurrentGroup(numberOfGroups);
            var creature2 = new Creature(1, 10, 1, 1, 1, 2).AddOHAttack().SetCurrentGroup(numberOfGroups);

            creature1.LogCreatureStats();
            creature2.LogCreatureStats();

            var attack = creature1.SelectAttack(mobile: true);
            string message = attack.MakeAttack(creature1, creature2);
            Console.WriteLine(message);

            creature1.LogCreatureStats();
            creature2.LogCreatureStats();

            var team1 = new List<Creature> { creature1 };
            var team2 = new List<Creature> { creature2 };

            //team1.ForEach(creature => ProcessTurn(creature, team2));

            // 1. If it has two-handed melee and an enemy is on their space, swing
            // 2. If it has two-handed ranged and an enemy is in an opposing space, shoot
            // 3. If it has one-handed melee and there is an enemy in a space it can reach, go to it and swing (go to the group with the fewest enemies)
            // 4. If it has one-handed ranged shoot at a space if no enemy is on your space
            // 5. If it has one-handed melee and there is an enemy on its space, swing
            // 6. If it is natural-weapon attack a creature on its space
            // 7. If it is natural-weapon move and attack the nearest creature with the fewest creatures on the space
            // 8. If this line is hit then no enemies are left alive, your side wins!


        }
        
        public static void ProcessTurn(Creature creature, List<Creature> team)
        {
            var atSpace = team.Where(x => x.currentGroup == creature.currentGroup).ToList();
            var otherSpaces = team.Where(x => x.currentGroup == creature.currentGroup).ToList();

            var attack = creature.SelectAttack(atSpace.Count == 0);
            var atTarget = Dice.Choose(atSpace);
            var otherTarget = Dice.Choose(otherSpaces);

            if (attack.weaponType == Dice.enWeaponTypes.TH) attack.MakeAttack(creature, atTarget);
            else if (attack.weaponType == Dice.enWeaponTypes.OH) attack.MakeAttack(creature, atTarget);







        }
       
    }
}
