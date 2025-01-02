-- Inserir dados na tabela "users"
INSERT INTO "users" (id, name, email, password, phone)
VALUES
(1, 'João Silva', 'joao.silva@example.pt', 'joao123', '912345678'),
(2, 'Maria Santos', 'maria.santos@example.pt', 'maria123', '913456789'),
(3, 'Ana Costa', 'ana.costa@example.pt', 'ana123', '914567890'),
(4, 'Pedro Carvalho', 'pedro.carvalho@example.pt', 'pedro123', '915678901'),
(5, 'Sofia Almeida', 'sofia.almeida@example.pt', 'sofia123', '916789012'),
(6, 'Rui Fernandes', 'rui.fernandes@example.pt', 'rui123', '917890123'),
(7, 'Beatriz Oliveira', 'beatriz.oliveira@example.pt', 'beatriz123', '918901234'),
(8, 'Tiago Pereira', 'tiago.pereira@example.pt', 'tiago123', '919012345'),
(9, 'Inês Martins', 'ines.martins@example.pt', 'ines123', '920123456'),
(10, 'Carlos Rodrigues', 'carlos.rodrigues@example.pt', 'carlos123', '921234567');

-- Inserir dados na tabela "teams"
INSERT INTO "teams" (id, leader, name, description, capacity, invite_Code)
VALUES
(1, 1, 'Equipa Alpha', 'Equipa estratégica de elite', 5, 'ALPHA2024'),
(2, 2, 'Equipa Beta', 'Grupo descontraído para diversão', 6, 'BETA2024'),
(3, 3, 'Equipa Gamma', 'Focada em jogos competitivos', 4, 'GAMMA2024'),
(4, 4, 'Equipa Delta', 'Equipa com habilidades mistas', 8, 'DELTA2024'),
(5, 5, 'Equipa Omega', 'Equipa para novos membros', 10, 'OMEGA2024');

-- Inserir dados na tabela "clan_members"
INSERT INTO "clan_members" (user_id, clan_id)
VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 2),
(5, 3),
(6, 3),
(7, 4),
(8, 4),
(9, 5),
(10, 5);

-- Inserir dados na tabela "games" (IDs explícitos para corresponder ao que será usado em "players")
INSERT INTO "games" (id, host_id, date, address, capacity, price, is_private, share_code, status)
VALUES
(1, 1, '2024-01-15 14:00:00', 'Rua Liberdade, Lisboa', 10, 20.00, FALSE, 'PARTILHA2024', 'Scheduled'),
(2, 2, '2024-01-20 16:30:00', 'Av. Aliados 456, Porto', 8, 15.00, TRUE, 'PRIVADO2024', 'Scheduled'),
(3, 3, '2024-01-25 18:00:00', 'Praça do Comércio, Lisboa', 12, 25.00, FALSE, 'PUBLICO2024', 'In Progress'),
(4, 4, '2024-02-01 20:00:00', 'Rua Santa Catarina 101, Porto', 6, 10.00, TRUE, 'FECHADO2024', 'Completed'),
(5, 5, '2024-02-05 15:00:00', 'EN 202, Braga', 15, 30.00, FALSE, 'LIVRE2024', 'Scheduled');

-- Inserir dados na tabela "players"
INSERT INTO "players" (user_id, match_id)
VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 2),
(5, 3),
(6, 3),
(7, 4),
(8, 4),
(9, 5),
(10, 5);

-- Inserir dados na tabela "notifications"
INSERT INTO "notifications" (match_id, message, seen)
VALUES
(1, 'Jogo agendado para 15/01 às 14:00.', FALSE),
(2, 'Jogo agendado para 20/01 às 16:30.', FALSE),
(3, 'Jogo em progresso!', TRUE),
(4, 'Jogo concluído.', TRUE),
(5, 'Jogo agendado para 5/02 às 15:00.', FALSE);