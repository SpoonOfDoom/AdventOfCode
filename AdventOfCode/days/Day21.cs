using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    public class Day21 : Day
    {
        public Day21() : base(21) { }

        
        private int BossStartHP, BossDamage, BossArmor;
        private List<int> losingCosts = new List<int>();
        private List<int> winningCosts = new List<int>();

        public class Item : IComparable<Item>
        {
            public enum ItemType
            {
                Weapon,
                Armor,
                Ring
            }

            public ItemType Type;
            public string Name;
            public int Cost;
            public int Damage;
            public int Armor;

            public Item()
            {
                
            }

            public bool Equals(Item otherItem)
            {
                return Type == otherItem.Type && Cost == otherItem.Cost && Damage == otherItem.Damage;
                //return Name == otherItem.Name; //performance difference because of string comparison?
            }

            public Item Clone()
            {
                Item item = new Item {Cost = Cost, Name = Name, Damage = Damage, Type = Type, Armor = Armor};
                return item;
            }

            public override string ToString()
            {
                return $"{Name} - Cost: {Cost}, Damage: {Damage}, Armor: {Armor}";
            }

            public int CompareTo(Item other)
            {
                return Equals(other) ? 0 : Cost.CompareTo(other.Cost);
            }
        }

        class Combatant
        {
            public int Hitpoints = 100;

            public Item Weapon;
            public Item ArmorItem;
            public Item Ring1;
            public Item Ring2;

            public int TotalCost => (Weapon?.Cost ?? 0) + (ArmorItem?.Cost ?? 0) + (Ring1?.Cost ?? 0) + (Ring2?.Cost ?? 0);

            public int TotalStrength => (Weapon?.Damage ?? 0) + (Ring1?.Damage ?? 0) + (Ring2?.Damage ?? 0);

            public int TotalArmor => (ArmorItem?.Armor ?? 0) + (Ring1?.Armor ?? 0) + (Ring2?.Armor ?? 0);

            public Combatant Clone()
            {
                var armorItem = ArmorItem?.Clone();
                var weapon = Weapon?.Clone();
                var ring1 = Ring1?.Clone();
                var ring2 = Ring2?.Clone();

                Combatant combatant = new Combatant {Hitpoints = Hitpoints, ArmorItem = armorItem, Weapon = weapon, Ring1 = ring1, Ring2 = ring2};

                return combatant;
            }
        }
        
        private List<Item> InitialiseShopItems()
        {
            var shopItems = new List<Item>
                            {
                                new Item { Type = Item.ItemType.Weapon, Name = "Dagger", Cost = 8, Damage = 4, Armor = 0 },
                                new Item { Type = Item.ItemType.Weapon, Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0 },
                                new Item { Type = Item.ItemType.Weapon, Name = "Warhammer", Cost = 25, Damage = 6, Armor = 0},
                                new Item { Type = Item.ItemType.Weapon, Name = "Longsword", Cost = 40, Damage = 7, Armor = 0 },
                                new Item { Type = Item.ItemType.Weapon, Name = "Greataxe", Cost = 74, Damage = 8, Armor = 0 },

                                new Item { Type = Item.ItemType.Armor, Name = "Leather", Cost = 13, Damage = 0, Armor = 1 },
                                new Item { Type = Item.ItemType.Armor, Name = "Chainmail", Cost = 31, Damage = 0, Armor = 2 },
                                new Item { Type = Item.ItemType.Armor, Name = "Splintmail", Cost = 53, Damage = 0, Armor = 3 },
                                new Item { Type = Item.ItemType.Armor, Name = "Bandedmail", Cost = 75, Damage = 0, Armor = 4 },
                                new Item { Type = Item.ItemType.Armor, Name = "Platemail", Cost = 102, Damage = 0, Armor = 5 },

                                new Item { Type = Item.ItemType.Ring, Name = "Defense +1", Cost = 20, Damage = 0, Armor = 1 },
                                new Item { Type = Item.ItemType.Ring, Name = "Damage +1", Cost = 25, Damage = 1, Armor = 0 },
                                new Item { Type = Item.ItemType.Ring, Name = "Defense +2", Cost = 40, Damage = 0, Armor = 2 },
                                new Item { Type = Item.ItemType.Ring, Name = "Damage +2", Cost = 50, Damage = 2, Armor = 0 },
                                new Item { Type = Item.ItemType.Ring, Name = "Defense +3", Cost = 80, Damage = 0, Armor = 3 },
                                new Item { Type = Item.ItemType.Ring, Name = "Damage +3", Cost = 100, Damage = 3, Armor = 0 }
                            };


            return shopItems;
        }
        
        private int SimulateFight(Combatant boss, Combatant player)
        {
            bool playerTurn = true;
            
            int playerHitpoints = player.Hitpoints;
            int playerTotalStrength = player.TotalStrength;
            int playerTotalArmor = player.TotalArmor;

            int bossHitpoints = boss.Hitpoints;
            int bossTotalArmor = boss.TotalArmor;
            int bossTotalStrength = boss.TotalStrength;

            while (bossHitpoints > 0 && playerHitpoints > 0)
            {
                if (playerTurn)
                {
                    bossHitpoints -= Math.Max(1, playerTotalStrength - bossTotalArmor);
                }
                else
                {
                    playerHitpoints -= Math.Max(1, bossTotalStrength - playerTotalArmor);
                }
                playerTurn = !playerTurn;
            }

            int result = bossHitpoints - playerHitpoints;
            return result;
        }

        protected override object GetSolutionPart1()
        {
            for (int i = 0; i < InputLines.Count; i++)
            {
                string line = InputLines[i];
                int number = line.Split(new[] { ": " }, StringSplitOptions.None)[1].ToInt();
                if (i == 0)
                {
                    BossStartHP = number;
                }
                else if (i == 1)
                {
                    BossDamage = number;
                }
                else if (i == 2)
                {
                    BossArmor = number;
                }
            }

            var bossWeapon = new Item() { Damage = BossDamage };
            var bossArmor = new Item() { Armor = BossArmor };
            var boss = new Combatant { Hitpoints = BossStartHP, Weapon = bossWeapon, ArmorItem = bossArmor };
            var initialItems = InitialiseShopItems();

            var possiblePlayerCombos = new List<Combatant>();
            foreach (Item weapon in initialItems.Where(i => i.Type == Item.ItemType.Weapon))
            {
                Combatant c = new Combatant { Hitpoints = 100, Weapon = weapon };
                possiblePlayerCombos.Add(c);

                Combatant cWithoutArmor = c.Clone();
                foreach (Item ring1 in initialItems.Where(i => i.Type == Item.ItemType.Ring))
                {
                    Combatant c3 = cWithoutArmor.Clone();
                    c3.Ring1 = ring1;
                    possiblePlayerCombos.Add(c3);

                    foreach (Item ring2 in initialItems.Where(i => i.Type == Item.ItemType.Ring && !i.Equals(ring1)))
                    {
                        Combatant c4 = c3.Clone();
                        c4.Ring2 = ring2;
                        possiblePlayerCombos.Add(c4);
                    }
                }

                foreach (Item armor in initialItems.Where(i => i.Type == Item.ItemType.Armor))
                {
                    Combatant c2 = c.Clone();
                    c2.ArmorItem = armor;
                    possiblePlayerCombos.Add(c2);

                    foreach (Item ring1 in initialItems.Where(i => i.Type == Item.ItemType.Ring))
                    {
                        Combatant c3 = c2.Clone();
                        c3.Ring1 = ring1;
                        possiblePlayerCombos.Add(c3);

                        foreach (Item ring2 in initialItems.Where(i => i.Type == Item.ItemType.Ring && !i.Equals(ring1)))
                        {
                            Combatant c4 = c3.Clone();
                            c4.Ring2 = ring2;
                            possiblePlayerCombos.Add(c4);
                        }
                    }
                }
            }
            Console.WriteLine($"Possible combos: {possiblePlayerCombos.Count}");
            
            foreach (Combatant p in possiblePlayerCombos)
            {
                int fightResult = SimulateFight(boss, p);
                if (fightResult > 0)
                {
                    losingCosts.Add(p.TotalCost);
                }

                else
                {
                    winningCosts.Add(p.TotalCost);
                }
            }

            int result = winningCosts.Min();
            return result.ToString(); //91
        }

        protected override object GetSolutionPart2()
        {
            int result = losingCosts.Max();
            return result.ToString(); //158
        }
    }
}
