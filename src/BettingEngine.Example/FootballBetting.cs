using System;
using System.Collections.Generic;
using System.Linq;
using BettingEngine.Betting;

namespace BettingEngine.Example
{
    public class FootballBetting
    {
        private readonly SingleChoiceBet _bet;
        private readonly List<IWager<IResult>> _wagers;

        public FootballBetting()
        {
            _bet = new SingleChoiceBet(PossibleResults.AllWithDescription.Select(_ => _.Result));
            _wagers = new List<IWager<IResult>>();

            Console.WriteLine("Bet has been created.");
            Console.WriteLine();
        }

        public void OfferWagers()
        {
            while (true)
            {
                PrintOdds();
                Console.WriteLine("[q] Quit and show outcomes");

                // Get wager's expected results.
                IResult expectedResult;
                Console.WriteLine("Select a result to place a bet: ");
                while (true)
                {
                    Console.Write("> ");
                    try
                    {
                        string line = Console.ReadLine()?.TrimEnd('\r', '\n');
                        if (line == "q") {
                            PrintOutcomes();
                            return;
                        }

                        var index = int.Parse(line);
                        expectedResult = PossibleResults.AllWithDescription[index - 1].Result;
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Unknown format of input. Use 1, 2 or 3 to select a result.");
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("Unknown index. Use indices 1, 2 or 3 to select a result.");
                    }
                }

                // Get wager's stake value.
                decimal stakeValue;
                Console.WriteLine("Specify stake value: ");
                while (true)
                {
                    Console.Write("> ");
                    try
                    {
                        stakeValue = decimal.Parse(Console.ReadLine()?.TrimEnd('\r', '\n'));
                        break;
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine($"Unknown format of input. Specify a decimal value (e.g. '{42M:F2}').");
                    }
                }

                _wagers.Add(_bet.AddExpectedResults(expectedResult, stakeValue));
                Console.WriteLine("Wager has been created.");
                Console.WriteLine();
            }
        }

        private void PrintOdds()
        {
            Console.WriteLine("Odds:");
            var maximumDescriptionLength = PossibleResults.AllWithDescription.Select(_ => _.Description.Length).Max();
            var index = 0;
            foreach (var (result, description) in PossibleResults.AllWithDescription)
            {
                var odds = _bet.GetOdds(result, result);
                Console.WriteLine(
                    $"[{index + 1}] {description.PadRight(maximumDescriptionLength)} | ${odds:F2} for ${1M:F2}");
                ++index;
            }

        }

      private void PrintOutcomes()
      {
        Console.WriteLine("Outcomes:");
        // var maximumDescriptionLength = PossibleResults.AllWithDescription.Select(_ => _.Description.Length).Max();

        var nameDict = new Dictionary<IResult, string>();
        foreach (var (_result, _description) in PossibleResults.AllWithDescription)
          nameDict.Add(_result, _description);

        foreach (var (result, description) in PossibleResults.AllWithDescription)
        {
          Console.WriteLine($"If the result is {description}, then ");

          foreach (var wager in _wagers)
          {
            var outcome = _bet.GetOutcome(wager, result);
            if (outcome.Type == OutcomeType.Win)
              Console.WriteLine($"The wager of {wager.Stake.Value:F2} on {nameDict[wager.ExpectedResults]} receives outcome of {outcome.Type} for {outcome.Winnings:F2}.");
            else
              Console.WriteLine($"The wager of {wager.Stake.Value:F2} on {nameDict[wager.ExpectedResults]} receives outcome of {outcome.Type}.");
          }
        }
      }

        public static void Main()
        {
            new FootballBetting().OfferWagers();
        }

        public static class PossibleResults
        {
            public static readonly IResult HomeTeamWins = new Result();
            public static readonly IResult VisitingTeamWins = new Result();
            public static readonly IResult Draw = new Result();

            public static (IResult Result, string Description)[] AllWithDescription => new[]
            {
                (HomeTeamWins, "Home Team Wins"),
                (VisitingTeamWins, "Visitor Team Wins"),
                (Draw, "Draw")
            };
        }
    }
}
