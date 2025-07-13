using System;
using System.Collections.Generic;

namespace CYS.Models
{
    public class HerdInfo
    {
        public int TotalAnimals { get; set; }
        public int ActiveAnimals { get; set; }
        public int InactiveAnimals { get; set; }
        public double AverageWeight { get; set; }
        public List<CategoryInfo> Categories { get; set; } = new();
        public List<GenderInfo> GenderDistribution { get; set; } = new();
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class CategoryInfo
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class GenderInfo
    {
        public string Gender { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
