using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;

/// <summary>
/// ScriptHookVDotNet script for GTA V Legacy.
/// police3 appears in Los Santos and Davis; sheriff appears in Blaine County.
/// Place this .cs file in the game's "scripts" folder.
/// </summary>
public class RegionalPoliceSpawner : Script
{
    private readonly List<SpawnPoint> spawnPoints = new List<SpawnPoint>
    {
        // Los Santos: police3
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(-450.0f, -350.0f, 35.0f), 145.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(-200.0f, -200.0f, 38.0f), 90.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(250.0f, -500.0f, 40.0f), 200.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(800.0f, -210.0f, 40.0f), 300.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(1180.0f, 290.0f, 80.0f), 120.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(350.0f, -300.0f, 40.0f), 200.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(-600.0f, -900.0f, 25.0f), 270.0f),
        new SpawnPoint("Los Santos", VehicleHash.Police3, new Vector3(100.0f, -800.0f, 30.0f), 90.0f),

        // Blaine County: sheriff
        new SpawnPoint("Blaine County", VehicleHash.Sheriff, new Vector3(-380.0f, 6200.0f, 30.0f), 145.0f),
        new SpawnPoint("Blaine County", VehicleHash.Sheriff, new Vector3(-200.0f, 6300.0f, 32.0f), 90.0f),
        new SpawnPoint("Blaine County", VehicleHash.Sheriff, new Vector3(100.0f, 6500.0f, 35.0f), 200.0f),
        new SpawnPoint("Blaine County", VehicleHash.Sheriff, new Vector3(400.0f, 6400.0f, 33.0f), 300.0f),
        new SpawnPoint("Blaine County", VehicleHash.Sheriff, new Vector3(-500.0f, 6100.0f, 28.0f), 120.0f),

        // Davis: sheriff, as requested for the Davis area
        new SpawnPoint("Davis", VehicleHash.Sheriff, new Vector3(1200.0f, 6500.0f, 65.0f), 145.0f),
        new SpawnPoint("Davis", VehicleHash.Sheriff, new Vector3(1400.0f, 6600.0f, 68.0f), 90.0f),
        new SpawnPoint("Davis", VehicleHash.Sheriff, new Vector3(1600.0f, 6700.0f, 70.0f), 200.0f),
        new SpawnPoint("Davis", VehicleHash.Sheriff, new Vector3(1800.0f, 6400.0f, 72.0f), 250.0f)
    };

    private readonly Dictionary<SpawnPoint, Vehicle> spawnedVehicles = new Dictionary<SpawnPoint, Vehicle>();
    private int timer;

    public RegionalPoliceSpawner()
    {
        Interval = 1000;
        Tick += OnTick;
        Aborted += OnAborted;
    }

    private void OnTick(object sender, EventArgs e)
    {
        timer += Interval;
        if (timer < 10000)
            return;

        timer = 0;
        Vector3 playerPosition = Game.Player.Character.Position;

        foreach (SpawnPoint point in spawnPoints)
        {
            Vehicle currentVehicle;
            if (spawnedVehicles.TryGetValue(point, out currentVehicle))
            {
                if (currentVehicle == null || !currentVehicle.Exists() ||
                    currentVehicle.Position.DistanceTo(playerPosition) > 600.0f)
                {
                    DeleteVehicle(currentVehicle);
                    spawnedVehicles.Remove(point);
                }
                continue;
            }

            // Vehicles are streamed in only when the player is close to their location.
            if (point.Position.DistanceTo(playerPosition) <= 350.0f)
            {
                Vehicle vehicle = CreatePoliceVehicle(point);
                if (vehicle != null)
                    spawnedVehicles.Add(point, vehicle);
            }
        }
    }

    private Vehicle CreatePoliceVehicle(SpawnPoint point)
    {
        Model model = new Model(point.Model);
        if (!model.IsInCdImage || !model.IsValid)
            return null;

        model.Request(5000);
        if (!model.IsLoaded)
            return null;

        Vehicle vehicle = World.CreateVehicle(model, point.Position, point.Heading);
        model.MarkAsNoLongerNeeded();

        if (vehicle == null || !vehicle.Exists())
            return null;

        vehicle.IsPersistent = true;
        vehicle.IsEngineRunning = false;
        vehicle.IsSirenActive = false;
        vehicle.CanTiresBurst = false;
        vehicle.LockStatus = VehicleLockStatus.Unlocked;
        return vehicle;
    }

    private void OnAborted(object sender, EventArgs e)
    {
        foreach (Vehicle vehicle in spawnedVehicles.Values)
            DeleteVehicle(vehicle);
        spawnedVehicles.Clear();
    }

    private void DeleteVehicle(Vehicle vehicle)
    {
        if (vehicle != null && vehicle.Exists())
            vehicle.Delete();
    }

    private sealed class SpawnPoint
    {
        public readonly string Area;
        public readonly VehicleHash Model;
        public readonly Vector3 Position;
        public readonly float Heading;

        public SpawnPoint(string area, VehicleHash model, Vector3 position, float heading)
        {
            Area = area;
            Model = model;
            Position = position;
            Heading = heading;
        }
    }
}
