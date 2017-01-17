using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode2016.Tools;

namespace AdventOfCode.Days
{
    public class Day21 : Day
    {
        public Day21() : base(21) { }

        
        static int BossStartHP, BossDamage;

        static string[] PossibleOptions = {
                                              "UpgradeWeapon",
                                              "UpgradeArmor",
                                              "UpgradeRings"
                                          };

        public class Item
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
                Item item = new Item {Cost = this.Cost, Name = this.Name, Damage = this.Damage, Type = this.Type};
                return item;
            }
        }

        class Combatant
        {
            public int Hitpoints = 50;
            //public int Mana = 500;
            public int Armor = 2;

            public Item Weapon = null;
            public Item ArmorItem = null;
            public List<Item> Rings = new List<Item>();

            public int TotalCost
            {
                get { return Weapon.Cost + (ArmorItem?.Cost ?? 0) + Rings.Sum(s => s.Cost); }
            }

            public int TotalStrength
            {
                get { return Weapon.Damage + Rings.Sum(r => r.Damage); }
            }

            public int TotalArmor
            {
                get { return ArmorItem?.Armor ?? 0 + Rings.Sum(r => r.Armor); }
            }

            public void ReplaceWeakestRing(Item newRing)
            {
                //todo: find and replace weakest ring
            }

            public bool Equals(Combatant otherCombatant)
            {
                bool weapon = Weapon != null && otherCombatant.Weapon != null && Weapon.Equals(otherCombatant.Weapon);
                if (!weapon)
                {
                    return false;
                }

                bool armor = (ArmorItem == null && otherCombatant.Weapon == null) || (ArmorItem != null && otherCombatant.ArmorItem != null && ArmorItem.Equals(otherCombatant.Weapon));
                if (!armor)
                {
                    return false;
                }

                if (Rings.Count != otherCombatant.Rings.Count)
                {
                    return false;
                }
                if (Rings.Count == 0)
                {
                    return true;
                }
                for (int i = 0; i < Rings.Count; i++)
                {
                    if (!Rings[i].Equals(otherCombatant.Rings[i]))
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
                var rings = new List<Item>();
                foreach (Item ring in Rings)
                {
                    rings.Add(ring.Clone());
                }

                Combatant combatant = new Combatant {Hitpoints = this.Hitpoints, Armor = this.Armor, ArmorItem = armorItem, Weapon = weapon, Rings = rings};

                return combatant;
            }
        }

        class GameState : ISearchNode
        {
            public Combatant Player;
            public Combatant Boss;

            public int? FightResult = null;


            public int Cost { get; set; }

            public List<object> Actions { get; set; }
            List<Item> ShopItems = new List<Item>();

            public string VerboseInfo => $"Boss HP: {Boss.Hitpoints}\nPlayer HP: {Player.Hitpoints}";

            public GameState Clone()
            {
                var boss = Boss.Clone();
                var player = Player.Clone();
                
                var actions = new List<object>();
                foreach (object action in Actions)
                {
                    actions.Add(action);
                }

                int cost = this.Cost;

                var shopItems = new List<Item>();
                foreach (Item item in ShopItems)
                {
                    shopItems.Add(item.Clone());
                }

                var clonedState = new GameState {Player = player, Boss = boss, Cost = cost, Actions = actions, ShopItems = shopItems};
                return clonedState;
            }

            public void PlayerPunch()
            {
                int damage = Player.TotalStrength; //todo: add ring effects

                damage = Math.Max(1, damage - Boss.Armor);
                
                Boss.Hitpoints -= damage;
            }
            
            public void BossPunch()
            {
                int damage = BossDamage;
                
                damage = Math.Max(1, damage - Player.TotalArmor); //todo: add ring effects
                
                Player.Hitpoints -= damage;
            }

            void BuyItem(Item item)
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
                        if (Player.Rings.Count < 2)
                        {
                            Player.Rings.Add(item);
                        }
                        else
                        {
                            Player.ReplaceWeakestRing(item);
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
                while (Boss.Hitpoints > 0 && Player.Hitpoints > 0)
                {
                    if (playerTurn)
                    {
                        PlayerPunch();
                    }
                    else
                    {
                        BossPunch();
                    }
                    playerTurn = !playerTurn;
                }
                return Boss.Hitpoints - Player.Hitpoints;
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
                
                foreach (var option in Enum.GetValues(typeof(Item.ItemType)).Cast<Item.ItemType>())
                {
                    
                    if (Player.Weapon == null && option != Item.ItemType.Weapon)
                    {
                        continue;
                    }
                    GameState newState = this.Clone();
                    Item betterItem = newState.GetNextBestItemFromShop(option);

                    if (betterItem == null)
                    {
                        continue;
                    }

                    FightResult = newState.SimulateFight();
                    
                    
                    actions.Add(new ExpandAction { action = option, cost = Player.TotalCost, result = newState });
                }
                return actions;
            }

            
            public bool Equals(ISearchNode goalState)
            {
                var state = goalState as GameState;
                bool equals = state.Player.Equals(Player);
                return equals;
            }

            public float GetHeuristic(ISearchNode goalState)
            {
                float priority = 0;
                priority += GetCheapestUpgradeCost();
                return priority;
            }

            public int GetUpgradeCost(Item.ItemType type)
            {
                int upgradeCost = int.MinValue;
                switch (type)
                {
                    case Item.ItemType.Weapon:
                        break;
                    case Item.ItemType.Armor:
                        break;
                    case Item.ItemType.Ring:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
                return upgradeCost;
            }

            int GetCheapestUpgradeCost()
            {
                int minCost = int.MaxValue;
                foreach (Item.ItemType itemType in Enum.GetValues(typeof(Item.ItemType)).Cast<Item.ItemType>())
                {
                    
                }
                return minCost;
            }

            public float GetTentativeCost(ISearchNode goalState)
            {
                return Cost + GetHeuristic(goalState);
            }

            public bool IsGoalState(ISearchNode gameState)
            {
                var state = gameState as GameState;
                return state.SimulateFight() <= 0;
            }
        }

        private List<Item> InitialiseShopItems()
        {
            var shopItems = new List<Item>();
            shopItems.Add(new Item {Type = Item.ItemType.Weapon, Name = "abc", Cost = 123, Damage = 1, Armor = 2});
            shopItems.Add(new Item { Type = Item.ItemType.Weapon, Name = "Dagger", Cost = 8, Damage = 4, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Weapon, Name = "Shortsword", Cost = 10, Damage = 5, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Weapon, Name = "Warhammer", Cost = 25, Damage = 6, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Weapon, Name = "Longsword", Cost = 40, Damage = 7, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Weapon, Name = "Greataxe", Cost = 74, Damage = 8, Armor = 0 });

            shopItems.Add(new Item { Type = Item.ItemType.Armor, Name = "Leather", Cost = 13, Damage = 0, Armor = 1 });
            shopItems.Add(new Item { Type = Item.ItemType.Armor, Name = "Chainmail", Cost = 31, Damage = 0, Armor = 2 });
            shopItems.Add(new Item { Type = Item.ItemType.Armor, Name = "Splintmail", Cost = 53, Damage = 0, Armor = 3 });
            shopItems.Add(new Item { Type = Item.ItemType.Armor, Name = "Bandedmail", Cost = 75, Damage = 0, Armor = 4 });
            shopItems.Add(new Item { Type = Item.ItemType.Armor, Name = "Platemail", Cost = 102, Damage = 0, Armor = 5 });

            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Damage +1", Cost = 25, Damage = 1, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Damage +2", Cost = 50, Damage = 2, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Damage +3", Cost = 100, Damage = 3, Armor = 0 });
            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Defense +1", Cost = 20, Damage = 0, Armor = 1 });
            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Defense +2", Cost = 40, Damage = 0, Armor = 2 });
            shopItems.Add(new Item { Type = Item.ItemType.Ring, Name = "Defense +3", Cost = 80, Damage = 0, Armor = 3 });
            return shopItems;
        }
        
        public override string GetSolutionPart1()
        {
            for (int i = 0; i < inputLines.Count; i++)
            {
                string line = inputLines[i];
                int number = line.Split(new[]
                                    {
                                            ": "
                                        },
                                    StringSplitOptions.None)[1].ToInt();
                if (i == 0)
                {
                    BossStartHP = number;
                }
                if (i == 1)
                {
                    BossDamage = number;
                }
            }
            
            var boss = new Combatant {Hitpoints = BossStartHP};
            var player = new Combatant {Hitpoints = 50};

            var startState = new GameState {Boss = boss, Player = player, Cost = 0, Actions = new List<object>()};
            var goalState = new GameState();

            var result = new AStar().GetMinimumCost(startState, goalState); //1824
            return result.ToString();
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}