using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode.Extensions;
// ReSharper disable CommentTypo

// ReSharper disable StringLiteralTypo

namespace AdventOfCode.Days._2017
{
    // ReSharper disable once UnusedMember.Global
    class Day07 : Day
	{
		public Day07() : base(2017, 7) {}

		private Branch constructedBranch;
		
		private class Branch
		{
			public override string ToString()
			{
				return $"{Name} - {RecursiveWeight}";
			}

			public string Name;
			public int Weight;
			public int RecursiveWeight => Weight + Children.Sum(c => c.RecursiveWeight);
			public List<string> UnattachedChildren = new List<string>();
			
			public Branch Parent;

			public List<Branch> Children = new List<Branch>();

			public bool HasChildren => Children.Count > 0;
			public bool HasUnattachedChildrenRecursive => UnattachedChildren.Count > 0 || Children.Any(c => c.HasUnattachedChildrenRecursive);

			public Branch(string name, int weight, Branch parent = null)
			{
				Name = name;
				Weight = weight;

				if (parent != null)
				{
					Parent = parent;
				}
			}

			public void AttachChild(Branch child)
			{
				child.Parent = this;
				Children.Add(child);
			}

			public bool AreChildrenBalanced
			{
				get
				{
					if (!HasChildren)
					{
						return true;
					}

					int w = Children[0].RecursiveWeight;
					for (int i = 1; i < Children.Count; i++)
					{
						if (Children[i].RecursiveWeight != w)
						{
							return false;
						}
					}

					return true;
				}
			}
		}
		
		private static readonly Regex Regex = new Regex(@"^(\w+) \((\d+)\)( -> )?(.*)");


		private static List<Branch> ParseInput(List<string> lines)
		{
			var branches = new List<Branch>();
			foreach (string line in lines)
			{
				Match m = Regex.Match(line);

				string name = m.Groups[1].Value;
				int weight = m.Groups[2].Value.ToInt();
				var branch = new Branch(name, weight);
				if (m.Groups[3].Value != string.Empty)
				{
					branch.UnattachedChildren.AddRange(m.Groups[4].Value.Split(',').Select(s => s.Trim()));
				}

				branches.Add(branch);
			}
			return branches;
		}
		
		private static void SearchAndAttachChildren(int parentIndex, List<Branch> branches)
		{
			Branch parent = branches[parentIndex];
			if (!parent.HasUnattachedChildrenRecursive)
			{
				return;
			}

			while (parent.UnattachedChildren.Count == 0)
			{
				parent = parent.Children.First(c => c.HasUnattachedChildrenRecursive);
			}
			
			foreach (string childName in parent.UnattachedChildren)
			{
				int childIndex = branches.FindIndex(b => b.Name == childName);
				Branch child = branches[childIndex];
				parent.AttachChild(child);
				branches.RemoveAt(childIndex);
			}
			parent.UnattachedChildren.Clear();
		}

		private static List<Branch> FindUnbalancedBranches(Branch parent)
		{
			List<Branch> unbalancedBranches = new List<Branch>();
			if (!parent.AreChildrenBalanced)
			{
				unbalancedBranches.Add(parent);
			}

			foreach (Branch child in parent.Children)
			{
				unbalancedBranches.AddRange(FindUnbalancedBranches(child));
			}

			return unbalancedBranches;
		}
		
	    protected override object GetSolutionPart1()
	    {
		    /*
             * --- Day 7: Recursive Circus ---

				Wandering further through the circuits of the computer, you come upon a tower of programs that have gotten themselves into a bit of trouble. A recursive algorithm
				has gotten out of hand, and now they're balanced precariously in a large tower.

				One program at the bottom supports the entire tower. It's holding a large disc, and on the disc are balanced several more sub-towers. At the bottom of these
				sub-towers, standing on the bottom disc, are other programs, each holding their own disc, and so on. At the very tops of these sub-sub-sub-...-towers, many programs
				stand simply keeping the disc below them balanced but with no disc of their own.

				You offer to help, but first you need to understand the structure of these towers. You ask each program to yell out their name, their weight, and (if they're
				holding a disc) the names of the programs immediately above them balancing on that disc. You write this information down (your puzzle input). Unfortunately, in
				their panic, they don't do this in an orderly fashion; by the time you're done, you're not sure which program gave which information.

				For example, if your list is the following:

				pbga (66)
				xhth (57)
				ebii (61)
				havc (66)
				ktlj (57)
				fwft (72) -> ktlj, cntj, xhth
				qoyq (66)
				padx (45) -> pbga, havc, qoyq
				tknk (41) -> ugml, padx, fwft
				jptl (61)
				ugml (68) -> gyxo, ebii, jptl
				gyxo (61)
				cntj (57)

				...then you would be able to recreate the structure of the towers that looks like this:

				                gyxo
				              /     
				         ugml - ebii
				       /      \     
				      |         jptl
				      |        
				      |         pbga
				     /        /
				tknk --- padx - havc
				     \        \
				      |         qoyq
				      |             
				      |         ktlj
				       \      /     
				         fwft - cntj
				              \     
				                xhth

				In this example, tknk is at the bottom of the tower (the bottom program), and is holding up ugml, padx, and fwft. Those programs are, in turn, holding up other
				programs; in this example, none of those programs are holding up any other programs, and are all the tops of their own towers. (The actual tower balancing in front
				of you is much larger.)

				Before you're ready to help them, you need to make sure your information is correct. What is the name of the bottom program?


             */

		    #region Testrun

		    List<string> testInputLines = new List<string>
		    {
			    "pbga (66)",
			    "xhth (57)",
			    "ebii (61)",
			    "havc (66)",
			    "ktlj (57)",
			    "fwft (72) -> ktlj, cntj, xhth",
			    "qoyq (66)",
			    "padx (45) -> pbga, havc, qoyq",
			    "tknk (41) -> ugml, padx, fwft",
			    "jptl (61)",
			    "ugml (68) -> gyxo, ebii, jptl",
			    "gyxo (61)",
			    "cntj (57)",
		    };

		    Console.WriteLine("Test");
		    List<Branch> testBranches = ParseInput(testInputLines);

		    testBranches = testBranches.OrderByDescending(t => t.UnattachedChildren.Count).ThenBy(t => t.Name).ToList();
		    while (testBranches.Count > 1)
		    {
			    int parentIndex = testBranches.FindIndex(b => b.HasUnattachedChildrenRecursive);
			    SearchAndAttachChildren(parentIndex, testBranches);
		    }

		    if (testBranches[0].Name != "tknk")
		    {
			    throw new Exception($"Test failed! Expected: tknk, Actual: {testBranches[0].Name}");
		    }
		    
		    #endregion

		    List<Branch> branches = ParseInput(InputLines).OrderByDescending(b => b.UnattachedChildren.Count).ToList();
		    while (branches.Count > 1)
		    {
			    int parentIndex = branches.FindIndex(b => b.HasUnattachedChildrenRecursive);
			    SearchAndAttachChildren(parentIndex, branches);
		    }

		    constructedBranch = branches[0];
		    return constructedBranch.Name;
	    }

