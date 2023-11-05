namespace RoomBookingSystem.Database.Entities;

public class Room{
    public int Id {get;set;}
    //default = use default value
    //! =  The unary postfix ! operator is the null-forgiving, or null-suppression, operator. In an enabled nullable annotation context, you use the null-forgiving operator to suppress all nullable warnings for the preceding expression. 
    public String Name {get;set;} = default!;
    public String Description {get;set;} = default!;
}