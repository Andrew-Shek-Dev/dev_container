namespace RoomBookingSystem.Database.Entities; //Java = package..

using Microsoft.EntityFrameworkCore; //Java = import..

public static class SeedData{
    public static void Seed(ModelBuilder modelBuilder){
        modelBuilder.Entity<Room>().HasData(
            new Room {Id=1,Name="Room#1",Description="1/F"},
            new Room {Id=2,Name="Room#2",Description="2/F"},
            new Room {Id=3,Name="Room#3",Description="3/F"}
        );
    }
}