	    protected override object GetSolutionPart2()
	    {
		    /*
		     * --- Part Two ---

				The programs explain the situation: they can't get down. Rather, they could get down, if they weren't expending all of their energy trying to keep the tower
				balanced. Apparently, one program has the wrong weight, and until it's fixed, they're stuck here.

				For any program holding a disc, each program standing on that disc forms a sub-tower. Each of those sub-towers are supposed to be the same weight, or the disc
				itself isn't balanced. The weight of a tower is the sum of the weights of the programs in that tower.

				In the example above, this means that for ugml's disc to be balanced, gyxo, ebii, and jptl must all have the same weight, and they do: 61.

				However, for tknk to be balanced, each of the programs standing on its disc and all programs above it must each match. This means that the following sums must all
				be the same:

				    ugml + (gyxo + ebii + jptl) = 68 + (61 + 61 + 61) = 251
				    padx + (pbga + havc + qoyq) = 45 + (66 + 66 + 66) = 243
				    fwft + (ktlj + cntj + xhth) = 72 + (57 + 57 + 57) = 243

				As you can see, tknk's disc is unbalanced: ugml's stack is heavier than the other two. Even though the nodes above ugml are balanced, ugml itself is too heavy:
				it needs to be 8 units lighter for its stack to weigh 243 and keep the towers balanced. If this change were made, its weight would be 60.

				Given that exactly one program is the wrong weight, what would its weight need to be to balance the entire tower?

		     */
		    
		    List<Branch> unbalanced = FindUnbalancedBranches(constructedBranch);
		    
		    //There's probably a much better way to just find the one unbalanced branch that's highest in the tree, but I'm tired and my brain is not working right today.
		    //So I just get all unbalanced ones, remove the root, then remove all that have children in here, and are hopefully left with one.
		    unbalanced.RemoveAll(b => b.Parent == null);
		    while (unbalanced.Any(b => unbalanced.Any(x => x.Name == b.Parent.Name)))
		    {
			    unbalanced.RemoveAll(b => unbalanced.Any(x => x.Parent.Name == b.Name));
		    }

		    if (unbalanced.Count > 1)
		    {
			    throw new Exception("Something went horribly wrong.");
		    }
		    
		    Branch evilBranch = unbalanced[0];
		    if (evilBranch.Children.Count <= 2)
		    {
			    throw new Exception("This shouldn't happen, this way we can't unambiguously say which one program is wrong.");
		    }

		    //if only one program has the wrong weight, and all should have the same weight, there can only be two possible amounts, so this should give us a list with two items.
		    List<int> uniqueWeights = evilBranch.Children.Select(c => c.RecursiveWeight).Distinct().ToList();
		    
		    int countOne = evilBranch.Children.Count(c => c.RecursiveWeight == uniqueWeights[0]);
		    //if the first weight only appears once, it must be the wrong one.
		    if (countOne == 1)
		    {
			    Branch child = evilBranch.Children.Find(c => c.RecursiveWeight == uniqueWeights[0]);
			    return child.Weight + (uniqueWeights[1] - uniqueWeights[0]);
		    }
		    else
		    {
			    Branch child = evilBranch.Children.Find(c => c.RecursiveWeight == uniqueWeights[1]);
			    return child.Weight + (uniqueWeights[0] - uniqueWeights[1]);
		    }
	    }
	}
}
