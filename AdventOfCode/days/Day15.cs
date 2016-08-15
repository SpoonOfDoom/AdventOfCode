using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Days;
using AdventOfCode.Extensions;

namespace AdventOfCode.Days
{
    class Ingredient
    {
        public string Name;
        public int Capacity;
        public int Durability;
        public int Flavor;
        public int Texture;
        public int Calories;

        public static Dictionary<string, Ingredient> Ingredients = new Dictionary<string, Ingredient>();
    }

    class Cookie
    {
        private Dictionary<string, int> ingredients; //Inredients and their amounts

        public int TotalCapacity { get { return ingredients.Select(i => Ingredient.Ingredients[i.Key].Capacity * i.Value).Sum(); } }
        public int TotalDurability { get { return ingredients.Select(i => Ingredient.Ingredients[i.Key].Durability * i.Value).Sum(); } }
        public int TotalFlavor { get { return ingredients.Select(i => Ingredient.Ingredients[i.Key].Flavor * i.Value).Sum(); } }
        public int TotalTexture { get { return ingredients.Select(i => Ingredient.Ingredients[i.Key].Texture * i.Value).Sum(); } }
        public int TotalCalories { get { return ingredients.Select(i => Ingredient.Ingredients[i.Key].Calories * i.Value).Sum(); } }
        public int TotalAmount { get { return ingredients.Values.Sum(); } }

        public void AddIngredient(string ingredient, int amount)
        {
            if (ingredients.Values.Sum() > 100)
            {
                throw new Exception("Too much stuff in this cookie!");
            }

            ingredients[ingredient] = amount;
        }
        public int GetScore()
        {
            if (TotalCapacity <= 0 || TotalDurability <= 0 || TotalFlavor <= 0 || TotalTexture <= 0)
            {
                return 0;
            }
            return TotalCapacity*TotalDurability*TotalFlavor*TotalTexture;
        }

        public Cookie()
        {
            ingredients = new Dictionary<string, int>();
        }
        public Cookie(Cookie c)
        {
            ingredients = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> kvp in c.ingredients)
            {
                AddIngredient(kvp.Key, kvp.Value);
            }
        }
    }

    public class Day15 : Day
    {
        public Day15() : base(15) { }

        private Regex regLine = new Regex(@"^(\w+)[^0-9-]+(-?\d+)[^0-9-]+(-?\d+)[^0-9-]+(-?\d+)[^0-9-]+(-?\d+)[^0-9-]+(-?\d+)");
        private List<Ingredient> ingredients = new List<Ingredient>();

        private Ingredient parseLine(string line)
        {
            var groups = regLine.Match(line).Groups;
            var i = new Ingredient
            {
                Name = groups[1].Value,
                Capacity = groups[2].Value.ToInt(),
                Durability = groups[3].Value.ToInt(),
                Flavor = groups[4].Value.ToInt(),
                Texture = groups[5].Value.ToInt(),
                Calories = groups[6].Value.ToInt()
            };
            Ingredient.Ingredients.Add(i.Name, i);
            return i;
        }

        private int getScore(Cookie cookie, List<Ingredient> remainingIngredients)
        {
            if (cookie.TotalAmount == 100)
            {
                return cookie.GetScore();
            }

            List<int> scores = new List<int>();
            foreach (var ingredient in remainingIngredients)
            {
                if (remainingIngredients.Count == 1)
                {
                    Cookie c = new Cookie(cookie);

                    c.AddIngredient(ingredient.Name, 100 - c.TotalAmount);
                    scores.Add(getScore(c, remainingIngredients.Where(ing => ing != ingredient).ToList()));
                }
                else
                {
                    for (int i = 1; i <= 100 - cookie.TotalAmount; i++)
                    {
                        Cookie c = new Cookie(cookie);

                        c.AddIngredient(ingredient.Name, i);
                        scores.Add(getScore(c, remainingIngredients.Where(ing => ing != ingredient).ToList()));
                    }
                }
            }
            return scores.Max();
        }

        public override string GetSolutionPart1()
        {
            foreach (string line in inputLines)
            {
                ingredients.Add(parseLine(line));
            }
            
            Cookie c = new Cookie();

            var score = getScore(c, ingredients);
            return score.ToString();
        }

        public override string GetSolutionPart2()
        {
            return string.Empty;
        }
    }
}