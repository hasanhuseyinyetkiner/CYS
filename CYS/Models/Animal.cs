using System;
using System.Collections.Generic;

namespace CYS.Models
{
    public class Animal
    {
        public int id { get; set; }
        public int userId { get; set; }
        public User user { get; set; }
        public string rfidKodu { get; set; }
        public string kupeIsmi { get; set; }
        public string cinsiyet { get; set; }
        public string agirlik { get; set; }
        public int aktif { get; set; }
        public DateTime tarih { get; set; }
        public int kategoriId { get; set; }
        public Kategori kategori { set; get; }
        public List<HayvanKriterUnsur> ozellikler { get; set; }
        public string requestId { set; get; }
        public DateTime sonGuncelleme { get; set; }

        // Properties needed for BirthWeightMeasurementRepository - Read-only properties
        public string Name => kupeIsmi;
        public string EarTag => kupeIsmi;
        public string Type => kategori?.kategoriAdi;

        // Properties for SyncController - Read-only properties
        public string RfidNumber => rfidKodu;
        public string TagNumber => kupeIsmi;
        
        // Properties for ApplicationDbContext - Navigation properties
        public string RfidTag => rfidKodu;
        public virtual ICollection<MilkAnalysisResult> MilkAnalysisResults { get; set; } = new List<MilkAnalysisResult>();
        public virtual ICollection<MilkSample> MilkSamples { get; set; } = new List<MilkSample>();
        public virtual ICollection<AgirlikOlcum> WeightMeasurements { get; set; } = new List<AgirlikOlcum>();
        
        // Properties for DogumController - Writeable with backing fields
        private string _species;
        public string Species 
        { 
            get => _species ?? kategori?.kategoriAdi; 
            set => _species = value; 
        }
        
        private string _gender;
        public string Gender 
        { 
            get => _gender ?? cinsiyet; 
            set => _gender = value; 
        }
        
        public DateTime DateOfBirth 
        { 
            get => tarih; 
            set => tarih = value; 
        }
        
        public int? ParentAnimalId { get; set; } // Anne hayvan ID'si
        public int? FatherAnimalId { get; set; } // Baba hayvan ID'si

        // Map from Hayvan
        public static implicit operator Animal(Hayvan hayvan)
        {
            if (hayvan == null) return null;
            
            return new Animal
            {
                id = hayvan.id,
                userId = hayvan.userId,
                user = hayvan.user,
                rfidKodu = hayvan.rfidKodu,
                kupeIsmi = hayvan.kupeIsmi,
                cinsiyet = hayvan.cinsiyet,
                agirlik = hayvan.agirlik,
                aktif = hayvan.aktif,
                tarih = hayvan.tarih,
                kategoriId = hayvan.kategoriId,
                kategori = hayvan.kategori,
                ozellikler = hayvan.ozellikler,
                requestId = hayvan.requestId,
                sonGuncelleme = hayvan.sonGuncelleme
            };
        }
    }
}