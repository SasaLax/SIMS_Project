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


INSERT INTO buildings (id, address_id, neighbourhood, location_id, number_of_floors, manager_jmbg) VALUES
('ZGRADA-BG-01', -1, 'Stari Grad', -1, 5, '1505000710011'), -- Upravnik Marko (id: -1)
('ZGRADA-NS-01', -2, 'Limman 3', -2, 8, '2208998715022'),  -- Upravnik Ana (id: -2)
('ZGRADA-NI-01', -3, 'Medijana', -3, 4, '1003001710033'),  -- Upravnik Petar (id: -3)
('ZGRADA-BG-02', -4, 'Vračar', -1, 6, NULL);               -- Zgrada bez upravnika (NULL)

INSERT INTO apartments (id, description, number_of_rooms, max_number_of_residents, building_id) VALUES
(10, 'Dvosoban stan, pogled na ulicu', 2, 4, 'ZGRADA-BG-01'),
(11, 'Garsonjera, dvorišno orijentisana', 1, 2, 'ZGRADA-BG-01'),
(12, 'Trosoban stan sa terasom', 3, 6, 'ZGRADA-BG-01'),
(10, 'Luksuzan penthaus na vrhu', 4, 5, 'ZGRADA-NS-01'),
(11, 'Jednosoban stan, namešten', 1, 2, 'ZGRADA-NS-01'),
(12, 'Dvosoban stan, renoviran', 2, 4, 'ZGRADA-NS-01'),
(101, 'Mali stan u prizemlju', 1, 2, 'ZGRADA-NI-01'),
(102, 'Dvosoban komforan stan', 2, 4, 'ZGRADA-NI-01');