-- Script de patrullas policiales GTA 5 Legacy
-- police3 en Los Santos
-- sheriff en Blaine County y partes de David

local policeCars = {}

local function spawnVehicle(modelHash, x, y, z, heading)
    local model = GetHashKey(modelHash)
    RequestModel(model)
    while not HasModelLoaded(model) do
        Citizen.Wait(50)
    end

    local vehicle = CreateVehicle(model, x, y, z, heading, true, false)
    SetVehicleOnGroundProperly(vehicle)
    SetVehicleEngineOn(vehicle, true, true, false)
    return vehicle
end

local function clearPoliceVehicles()
    for _, veh in ipairs(policeCars) do
        if DoesEntityExist(veh) then
            DeleteVehicle(veh)
        end
    end
    policeCars = {}
end

-- Coordenadas de Los Santos para police3
local losSantosCoords = {
    {x = -450.0, y = -350.0, z = 35.0, h = 145.0},
    {x = -200.0, y = -200.0, z = 38.0, h = 90.0},
    {x = 250.0, y = -500.0, z = 40.0, h = 200.0},
    {x = 800.0, y = -210.0, z = 40.0, h = 300.0},
    {x = 1180.0, y = 290.0, z = 80.0, h = 120.0},
    {x = 350.0, y = -300.0, z = 40.0, h = 200.0},
    {x = -600.0, y = -900.0, z = 25.0, h = 270.0},
    {x = 100.0, y = -800.0, z = 30.0, h = 90.0},
    {x = 500.0, y = -1000.0, z = 35.0, h = 180.0},
    {x = 1500.0, y = -500.0, z = 65.0, h = 250.0},
}

-- Coordenadas de Blaine County para sheriff
local blainCountyCoords = {
    {x = -380.0, y = 6200.0, z = 30.0, h = 145.0},
    {x = -200.0, y = 6300.0, z = 32.0, h = 90.0},
    {x = 100.0, y = 6500.0, z = 35.0, h = 200.0},
    {x = 400.0, y = 6400.0, z = 33.0, h = 300.0},
    {x = -500.0, y = 6100.0, z = 28.0, h = 120.0},
}

-- Coordenadas de David (partes de David) para sheriff
local davidCoords = {
    {x = 1200.0, y = 6500.0, z = 65.0, h = 145.0},
    {x = 1400.0, y = 6600.0, z = 68.0, h = 90.0},
    {x = 1600.0, y = 6700.0, z = 70.0, h = 200.0},
    {x = 1800.0, y = 6400.0, z = 72.0, h = 250.0},
}

Citizen.CreateThread(function()
    while true do
        Citizen.Wait(10000)

        clearPoliceVehicles()

        -- Spawnear police3 en Los Santos
        for i = 1, #losSantosCoords do
            local pos = losSantosCoords[i]
            local veh = spawnVehicle("police3", pos.x, pos.y, pos.z, pos.h)
            table.insert(policeCars, veh)
        end

        -- Spawnear sheriff en Blaine County
        for i = 1, #blainCountyCoords do
            local pos = blainCountyCoords[i]
            local veh = spawnVehicle("sheriff", pos.x, pos.y, pos.z, pos.h)
            table.insert(policeCars, veh)
        end

        -- Spawnear sheriff en David
        for i = 1, #davidCoords do
            local pos = davidCoords[i]
            local veh = spawnVehicle("sheriff", pos.x, pos.y, pos.z, pos.h)
            table.insert(policeCars, veh)
        end
    end
end)

print("^2Police Spawner iniciado^7")
