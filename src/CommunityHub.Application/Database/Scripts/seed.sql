INSERT INTO users (id, jmbg, email, password, name, surname, phone_number, user_type) VALUES
(-1, '1505000710011', 'marko@gmail.com', 'marko123', 'Marko', 'Marković', '+38765111222', 'Manager'),
(-2, '2208998715022', 'ana@gmail.com', 'ana123', 'Ana', 'Anić', '+38765333444', 'Manager'),
(-3, '1003001710033', 'petar@gmail.com', 'petar123', 'Petar', 'Petrović', '+38765555666', 'Manager'),
(-4, '3011999710044', 'jovana@gmail.com', 'jovana123', 'Jovana', 'Jovanović', '+38765777888', 'Administrator'),
(-5, '2501002710055', 'stefan@gmail.com', 'stefan123', 'Stefan', 'Stefanović', '+38765999000', 'Resident'),
(-6, '0807000710066', 'milica@gmail.com', 'milica123', 'Milica', 'Milić', '+38766111222', 'Resident'),
(-7, '1812997710077', 'nikola@gmail.com', 'nikola123', 'Nikola', 'Nikolić', '+38766333444', 'Resident'),
(-8, '0509001710088', 'ivana@gmail.com', 'ivana123', 'Ivana', 'Ivanović', '+38766555666', 'Resident');

INSERT INTO posts (id, title, content, created_at, user_id) VALUES
(-1, 'Moj prvi post', 'Ovo je moj prvi post na platformi!', '2024-01-15 10:30:00', -1),
(-2, 'Zanimljiv dan', 'Danas sam imao vrlo zanimljiv dan na fakultetu.', '2024-01-17 09:15:00', -1),
(-3, 'Programiranje', 'Učim C# i WPF, jako je interesantno!', '2024-01-20 14:30:00', -1),
(-4, 'Dobrodošli', 'Drago mi je što sam ovde.', '2024-01-16 14:20:00', -2),
(-5, 'Vikend planovi', 'Šta planirate za vikend?', '2024-01-19 16:00:00', -2),
(-6, 'Nova knjiga', 'Počela sam da čitam novu knjigu o programiranju.', '2024-01-22 11:00:00', -2),
(-7, 'Nova tema', 'Želim da podelim nešto interesantno o bazama podataka.', '2024-01-18 11:45:00', -3),
(-8, 'SQL je moćan', 'SQL je neverovatno moćan jezik za rad sa podacima.', '2024-01-21 10:00:00', -3),
(-9, 'Postgres', 'Postgres je odličan sistem za upravljanje bazama.', '2024-01-23 15:30:00', -3),
(-10, 'Učenje programiranja', 'Programiranje je fascinantno!', '2024-01-20 08:30:00', -4),
(-11, 'Moj projekat', 'Radim na zanimljivom projektu.', '2024-01-22 13:15:00', -4),
(-12, 'Pozdrav svima', 'Zdravo svima iz zajednice!', '2024-01-21 12:00:00', -5),
(-13, 'Sport', 'Volim da trčim ujutru pre posla.', '2024-01-23 07:00:00', -5),
(-14, 'Muzika', 'Slušam jazz muziku dok programiram.', '2024-01-24 18:30:00', -5),
(-15, 'Moje iskustvo', 'Delim svoje iskustvo sa bazama podataka.', '2024-01-22 15:30:00', -6),
(-16, 'Kafa', 'Najbolja kafa je ujutru!', '2024-01-24 08:00:00', -6),
(-17, 'Fudbal', 'Volim da gledam fudbal vikendom.', '2024-01-21 16:00:00', -7),
(-18, 'Putovanje', 'Planiram putovanje na more.', '2024-01-24 12:00:00', -7),
(-19, 'Film', 'Pogledala sam odličan film sinoć.', '2024-01-22 20:00:00', -8),
(-20, 'Trening', 'Redovno treniram u teretani.', '2024-01-25 06:30:00', -8);


INSERT INTO locations (id, city, country) VALUES
(-1, 'Beograd', 'Srbija'),
(-2, 'Novi Sad', 'Srbija'),
(-3, 'Niš', 'Srbija');

-- 2. INSERT ZA ADRESE (addresses)
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