namespace task04tests;
using System;
using System.Collections.Generic;
using Xunit;
using task04;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Fighter_ShouldBeWeakerThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.FirePower < cruiser.FirePower);
    }

    [Fact]
    public void Cruiser_Fire_ShouldDecreaseAmmo()
    {
        var cruiser = new Cruiser();
        int before = cruiser.Ammo;
        cruiser.Fire();
        Assert.Equal(before - 1, cruiser.Ammo);
    }

    [Fact]
    public void Cruiser_FireWhenEmptyAmmo_ShouldNotGoNegative()
    {
        var cruiser = new Cruiser();
        while(cruiser.Ammo > 0)
        {
            cruiser.Fire();
        }
        cruiser.Fire();
        Assert.Equal(0, cruiser.Ammo);
    }
    [Fact]
    public void Fighter_MoveForward_ShouldIncreaseDistance()
    {
        var fighter = new Fighter();
        fighter.MoveForward();
        Assert.Equal(100, fighter.Distance);
    }

    [Fact]
    public void Fighter_NegativeAngle_ShoulWrapCorrect()
    {
        var fighter = new Fighter();
        fighter.Rotate(-90);
        Assert.Equal(270, fighter.Angle);
    }
    [Fact]
    public void AllShips_ShouldImplementInterface()
    {
        List<ISpaceship> fleet = new List<ISpaceship>
        {
            new Cruiser(),
            new Fighter()
        };
    
        foreach (var ship in fleet)
        {
            ship.MoveForward();
            ship.Rotate(45);
            ship.Fire();
        }
    }
}