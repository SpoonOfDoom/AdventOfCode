using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode.Extensions;
using AdventOfCode.Tools;

namespace AdventOfCode.Days._2015
{
    /// <summary>
    /// 
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public class Day22 : Day
    {
        public Day22() : base(2015, 22) { }

        static Dictionary<Spell, int> SpellCost = new Dictionary<Spell, int>
                                                  {
                                                      {Spell.MagicMissile, 53},
                                                      {Spell.Drain, 73},
                                                      {Spell.Shield, 113},
                                                      {Spell.Poison, 173},
                                                      {Spell.Recharge, 229}
                                                  };

        static Dictionary<Spell, int> SpellDamage = new Dictionary<Spell, int>
                                                    {
                                                        {Spell.MagicMissile, 4},
                                                        {Spell.Drain, 2},
                                                        {Spell.Shield, 7},
                                                        {Spell.Poison, 3},
                                                        {Spell.Recharge, 101}
                                                    };

        private static int bossStartHP, bossDamage;

        enum Spell
        {
            MagicMissile,
            Drain,
            Shield,
            Poison,
            Recharge
        }

        class Effect : IComparable<Effect>
        {
            public Spell EffectType;
            public int TurnsLeft = 0;
            public int Power = 0;

            public Effect()
            {
                
            }
            public Effect(Spell effectType)
            {
                EffectType = effectType;
                switch (effectType)
                {
                    case Spell.Shield:
                        TurnsLeft = 6;
                        Power = 7;
                        break;
                    case Spell.Poison:
                        TurnsLeft = 6;
                        Power = 3;
                        break;
                    case Spell.Recharge:
                        TurnsLeft = 5;
                        Power = 101;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(effectType), effectType, null);
                }
            }

            public void ApplyEffect(GameState state)
            {
                switch (EffectType)
                {
                    case Spell.Shield:
                        break;
                    case Spell.Poison:
                        state.Boss.Hitpoints -= Power;
                        break;
                    case Spell.Recharge:
                        state.Player.Mana += Power;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                TurnsLeft--;
            }

            public override string ToString()
            {
                return $"{EffectType}P{Power}T{TurnsLeft}";
            }

            public int CompareTo(Effect other)
            {
                if (EffectType == other.EffectType && Power == other.Power && TurnsLeft == other.TurnsLeft)
                {
                    return 0;
                }
                return -EffectType.CompareTo(other.EffectType);
            }

            public string GetHash()
            {
                return $"{EffectType}_{TurnsLeft}";
            }
        }

        private class Combatant
        {
            public int Hitpoints = 50;
            public int Mana = 500;

            public string GetHash()
            {
                return $"{Hitpoints}_{Mana}";
            }
        }

        class GameState : ISearchNode
        {
            public int Turn = 1;
            public Combatant Player;
            public Combatant Boss;

            public List<Effect> ActiveEffects = new List<Effect>();

            public int Cost { get; set; }

            public List<object> Actions { get; set; }

            private string ActiveSpellsToString
            {
                get
                {
                    string s = "";
                    for (int i = 0; i < ActiveEffects.Count; i++)
                    {
                        Effect effect = ActiveEffects[i];
                        s += $"spell{i}: {effect}\n";
                    }
                    return s;
                }
            }

            public string VerboseInfo 
                => $"Boss HP: {Boss.Hitpoints}\nPlayer HP: {Player.Hitpoints}\nPlayer Mana: {Player.Mana}\nActive Effects: {ActiveEffects.Count}\nTurn: {Turn.ToString("D3")}\n{ActiveSpellsToString}";

            public string StringHash
            {
                get
                {
                    if (stringHash is null)
                    {
                        CreateHash();
                    }

                    return stringHash;
                }
            }

            public long NumericHash { get; }

            public bool Hardmode = false;
            private string stringHash;

            public GameState Clone()
            {
                var boss = new Combatant {Hitpoints = Boss.Hitpoints, Mana = Boss.Mana};
                var player = new Combatant {Hitpoints = Player.Hitpoints, Mana = Player.Mana};
                var activeEffects = new List<Effect>();
                foreach (Effect effect in ActiveEffects)
                {
                    activeEffects.Add(new Effect {EffectType = effect.EffectType, Power = effect.Power, TurnsLeft = effect.TurnsLeft});
                }
                var actions = new List<object>();
                foreach (object action in Actions)
                {
                    Spell s = (Spell) action;
                    actions.Add(s);
                }
                int cost = Cost;

                var clonedState = new GameState {Player = player, Boss = boss, ActiveEffects = activeEffects, Turn = Turn, Cost = cost, Actions = actions, Hardmode = Hardmode};
                return clonedState;
            }

            private bool CastSpell(Spell spellType)
            {
                if (ActiveEffects.Any(effect => effect.EffectType == spellType))
                {
                    return false;
                }
                Player.Mana -= SpellCost[spellType];
                
                switch (spellType)
                {
                    case Spell.MagicMissile:
                        Boss.Hitpoints -= 4;
                        break;
                    case Spell.Drain:
                        Boss.Hitpoints -= 2;
                        Player.Hitpoints += 2;
                        break;
                    case Spell.Shield:
                        ActiveEffects.Add(new Effect(spellType));
                        break;
                    case Spell.Poison:
                        ActiveEffects.Add(new Effect(spellType));
                        break;
                    case Spell.Recharge:
                        ActiveEffects.Add(new Effect(spellType));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null);
                }
                if (ActiveEffects.Count > 1)
                {
                    ActiveEffects.Sort();
                }
                return true;
            }

            private void BossPunch()
            {
                var shieldEffect = ActiveEffects.Where(effect => effect.EffectType == Spell.Shield);
                int damage = bossDamage;
                if (shieldEffect.Any()) //todo: why is this not just a count check?
                {
                    damage = Math.Max(1, damage - shieldEffect.First().Power);
                }

                Player.Hitpoints -= damage;
            }

            public HashSet<ExpandAction> ExpandNode()
            {
                var actions = new HashSet<ExpandAction>();
                
                foreach (KeyValuePair<Spell, int> keyValuePair in SpellCost)
                {
                    if (Player.Mana < keyValuePair.Value)
                    {
                        continue;
                    }
                    
                    GameState newState = Clone();
                    newState.Turn++;
                    if (!newState.CastSpell(keyValuePair.Key))
                    {
                        //trying to cast effect that's already active
                        continue;
                    }

                    foreach (Effect effect in newState.ActiveEffects)
                    {
                        effect.ApplyEffect(newState);
                    }
                    newState.ActiveEffects.RemoveAll(spell => spell.TurnsLeft <= 0);

                    if (newState.Boss.Hitpoints > 0)
                    {
                        newState.BossPunch();
                    }

                    if (Hardmode)
                    {
                        newState.Player.Hitpoints--;
                    }
                    if (newState.Player.Hitpoints <= 0)
                    {
                        continue;
                    }
                    
                    foreach (Effect effect in newState.ActiveEffects)
                    {
                        effect.ApplyEffect(newState);
                    }
                    newState.ActiveEffects.RemoveAll(spell => spell.TurnsLeft <= 0);

                    actions.Add(new ExpandAction { Action = keyValuePair.Key, Cost = keyValuePair.Value, Result = newState });
                }
                return actions;
            }

            private string GetActiveEffectsHash()
            {
                return string.Join("_", ActiveEffects.Select(e => e.GetHash()));
            }

            public bool Equals(ISearchNode otherState)
            {
                var state = otherState as GameState;
                return state.StringHash == stringHash;
            }

            public float GetHeuristic(ISearchNode goalState)
            {
                //return 1;
                if (Boss.Hitpoints <= 0)
                {
                    return 0;
                }
                float maxPossibleDamage = 3 + 6.33333333333f; //average cost per round: 73
                float priority = 0; //the cheapest spell we have costs 53, so that's the minimum cost
                //int bossHpScore = (int) Math.Pow(Boss.Hitpoints * 10, 2); //high boss HP is our main criteria, and should also be bigger than the sum of the other positive aspects we subtract later
                float bossHpScore = (Boss.Hitpoints / maxPossibleDamage) * 53 ; //maximum amount of damage times cost of cheapest spell
                float playerHpScore = Player.Hitpoints; //A player with little HP is less likely to survive until the boss drops
                float activeEffectsScore = ActiveEffects.Count; //Effects are all good for the player, so we give a good bonus for every one that's active
                float playerManaScore = Player.Mana / 53f; //Everything else being the same, a player with more mana is better off than a player with less mana. Divide by the cost of the cheapest spell, so that the impact isn't too big

                priority += bossHpScore;
                priority -= playerHpScore;
                priority -= activeEffectsScore;
                priority -= playerManaScore;

                //priority = Math.Max(53, priority); //Going below 1 might eff up everything

                //jesus christ, Past!Chris, this doesn't look admissible at all, or even like a proper heuristic. But I guess it works in this case...
                //todo: find proper heuristic maybe?
                return priority;
            }

            public void CreateHash()
            {
                stringHash = $"{Player.GetHash()}_{Boss.GetHash()}_{GetActiveEffectsHash()}";
            }

            public bool IsGoalState(ISearchNode otherState = null)
            {
                return Player.Hitpoints > 0 && Boss.Hitpoints <= 0;
            }
        }


