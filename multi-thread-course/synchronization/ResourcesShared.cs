using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace synchronization
{
    public static class ResourcesShared
    {
        private class Character
        {
            public int Health { get; private set; } = 100;

            public void Hit(int damage)
            {
                Health -= damage;
            }

            public void Heal(int health)
            {
                Health += health;
            }
        }

        /// <summary>
        /// When using the ++ (as example, here we use += and -=) operator to increment a variable
        /// in C#, here are the main steps that happen at the CPU level:
        /// <b>Load</b> - The current value is loaded from memory into a register.
        /// <b>Increment</b> - The value in the register is incremented by 1. This is a simple
        /// hardware supported operation.
        /// <b>Store</b> - The incremented value is stored back to memory in the variable's location.
        /// </summary>
        public static void TestRaceCondition()
        {
            Character c = new Character();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                Task t1 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        c.Hit(10);
                    }
                });
                tasks.Add(t1);

                Task t2 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        c.Heal(10);
                    }
                });
                tasks.Add(t2);
            }

            Task.WaitAll(tasks.ToArray());

            /*
             * So the result in each iteration will be different because one of the threads is
             * overriding the result of the other, because both read the value at the same time,
             * but one store the value before the other.
             */
            Console.WriteLine($"Resulting health = { c.Health }");
        }
    }
}