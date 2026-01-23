-- Create database
CREATE DATABASE lessons;

-- Connect to database
\c lessons;

-- Create tables
CREATE TABLE subjects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE teachers (
    id SERIAL PRIMARY KEY,
    full_name VARCHAR(255) NOT NULL
);

CREATE TABLE rooms (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL
);

CREATE TABLE schedule_templates (
    id SERIAL PRIMARY KEY,
    subject_id INT REFERENCES subjects(id),
    teacher_id INT REFERENCES teachers(id),
    room_id INT REFERENCES rooms(id),
    day_of_week INT NOT NULL CHECK (day_of_week BETWEEN 1 AND 7),
    lesson_order INT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    is_active BOOLEAN DEFAULT true
);

CREATE TABLE actual_lessons (
    id SERIAL PRIMARY KEY,
    subject_id INT REFERENCES subjects(id),
    teacher_id INT REFERENCES teachers(id),
    room_id INT REFERENCES rooms(id),
    date DATE NOT NULL,
    lesson_order INT NOT NULL,
    start_time TIME NOT NULL,
    end_time TIME NOT NULL,
    is_cancelled BOOLEAN DEFAULT false
);

CREATE TABLE homework (
    id SERIAL PRIMARY KEY,
    date DATE NOT NULL,
    lesson_order INT NOT NULL,
    homework_text TEXT,
    UNIQUE(date, lesson_order)
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL DEFAULT 'user'
);

-- Insert sample data
INSERT INTO subjects (name) VALUES ('Math'), ('Physics'), ('Chemistry');
INSERT INTO teachers (full_name) VALUES ('Ivanov I.I.'), ('Petrov P.P.'), ('Sidorov S.S.');
INSERT INTO rooms (name) VALUES ('101'), ('102'), ('103');

-- Insert schedule template for Monday (day_of_week = 1)
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(1, 1, 1, 1, 1, '08:00', '09:30'),
(2, 2, 2, 1, 2, '09:45', '11:15'),
(3, 3, 3, 1, 3, '11:30', '13:00');

-- Insert schedule template for Wednesday (day_of_week = 3)
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(1, 1, 1, 3, 1, '08:00', '09:30'),
(2, 2, 2, 3, 2, '09:45', '11:15'),
(3, 3, 3, 3, 3, '11:30', '13:00');

-- Insert schedule template for Sunday (day_of_week = 7)
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(1, 1, 1, 7, 1, '08:00', '09:30'),
(2, 2, 2, 7, 2, '09:45', '11:15'),
(3, 3, 3, 7, 3, '11:30', '13:00');

-- Insert admin user (password: admin123)
INSERT INTO users (username, password_hash, role) VALUES ('admin', '$2a$11$abcdefghijklmnopqrstuv', 'admin');

-- Insert editor user (password: editor)
INSERT INTO users (username, password_hash, role) VALUES ('editor', '$2b$11$edLkXbrPClN3zF1jt4PPKuCHJRTayTvZ8SR5X3L0Wg6YBAkyur7Jy', 'editor');
