namespace RoomBookingSystem.Database;

using Microsoft.EntityFrameworkCore;
using RoomBookingSystem.Database.Entities;

public class RoomBookingSystemDbContext : DbContext{
    public DbSet<Room>? Rooms {get;set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)=>
        optionsBuilder.UseSqlServer("Server=sql_server;Database=CAD001_Demo;User Id=sa;Password=P@ssw0rd;Integrated Security=false;TrustServerCertificate=true;");
    protected override void OnModelCreating(ModelBuilder modelBuilder)=> SeedData.Seed(modelBuilder);
}