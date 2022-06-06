using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeWeeksSimulator
{
    internal static class Simulator
    {
        public static void Run() {

            var creature1 = new Creature(1, 7, 1, 1, 1, 2).AddOHAttack();
            var creature2 = new Creature(1, 8, 1, 1, 1, 2).AddOHAttack();

            creature1.LogCreatureStats();
            creature2.LogCreatureStats();

            var attack = creature1.SelectAttack(mobile: true);

            string message = attack.MakeAttack(creature1, creature2);

            Console.WriteLine(message);

            //test change



        }
    }
}
