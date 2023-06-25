namespace SwEngHomework.DescriptiveStatistics {
    public class StatsCalculator : IStatsCalculator {

        /// <summary>
        /// Calculates the average, median, and range of a list of contributions
        /// Only valid contributions will be considered in the calculations
        /// </summary>
        /// <param name="semicolonDelimitedContributions">A list of contributions delimited by semicolon.</param>
        /// <returns>A Stats object with average, median, and range.</returns>
        public Stats Calculate(string semicolonDelimitedContributions) {
            // Instantiate a new Stats object to return
            // Default to zero for all values in case nothing is found to calculate
            var result = new Stats() {
                Average = 0,
                Median = 0,
                Range = 0
            };

            // Parse the input string into a list of doubles
            var runningTotal = 0d;
            var contributionStrings = semicolonDelimitedContributions.Split(';');
            var contributions = new List<double>();

            foreach (var contributionAsString in contributionStrings) {
                if (double.TryParse(contributionAsString.Replace(" ", string.Empty).Replace("$", string.Empty), out double contribution)) {
                    // Only increment the runningTotal if we were able to parse the contribution
                    runningTotal += contribution;
                    contributions.Add(contribution);
                }
            }

            // Sort the list to make the median calculation easier
            contributions = contributions.OrderBy(c => c).ToList();

            if (contributions.Count > 0) {
                result.Average = Math.Round(runningTotal / contributions.Count, 2);
                result.Range = Math.Round(contributions.Max() - contributions.Min(), 2);

                if (contributions.Count % 2 == 0) {
                    // Even number of contributions; so we must average the two middle values
                    var firstMiddleValue = contributions.ElementAt((contributions.Count / 2) - 1);
                    var secondMiddleValue = contributions.ElementAt(contributions.Count / 2);
                    result.Median = Math.Round((firstMiddleValue + secondMiddleValue) / 2d, 2);
                }
                else {
                    // Odd number of contributions; so we can just take the middle value
                    result.Median = contributions.ElementAt(contributions.Count / 2);
                }
            }

            // Return the result
            return result;
        }
    }
}
