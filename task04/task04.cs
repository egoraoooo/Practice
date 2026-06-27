namespace task04;
using System;

public interface ISpaceship
{
    void MoveForward();      // Движение вперед
    void Rotate(int angle);  // Поворот на угол (градусы)
    void Fire();             // Выстрел ракетой
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
}

public class Cruiser : ISpaceship
{
    public void MoveForward()
    {
        Distance += Speed;
    }
    public void Rotate(int angle)
    {
        Angle = (Angle + angle) % 360;
        if (Angle < 0) Angle += 360;
    }

    public void Fire()
    {
        if (Ammo > 0)
        {
            Ammo--;
            Console.WriteLine("ВЫСТРЕЛ ИЗ ОРУДИЙ");
        }
        else
        {
            Console.WriteLine("Нечем стрелять...");
        }
    }

    public int Speed => 50;
    public int FirePower => 100;

    public int Distance {get; set; }
    public int Angle {get; set; }
    public int Ammo {get; set; } = 5;
}

public class Fighter : ISpaceship
{
    public void MoveForward()
    {
        Distance += Speed;
    }
    public void Rotate(int angle)
    {
        Angle = (Angle + angle) % 360;
        if (Angle < 0) Angle += 360;
    }

    public void Fire()
    {
        if (Ammo > 0)
        {
            Ammo--;
            Console.WriteLine("ВЫСТРЕЛ ИЗ ОРУДИЙ");
        }
        else
        {
            Console.WriteLine("Нечем стрелять...");
        }
    }

    public int Speed => 100;
    public int FirePower => 50;

    public int Distance {get; set; }
    public int Angle {get; set; }
    public int Ammo {get; set; } = 10;
}