using CYS.Data;
using CYS.Models;
using CYS.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYS.Repos
{
    public class BirthWeightMeasurementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AnimalRepository _animalRepository;

        public BirthWeightMeasurementRepository(ApplicationDbContext context, AnimalRepository animalRepository)
        {
            _context = context;
            _animalRepository = animalRepository;
        }

        public async Task<List<BirthWeightMeasurementDTO>> GetAllBirthWeightMeasurementsAsync()
        {
            return await _context.BirthWeightMeasurements
                .Include(w => w.Animal)
                .Select(w => new BirthWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    BirthDate = w.BirthDate,
                    BirthPlace = w.BirthPlace,
                    MeasurementDate = w.MeasurementDate,
                    RFID = w.RFID,
                    MotherRFID = w.MotherRFID,
                    Notes = w.Notes,
                    UserId = w.UserId,
                    AnimalName = w.Animal != null ? w.Animal.Name : null,
                    AnimalEarTag = w.Animal != null ? w.Animal.EarTag : null,
                    AnimalType = w.Animal != null ? w.Animal.Type : null
                })
                .OrderByDescending(w => w.MeasurementDate)
                .ToListAsync();
        }

        public async Task<List<BirthWeightMeasurementDTO>> GetBirthWeightMeasurementsByAnimalIdAsync(int animalId)
        {
            return await _context.BirthWeightMeasurements
                .Include(w => w.Animal)
                .Where(w => w.AnimalId == animalId)
                .Select(w => new BirthWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    BirthDate = w.BirthDate,
                    BirthPlace = w.BirthPlace,
                    MeasurementDate = w.MeasurementDate,
                    RFID = w.RFID,
                    MotherRFID = w.MotherRFID,
                    Notes = w.Notes,
                    UserId = w.UserId,
                    AnimalName = w.Animal != null ? w.Animal.Name : null,
                    AnimalEarTag = w.Animal != null ? w.Animal.EarTag : null,
                    AnimalType = w.Animal != null ? w.Animal.Type : null
                })
                .OrderByDescending(w => w.MeasurementDate)
                .ToListAsync();
        }

        public async Task<List<BirthWeightMeasurementDTO>> GetBirthWeightMeasurementsByRfidAsync(string rfid)
        {
            return await _context.BirthWeightMeasurements
                .Include(w => w.Animal)
                .Where(w => w.RFID == rfid)
                .Select(w => new BirthWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    BirthDate = w.BirthDate,
                    BirthPlace = w.BirthPlace,
                    MeasurementDate = w.MeasurementDate,
                    RFID = w.RFID,
                    MotherRFID = w.MotherRFID,
                    Notes = w.Notes,
                    UserId = w.UserId,
                    AnimalName = w.Animal != null ? w.Animal.Name : null,
                    AnimalEarTag = w.Animal != null ? w.Animal.EarTag : null,
                    AnimalType = w.Animal != null ? w.Animal.Type : null
                })
                .OrderByDescending(w => w.MeasurementDate)
                .ToListAsync();
        }

        public async Task<BirthWeightMeasurementDTO> GetBirthWeightMeasurementByIdAsync(int id)
        {
            var measurement = await _context.BirthWeightMeasurements
                .Include(w => w.Animal)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (measurement == null)
                return null;

            var dto = new BirthWeightMeasurementDTO
            {
                Id = measurement.Id,
                AnimalId = measurement.AnimalId,
                Weight = measurement.Weight,
                BirthDate = measurement.BirthDate,
                BirthPlace = measurement.BirthPlace,
                MeasurementDate = measurement.MeasurementDate,
                RFID = measurement.RFID,
                MotherRFID = measurement.MotherRFID,
                Notes = measurement.Notes,
                UserId = measurement.UserId,
                AnimalName = measurement.Animal?.Name,
                AnimalEarTag = measurement.Animal?.EarTag,
                AnimalType = measurement.Animal?.Type
            };

            // Anne hayvan bilgilerini ekleyebiliriz
            if (!string.IsNullOrEmpty(measurement.MotherRFID))
            {
                var motherAnimal = await _animalRepository.GetAnimalByRFIDAsync(measurement.MotherRFID);
                if (motherAnimal != null)
                {
                    dto.MotherName = motherAnimal.Name;
                    dto.MotherEarTag = motherAnimal.EarTag;
                }
            }

            return dto;
        }

        public async Task<int> AddBirthWeightMeasurementAsync(BirthWeightMeasurementDTO measurementDto)
        {
            var measurement = new BirthWeightMeasurement
            {
                AnimalId = measurementDto.AnimalId,
                Weight = measurementDto.Weight,
                BirthDate = measurementDto.BirthDate,
                BirthPlace = measurementDto.BirthPlace,
                MeasurementDate = measurementDto.MeasurementDate,
                RFID = measurementDto.RFID,
                MotherRFID = measurementDto.MotherRFID,
                Notes = measurementDto.Notes,
                UserId = measurementDto.UserId,
                CreatedAt = DateTime.Now
            };

            _context.BirthWeightMeasurements.Add(measurement);
            await _context.SaveChangesAsync();

            return measurement.Id;
        }

        public async Task<bool> UpdateBirthWeightMeasurementAsync(int id, BirthWeightMeasurementDTO measurementDto)
        {
            var measurement = await _context.BirthWeightMeasurements.FindAsync(id);

            if (measurement == null)
                return false;

            measurement.AnimalId = measurementDto.AnimalId;
            measurement.Weight = measurementDto.Weight;
            measurement.BirthDate = measurementDto.BirthDate;
            measurement.BirthPlace = measurementDto.BirthPlace;
            measurement.MeasurementDate = measurementDto.MeasurementDate;
            measurement.RFID = measurementDto.RFID;
            measurement.MotherRFID = measurementDto.MotherRFID;
            measurement.Notes = measurementDto.Notes;
            measurement.UpdatedAt = DateTime.Now;

            _context.BirthWeightMeasurements.Update(measurement);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteBirthWeightMeasurementAsync(int id)
        {
            var measurement = await _context.BirthWeightMeasurements.FindAsync(id);

            if (measurement == null)
                return false;

            _context.BirthWeightMeasurements.Remove(measurement);
            await _context.SaveChangesAsync();

            return true;
        }
    }
} 