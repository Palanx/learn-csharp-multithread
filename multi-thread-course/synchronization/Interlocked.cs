using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace synchronization
{
    public static class Interlocked
    {
        private class Character
        {
            private int _armor;
            private int _health = 100;

            public int Health
            {
                get => _health;
                private set => _health = value;
            }

            public int Armor {
                get => _armor;
                private set => _armor = value;
            }

            public void Hit(int damage)
            {
                /*
                 * This atomic operation has a subtraction non-atomic, because _armor could change
                 * in another thread, so to solve this we need to use lock
                 */
                System.Threading.Interlocked.Add(ref _health, -(damage-_armor));
            }

            public void Heal(int health)
            {
                // This atomic operation approach is fine
                System.Threading.Interlocked.Add(ref _health, health);
            }

            public void CastArmorSpell(bool isPositive)
            {
                if (isPositive)
                {
                    // This atomic operation approach is fine
                    System.Threading.Interlocked.Increment(ref _armor);
                }
                else
                {
                    // This atomic operation approach is fine
                    System.Threading.Interlocked.Decrement(ref _armor);
                }
            }
        }

        public static void Swap(object obj1, object obj2)
        {
            // Each operation is atomic but the entire Swap operation isn't atomic
            object obj1Ref = System.Threading.Interlocked.Exchange(ref obj1, obj2);
            // Because obj2 could change here and the outcome will be unexpected
            System.Threading.Interlocked.Exchange(ref obj2, obj1Ref);
        }

        public static void TestInterlocked()
        {
            Character c = new Character();
            List<Task> tasks = new List<Task>();

            for (int i = 0; i < 100; i++)
            {
                Task t1 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        c.CastArmorSpell(false);
                    }
                });
                tasks.Add(t1);

                Task t2 = Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 100; j++)
                    {
                        c.CastArmorSpell(true);
                    }
                });
                tasks.Add(t2);
            }

            Task.WaitAll(tasks.ToArray());

            Console.WriteLine($"Resulting armor = { c.Armor }");
        }
    }

    /// <summary>
    /// This is a real atomic operation to instantiate a singleton
    /// </summary>
    public static class Lazy<T> where T : class, new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                // If current is null, we need to create a new instance
                if (_instance == null)
                {
                    // Attempt to create, It will only set if previous was null
                    System.Threading.Interlocked.CompareExchange(ref _instance, new T(), (T)null);
                }

                return _instance;
            }
        }
    }
}