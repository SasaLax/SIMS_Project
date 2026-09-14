TRUNCATE TABLE requests, apartments, buildings, addresses, locations, users RESTART IDENTITY CASCADE;

INSERT INTO users (id, jmbg, email, password, name, surname, phone_number, user_type) VALUES
(-1, '1505000710011', 'marko@gmail.com', 'marko123', 'Marko', 'Marković', '+38765111222', 'Manager'),
(-2, '2208998715022', 'ana@gmail.com', 'ana123', 'Ana', 'Anić', '+38765333444', 'Manager'),
(-3, '1003001710033', 'petar@gmail.com', 'petar123', 'Petar', 'Petrović', '+38765555666', 'Manager'),
(-4, '3011999710044', 'jovana@gmail.com', 'jovana123', 'Jovana', 'Jovanović', '+38765777888', 'Administrator'),
(-5, '2501002710055', 'stefan@gmail.com', 'stefan123', 'Stefan', 'Stefanović', '+38765999000', 'Resident'),
(-6, '0807000710066', 'milica@gmail.com', 'milica123', 'Milica', 'Milić', '+38766111222', 'Resident'),
(-7, '1812997710077', 'nikola@gmail.com', 'nikola123', 'Nikola', 'Nikolić', '+38766333444', 'Resident'),
(-8, '0509001710088', 'ivana@gmail.com', 'ivana123', 'Ivana', 'Ivanović', '+38766555666', 'Resident');

INSERT INTO locations (id, city, country) VALUES
(-1, 'Beograd', 'Srbija'),
(-2, 'Novi Sad', 'Srbija'),
(-3, 'Niš', 'Srbija');

INSERT INTO addresses (id, street, number) VALUES
(-1, 'Knez Mihailova', 10),
(-2, 'Bulevar Oslobođenja', 45),
(-3, 'Cara Dušana', 122),
(-4, 'Njegoševa', 8);


-- Insert buildings and return their generated IDs
WITH inserted_buildings AS (
    INSERT INTO buildings (id, building_code, address_id, neighbourhood, location_id, number_of_floors, manager_jmbg, status) 
    VALUES
        (-1, 'ZGRADA-BG-01', -1, 'Stari Grad', -1, 5, '1505000710011', 'Approved'),
        (-2, 'ZGRADA-NS-01', -2, 'Liman 3',    -2, 8, '2208998715022', 'Approved'),
        (-3, 'ZGRADA-NI-01', -3, 'Medijana',   -3, 4, '1003001710033', 'Approved'),
        (-4, 'ZGRADA-BG-02', -4, 'Vračar',     -1, 6, NULL,            'Approved')
    ON CONFLICT (id) DO UPDATE 
    SET 
        building_code    = EXCLUDED.building_code,
        neighbourhood    = EXCLUDED.neighbourhood,
        address_id       = EXCLUDED.address_id,
        location_id      = EXCLUDED.location_id,
        number_of_floors = EXCLUDED.number_of_floors,
        manager_jmbg     = EXCLUDED.manager_jmbg,
        status           = EXCLUDED.status
    RETURNING id, building_code
),
inserted_apartments AS (
    INSERT INTO apartments (apartment_number, description, number_of_rooms, max_number_of_residents, building_id)
    SELECT ap_data.apartment_number, ap_data.description, ap_data.number_of_rooms, ap_data.max_number_of_residents, ib.id
    FROM ( VALUES
        (1, 'Dvosoban stan, pogled na ulicu', 2, 4, 'ZGRADA-BG-01'),
        (2, 'Garsonjera, dvorišno orijentisana', 1, 2, 'ZGRADA-BG-01'),
        (3, 'Trosoban stan sa velikom terasom', 3, 5, 'ZGRADA-BG-01'),

        (1, 'Luksuzan četvorosoban penthaus na vrhu', 4, 6, 'ZGRADA-NS-01'),
        (2, 'Jednosoban stan, moderno namešten', 1, 1, 'ZGRADA-NS-01'),
        (3, 'Komforan trosoban stan, renoviran', 3, 4, 'ZGRADA-NS-01'),

        (1, 'Mala garsonjera u prizemlju', 1, 2, 'ZGRADA-NI-01'),
        (2, 'Dvosoban stan sa balkonom', 2, 3, 'ZGRADA-NI-01'),
        (3, 'Četvorosoban porodični stan', 4, 7, 'ZGRADA-NI-01'),

        (1, 'Jednoiposoban stan, tih i svetao', 2, 3, 'ZGRADA-BG-02'),
        (2, 'Petosoban dvoetažni stan', 5, 8, 'ZGRADA-BG-02'),
        (3, 'Trosoban stan sa pogledom na park', 3, 5, 'ZGRADA-BG-02')
    ) AS ap_data(apartment_number, description, number_of_rooms, max_number_of_residents, building_code)
    JOIN inserted_buildings ib ON ap_data.building_code = ib.building_code
    RETURNING id, apartment_number, building_id
)
-- Now insert requests using the generated building IDs
INSERT INTO requests (user_id, building_id, apartment_number, created_at, status, rejection_reason, handled_by, handled_at)
SELECT
    req_data.user_id, ib.id, req_data.apartment_number, req_data.created_at, req_data.status, req_data.rejection_reason, req_data.handled_by, req_data.handled_at
FROM ( VALUES
    (-5, 'ZGRADA-BG-01', 10, '2025-11-01 10:00:00'::timestamp, 'Pending', NULL, NULL, NULL),
    (-6, 'ZGRADA-BG-01', 11, '2025-11-02 11:00:00'::timestamp, 'Approved', NULL, -1, '2025-11-03 09:00:00'::timestamp),
    (-7, 'ZGRADA-NS-01', 12, '2025-11-04 12:30:00'::timestamp, 'Rejected', 'Apartment already assigned', -2, '2025-11-05 14:00:00'::timestamp)
) AS req_data(user_id, building_code, apartment_number, created_at, status, rejection_reason, handled_by, handled_at)
JOIN buildings ib ON req_data.building_code = ib.building_code;