using System;

public class Program {
    public static void Main() {
        // Standard BCrypt for 123456
        Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("123456"));
    }
}
