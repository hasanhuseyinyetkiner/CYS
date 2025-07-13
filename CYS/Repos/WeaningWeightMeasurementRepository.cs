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
    public class WeaningWeightMeasurementRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly AnimalRepository _animalRepository;

        public WeaningWeightMeasurementRepository(ApplicationDbContext context, AnimalRepository animalRepository)
        {
            _context = context;
            _animalRepository = animalRepository;
        }

        public async Task<List<WeaningWeightMeasurementDTO>> GetAllWeaningWeightMeasurementsAsync()
        {
            return await _context.WeaningWeightMeasurements
                .Include(w => w.Animal)
                .Select(w => new WeaningWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    WeaningDate = w.WeaningDate,
                    WeaningAge = w.WeaningAge,
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

        public async Task<List<WeaningWeightMeasurementDTO>> GetWeaningWeightMeasurementsByAnimalIdAsync(int animalId)
        {
            return await _context.WeaningWeightMeasurements
                .Include(w => w.Animal)
                .Where(w => w.AnimalId == animalId)
                .Select(w => new WeaningWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    WeaningDate = w.WeaningDate,
                    WeaningAge = w.WeaningAge,
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

        public async Task<List<WeaningWeightMeasurementDTO>> GetWeaningWeightMeasurementsByRfidAsync(string rfid)
        {
            return await _context.WeaningWeightMeasurements
                .Include(w => w.Animal)
                .Where(w => w.RFID == rfid)
                .Select(w => new WeaningWeightMeasurementDTO
                {
                    Id = w.Id,
                    AnimalId = w.AnimalId,
                    Weight = w.Weight,
                    WeaningDate = w.WeaningDate,
                    WeaningAge = w.WeaningAge,
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

        public async Task<WeaningWeightMeasurementDTO> GetWeaningWeightMeasurementByIdAsync(int id)
        {
            var measurement = await _context.WeaningWeightMeasurements
                .Include(w => w.Animal)
                .FirstOrDefaultAsync(w => w.Id == id);

            if (measurement == null)
                return null;

            var dto = new WeaningWeightMeasurementDTO
            {
                Id = measurement.Id,
                AnimalId = measurement.AnimalId,
                Weight = measurement.Weight,
                WeaningDate = measurement.WeaningDate,
                WeaningAge = measurement.WeaningAge,
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

        public async Task<int> AddWeaningWeightMeasurementAsync(WeaningWeightMeasurementDTO measurementDto)
        {
            var measurement = new WeaningWeightMeasurement
            {
                AnimalId = measurementDto.AnimalId,
                Weight = measurementDto.Weight,
                WeaningDate = measurementDto.WeaningDate,
                WeaningAge = measurementDto.WeaningAge,
                MeasurementDate = measurementDto.MeasurementDate,
                RFID = measurementDto.RFID,
                MotherRFID = measurementDto.MotherRFID,
                Notes = measurementDto.Notes,
                UserId = measurementDto.UserId,
                CreatedAt = DateTime.Now
            };

            _context.WeaningWeightMeasurements.Add(measurement);
            await _context.SaveChangesAsync();

            return measurement.Id;
        }

        public async Task<bool> UpdateWeaningWeightMeasurementAsync(int id, WeaningWeightMeasurementDTO measurementDto)
        {
            var measurement = await _context.WeaningWeightMeasurements.FindAsync(id);

            if (measurement == null)
                return false;

            measurement.AnimalId = measurementDto.AnimalId;
            measurement.Weight = measurementDto.Weight;
            measurement.WeaningDate = measurementDto.WeaningDate;
            measurement.WeaningAge = measurementDto.WeaningAge;
            measurement.MeasurementDate = measurementDto.MeasurementDate;
            measurement.RFID = measurementDto.RFID;
            measurement.MotherRFID = measurementDto.MotherRFID;
            measurement.Notes = measurementDto.Notes;
            measurement.UpdatedAt = DateTime.Now;

            _context.WeaningWeightMeasurements.Update(measurement);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteWeaningWeightMeasurementAsync(int id)
        {
            var measurement = await _context.WeaningWeightMeasurements.FindAsync(id);

            if (measurement == null)
                return false;

            _context.WeaningWeightMeasurements.Remove(measurement);
            await _context.SaveChangesAsync();

            return true;
        }
    }
} 