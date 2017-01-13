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
        static int BossStartHP, BossDamage;

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

            public bool Equals(Effect otherEffect)
            {
                return EffectType == otherEffect.EffectType && TurnsLeft == otherEffect.TurnsLeft;
            }
            
            public int CompareTo(Effect other)
            {
                if (EffectType == other.EffectType && Power == other.Power && TurnsLeft == other.TurnsLeft)
                {
                    return 0;
                }
                return -EffectType.CompareTo(other.EffectType);
            }
        }

        class Combatant
        {
            public int Hitpoints = 50;
            public int Mana = 500;

            public bool Equals(Combatant otherCombatant)
            {
                bool equals = Hitpoints == otherCombatant.Hitpoints && Mana == otherCombatant.Mana;
                return equals;
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

            public string ActiveSpellsToString
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
            {
                get
                {
                    return $"Boss HP: {Boss.Hitpoints}\nPlayer HP: {Player.Hitpoints}\nPlayer Mana: {Player.Mana}\nActive Effects: {ActiveEffects.Count}\nTurn: {Turn.ToString("D3")}\n{ActiveSpellsToString}";
                }
                set { throw new NotImplementedException(); } }

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
                int cost = this.Cost;

                var clonedState = new GameState {Player = player, Boss = boss, ActiveEffects = activeEffects, Turn = Turn, Cost = cost, Actions = actions};
                return clonedState;
            }

            public bool CastSpell(Spell spellType)
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

            public void BossPunch()
            {
                var shieldEffect = ActiveEffects.Where(effect => effect.EffectType == Spell.Shield);
                int damage = BossDamage;
                if (shieldEffect.Any())
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
                    
                    GameState newState = this.Clone();
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

                    if (newState.Player.Hitpoints <= 0)
                    {
                        continue;
                    }
                    
                    foreach (Effect effect in newState.ActiveEffects)
                    {
                        effect.ApplyEffect(newState);
                    }
                    newState.ActiveEffects.RemoveAll(spell => spell.TurnsLeft <= 0);

                    actions.Add(new ExpandAction { action = keyValuePair.Key, cost = keyValuePair.Value, result = newState });
                }
                return actions;
            }

            private bool ActiveEffectsEqual(GameState otherState)
            {

                if (ActiveEffects.Count != otherState.ActiveEffects.Count)
                {
                    return false;
                }
                if (ActiveEffects.Count == 0)
                {
                    return true;
                }
                for (int i = 0; i < ActiveEffects.Count; i++)
                {
                    Effect effect = ActiveEffects[i];
                    if (!effect.Equals(otherState.ActiveEffects[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool Equals(ISearchNode goalState)
            {
                var state = goalState as GameState;
                //return VerboseInfo == state.VerboseInfo;
                bool equals = (state.Player.Equals(Player) && state.Boss.Equals(Boss)
                                && ActiveEffectsEqual(state));
                return equals;
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
                //int bossHpScore = (int) Math.Pow(Boss.Hitpoints * 10, 2); //high boss HP is our main criteria, and should also be bigger than the sum of the other positive aspects we substract later
                float bossHpScore = (Boss.Hitpoints / maxPossibleDamage) * 53 ; //maximum amount of damage times cost of cheapest spell
                float playerHpScore = Player.Hitpoints; //A player with little HP is less likely to survive until the boss drops
                float activeEffectsScore = ActiveEffects.Count; //Effects are all good for the player, so we give a good bonus for every one that's active
                float playerManaScore = Player.Mana / 53f; //Everything else being the same, a player with more mana is better off than a player with less mana. Divide by the cost of the cheapest spell, so that the impact isn't too big

                priority += bossHpScore;
                priority -= playerHpScore;
                priority -= activeEffectsScore;
                priority -= playerManaScore;



                //priority = Math.Max(53, priority); //Going below 1 might eff up everything

                return priority;
            }

            public float GetTentativeCost(ISearchNode goalState)
            {
                return Cost + GetHeuristic(goalState);
            }

            public bool IsGoalState(ISearchNode gameState)
            {
                var state = gameState as GameState;
                if (state.Boss.Hitpoints <= 0)
                {
                    Console.WriteLine("STOP!");
                }
                return state.Player.Hitpoints > 0 && state.Boss.Hitpoints <= 0;
            }
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
            
            var boss = new Combatant {Hitpoints = BossStartHP, Mana = 0};
            var player = new Combatant {Hitpoints = 50, Mana = 500};

            var startState = new GameState {Boss = boss, Player = player, ActiveEffects = new List<Effect>(), Cost = 0, Turn = 0, Actions = new List<object>()};
            var goalState = new GameState {Boss = new Combatant {Hitpoints = 0} };

            var result = new AStar().GetMinimumCost(startState, goalState, verbose: true);
            return base.GetSolutionPart1();
        }

        public override string GetSolutionPart2()
        {
            return base.GetSolutionPart2();
        }
    }
}