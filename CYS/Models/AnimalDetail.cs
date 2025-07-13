using System;
using System.Collections.Generic;

namespace CYS.Models
{
    public class AnimalDetail
    {
        public int Id { get; set; }
        public string RfidTag { get; set; } = string.Empty;
        public string EarTag { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Breed { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }
        public double? InitialWeight { get; set; }
        public double? CurrentWeight { get; set; }
        public string Status { get; set; } = "active";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<WeightMeasurement> WeightHistory { get; set; } = new();
        public List<MilkAnalysisResult> MilkAnalysisResults { get; set; } = new();
        public List<MilkSample> MilkSamples { get; set; } = new();

        // Static method to convert from Animal
        public static AnimalDetail FromAnimal(Animal animal)
        {
            return new AnimalDetail
            {
                Id = animal.id,
                RfidTag = animal.rfidKodu,
                EarTag = animal.kupeIsmi,
                Name = animal.kupeIsmi,
                Gender = animal.cinsiyet,
                Status = animal.aktif == 1 ? "active" : "inactive",
                CreatedAt = animal.tarih,
                UpdatedAt = animal.sonGuncelleme,
                UserId = animal.userId,
                CategoryId = animal.kategoriId,
                CategoryName = animal.kategori?.kategoriAdi
            };
        }
    }
}