        /// <inheritdoc />
        protected override object GetSolutionPart1()
        {
            for (int i = 0; i < InputLines.Count; i++)
            {
                string line = InputLines[i];
                int number = line.Split(new[]
                                    {
                                            ": "
                                        },
                                    StringSplitOptions.None)[1].ToInt();
                if (i == 0)
                {
                    bossStartHP = number;
                }
                if (i == 1)
                {
                    bossDamage = number;
                }
            }
            
            var boss = new Combatant {Hitpoints = bossStartHP, Mana = 0};
            var player = new Combatant {Hitpoints = 50, Mana = 500};

            var startState = new GameState {Boss = boss, Player = player, ActiveEffects = new List<Effect>(), Cost = 0, Turn = 0, Actions = new List<object>()};

            var result = new AStar().GetMinimumCost(startState); //1824
            return result.ToString();
        }

        /// <inheritdoc />
        protected override object GetSolutionPart2()
        {
            var boss = new Combatant { Hitpoints = bossStartHP, Mana = 0 };
            var player = new Combatant { Hitpoints = 50, Mana = 500 };

            var startState = new GameState { Boss = boss, Player = player, ActiveEffects = new List<Effect>(), Cost = 0, Turn = 0, Actions = new List<object>(), Hardmode = true };

            var result = new AStar().GetMinimumCost(startState); //1937
            return result.ToString();
        }
    }
}
