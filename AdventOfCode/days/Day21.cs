using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdventOfCode.Extensions;
using AdventOfCode2016.Tools;
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_Elsewhere
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_BuiltInTypes

namespace AdventOfCode.Days
{
    public class Day21 : Day
    {
        public Day21() : base(21) { }

        
        static int BossStartHP, BossDamage, BossArmor;
        private int lowestFightResult = int.MaxValue;
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
            public int Hitpoints = 50;
            //public int Mana = 500;
            public int Armor = 0;

            public Item Weapon;
            public Item ArmorItem;
            public Item Ring1;
            public Item Ring2;

            public int TotalCost
            {
                get { return (Weapon?.Cost ?? 0) + (ArmorItem?.Cost ?? 0) + (Ring1?.Cost ?? 0) + (Ring2?.Cost ?? 0); }
            }

            public int TotalStrength
            {
                get { return (Weapon?.Damage ?? 0) + (Ring1?.Damage ?? 0) + (Ring2?.Damage ?? 0); }
            }

            public int TotalArmor
            {
                get { return (ArmorItem?.Armor ?? 0) + (Ring1?.Armor ?? 0) + (Ring2?.Armor ?? 0); }
            }
            
            public bool Equals(Combatant otherCombatant)
            {
                bool weapon = Weapon != null && otherCombatant.Weapon != null && Weapon.Equals(otherCombatant.Weapon);
                if (!weapon)
                {
                    return false;
                }

                bool armor = (ArmorItem == null && otherCombatant.ArmorItem == null) || (ArmorItem != null && otherCombatant.ArmorItem != null && ArmorItem.Equals(otherCombatant.ArmorItem));
                if (!armor)
                {
                    return false;
                }
                List<Item> Rings = new List<Item>();
                List<Item> otherRings = new List<Item>();
                if (Ring1 != null)
                {
                    Rings.Add(Ring1);
                }
                if (Ring2 != null)
                {
                    Rings.Add(Ring2);
                }

                if (otherCombatant.Ring1 != null)
                {
                    otherRings.Add(otherCombatant.Ring1);
                }
                if (otherCombatant.Ring2 != null)
                {
                    otherRings.Add(otherCombatant.Ring2);
                }
                Rings.Sort();
                otherRings.Sort();

                if (Rings.Count != otherRings.Count)
                {
                    return false;
                }
                if (Rings.Count == 0)
                {
                    return true;
                }
                for (int i = 0; i < Rings.Count; i++)
                {
                    if (!Rings[i].Equals(otherRings[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public Combatant Clone()
            {
                var armorItem = ArmorItem?.Clone(); //works if null?
                var weapon = Weapon?.Clone();
                var ring1 = Ring1?.Clone();
                var ring2 = Ring2?.Clone();

                Combatant combatant = new Combatant {Hitpoints = Hitpoints, Armor = Armor, ArmorItem = armorItem, Weapon = weapon, Ring1 = ring1, Ring2 = ring2};

                return combatant;
            }
        }

        class GameState : ISearchNode
        {
            public Combatant Player;
            public Combatant Boss;

            private int? FightResult = null;
            
            public int Cost { get; set; }

            public List<object> Actions { get; set; }
            public List<Item> ShopItems = new List<Item>();


            public string VerboseInfo => $"Fight result: {FightResult}     \nPlayer Total cost: {Player.TotalCost}     \n{PrintItemList()}     ";

            private string PrintItemList()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"Weapon: {Player.Weapon}    \n");
                sb.Append(Player.ArmorItem != null ? $"Armor: {Player.ArmorItem}     \n" : "Armor: none     \n");
                sb.Append(Player.ArmorItem != null ? $"Ring1: {Player.Ring1}     \n" : "Ring1: none     \n");
                sb.Append(Player.ArmorItem != null ? $"Ring2: {Player.Ring2}     \n" : "Ring2: none     \n");
                return sb.ToString();
            }
            private GameState Clone()
            {
                var boss = Boss.Clone();
                var player = Player.Clone();
                
                var actions = new List<object>();
                foreach (object action in Actions)
                {
                    actions.Add(action);
                }

                int cost = Cost;

                var shopItems = new List<Item>();
                foreach (Item item in ShopItems)
                {
                    shopItems.Add(item.Clone());
                }

                var clonedState = new GameState {Player = player, Boss = boss, Cost = cost, Actions = actions, ShopItems = shopItems};
                return clonedState;
            }

            public void PlayerPunch(ref int bossHitpoints)
            {
                int damage = Player.TotalStrength;

                damage = Math.Max(1, damage - Boss.TotalArmor);
                
                bossHitpoints -= damage;
            }
            
            public void BossPunch(ref int playerHitpoints)
            {
                int damage = BossDamage;
                
                damage = Math.Max(1, damage - Player.TotalArmor);
                
                playerHitpoints -= damage;
            }

            void BuyItem(Item item, int targetSlot)
            {
                switch (item.Type)
                {
                    case Item.ItemType.Weapon:
                        Player.Weapon = item;
                        break;
                    case Item.ItemType.Armor:
                        Player.ArmorItem = item;
                        break;
                    case Item.ItemType.Ring:
                        if (targetSlot == 1)
                        {
                            Player.Ring1 = item;
                        }
                        else
                        {
                            Player.Ring2 = item;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            int SimulateFight()
            {
                if (FightResult.HasValue)
                {
                    return FightResult.Value;
                }
                bool playerTurn = true;
                int bossHitpoints = Boss.Hitpoints;
                int playerHitpoints = Player.Hitpoints;
                while (bossHitpoints > 0 && playerHitpoints > 0)
                {
                    if (playerTurn)
                    {
                        PlayerPunch(ref bossHitpoints);
                    }
                    else
                    {
                        BossPunch(ref playerHitpoints);
                    }
                    playerTurn = !playerTurn;
                }

                int result = bossHitpoints - playerHitpoints;
                return result;
            }

            Item GetNextBestItemFromShop(Item.ItemType itemType)
            {
                var newItem = ShopItems.FirstOrDefault(item => item.Type == itemType);
                ShopItems.Remove(newItem);
                return newItem;
            }

            public HashSet<ExpandAction> ExpandNode()
            {
                var actions = new HashSet<ExpandAction>();
                int slot = 1;
                Item.ItemType[] options = {
                                              Item.ItemType.Weapon,
                                              Item.ItemType.Armor,
                                              Item.ItemType.Ring,
                                              Item.ItemType.Ring,
                                          };
                foreach (var option in options)
                {
                    if (Player.Weapon == null && option != Item.ItemType.Weapon)
                    {
                        continue;
                    }
                    GameState newState = Clone();
                    Item betterItem = newState.GetNextBestItemFromShop(option);
                    

                    if (betterItem == null)
                    {
                        continue;
                    }
                    
                    newState.BuyItem(betterItem, slot);
                    
                    newState.FightResult = newState.SimulateFight();
                    int upgradeCost = GetUpgradeCost(betterItem, slot);
                    if (option == Item.ItemType.Ring)
                    {
                        slot++;
                    }
                    actions.Add(new ExpandAction { action = option, cost = upgradeCost, result = newState });
                }
                return actions;
            }

            
            public bool Equals(ISearchNode goalState)
            {
                var state = (GameState) goalState;
                bool equals = state.Player.Equals(Player);
                return equals;
            }

            public float GetHeuristic(ISearchNode goalState)
            {
                if (IsGoalState(goalState))
                {
                    return 0;
                }
                float priority = 0;
                priority += GetCheapestUpgradeCost().Item2;
                return priority;
            }

            public int GetUpgradeCost(Item.ItemType type, int slot)
            {
                int upgradeCost = int.MinValue;
                switch (type)
                {
                    case Item.ItemType.Weapon:
                        upgradeCost = ShopItems.First(item => item.Type == Item.ItemType.Weapon).Cost - Player.Weapon.Cost;
                        break;
                    case Item.ItemType.Armor:
                        upgradeCost = ShopItems.First(item => item.Type == Item.ItemType.Armor).Cost - (Player.ArmorItem?.Cost ?? 0);
                        break;
                    case Item.ItemType.Ring:
                        if (slot == 1)
                        {
                            upgradeCost = ShopItems.First(item => item.Type == Item.ItemType.Ring).Cost - (Player.Ring1?.Cost ?? 0);
                        }
                        else
                        {
                            upgradeCost = ShopItems.First(item => item.Type == Item.ItemType.Ring).Cost - (Player.Ring2?.Cost ?? 0);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                return upgradeCost;
            }

            public int GetUpgradeCost(Item compareItem, int slot)
            {
                int upgradeCost = int.MinValue;
                switch (compareItem.Type)
                {
                    case Item.ItemType.Weapon:
                        upgradeCost = compareItem.Cost - (Player.Weapon?.Cost ?? 0);
                        break;
                    case Item.ItemType.Armor:
                        upgradeCost = compareItem.Cost - (Player.ArmorItem?.Cost ?? 0);
                        break;
                    case Item.ItemType.Ring:
                        if (slot == 1)
                        {
                            upgradeCost = compareItem.Cost - (Player.Ring1?.Cost ?? 0);
                        }
                        else
                        {
                            upgradeCost = compareItem.Cost - (Player.Ring2?.Cost ?? 0);
                        }
                        
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return upgradeCost;
            }

            Tuple<Item.ItemType, int> GetCheapestUpgradeCost()
            {
                int minCost = int.MaxValue;
                Item.ItemType cheapestType = Item.ItemType.Weapon;
                int slot = 1;
                Item.ItemType[] options = {
                                              Item.ItemType.Weapon,
                                              Item.ItemType.Armor,
                                              Item.ItemType.Ring,
                                              Item.ItemType.Ring,
                                          };
                foreach (Item.ItemType itemType in options)
                {
                    if (ShopItems.Any(item => item.Type == itemType))
                    {
                        var cost = GetUpgradeCost(itemType, slot);
                        if (cost < minCost)
                        {
                            minCost = cost;
                            cheapestType = itemType;
                        }
                        if (itemType == Item.ItemType.Ring)
                        {
                            slot++;
                        }
                    }
                    
                }
                return Tuple.Create(cheapestType, minCost);
            }

            public bool IsGoalState(ISearchNode gameState)
            {
                if (Player.Weapon == null) //edge case for start state
                {
                    return false;
                }
                return SimulateFight() <= 0;
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
        
        public override string GetSolutionPart1()
        {
            for (int i = 0; i < inputLines.Count; i++)
            {
                string line = inputLines[i];
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
            
            
            var bossWeapon = new Item() {Damage = BossDamage};
            var bossArmor = new Item() {Armor = BossArmor};
            var boss = new Combatant { Hitpoints = BossStartHP, Weapon = bossWeapon, ArmorItem = bossArmor};
            var player = new Combatant {Hitpoints = 100};

            var startState = new GameState {Boss = boss, Player = player, Cost = 0, Actions = new List<object>(), ShopItems = InitialiseShopItems()};
            
            var goalState = new GameState();

            var result = new AStar().GetMinimumCost(startState, goalState, verbose:true);
            return result.ToString(); //91
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}