-- Criação do esquema da base de dados
CREATE TABLE "users" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(30) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(50) NOT NULL,
    phone VARCHAR(15) UNIQUE NOT NULL
);

CREATE TABLE "teams" (
    id SERIAL PRIMARY KEY,
    leader INT NOT NULL,
    name VARCHAR(30) UNIQUE NOT NULL,
    description VARCHAR(50),
    capacity INT,
    invite_Code VARCHAR(20),
    CONSTRAINT fk_leader FOREIGN KEY (leader) REFERENCES "users"(id)
);

CREATE TABLE "clan_members" (
    user_id INT NOT NULL,
    clan_id INT NOT NULL,
    PRIMARY KEY (user_id, clan_id),
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES "users"(id),
    CONSTRAINT fk_clan FOREIGN KEY (clan_id) REFERENCES "teams"(id)
);

CREATE TABLE "games" (
    id SERIAL PRIMARY KEY,
    host_id INT NOT NULL,
    date TIMESTAMP NOT NULL,
    local VARCHAR(200),
    capacity INT,
    price NUMERIC(10, 2),
    private BOOLEAN DEFAULT FALSE,
    share_code VARCHAR(15),
    status VARCHAR(50) CHECK (status IN ('Scheduled', 'In Progress', 'Completed', 'Canceled')) DEFAULT 'Scheduled',
    CONSTRAINT fk_host FOREIGN KEY (host_id) REFERENCES "users"(id)
);

CREATE TABLE "players" (
    user_id INT NOT NULL,
    match_id INT NOT NULL,
    PRIMARY KEY (user_id, match_id),
    CONSTRAINT fk_player_user FOREIGN KEY (user_id) REFERENCES "users"(id),
    CONSTRAINT fk_player_match FOREIGN KEY (match_id) REFERENCES "games"(id)
);

CREATE TABLE "notifications" (
    id SERIAL PRIMARY KEY,
    match_id INT NOT NULL,
    message VARCHAR(50) NOT NULL,
    seen BOOLEAN DEFAULT FALSE,
    timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_notification_match FOREIGN KEY (match_id) REFERENCES "games"(id)
);