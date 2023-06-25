using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace SwEngHomework.Commissions {
    public class CommissionCalculator : ICommissionCalculator {
        public IDictionary<string, double> CalculateCommissionsByAdvisor(string jsonInput) {
            var results = new ConcurrentDictionary<string, double>();

            var data = JsonConvert.DeserializeObject<Root>(jsonInput);

            if (data == null) {
                return results;
            }
            // Convert list to dictionary to make lookups more efficient
            var advisors = data.advisors.ToDictionary(x => x.name, x => x.level);

            // Default the result to 0 for all advisors
            foreach (var advisor in advisors) {
                results.TryAdd(advisor.Key, 0);
            }

            // Process the accounts in parallel
            Parallel.ForEach(data.accounts, account => {
                var level = advisors[account.advisor];
                var commissionPercentage = GetCommissionPercentageByLevel(level);
                var basisPoints = GetBasisPoints(account.presentValue);

                results.AddOrUpdate(account.advisor, 0, (key, oldValue) => oldValue + account.presentValue * basisPoints * commissionPercentage / 100);
            });

            // Round all the results
            var result = new Dictionary<string, double>();
            foreach (var advisor in results) {
                result.Add(advisor.Key, Math.Round(advisor.Value, 2));
            }

            return result;
        }

        private double GetCommissionPercentageByLevel(string level) {
            return level switch {
                "Senior" => 1.0,
                "Experienced" => 0.5,
                "Junior" => 0.25,
                _ => 0.25,
            };
        }
        private double GetBasisPoints(double amount) {
            if (amount >= 100000) {
                return 0.07;
            }
            else if (amount >= 50000) {
                return 0.06;
            }
            else {
                return 0.05;
            }
        }
    }

    public class Account {
        public string advisor { get; set; }
        public double presentValue { get; set; }
    }

    public class Advisor {
        public string name { get; set; }
        public string level { get; set; }
    }

    public class Root {
        public List<Advisor> advisors { get; set; }
        public ConcurrentBag<Account> accounts { get; set; }
    }
}
