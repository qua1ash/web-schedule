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
INSERT INTO subjects (name) VALUES ('Math'), ('Physics'), ('Chemistry'), ('History'), ('Literature'), ('Biology');
INSERT INTO teachers (full_name) VALUES ('Ivanov I.I.'), ('Petrov P.P.'), ('Sidorov S.S.'), ('Smith J.J.'), ('Johnson K.K.'), ('Brown L.L.');
INSERT INTO rooms (name) VALUES ('101'), ('102'), ('103'), ('104'), ('105'), ('106');

-- Insert schedule templates for all weekdays (Monday is off, Saturday is on)
-- Tuesday
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(1, 1, 1, 2, 1, '08:00', '09:30'),
(2, 2, 2, 2, 2, '09:45', '11:15'),
(3, 3, 3, 2, 3, '11:30', '13:00');

-- Wednesday
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(1, 1, 1, 3, 1, '08:00', '09:30'),
(2, 2, 2, 3, 2, '09:45', '11:15'),
(3, 3, 3, 3, 3, '11:30', '13:00');

-- Thursday
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(4, 4, 4, 4, 1, '08:00', '09:30'),
(5, 5, 5, 4, 2, '09:45', '11:15'),
(6, 6, 6, 4, 3, '11:30', '13:00');

-- Friday
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(4, 4, 4, 5, 1, '08:00', '09:30'),
(5, 5, 5, 5, 2, '09:45', '11:15'),
(6, 6, 6, 5, 3, '11:30', '13:00');

-- Saturday
INSERT INTO schedule_templates (subject_id, teacher_id, room_id, day_of_week, lesson_order, start_time, end_time)
VALUES
(4, 4, 4, 6, 1, '08:00', '09:30'),
(5, 5, 5, 6, 2, '09:45', '11:15'),
(6, 6, 6, 6, 3, '11:30', '13:00');

-- Insert admin user (password: admin123)
INSERT INTO users (username, password_hash, role) VALUES ('admin', '$2a$11$abcdefghijklmnopqrstuv', 'admin');

-- Insert editor user (password: editor)
INSERT INTO users (username, password_hash, role) VALUES ('editor', '$2b$11$edLkXbrPClN3zF1jt4PPKuCHJRTayTvZ8SR5X3L0Wg6YBAkyur7Jy', 'editor');

-- Insert actual lessons for the academic year 2025-2026
INSERT INTO actual_lessons (subject_id, teacher_id, room_id, date, lesson_order, start_time, end_time, is_cancelled)
SELECT
    CASE WHEN EXTRACT(week FROM gs.date) % 4 < 2 THEN st.subject_id ELSE ((st.subject_id - 1) % 3) + 4 END,
    CASE WHEN EXTRACT(week FROM gs.date) % 4 < 2 THEN st.teacher_id ELSE ((st.teacher_id - 1) % 3) + 4 END,
    CASE WHEN EXTRACT(week FROM gs.date) % 4 < 2 THEN st.room_id ELSE ((st.room_id - 1) % 3) + 4 END,
    gs.date,
    st.lesson_order,
    st.start_time,
    st.end_time,
    false
FROM generate_series('2025-09-01'::date, '2026-05-31'::date, '1 day'::interval) AS gs(date)
JOIN schedule_templates st ON st.day_of_week = EXTRACT(dow FROM gs.date)
WHERE EXTRACT(dow FROM gs.date) IN (2,3,4,5,6); -- Tuesday to Saturday