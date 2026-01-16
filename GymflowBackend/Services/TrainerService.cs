using Microsoft.EntityFrameworkCore;
using GymFlow_Backend_Phase4E.Data;
using GymFlow_Backend_Phase4E.Models;

namespace GymFlow_Backend_Phase4E.Services
{
    public class TrainerService
    {
        private readonly GymFlowDbContext _db;

        public TrainerService(GymFlowDbContext db)
        {
            _db = db;
        }

        // ============================
        // CREATE TRAINER PROFILE
        // ============================
        public async Task<Trainer> CreateTrainer(
            string userId,
            string specialty,
            string bio)
        {
            var trainer = new Trainer
            {
                UserId = userId,
                Specialty = specialty,
                Bio = bio
            };

            _db.Trainers.Add(trainer);
            await _db.SaveChangesAsync();
            return trainer;
        }

        // ============================
        // GET TRAINER BY USER ID
        // ============================
        public async Task<Trainer?> GetByUserId(string userId)
        {
            return await _db.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.UserId == userId);
        }

        // ============================
        // GET ALL TRAINERS
        // ============================
        public async Task<List<Trainer>> GetAll()
        {
            return await _db.Trainers
                .Include(t => t.User)
                .ToListAsync();
        }

        // ============================
        // GET TRAINER BY TRAINER ID
        // ============================
        public async Task<Trainer?> GetById(int id)
        {
            return await _db.Trainers
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // ============================
        // SEARCH TRAINERS BY SPECIALTY
        // ============================
        public async Task<List<Trainer>> Search(string specialty)
        {
            return await _db.Trainers
                .Where(t => t.Specialty.Contains(specialty))
                .Include(t => t.User)
                .ToListAsync();
        }

        // ============================
        // UPDATE TRAINER (PROFILE IMAGE, BIO, ETC.)
        // ============================
        public async Task UpdateTrainer(Trainer trainer)
        {
            _db.Trainers.Update(trainer);
            await _db.SaveChangesAsync();
        }
    }
}
