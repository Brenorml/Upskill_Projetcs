-- Criação do banco de dados
CREATE DATABASE BibliotecaXPTO;
GO

-- Usar a database
USE BibliotecaXPTO;
GO

-- Tabela de leitores
CREATE TABLE Usuario (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    DataNascimento DATE NOT NULL,
    Email NVARCHAR(150) UNIQUE NOT NULL,
    Telefone NVARCHAR(20) NOT NULL,
    DataRegisto DATE DEFAULT GETDATE(),
	Username NVARCHAR(50) NOT NULL,
	PalavraPasse VARBINARY(MAX) NOT NULL,
	TipoUser VARCHAR(15) CHECK (TipoUser IN ('Leitor', 'Bibliotecario')) DEFAULT 'Leitor',
    Ativo BIT DEFAULT 1
);

-- Tabela de núcleos
CREATE TABLE Nucleos (
    NucleoID INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL UNIQUE,
    Endereco NVARCHAR(255) NOT NULL
);

-- Tabela de obras
CREATE TABLE Obras (
    ObraID INT IDENTITY(1,1) PRIMARY KEY,
    Titulo NVARCHAR(150) NOT NULL,
    Autor NVARCHAR(100) NOT NULL,
    AnoPublicacao INT,
    Genero NVARCHAR(50),
    Descricao TEXT
);

-- Tabela de Imagens
CREATE TABLE Imagens(
	ID_ObraID INT,
	Capa VARBINARY(MAX)
	FOREIGN KEY (ID_ObraID) REFERENCES Obras(ObraID)
);

-- Tabela de exemplares
CREATE TABLE Exemplares (
    ExemplarID INT IDENTITY(1,1) PRIMARY KEY,
    ObraID INT NOT NULL,
    NucleoID INT NOT NULL,
    Disponivel BIT DEFAULT 1,
    FOREIGN KEY (ObraID) REFERENCES Obras(ObraID),
    FOREIGN KEY (NucleoID) REFERENCES Nucleos(NucleoID)
);

-- Tabela de requisições
CREATE TABLE Requisicoes (
    RequisicaoID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    ExemplarID INT NOT NULL,
    DataRequisicao DATE NOT NULL DEFAULT GETDATE(),
	DataDevolucao Date,
    FOREIGN KEY (UserID) REFERENCES Usuario(UserID) ON DELETE CASCADE,
    FOREIGN KEY (ExemplarID) REFERENCES Exemplares(ExemplarID)
);

-- Tabela de histórico de requisições
CREATE TABLE HistoricoRequisicoes (
    IdHistorico INT IDENTITY(1,1) PRIMARY KEY,
    RequisicaoID INT,
    UserID INT,
    NomeUser NVARCHAR(255),
    Telefone NVARCHAR(20),
    TituloObra NVARCHAR(255),
    ExemplarID INT,
    Nucleo NVARCHAR(100),
    DataRequisicao DATE,
    DataDevolucao DATE
);

-- Inserir dados na tabela usuario
INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, DataRegisto, Username, PalavraPasse, TipoUser, Ativo) VALUES 
('Alice Martins', '1988-03-15', 'alice.martins@example.com', '987654321', '2019-01-15', 'alicem', HASHBYTES('SHA2_256', 'password1'), 'Leitor', 1),
('Bruno Silva', '1990-07-22', 'bruno.silva@example.com', '912345678', '2018-02-20', 'brunos', HASHBYTES('SHA2_256', 'password2'), 'Bibliotecario', 1),
('Carla Andrade', '1995-01-10', 'carla.andrade@example.com', '965432198', '2019-03-25', 'carlaa', HASHBYTES('SHA2_256', 'password3'), 'Leitor', 1),
('Diego Souza', '1980-04-18', 'diego.souza@example.com', '934567123', '2019-04-10', 'diegos', HASHBYTES('SHA2_256', 'password4'), 'Leitor', 0),
('Elena Gomes', '1992-09-30', 'elena.gomes@example.com', '956789012', '2019-05-05', 'elenag', HASHBYTES('SHA2_256', 'password5'), 'Leitor', 1),
('Fernando Costa', '1983-06-07', 'fernando.costa@example.com', '923456789', '2018-06-15', 'fernandoc', HASHBYTES('SHA2_256', 'password6'), 'Bibliotecario', 1),
('Gabriel Menezes', '1985-12-25', 'gabriel.menezes@example.com', '987123456', '2019-07-20', 'gabrielm', HASHBYTES('SHA2_256', 'password7'), 'Leitor', 1),
('Helena Ramos', '1998-08-14', 'helena.ramos@example.com', '932156789', '2019-08-10', 'helenar', HASHBYTES('SHA2_256', 'password8'), 'Leitor', 1),
('Igor Nunes', '1991-11-11', 'igor.nunes@example.com', '921234567', '2020-09-12', 'igorn', HASHBYTES('SHA2_256', 'password9'), 'Leitor', 1),
('Julia Ferreira', '1993-02-03', 'julia.ferreira@example.com', '954321678', '2020-10-05', 'juliaf', HASHBYTES('SHA2_256', 'password10'), 'Leitor', 0),
('Lucas Vieira', '1984-11-15', 'lucas.vieira@example.com', '912345987', '2020-11-01', 'lucasv', HASHBYTES('SHA2_256', 'password11'), 'Leitor', 1),
('Maria Silva', '1979-12-25', 'maria.silva@example.com', '912543789', '2020-11-25', 'marias', HASHBYTES('SHA2_256', 'password12'), 'Leitor', 0),
('Nina Oliveira', '1987-07-04', 'nina.oliveira@example.com', '915432987', '2020-01-10', 'ninao', HASHBYTES('SHA2_256', 'password13'), 'Leitor', 1),
('Otávio Campos', '1996-03-18', 'otavio.campos@example.com', '918654321', '2020-02-20', 'otavioc', HASHBYTES('SHA2_256', 'password14'), 'Leitor', 1),
('Patricia Souza', '1999-06-05', 'patricia.souza@example.com', '913245678', '2020-03-15', 'pats', HASHBYTES('SHA2_256', 'password15'), 'Leitor', 1),
('Quintino Santos', '1990-10-22', 'quintino.santos@example.com', '911234567', '2020-04-02', 'quints', HASHBYTES('SHA2_256', 'password16'), 'Leitor', 1),
('Renata Lopes', '1991-08-10', 'renata.lopes@example.com', '911987654', '2020-05-10', 'renatal', HASHBYTES('SHA2_256', 'password17'), 'Leitor', 1),
('Samuel Mendes', '1986-01-19', 'samuel.mendes@example.com', '910234567', '2020-06-22', 'samm', HASHBYTES('SHA2_256', 'password18'), 'Leitor', 1),
('Tatiana Lima', '1989-11-30', 'tatiana.lima@example.com', '914567890', '2020-07-18', 'tatil', HASHBYTES('SHA2_256', 'password19'), 'Leitor', 0),
('Ulisses Rocha', '1994-05-17', 'ulisses.rocha@example.com', '912987654', '2020-08-14', 'ulir', HASHBYTES('SHA2_256', 'password20'), 'Leitor', 1);

-- Inserir dados na tabela de núcleos
INSERT INTO Nucleos (Nome, Endereco) VALUES
('Central Lisboa', 'Rua das Flores'),
('Núcleo Porto', 'Rua da Hora'),
('Núcleo Coimbra', 'Rua do Navio'),
('Núcleo Faro', 'Avenida do Sol'),
('Núcleo Braga', 'Travessa do Castelo'),
('Núcleo Aveiro', 'Rua dos Pescadores'),
('Núcleo Setúbal', 'Rua da Praia'),
('Núcleo Évora', 'Largo da Sé'),
('Núcleo Viseu', 'Avenida Principal'),
('Núcleo Leiria', 'Rua do Comércio');

-- Inserir dados na tabela de obras
INSERT INTO Obras (Titulo, Autor, AnoPublicacao, Genero, Descricao) VALUES
('O Senhor dos Anéis', 'J.R.R. Tolkien', 1954, 'Fantasia', 'Uma das maiores obras de fantasia.'),
('1984', 'George Orwell', 1949, 'Distopia', 'Uma obra sobre um futuro totalitário.'),
('Dom Quixote', 'Miguel de Cervantes', 1605, 'Clássico', 'A história de um cavaleiro que luta contra moinhos de vento.'),
('Moby Dick', 'Herman Melville', 1851, 'Aventura', 'A perseguição de uma baleia branca.'),
('Pride and Prejudice', 'Jane Austen', 1813, 'Romance', 'Um romance clássico da literatura inglesa.'),
('Cem Anos de Solidão', 'Gabriel García Márquez', 1967, 'Realismo Mágico', 'A saga da família Buendía ao longo de várias gerações.'),
('O Grande Gatsby', 'F. Scott Fitzgerald', 1925, 'Romance', 'A história de Jay Gatsby e sua busca pelo sonho americano.'),
('O Sol é para Todos', 'Harper Lee', 1960, 'Ficção', 'Uma história sobre racismo e justiça no sul dos EUA.'),
('O Hobbit', 'J.R.R. Tolkien', 1937, 'Fantasia', 'A aventura de Bilbo Bolseiro em uma jornada inesperada.'),
('O Código Da Vinci', 'Dan Brown', 2003, 'Mistério', 'Uma investigação envolvendo símbolos e segredos históricos.'),
('O Pequeno Príncipe', 'Antoine de Saint-Exupéry', 1943, 'Infantil', 'A história filosófica de um príncipe que viaja por planetas.'),
('A Revolução dos Bichos', 'George Orwell', 1945, 'Fábula', 'Uma alegoria política sobre a Revolução Russa.'),
('O Morro dos Ventos Uivantes', 'Emily Brontë', 1847, 'Romance', 'A intensa história de amor e vingança entre Heathcliff e Catherine.'),
('A Metamorfose', 'Franz Kafka', 1915, 'Surrealismo', 'A história de um homem que se transforma em um inseto gigante.'),
('O Apanhador no Campo de Centeio', 'J.D. Salinger', 1951, 'Ficção', 'A crise de identidade de Holden Caulfield, um adolescente em crise.'),
('O Alquimista', 'Paulo Coelho', 1988, 'Ficção', 'A jornada de Santiago em busca de seu tesouro pessoal.'),
('Crime e Castigo', 'Fiódor Dostoiévski', 1866, 'Drama', 'A luta moral de Raskólnikov após cometer um assassinato.'),
('A Menina que Roubava Livros', 'Markus Zusak', 2005, 'Drama', 'Uma menina encontra consolo nos livros durante a Segunda Guerra Mundial.'),
('A Ilha do Tesouro', 'Robert Louis Stevenson', 1883, 'Aventura', 'A clássica história de piratas e a busca por um tesouro perdido.'),
('Frankenstein', 'Mary Shelley', 1818, 'Terror', 'A história de Victor Frankenstein e a criação de um monstro que desafia a moral e a ciência.');

-- Inserir dados na tabela de Imagens(capas)
INSERT INTO Imagens (ID_ObraID,Capa) VALUES 
(1, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa1.jpg', SINGLE_BLOB) AS Capa)),
(2, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa2.jpg', SINGLE_BLOB) AS Capa)),
(3, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa3.jpg', SINGLE_BLOB) AS Capa)),
(4, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa4.jpg', SINGLE_BLOB) AS Capa)),
(5, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa5.jpg', SINGLE_BLOB) AS Capa)),
(6, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa6.jpg', SINGLE_BLOB) AS Capa)),
(7, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa7.jpg', SINGLE_BLOB) AS Capa)),
(8, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa8.jpg', SINGLE_BLOB) AS Capa)),
(9, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa9.jpg', SINGLE_BLOB) AS Capa)),
(10, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa10.jpg', SINGLE_BLOB) AS Capa)),
(11, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa11.jpg', SINGLE_BLOB) AS Capa)),
(12, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa12.jpg', SINGLE_BLOB) AS Capa)),
(13, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa13.jpg', SINGLE_BLOB) AS Capa)),
(14, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa14.jpg', SINGLE_BLOB) AS Capa)),
(15, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa15.jpg', SINGLE_BLOB) AS Capa)),
(16, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa16.jpg', SINGLE_BLOB) AS Capa)),
(17, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa17.jpg', SINGLE_BLOB) AS Capa)),
(18, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa18.jpg', SINGLE_BLOB) AS Capa)),
(19, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa19.jpg', SINGLE_BLOB) AS Capa)),
(20, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa20.jpg', SINGLE_BLOB) AS Capa));

-- Inserir dados na tabela de exemplares
INSERT INTO Exemplares (ObraID, NucleoID, Disponivel) VALUES
(1, 1, 1), (1, 2, 1), (1, 3, 1),(1, 4, 1),(1, 5, 1),(1, 6, 1),(1, 7, 1),(1, 8, 1),(1, 9, 1),(1, 10, 1),
(2, 1, 1), (2, 2, 1), (2, 3, 1),(2, 4, 1),(2, 5, 1),(2, 6, 1),(2, 7, 1),(2, 8, 1),(2, 9, 1),(2, 10, 1),
(3, 1, 1), (3, 2, 1), (3, 3, 1),(3, 4, 1),(3, 5, 1),(3, 6, 1),(3, 7, 1),(3, 8, 1),(3, 9, 1),(3, 10, 1),
(4, 1, 1), (4, 2, 1), (4, 3, 1),(4, 4, 1),(4, 5, 1),(4, 6, 1),(4, 7, 1),(4, 8, 1),(4, 9, 1),(4, 10, 1),
(5, 1, 0), (5, 2, 1), (5, 3, 1),(5, 4, 1),(5, 5, 1),(5, 6, 1),(5, 7, 1),(5, 8, 1),(5, 9, 1),(5, 10, 1),
(6, 1, 1), (6, 2, 1), (6, 3, 1),(6, 4, 0),(6, 5, 1),(6, 6, 1),(6, 7, 1),(6, 8, 1),(6, 9, 1),(6, 10, 1),
(7, 1, 1), (7, 2, 1), (7, 3, 1),(7, 4, 1),(7, 5, 1),(7, 6, 1),(7, 7, 1),(7, 8, 1),(7, 9, 1),(7, 10, 1),
(8, 1, 1), (8, 2, 1), (8, 3, 1),(8, 4, 1),(8, 5, 1),(8, 6, 1),(8, 7, 1),(8, 8, 0),(8, 9, 0),(8, 10, 1),
(9, 1, 1), (9, 2, 1), (9, 3, 0),(9, 4, 1),(9, 5, 1),(9, 6, 1),(9, 7, 1),(9, 8, 1),(9, 9, 1),(9, 10, 1),
(10, 1, 0), (10, 2, 1), (10, 3, 1),(10, 4, 1),(10, 5, 1),(10, 6, 1),(10, 7, 1),(10, 8, 1),(10, 9, 1),(10, 10, 1),
(11, 1, 1), (11, 2, 1), (11, 3, 1),(11, 4, 1),(11, 5, 1),(11, 6, 1),(11, 7, 1),(11, 8, 1),(11, 9, 1),(11, 10, 1),
(12, 1, 1),(12, 2, 1),(12, 3, 1),(12, 4, 1),(12, 5, 1),(12, 6, 1),(12, 7, 1),(12, 8, 1),(12, 9, 1),(12, 10, 1),
(13, 1, 1),(13, 2, 1),(13, 3, 1),(13, 4, 1),(13, 5, 1),(13, 6, 1),(13, 7, 1),(13, 8, 1),(13, 9, 1),(13, 10, 1),
(14, 1, 1),(14, 2, 1),(14, 3, 1),(14, 4, 1),(14, 5, 1),(14, 6, 1),(14, 7, 1),(14, 8, 1),(14, 9, 1),(14, 10, 1),
(15, 1, 1),(15, 2, 1),(15, 3, 1),(15, 4, 1),(15, 5, 1),(15, 6, 1),(15, 7, 1),(15, 8, 1),(15, 9, 1),(15, 10, 1),
(16, 1, 1),(16, 2, 1),(16, 3, 1),(16, 4, 1),(16, 5, 1),(16, 6, 1),(16, 7, 1),(16, 8, 1),(16, 9, 1),(16, 10, 1),
(17, 1, 1),(17, 2, 1),(17, 3, 1),(17, 4, 1),(17, 5, 1),(17, 6, 1),(17, 7, 1),(17, 8, 1),(17, 9, 1),(17, 10, 1),
(18, 1, 1),(18, 2, 1),(18, 3, 1),(18, 4, 1),(18, 5, 1),(18, 6, 1),(18, 7, 1),(18, 8, 1),(18, 9, 1),(18, 10, 1),
(19, 1, 1),(19, 2, 1),(19, 3, 1),(19, 4, 1),(19, 5, 1),(19, 6, 1),(19, 7, 1),(19, 8, 1),(19, 9, 1),(19, 10, 1),
(20, 1, 1),(20, 2, 1),(20, 3, 1),(20, 4, 1),(20, 5, 1),(20, 6, 1),(20, 7, 1),(20, 8, 1),(20, 9, 1),(20, 10, 1),
-- segunda linha de exemplares
(1, 1, 1), (1, 2, 1), (1, 3, 1),(1, 4, 1),(1, 5, 1),(1, 6, 1),(1, 7, 1),(1, 8, 1),(1, 9, 1),(1, 10, 1),
(2, 1, 1), (2, 2, 1), (2, 3, 1),(2, 4, 1),(2, 5, 1),(2, 6, 1),(2, 7, 1),(2, 8, 1),(2, 9, 1),(2, 10, 1),
(3, 1, 1), (3, 2, 1), (3, 3, 1),(3, 4, 1),(3, 5, 1),(3, 6, 1),(3, 7, 1),(3, 8, 1),(3, 9, 1),(3, 10, 1),
(4, 1, 1), (4, 2, 1), (4, 3, 1),(4, 4, 1),(4, 5, 1),(4, 6, 1),(4, 7, 1),(4, 8, 1),(4, 9, 1),(4, 10, 1),
(5, 1, 1), (5, 2, 1), (5, 3, 1),(5, 4, 1),(5, 5, 1),(5, 6, 1),(5, 7, 1),(5, 8, 1),(5, 9, 1),(5, 10, 1),
(6, 1, 1), (6, 2, 1), (6, 3, 1),(6, 4, 1),(6, 5, 1),(6, 6, 1),(6, 7, 1),(6, 8, 1),(6, 9, 1),(6, 10, 1),
(7, 1, 1), (7, 2, 1), (7, 3, 1),(7, 4, 1),(7, 5, 1),(7, 6, 1),(7, 7, 1),(7, 8, 1),(7, 9, 1),(7, 10, 1),
(8, 1, 1), (8, 2, 1), (8, 3, 1),(8, 4, 1),(8, 5, 1),(8, 6, 1),(8, 7, 1),(8, 8, 1),(8, 9, 1),(8, 10, 1),
(9, 1, 1), (9, 2, 1), (9, 3, 1),(9, 4, 1),(9, 5, 1),(9, 6, 1),(9, 7, 1),(9, 8, 1),(9, 9, 1),(9, 10, 1),
(10, 1, 1), (10, 2, 1), (10, 3, 1),(10, 4, 1),(10, 5, 1),(10, 6, 1),(10, 7, 1),(10, 8, 1),(10, 9, 1),(10, 10, 1),
(11, 1, 1), (11, 2, 1), (11, 3, 1),(11, 4, 1),(11, 5, 1),(11, 6, 1),(11, 7, 1),(11, 8, 1),(11, 9, 1),(11, 10, 1),
(12, 1, 1),(12, 2, 1),(12, 3, 1),(12, 4, 1),(12, 5, 1),(12, 6, 1),(12, 7, 1),(12, 8, 1),(12, 9, 1),(12, 10, 1),
(13, 1, 1),(13, 2, 1),(13, 3, 1),(13, 4, 1),(13, 5, 1),(13, 6, 1),(13, 7, 1),(13, 8, 1),(13, 9, 1),(13, 10, 1),
(14, 1, 1),(14, 2, 1),(14, 3, 1),(14, 4, 1),(14, 5, 1),(14, 6, 1),(14, 7, 1),(14, 8, 1),(14, 9, 1),(14, 10, 1),
(15, 1, 1),(15, 2, 1),(15, 3, 1),(15, 4, 1),(15, 5, 1),(15, 6, 1),(15, 7, 1),(15, 8, 1),(15, 9, 1),(15, 10, 1),
(16, 1, 1),(16, 2, 1),(16, 3, 1),(16, 4, 1),(16, 5, 1),(16, 6, 1),(16, 7, 1),(16, 8, 1),(16, 9, 1),(16, 10, 1),
(17, 1, 1),(17, 2, 1),(17, 3, 1),(17, 4, 1),(17, 5, 1),(17, 6, 1),(17, 7, 1),(17, 8, 1),(17, 9, 1),(17, 10, 1),
(18, 1, 1),(18, 2, 1),(18, 3, 1),(18, 4, 1),(18, 5, 1),(18, 6, 1),(18, 7, 1),(18, 8, 1),(18, 9, 1),(18, 10, 1),
(19, 1, 1),(19, 2, 1),(19, 3, 1),(19, 4, 1),(19, 5, 1),(19, 6, 1),(19, 7, 1),(19, 8, 1),(19, 9, 1),(19, 10, 1),
(20, 1, 1),(20, 2, 1),(20, 3, 1),(20, 4, 1),(20, 5, 1),(20, 6, 1),(20, 7, 1),(20, 8, 1),(20, 9, 1),(20, 10, 1);

-- Inserir requisições para alguns dos 20 usuários
INSERT INTO Requisicoes (UserID, ExemplarID, DataRequisicao, DataDevolucao) VALUES
(1, 1, '2022-11-01', '2022-11-20'),
(1, 2, '2022-11-02', '2022-11-28'),
(3, 11, '2024-10-03', '2024-10-17'),
(4, 12, '2024-09-04', '2024-09-18'),
(5, 21, '2024-05-05', '2024-05-19'),
(1, 22, '2022-06-06', '2022-06-30'),
(7, 31, '2024-06-07', '2024-06-21'),
(8, 32, '2024-03-08', '2024-03-22'),
(9, 41, '2024-02-09', '2024-02-23'),
(10, 42, '2024-01-10', '2024-01-24'),
(11, 51, '2024-11-11', Null),
(12, 52, '2024-09-12', '2024-09-26'),
(13, 63, '2024-10-13', '2024-10-27'),
(12, 64, '2024-11-14', Null),
(15, 75, '2024-08-15', '2024-08-29'),
(16, 76, '2024-10-16', '2024-10-30'),
(17, 88, '2024-08-17', Null),
(18, 89, '2024-11-14', Null),
(19, 93, '2024-11-14', Null),
(20, 101, '2024-11-05', Null);

-- Stored procedures --

-- Criar Stored Procedures

-- Procedures da biblioteca

-- 1. Consultar o número total de obras existentes
CREATE PROCEDURE ConsultarTotalObras AS
BEGIN
    SELECT COUNT(*) AS TotalObras FROM Obras;
END;
GO


-- 2. Consultar o número de obras por tipo de assunto
CREATE PROCEDURE ConsultarObrasPorGenero AS
BEGIN
    SELECT Genero, COUNT(*) AS NumeroObras FROM Obras
    GROUP BY Genero;
END;
GO


-- 3. Consultar as 10 obras mais requisitadas em um intervalo de tempo
CREATE PROCEDURE Top10ObrasRequisitadas (@DataInicio DATE, @DataFim DATE) AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 10 
        o.Titulo, 
        COUNT(r.RequisicaoID) AS TotalRequisicoes
    FROM Requisicoes r
    INNER JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
    INNER JOIN Obras o ON e.ObraID = o.ObraID
    WHERE r.DataRequisicao BETWEEN @DataInicio AND @DataFim
    GROUP BY o.Titulo
    ORDER BY TotalRequisicoes DESC;
END;
GO


-- 4. Consultar os núcleos por ordem decrescente de requisições
CREATE PROCEDURE NucleosPorRequisicoes (@DataInicio DATE, @DataFim DATE) AS
BEGIN
        SELECT n.Nome, COUNT(r.RequisicaoID) AS TotalRequisicoes
        FROM Requisicoes r
        JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
        JOIN Nucleos n ON e.NucleoID = n.NucleoID
        WHERE r.DataRequisicao BETWEEN @DataInicio AND @DataFim
        GROUP BY n.Nome
        ORDER BY TotalRequisicoes DESC;
END;
GO


-- 5. Acrescentar novas obras
CREATE PROCEDURE AdicionarObra 
	@Titulo VARCHAR(150), 
    @Autor VARCHAR(100), 
    @AnoPublicacao INT, 
    @Genero VARCHAR(50), 
    @Descricao TEXT, 
    @PathImage VARCHAR(100)
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO Obras (Titulo, Autor, AnoPublicacao, Genero, Descricao)
        VALUES (@Titulo, @Autor, @AnoPublicacao, @Genero, @Descricao);

        DECLARE @ObraID INT;
        SET @ObraID = SCOPE_IDENTITY();

        DECLARE @sql NVARCHAR(MAX);
        SET @sql = N'
            INSERT INTO Imagens (ID_ObraID, Capa) 
            SELECT @ObraID, BulkColumn 
            FROM OPENROWSET(BULK ''' + @PathImage + ''', SINGLE_BLOB) AS Capa;
        ';

        -- Executar a consulta dinâmica
        EXEC sp_executesql @sql, N'@ObraID INT', @ObraID;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT ERROR_MESSAGE();
    END CATCH
END;
GO


-- 6. Atualizar o numero de exemplares de determinada obra.

CREATE PROCEDURE AdicionarExemplaresAoNucleoPrincipal
    @BibliotecarioID INT,
    @ObraID INT,
    @QtdAdicionar INT
AS
BEGIN
    BEGIN TRANSACTION;

    IF @QtdAdicionar <= 0
    BEGIN
        PRINT 'Erro: A quantidade de exemplares a adicionar deve ser um número inteiro positivo.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1 
        FROM Usuario 
        WHERE UserID = @BibliotecarioID 
          AND TipoUser = 'Bibliotecario'
          AND Ativo = 1
    )
    BEGIN
        PRINT 'Operação permitida apenas para bibliotecários ativos.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Verificar se a obra existe
    IF NOT EXISTS (SELECT 1 FROM Obras WHERE ObraID = @ObraID)
    BEGIN
        PRINT 'Erro: A obra especificada não existe no catálogo de obras. Por favor adicione uma nova obra.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    -- Obter o ID do núcleo principal
    DECLARE @NucleoPrincipalID INT;
    SET @NucleoPrincipalID = (SELECT NucleoID FROM Nucleos WHERE Nome = 'Central Lisboa');

    IF @NucleoPrincipalID IS NULL
    BEGIN
        PRINT 'Erro: Núcleo principal não encontrado.';
        ROLLBACK TRANSACTION;
        RETURN;
    END

    DECLARE @i INT = 1;
    WHILE @i <= @QtdAdicionar
    BEGIN
        INSERT INTO Exemplares(ObraID, NucleoID)
        VALUES (@ObraID, @NucleoPrincipalID);

        SET @i = @i + 1;
    END

    COMMIT TRANSACTION;

    PRINT 'Novos exemplares adicionados ao núcleo principal.';
END;
GO


-- 7. Transferir um ou mais exemplares de uma obra, de um nucleo para outro 
CREATE PROCEDURE TransferirExemplar
    @BibliotecarioID INT,
    @ObraID INT,
    @OrigemNucleoID INT,
    @DestinoNucleoID INT,
    @QtdTransferir INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1
        FROM Usuario
        WHERE UserID = @BibliotecarioID
          AND TipoUser = 'Bibliotecario'
          AND Ativo = 1
    )
    BEGIN
        PRINT 'Operação permitida apenas para bibliotecários ativos.';
        RETURN;
    END

    -- Verificar se a obra existe no núcleo de origem
    IF NOT EXISTS (
        SELECT 1
        FROM Exemplares
        WHERE ObraID = @ObraID AND NucleoID = @OrigemNucleoID
    )
    BEGIN
        PRINT 'Erro: A obra especificada não possui exemplares no núcleo de origem.';
        RETURN;
    END

    -- Verificar se o núcleo de origem possui exemplares suficientes para transferência
    DECLARE @QtdOrigemDisponivel INT;
    SELECT @QtdOrigemDisponivel = COUNT(*)
    FROM Exemplares
    WHERE ObraID = @ObraID AND NucleoID = @OrigemNucleoID;

    IF @QtdOrigemDisponivel < (@QtdTransferir + 1)
    BEGIN
        PRINT 'Erro: A quantidade de exemplares no núcleo de origem é insuficiente para realizar a transferência. Cada núcleo deve manter pelo menos um exemplar disponível.';
        RETURN;
    END

    -- Realizar a transferência dos exemplares
    BEGIN TRANSACTION;

    -- Mover os exemplares para o núcleo de destino
    UPDATE TOP (@QtdTransferir) Exemplares
    SET NucleoID = @DestinoNucleoID
    WHERE ObraID = @ObraID AND NucleoID = @OrigemNucleoID
      AND Disponivel = 1;

    -- Verificar se a transferência foi concluída
    IF @@ROWCOUNT = @QtdTransferir
    BEGIN
        PRINT 'Transferência concluída com sucesso.';
        COMMIT TRANSACTION;
    END
    ELSE
    BEGIN
        PRINT 'Erro ao transferir os exemplares. Operação abortada.';
        ROLLBACK TRANSACTION;
    END
END;
GO


-- 8. Registar novos leitores 
CREATE PROCEDURE RegistrarNovoLeitor
    @BibliotecarioID INT,
    @Nome NVARCHAR(100),
    @DataNascimento DATE,
    @Email NVARCHAR(150),
    @Telefone NVARCHAR(20),
    @Username NVARCHAR(50),
    @PalavraPasse NVARCHAR(50)
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuario 
            WHERE UserID = @BibliotecarioID 
              AND TipoUser = 'Bibliotecario'
              AND Ativo = 1
        )
        BEGIN
            PRINT 'Erro: Apenas bibliotecários ativos podem registrar novos leitores.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF CHARINDEX(' ', @Nome) = 0
        BEGIN
            PRINT 'Erro: O nome deve conter pelo menos um espaço entre os dois nomes.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF CHARINDEX('@', @Email) = 0 OR CHARINDEX('.', @Email, CHARINDEX('@', @Email)) = 0
        BEGIN
            PRINT 'Erro: O email deve conter "@" e pelo menos um ponto após o "@".';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF LEN(@Telefone) <> 9 OR @Telefone LIKE '%[^0-9]%'
        BEGIN
            PRINT 'Erro: O número de telefone deve conter exatamente 9 dígitos numéricos.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM Usuario WHERE Email = @Email)
        BEGIN
            PRINT 'Erro: Já existe um usuário registrado com este email.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM Usuario WHERE Telefone = @Telefone)
        BEGIN
            PRINT 'Erro: Já existe um usuário registrado com este número de telefone.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, DataRegisto, Username, PalavraPasse, TipoUser, Ativo)
        VALUES (@Nome, @DataNascimento, @Email, @Telefone, GETDATE(), @Username, CONVERT(VARBINARY(MAX), @PalavraPasse), 'Leitor', 1);

        PRINT 'Novo leitor registrado com sucesso.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        PRINT 'Erro ao registrar o novo leitor. Transação revertida.';
        ROLLBACK TRANSACTION;
    END CATCH
END;
GO


-- 9. Suspender o acesso a requisicões a leitores que tenham procedido a devoluções atrasadas em mais que três ocasiões 
CREATE PROCEDURE SuspenderAcessoLeitoresComDevolucoesAtrasadas
    @BibliotecarioID INT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
 
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuario 
            WHERE UserID = @BibliotecarioID 
              AND TipoUser = 'Bibliotecario'
              AND Ativo = 1
        )
        BEGIN
            PRINT 'Erro: Apenas bibliotecários ativos podem suspender leitores.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        DECLARE @LimiteDevolucao DATE;
        SET @LimiteDevolucao = GETDATE();

        -- Identificar leitores com 3 ou mais devoluções atrasadas
        DECLARE @LeitoresSuspensos TABLE (UserID INT);

        -- Preencher a tabela temporária com os IDs dos leitores
        INSERT INTO @LeitoresSuspensos (UserID)
        SELECT R.UserID
        FROM Requisicoes R
        WHERE 
            -- Não devolvido e já passou o prazo de 15 dias
            (R.DataDevolucao IS NULL AND DATEADD(DAY, 15, R.DataRequisicao) < @LimiteDevolucao)
            -- Devolvido, mas após o prazo de 15 dias
            OR (R.DataDevolucao IS NOT NULL AND R.DataDevolucao > DATEADD(DAY, 15, R.DataRequisicao))
        GROUP BY R.UserID
        HAVING COUNT(*) >= 3;

        -- Suspender o acesso dos leitores identificados
        IF EXISTS (SELECT 1 FROM @LeitoresSuspensos)
        BEGIN
            UPDATE U
            SET U.Ativo = 0
            FROM Usuario U
            JOIN @LeitoresSuspensos L ON U.UserID = L.UserID
            WHERE U.TipoUser = 'Leitor';

            PRINT 'Acesso suspenso para leitores com mais de 3 devoluções atrasadas.';
        END
        ELSE
        BEGIN
            PRINT 'Nenhum leitor com mais de 3 devoluções atrasadas foi encontrado.';
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH

        PRINT 'Erro ao processar a suspensão. Transação revertida.';
        ROLLBACK TRANSACTION;
    END CATCH
END;
GO


-- 10. Reativar um leitor suspenso
CREATE PROCEDURE ReativarLeitor
    @BibliotecarioID INT,
    @LeitorID INT
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
   
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuario 
            WHERE UserID = @BibliotecarioID 
              AND TipoUser = 'Bibliotecario'
              AND Ativo = 1
        )
        BEGIN
            PRINT 'Erro: Apenas bibliotecários ativos podem reativar leitores.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar se o LeitorID existe na tabela de usuários
        IF NOT EXISTS (
            SELECT 1
            FROM Usuario
            WHERE UserID = @LeitorID 
              AND TipoUser = 'Leitor'
        )
        BEGIN
            PRINT 'Erro: O leitor especificado não existe.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar se o leitor especificado está atualmente inativo (Ativo = 0)
        IF NOT EXISTS (
            SELECT 1 
            FROM Usuario 
            WHERE UserID = @LeitorID 
              AND TipoUser = 'Leitor' 
              AND Ativo = 0
        )
        BEGIN
            PRINT 'Erro: O leitor especificado já está ativo.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Reativar o leitor
        UPDATE Usuario
        SET Ativo = 1
        WHERE UserID = @LeitorID;

        PRINT 'Leitor reativado com sucesso.';
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        PRINT 'Erro ao reativar o leitor. Transação revertida.';
        ROLLBACK TRANSACTION;
    END CATCH
END;
GO


-- 11 Eliminar leitores que estejam há mais de um ano sem fazer qualquer requisição, desde que não tenham nenhuma requisição ativa nesse momento
-- Função para verifica situação das requisições
CREATE FUNCTION VerificaSituacaoLeitor
    (@RequisicaoID INT)
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @Status NVARCHAR(20);
    DECLARE @PrazoDevolucao INT = 15;
    DECLARE @DiasPassados INT;

    -- Calcula os dias já passados desde a requisição
    SELECT @DiasPassados = DATEDIFF(DAY, DataRequisicao, GETDATE())
    FROM Requisicoes
    WHERE RequisicaoID = @RequisicaoID;

    -- Determina o status com base nos dias passados e no prazo
    SELECT @Status = CASE
                        WHEN @DiasPassados >= @PrazoDevolucao THEN 'ATRASO'
                        WHEN @PrazoDevolucao - @DiasPassados <= 3 THEN 'Devolução URGENTE'
                        WHEN @PrazoDevolucao - @DiasPassados <= 5 THEN 'Devolver em breve'
                        ELSE 'No prazo'
                     END;

    -- Retorna o status
    RETURN @Status;
END;
GO

-- Procedure que elimina usuário conforme critérios e adiciona dados no histórico
CREATE PROCEDURE EliminarLeitoresInativos
AS
BEGIN
    -- Tabela temporária para armazenar UserIDs que atendem aos critérios para exclusão
    DECLARE @LeitoresParaExcluir TABLE (UserID INT);

    -- Seleciona os UserIDs de leitores que estão há mais de um ano sem fazer uma requisição
    -- e que não têm nenhuma requisição ativa atualmente
    INSERT INTO @LeitoresParaExcluir (UserID)
    SELECT U.UserID
    FROM Usuario U
    LEFT JOIN Requisicoes R ON U.UserID = R.UserID
    WHERE U.TipoUser = 'Leitor'
    GROUP BY U.UserID
    HAVING (MAX(R.DataRequisicao) IS NULL OR DATEDIFF(YEAR, MAX(R.DataRequisicao), GETDATE()) >= 1) 
      AND NOT EXISTS (
          SELECT 1 
          FROM Requisicoes R2
          WHERE R2.UserID = U.UserID 
          AND dbo.VerificaSituacaoLeitor(R2.RequisicaoID) IN ('No prazo', 'Devolver em breve')
      );

    -- Insere os dados no HistoricoRequisicoes antes de excluir
    INSERT INTO HistoricoRequisicoes (
        RequisicaoID, UserID, NomeUser, Telefone, TituloObra, ExemplarID, Nucleo, DataRequisicao, DataDevolucao
    )
    SELECT 
        R.RequisicaoID,
        U.UserID,
        U.Nome AS NomeUser,
        U.Telefone,
        O.Titulo AS TituloObra,
        R.ExemplarID,
        N.Nome AS Nucleo,
        R.DataRequisicao,
        R.DataDevolucao
    FROM Requisicoes R
    INNER JOIN Usuario U ON R.UserID = U.UserID
    INNER JOIN Exemplares E ON R.ExemplarID = E.ExemplarID
    INNER JOIN Obras O ON E.ObraID = O.ObraID
    INNER JOIN Nucleos N ON E.NucleoID = N.NucleoID
    WHERE U.UserID IN (SELECT UserID FROM @LeitoresParaExcluir)
    
    UNION ALL
    
    -- Insere os leitores que nunca fizeram requisição (campos como RequisicaoID, NucleoID serão NULL)
    SELECT 
        NULL AS RequisicaoID,
        U.UserID,
        U.Nome AS NomeUser,
        U.Telefone,
        NULL AS TituloObra,
        NULL AS ExemplarID,
        NULL AS Nucleo,
        NULL AS DataRequisicao,
        NULL AS DataDevolucao
    FROM Usuario U
    WHERE U.UserID IN (SELECT UserID FROM @LeitoresParaExcluir)
      AND NOT EXISTS ( 
          SELECT 1 
          FROM Requisicoes R
          WHERE R.UserID = U.UserID
      );

    -- Exclui os leitores inativos da tabela Usuario
    DELETE FROM Usuario
    WHERE UserID IN (SELECT UserID FROM @LeitoresParaExcluir);

    PRINT 'Leitores inativos há mais de um ano e sem requisições ativas foram eliminados. O histórico foi salvo.';
END;
GO

-- Procedures do leitor

-- 1. Fazer o registo de leitor
CREATE PROCEDURE RegistrarLeitor
    @Nome NVARCHAR(100),
    @DataNascimento DATE,
    @Email NVARCHAR(150),
    @Telefone NVARCHAR(20),
    @Username NVARCHAR(50),
    @PalavraPasse NVARCHAR(50)
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY
        
        IF CHARINDEX(' ', @Nome) = 0
        BEGIN
            PRINT 'Erro: O nome deve conter pelo menos um espaço entre os dois nomes.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF CHARINDEX('@', @Email) = 0 OR CHARINDEX('.', @Email, CHARINDEX('@', @Email)) = 0
        BEGIN
            PRINT 'Erro: O email deve conter "@" e pelo menos um ponto após o "@".';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF LEN(@Telefone) <> 9 OR @Telefone LIKE '%[^0-9]%'
        BEGIN
            PRINT 'Erro: O número de telefone deve conter exatamente 9 dígitos numéricos.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM Usuario WHERE Email = @Email)
        BEGIN
            PRINT 'Erro: Já existe um usuário registrado com este email.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        IF EXISTS (SELECT 1 FROM Usuario WHERE Telefone = @Telefone)
        BEGIN
            PRINT 'Erro: Já existe um usuário registrado com este número de telefone.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, DataRegisto, Username, PalavraPasse, TipoUser, Ativo)
        VALUES (@Nome, @DataNascimento, @Email, @Telefone, GETDATE(), @Username, CONVERT(VARBINARY(MAX), @PalavraPasse), 'Leitor', 1);

        PRINT 'Novo leitor registrado com sucesso.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH

        PRINT 'Erro ao registrar o novo leitor. Transação revertida.';
        ROLLBACK TRANSACTION;
    END CATCH
END;
GO

-- 2. Cancelar a inscrição, devendo assumir-se que, nesse caso, é feita a devolução de todos os exemplares que possa ter requisitado e não tenha ainda devolvido
CREATE PROCEDURE CancelarInscricao
    @UserID INT
AS
BEGIN
    BEGIN TRY
        
        BEGIN TRANSACTION;

        -- Verificar se o usuário tem devoluções pendentes
        IF EXISTS (
            SELECT 1
            FROM Requisicoes r
            WHERE r.UserID = @UserID AND r.DataDevolucao IS NULL
        )
        BEGIN
            -- Atualizar a data de devolução das requisições pendentes
            UPDATE Requisicoes
            SET DataDevolucao = GETDATE()
            WHERE UserID = @UserID AND DataDevolucao IS NULL;

            -- Atualizar os exemplares correspondentes para Disponível = 1
            UPDATE Exemplares
            SET Disponivel = 1
            WHERE ExemplarID IN (
                SELECT ExemplarID
                FROM Requisicoes
                WHERE UserID = @UserID AND DataDevolucao = GETDATE()
            );

            PRINT 'Exemplares devolvidos com sucesso.';
        END;

        -- Desativar o usuário na tabela Usuario
        UPDATE Usuario
        SET Ativo = 0
        WHERE UserID = @UserID;

        PRINT 'Inscrição cancelada com sucesso.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        PRINT 'Erro ao cancelar inscrição e devolver exemplares.';
    END CATCH
END;
GO

--3. Pesquisar as obras disponiveis, em geral ou por nucleo e/ou por tema
CREATE PROCEDURE PesquisarObrasDisponiveis
    @NucleoID INT = NULL,
    @Tema NVARCHAR(50) = NULL,
    @Titulo NVARCHAR(100) = NULL,
    @Autor NVARCHAR(100) = NULL
AS
BEGIN
    IF @NucleoID IS NULL
    BEGIN
        -- Pesquisar obras disponíveis, agrupando por título (sem considerar núcleo)
        SELECT 
            o.Titulo AS Obra,
            o.Autor,
            o.AnoPublicacao,
            o.Genero AS Tema,
            SUM(CASE WHEN e.Disponivel = 1 THEN 1 ELSE 0 END) AS QuantidadeDisponivel
        FROM 
            Obras o
        JOIN 
            Exemplares e ON o.ObraID = e.ObraID
        WHERE 
            (@Tema IS NULL OR o.Genero = @Tema) AND
            (@Titulo IS NULL OR o.Titulo LIKE '%' + @Titulo + '%') AND
            (@Autor IS NULL OR o.Autor LIKE '%' + @Autor + '%')
        GROUP BY 
            o.Titulo, o.Autor, o.AnoPublicacao, o.Genero
        HAVING 
            SUM(CASE WHEN e.Disponivel = 1 THEN 1 ELSE 0 END) > 0
        ORDER BY 
            o.Titulo;
    END
    ELSE
    BEGIN
        -- Pesquisar obras disponíveis, filtrando por núcleo
        SELECT 
            o.Titulo AS Obra,
            o.Autor,
            o.AnoPublicacao,
            o.Genero AS Tema,
            n.Nome AS Nucleo,
            COUNT(e.ExemplarID) AS QuantidadeDisponivel
        FROM 
            Obras o
        JOIN 
            Exemplares e ON o.ObraID = e.ObraID
        JOIN 
            Nucleos n ON e.NucleoID = n.NucleoID
        WHERE 
            e.Disponivel = 1 AND
            n.NucleoID = @NucleoID AND
            (@Tema IS NULL OR o.Genero = @Tema) AND
            (@Titulo IS NULL OR o.Titulo LIKE '%' + @Titulo + '%') AND
            (@Autor IS NULL OR o.Autor LIKE '%' + @Autor + '%')
        GROUP BY 
            o.Titulo, o.Autor, o.AnoPublicacao, o.Genero, n.Nome
        HAVING 
            COUNT(e.ExemplarID) > 0
        ORDER BY 
            o.Titulo;
    END
END;
GO

-- 4.1 Obras requisitadas, em cada momento, por nucleo, devendo aparecer a indicação “ATRASO”, “Devolução URGENTE” ou “Devolver em breve”, caso o prazo já tenha sido ultrapassado, faltem menos de três dias para a devolução ou faltem cinco dias para a devolução, respetivamente--
CREATE PROCEDURE VerificarSituacaoAtual
    @UserID INT
AS
BEGIN
    SELECT 
        u.Nome AS Leitor,
        o.Titulo AS Obra,
        n.Nome AS Nucleo,
        r.DataRequisicao,
        CASE 
            WHEN GETDATE() > DATEADD(DAY, 15, r.DataRequisicao) THEN 'ATRASO'
            WHEN DATEDIFF(DAY, GETDATE(), DATEADD(DAY, 15, r.DataRequisicao)) <= 3 THEN 'Devolução URGENTE'
            WHEN DATEDIFF(DAY, GETDATE(), DATEADD(DAY, 15, r.DataRequisicao)) <= 5 THEN 'Devolver em breve'
            ELSE 'No Prazo'
        END AS StatusDevolucao
    FROM 
        Requisicoes r
    JOIN 
        Usuario u ON r.UserID = u.UserID
    JOIN 
        Exemplares e ON r.ExemplarID = e.ExemplarID
    JOIN 
        Obras o ON e.ObraID = o.ObraID
    JOIN 
        Nucleos n ON e.NucleoID = n.NucleoID
    WHERE 
        r.UserID = @UserID
        AND r.DataDevolucao IS NULL 
END;
GO

-- 4.2 Histórico de todas as obras requisitadas, podendo opcionalmente indicar um agrupamento por nucleo e/ou um intervalo de datas
CREATE PROCEDURE HistoricoRequisicoesLeitor
    @UserID INT,
    @NucleoID INT = NULL,
    @DataInicio DATE = NULL,
    @DataFim DATE = NULL
AS
BEGIN
    SELECT 
        u.Nome AS Leitor,
        o.Titulo AS Obra,
        n.Nome AS Nucleo,
        r.DataRequisicao,
        CASE 
            WHEN DataDevolucao IS NULL THEN 'Em Aberto'
            ELSE 'Devolvido'
        END AS StatusRequisicao
    FROM 
        Requisicoes r
    JOIN 
        Usuario u ON r.UserID = u.UserID
    JOIN 
        Exemplares e ON r.ExemplarID = e.ExemplarID
    JOIN 
        Obras o ON e.ObraID = o.ObraID
    JOIN 
        Nucleos n ON e.NucleoID = n.NucleoID
    WHERE 
        r.UserID = @UserID AND
        (@NucleoID IS NULL OR n.NucleoID = @NucleoID) AND
        (@DataInicio IS NULL OR r.DataRequisicao >= @DataInicio) AND
        (@DataFim IS NULL OR r.DataRequisicao <= @DataFim)
    ORDER BY 
        r.DataRequisicao DESC;
END;
GO

-- Requisitar exemplares
CREATE PROCEDURE FazerRequisicaoLivro
    @UserID INT,
    @ObraID INT,
    @NucleoID INT,
    @BibliotecarioID INT
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
            SELECT 1
            FROM Usuario
            WHERE UserID = @UserID
              AND TipoUser = 'Leitor'
              AND Ativo = 1
        )
        BEGIN
            PRINT 'Apenas leitores ativos podem fazer requisições.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar se o usuário já possui 4 requisições ativas
        DECLARE @RequisicoesAtivas INT;
        SELECT @RequisicoesAtivas = COUNT(*)
        FROM Requisicoes
        WHERE UserID = @UserID
          AND DataDevolucao IS NULL;

        IF @RequisicoesAtivas >= 4
        BEGIN
            PRINT 'O leitor já possui o limite de 4 requisições ativas.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar se há mais de um exemplar disponível no núcleo escolhido
        DECLARE @QtdDisponivel INT;
        SELECT @QtdDisponivel = COUNT(*)
        FROM Exemplares
        WHERE ObraID = @ObraID
          AND NucleoID = @NucleoID
          AND Disponivel = 1;

        -- Se não houver exemplares disponíveis ou só houver 1 disponível, não permitir a requisição
        IF @QtdDisponivel IS NULL OR @QtdDisponivel < 2
        BEGIN
            PRINT 'Não há exemplares suficientes para requisição neste núcleo. O núcleo deve ter pelo menos um exemplar disponível para consulta.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Verificar se o exemplar a ser requisitado é o único exemplar disponível no núcleo
        DECLARE @ExemplarID INT;
        SELECT TOP 1 @ExemplarID = ExemplarID
        FROM Exemplares
        WHERE ObraID = @ObraID
          AND NucleoID = @NucleoID
          AND Disponivel = 1;

        -- Se restar apenas 1 exemplar disponível, não permitir a requisição
        IF (SELECT COUNT(*) FROM Exemplares WHERE ObraID = @ObraID AND NucleoID = @NucleoID AND Disponivel = 1) = 1
        BEGIN
            PRINT 'Este é o único exemplar disponível no núcleo, e ele não pode ser requisitado.';
            ROLLBACK TRANSACTION;
            RETURN;
        END

        -- Atualizar o exemplar para inativo (ficar indisponível para consulta)
        UPDATE Exemplares
        SET Disponivel = 0
        WHERE ExemplarID = @ExemplarID;

        -- Adicionar a nova requisição
        INSERT INTO Requisicoes (UserID, ExemplarID, DataRequisicao)
        VALUES (@UserID, @ExemplarID, GETDATE());

        PRINT 'Requisição realizada com sucesso. Lembre-se de devolver o livro em até 15 dias.';

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Erro ao realizar a requisição. Verifique os dados inseridos e tente novamente.';
        PRINT ERROR_MESSAGE();
    END CATCH
END;
GO

--Fazer uma devolução
CREATE PROCEDURE DevolverObra
    @UserID INT,
    @RequisicaoID INT
AS
BEGIN
    DECLARE @TipoUser VARCHAR(15);
    SELECT @TipoUser = TipoUser FROM Usuario WHERE UserID = @UserID;

    IF @TipoUser <> 'Bibliotecario'
    BEGIN
        PRINT 'Acesso negado. Somente bibliotecários podem realizar devoluções.';
        RETURN;
    END

    BEGIN TRY
        -- Iniciar uma transação para garantir consistência
        BEGIN TRANSACTION;

        -- Atualizar a data de devolução na tabela Requisicoes
        UPDATE Requisicoes
        SET DataDevolucao = GETDATE()
        WHERE RequisicaoID = @RequisicaoID AND DataDevolucao IS NULL;

        -- Verificar qual o ExemplarID da requisição devolvida
        DECLARE @ExemplarID INT;
        SELECT @ExemplarID = ExemplarID 
        FROM Requisicoes 
        WHERE RequisicaoID = @RequisicaoID;

        -- Marcar o exemplar como disponível
        UPDATE Exemplares
        SET Disponivel = 1
        WHERE ExemplarID = @ExemplarID;

        COMMIT TRANSACTION;

        PRINT 'Devolução realizada com sucesso.';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT 'Erro ao processar a devolução.';
        PRINT ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE PROCEDURE PopularDB
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
		-- Inserir dados na tabela usuario
		INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, DataRegisto, Username, PalavraPasse, TipoUser, Ativo) VALUES 
		('Alice Martins', '1988-03-15', 'alice.martins@example.com', '987654321', '2019-01-15', 'alicem', HASHBYTES('SHA2_256', 'password1'), 'Leitor', 1),
		('Bruno Silva', '1990-07-22', 'bruno.silva@example.com', '912345678', '2018-02-20', 'brunos', HASHBYTES('SHA2_256', 'password2'), 'Bibliotecario', 1),
		('Carla Andrade', '1995-01-10', 'carla.andrade@example.com', '965432198', '2019-03-25', 'carlaa', HASHBYTES('SHA2_256', 'password3'), 'Leitor', 1),
		('Diego Souza', '1980-04-18', 'diego.souza@example.com', '934567123', '2019-04-10', 'diegos', HASHBYTES('SHA2_256', 'password4'), 'Leitor', 0),
		('Elena Gomes', '1992-09-30', 'elena.gomes@example.com', '956789012', '2019-05-05', 'elenag', HASHBYTES('SHA2_256', 'password5'), 'Leitor', 1),
		('Fernando Costa', '1983-06-07', 'fernando.costa@example.com', '923456789', '2018-06-15', 'fernandoc', HASHBYTES('SHA2_256', 'password6'), 'Bibliotecario', 1),
		('Gabriel Menezes', '1985-12-25', 'gabriel.menezes@example.com', '987123456', '2019-07-20', 'gabrielm', HASHBYTES('SHA2_256', 'password7'), 'Leitor', 1),
		('Helena Ramos', '1998-08-14', 'helena.ramos@example.com', '932156789', '2019-08-10', 'helenar', HASHBYTES('SHA2_256', 'password8'), 'Leitor', 1),
		('Igor Nunes', '1991-11-11', 'igor.nunes@example.com', '921234567', '2020-09-12', 'igorn', HASHBYTES('SHA2_256', 'password9'), 'Leitor', 1),
		('Julia Ferreira', '1993-02-03', 'julia.ferreira@example.com', '954321678', '2020-10-05', 'juliaf', HASHBYTES('SHA2_256', 'password10'), 'Leitor', 0),
		('Lucas Vieira', '1984-11-15', 'lucas.vieira@example.com', '912345987', '2020-11-01', 'lucasv', HASHBYTES('SHA2_256', 'password11'), 'Leitor', 1),
		('Maria Silva', '1979-12-25', 'maria.silva@example.com', '912543789', '2020-11-25', 'marias', HASHBYTES('SHA2_256', 'password12'), 'Leitor', 0),
		('Nina Oliveira', '1987-07-04', 'nina.oliveira@example.com', '915432987', '2020-01-10', 'ninao', HASHBYTES('SHA2_256', 'password13'), 'Leitor', 1),
		('Otávio Campos', '1996-03-18', 'otavio.campos@example.com', '918654321', '2020-02-20', 'otavioc', HASHBYTES('SHA2_256', 'password14'), 'Leitor', 1),
		('Patricia Souza', '1999-06-05', 'patricia.souza@example.com', '913245678', '2020-03-15', 'pats', HASHBYTES('SHA2_256', 'password15'), 'Leitor', 1),
		('Quintino Santos', '1990-10-22', 'quintino.santos@example.com', '911234567', '2020-04-02', 'quints', HASHBYTES('SHA2_256', 'password16'), 'Leitor', 1),
		('Renata Lopes', '1991-08-10', 'renata.lopes@example.com', '911987654', '2020-05-10', 'renatal', HASHBYTES('SHA2_256', 'password17'), 'Leitor', 1),
		('Samuel Mendes', '1986-01-19', 'samuel.mendes@example.com', '910234567', '2020-06-22', 'samm', HASHBYTES('SHA2_256', 'password18'), 'Leitor', 1),
		('Tatiana Lima', '1989-11-30', 'tatiana.lima@example.com', '914567890', '2020-07-18', 'tatil', HASHBYTES('SHA2_256', 'password19'), 'Leitor', 0),
		('Ulisses Rocha', '1994-05-17', 'ulisses.rocha@example.com', '912987654', '2020-08-14', 'ulir', HASHBYTES('SHA2_256', 'password20'), 'Leitor', 1);

		-- Inserir dados na tabela de núcleos
		INSERT INTO Nucleos (Nome, Endereco) VALUES
		('Central Lisboa', 'Rua das Flores'),
		('Núcleo Porto', 'Rua da Hora'),
		('Núcleo Coimbra', 'Rua do Navio'),
		('Núcleo Faro', 'Avenida do Sol'),
		('Núcleo Braga', 'Travessa do Castelo'),
		('Núcleo Aveiro', 'Rua dos Pescadores'),
		('Núcleo Setúbal', 'Rua da Praia'),
		('Núcleo Évora', 'Largo da Sé'),
		('Núcleo Viseu', 'Avenida Principal'),
		('Núcleo Leiria', 'Rua do Comércio');

		-- Inserir dados na tabela de obras
		INSERT INTO Obras (Titulo, Autor, AnoPublicacao, Genero, Descricao) VALUES
		('O Senhor dos Anéis', 'J.R.R. Tolkien', 1954, 'Fantasia', 'Uma das maiores obras de fantasia.'),
		('1984', 'George Orwell', 1949, 'Distopia', 'Uma obra sobre um futuro totalitário.'),
		('Dom Quixote', 'Miguel de Cervantes', 1605, 'Clássico', 'A história de um cavaleiro que luta contra moinhos de vento.'),
		('Moby Dick', 'Herman Melville', 1851, 'Aventura', 'A perseguição de uma baleia branca.'),
		('Pride and Prejudice', 'Jane Austen', 1813, 'Romance', 'Um romance clássico da literatura inglesa.'),
		('Cem Anos de Solidão', 'Gabriel García Márquez', 1967, 'Realismo Mágico', 'A saga da família Buendía ao longo de várias gerações.'),
		('O Grande Gatsby', 'F. Scott Fitzgerald', 1925, 'Romance', 'A história de Jay Gatsby e sua busca pelo sonho americano.'),
		('O Sol é para Todos', 'Harper Lee', 1960, 'Ficção', 'Uma história sobre racismo e justiça no sul dos EUA.'),
		('O Hobbit', 'J.R.R. Tolkien', 1937, 'Fantasia', 'A aventura de Bilbo Bolseiro em uma jornada inesperada.'),
		('O Código Da Vinci', 'Dan Brown', 2003, 'Mistério', 'Uma investigação envolvendo símbolos e segredos históricos.'),
		('O Pequeno Príncipe', 'Antoine de Saint-Exupéry', 1943, 'Infantil', 'A história filosófica de um príncipe que viaja por planetas.'),
		('A Revolução dos Bichos', 'George Orwell', 1945, 'Fábula', 'Uma alegoria política sobre a Revolução Russa.'),
		('O Morro dos Ventos Uivantes', 'Emily Brontë', 1847, 'Romance', 'A intensa história de amor e vingança entre Heathcliff e Catherine.'),
		('A Metamorfose', 'Franz Kafka', 1915, 'Surrealismo', 'A história de um homem que se transforma em um inseto gigante.'),
		('O Apanhador no Campo de Centeio', 'J.D. Salinger', 1951, 'Ficção', 'A crise de identidade de Holden Caulfield, um adolescente em crise.'),
		('O Alquimista', 'Paulo Coelho', 1988, 'Ficção', 'A jornada de Santiago em busca de seu tesouro pessoal.'),
		('Crime e Castigo', 'Fiódor Dostoiévski', 1866, 'Drama', 'A luta moral de Raskólnikov após cometer um assassinato.'),
		('A Menina que Roubava Livros', 'Markus Zusak', 2005, 'Drama', 'Uma menina encontra consolo nos livros durante a Segunda Guerra Mundial.'),
		('A Ilha do Tesouro', 'Robert Louis Stevenson', 1883, 'Aventura', 'A clássica história de piratas e a busca por um tesouro perdido.'),
		('Frankenstein', 'Mary Shelley', 1818, 'Terror', 'A história de Victor Frankenstein e a criação de um monstro que desafia a moral e a ciência.');

		-- Inserir dados na tabela de Imagens(capas)
		INSERT INTO Imagens (ID_ObraID,Capa) VALUES 
		(1, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa1.jpg', SINGLE_BLOB) AS Capa)),
		(2, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa2.jpg', SINGLE_BLOB) AS Capa)),
		(3, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa3.jpg', SINGLE_BLOB) AS Capa)),
		(4, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa4.jpg', SINGLE_BLOB) AS Capa)),
		(5, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa5.jpg', SINGLE_BLOB) AS Capa)),
		(6, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa6.jpg', SINGLE_BLOB) AS Capa)),
		(7, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa7.jpg', SINGLE_BLOB) AS Capa)),
		(8, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa8.jpg', SINGLE_BLOB) AS Capa)),
		(9, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa9.jpg', SINGLE_BLOB) AS Capa)),
		(10, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa10.jpg', SINGLE_BLOB) AS Capa)),
		(11, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa11.jpg', SINGLE_BLOB) AS Capa)),
		(12, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa12.jpg', SINGLE_BLOB) AS Capa)),
		(13, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa13.jpg', SINGLE_BLOB) AS Capa)),
		(14, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa14.jpg', SINGLE_BLOB) AS Capa)),
		(15, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa15.jpg', SINGLE_BLOB) AS Capa)),
		(16, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa16.jpg', SINGLE_BLOB) AS Capa)),
		(17, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa17.jpg', SINGLE_BLOB) AS Capa)),
		(18, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa18.jpg', SINGLE_BLOB) AS Capa)),
		(19, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa19.jpg', SINGLE_BLOB) AS Capa)),
		(20, (SELECT * FROM OPENROWSET(BULK 'C:\temp\img\capa20.jpg', SINGLE_BLOB) AS Capa));

		-- Inserir dados na tabela de exemplares
		INSERT INTO Exemplares (ObraID, NucleoID, Disponivel) VALUES
		(1, 1, 1), (1, 2, 1), (1, 3, 1),(1, 4, 1),(1, 5, 1),(1, 6, 1),(1, 7, 1),(1, 8, 1),(1, 9, 1),(1, 10, 1),
		(2, 1, 1), (2, 2, 1), (2, 3, 1),(2, 4, 1),(2, 5, 1),(2, 6, 1),(2, 7, 1),(2, 8, 1),(2, 9, 1),(2, 10, 1),
		(3, 1, 1), (3, 2, 1), (3, 3, 1),(3, 4, 1),(3, 5, 1),(3, 6, 1),(3, 7, 1),(3, 8, 1),(3, 9, 1),(3, 10, 1),
		(4, 1, 1), (4, 2, 1), (4, 3, 1),(4, 4, 1),(4, 5, 1),(4, 6, 1),(4, 7, 1),(4, 8, 1),(4, 9, 1),(4, 10, 1),
		(5, 1, 0), (5, 2, 1), (5, 3, 1),(5, 4, 1),(5, 5, 1),(5, 6, 1),(5, 7, 1),(5, 8, 1),(5, 9, 1),(5, 10, 1),
		(6, 1, 1), (6, 2, 1), (6, 3, 1),(6, 4, 0),(6, 5, 1),(6, 6, 1),(6, 7, 1),(6, 8, 1),(6, 9, 1),(6, 10, 1),
		(7, 1, 1), (7, 2, 1), (7, 3, 1),(7, 4, 1),(7, 5, 1),(7, 6, 1),(7, 7, 1),(7, 8, 1),(7, 9, 1),(7, 10, 1),
		(8, 1, 1), (8, 2, 1), (8, 3, 1),(8, 4, 1),(8, 5, 1),(8, 6, 1),(8, 7, 1),(8, 8, 0),(8, 9, 0),(8, 10, 1),
		(9, 1, 1), (9, 2, 1), (9, 3, 0),(9, 4, 1),(9, 5, 1),(9, 6, 1),(9, 7, 1),(9, 8, 1),(9, 9, 1),(9, 10, 1),
		(10, 1, 0), (10, 2, 1), (10, 3, 1),(10, 4, 1),(10, 5, 1),(10, 6, 1),(10, 7, 1),(10, 8, 1),(10, 9, 1),(10, 10, 1),
		(11, 1, 1), (11, 2, 1), (11, 3, 1),(11, 4, 1),(11, 5, 1),(11, 6, 1),(11, 7, 1),(11, 8, 1),(11, 9, 1),(11, 10, 1),
		(12, 1, 1),(12, 2, 1),(12, 3, 1),(12, 4, 1),(12, 5, 1),(12, 6, 1),(12, 7, 1),(12, 8, 1),(12, 9, 1),(12, 10, 1),
		(13, 1, 1),(13, 2, 1),(13, 3, 1),(13, 4, 1),(13, 5, 1),(13, 6, 1),(13, 7, 1),(13, 8, 1),(13, 9, 1),(13, 10, 1),
		(14, 1, 1),(14, 2, 1),(14, 3, 1),(14, 4, 1),(14, 5, 1),(14, 6, 1),(14, 7, 1),(14, 8, 1),(14, 9, 1),(14, 10, 1),
		(15, 1, 1),(15, 2, 1),(15, 3, 1),(15, 4, 1),(15, 5, 1),(15, 6, 1),(15, 7, 1),(15, 8, 1),(15, 9, 1),(15, 10, 1),
		(16, 1, 1),(16, 2, 1),(16, 3, 1),(16, 4, 1),(16, 5, 1),(16, 6, 1),(16, 7, 1),(16, 8, 1),(16, 9, 1),(16, 10, 1),
		(17, 1, 1),(17, 2, 1),(17, 3, 1),(17, 4, 1),(17, 5, 1),(17, 6, 1),(17, 7, 1),(17, 8, 1),(17, 9, 1),(17, 10, 1),
		(18, 1, 1),(18, 2, 1),(18, 3, 1),(18, 4, 1),(18, 5, 1),(18, 6, 1),(18, 7, 1),(18, 8, 1),(18, 9, 1),(18, 10, 1),
		(19, 1, 1),(19, 2, 1),(19, 3, 1),(19, 4, 1),(19, 5, 1),(19, 6, 1),(19, 7, 1),(19, 8, 1),(19, 9, 1),(19, 10, 1),
		(20, 1, 1),(20, 2, 1),(20, 3, 1),(20, 4, 1),(20, 5, 1),(20, 6, 1),(20, 7, 1),(20, 8, 1),(20, 9, 1),(20, 10, 1),
		-- segunda linha de exemplares
		(1, 1, 1), (1, 2, 1), (1, 3, 1),(1, 4, 1),(1, 5, 1),(1, 6, 1),(1, 7, 1),(1, 8, 1),(1, 9, 1),(1, 10, 1),
		(2, 1, 1), (2, 2, 1), (2, 3, 1),(2, 4, 1),(2, 5, 1),(2, 6, 1),(2, 7, 1),(2, 8, 1),(2, 9, 1),(2, 10, 1),
		(3, 1, 1), (3, 2, 1), (3, 3, 1),(3, 4, 1),(3, 5, 1),(3, 6, 1),(3, 7, 1),(3, 8, 1),(3, 9, 1),(3, 10, 1),
		(4, 1, 1), (4, 2, 1), (4, 3, 1),(4, 4, 1),(4, 5, 1),(4, 6, 1),(4, 7, 1),(4, 8, 1),(4, 9, 1),(4, 10, 1),
		(5, 1, 1), (5, 2, 1), (5, 3, 1),(5, 4, 1),(5, 5, 1),(5, 6, 1),(5, 7, 1),(5, 8, 1),(5, 9, 1),(5, 10, 1),
		(6, 1, 1), (6, 2, 1), (6, 3, 1),(6, 4, 1),(6, 5, 1),(6, 6, 1),(6, 7, 1),(6, 8, 1),(6, 9, 1),(6, 10, 1),
		(7, 1, 1), (7, 2, 1), (7, 3, 1),(7, 4, 1),(7, 5, 1),(7, 6, 1),(7, 7, 1),(7, 8, 1),(7, 9, 1),(7, 10, 1),
		(8, 1, 1), (8, 2, 1), (8, 3, 1),(8, 4, 1),(8, 5, 1),(8, 6, 1),(8, 7, 1),(8, 8, 1),(8, 9, 1),(8, 10, 1),
		(9, 1, 1), (9, 2, 1), (9, 3, 1),(9, 4, 1),(9, 5, 1),(9, 6, 1),(9, 7, 1),(9, 8, 1),(9, 9, 1),(9, 10, 1),
		(10, 1, 1), (10, 2, 1), (10, 3, 1),(10, 4, 1),(10, 5, 1),(10, 6, 1),(10, 7, 1),(10, 8, 1),(10, 9, 1),(10, 10, 1),
		(11, 1, 1), (11, 2, 1), (11, 3, 1),(11, 4, 1),(11, 5, 1),(11, 6, 1),(11, 7, 1),(11, 8, 1),(11, 9, 1),(11, 10, 1),
		(12, 1, 1),(12, 2, 1),(12, 3, 1),(12, 4, 1),(12, 5, 1),(12, 6, 1),(12, 7, 1),(12, 8, 1),(12, 9, 1),(12, 10, 1),
		(13, 1, 1),(13, 2, 1),(13, 3, 1),(13, 4, 1),(13, 5, 1),(13, 6, 1),(13, 7, 1),(13, 8, 1),(13, 9, 1),(13, 10, 1),
		(14, 1, 1),(14, 2, 1),(14, 3, 1),(14, 4, 1),(14, 5, 1),(14, 6, 1),(14, 7, 1),(14, 8, 1),(14, 9, 1),(14, 10, 1),
		(15, 1, 1),(15, 2, 1),(15, 3, 1),(15, 4, 1),(15, 5, 1),(15, 6, 1),(15, 7, 1),(15, 8, 1),(15, 9, 1),(15, 10, 1),
		(16, 1, 1),(16, 2, 1),(16, 3, 1),(16, 4, 1),(16, 5, 1),(16, 6, 1),(16, 7, 1),(16, 8, 1),(16, 9, 1),(16, 10, 1),
		(17, 1, 1),(17, 2, 1),(17, 3, 1),(17, 4, 1),(17, 5, 1),(17, 6, 1),(17, 7, 1),(17, 8, 1),(17, 9, 1),(17, 10, 1),
		(18, 1, 1),(18, 2, 1),(18, 3, 1),(18, 4, 1),(18, 5, 1),(18, 6, 1),(18, 7, 1),(18, 8, 1),(18, 9, 1),(18, 10, 1),
		(19, 1, 1),(19, 2, 1),(19, 3, 1),(19, 4, 1),(19, 5, 1),(19, 6, 1),(19, 7, 1),(19, 8, 1),(19, 9, 1),(19, 10, 1),
		(20, 1, 1),(20, 2, 1),(20, 3, 1),(20, 4, 1),(20, 5, 1),(20, 6, 1),(20, 7, 1),(20, 8, 1),(20, 9, 1),(20, 10, 1);

		-- Inserir requisições para alguns dos 20 usuários
		INSERT INTO Requisicoes (UserID, ExemplarID, DataRequisicao, DataDevolucao) VALUES
		(1, 1, '2022-11-01', '2022-11-20'),
		(1, 2, '2022-11-02', '2022-11-28'),
		(3, 11, '2024-10-03', '2024-10-17'),
		(4, 12, '2024-09-04', '2024-09-18'),
		(5, 21, '2024-05-05', '2024-05-19'),
		(1, 22, '2022-06-06', '2022-06-30'),
		(7, 31, '2024-06-07', '2024-06-21'),
		(8, 32, '2024-03-08', '2024-03-22'),
		(9, 41, '2024-02-09', '2024-02-23'),
		(10, 42, '2024-01-10', '2024-01-24'),
		(11, 51, '2024-11-11', Null),
		(12, 52, '2024-09-12', '2024-09-26'),
		(13, 63, '2024-10-13', '2024-10-27'),
		(12, 64, '2024-11-14', Null),
		(15, 75, '2024-08-15', '2024-08-29'),
		(16, 76, '2024-10-16', '2024-10-30'),
		(17, 88, '2024-08-17', Null),
		(18, 89, '2024-11-14', Null),
		(19, 93, '2024-11-14', Null),
		(20, 101, '2024-11-05', Null);

		COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT ERROR_MESSAGE();
    END CATCH
END;
GO

CREATE PROCEDURE EliminarConteudoTabelas
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
	DELETE FROM HistoricoRequisicoes;
	DELETE FROM Requisicoes;
	DELETE FROM Imagens;
	DELETE FROM Exemplares;
	DELETE FROM Obras;
	DELETE FROM Nucleos;
	DELETE FROM Usuario;

	DBCC CHECKIDENT('Requisicoes', RESEED, 0);
    DBCC CHECKIDENT('Exemplares', RESEED, 0);
	DBCC CHECKIDENT('HistoricoRequisicoes', RESEED, 0);
    DBCC CHECKIDENT('Obras', RESEED, 0);
    DBCC CHECKIDENT('Nucleos', RESEED, 0);
    DBCC CHECKIDENT('Usuario', RESEED, 0);

	COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        PRINT ERROR_MESSAGE();
    END CATCH
END;
GO

--Procedures do blibliotecario para executar

EXEC ConsultarTotalObras; -- 1

EXEC ConsultarObrasPorGenero; -- 2

EXEC Top10ObrasRequisitadas --3
	'2024-01-22',
	'2024-11-22';

EXEC NucleosPorRequisicoes --4
	'2018-01-22',
	'2024-11-22';

EXEC AdicionarObra -- 5
	'Memorias Postumas de Bras Cubas',
	'Machado de Assis',
	1880,
	'Romance',
	'Narrado em primeira pessoa, seu autor é Brás Cubas, um defunto-autor, isto é, um homem que já morreu e que deseja escrever a sua autobiografia.',
	'C:\temp\img\Capa21.jpg';

EXEC AdicionarExemplaresAoNucleoPrincipal -- 6
    @BibliotecarioID = 2,
    @ObraID = 2,
    @QtdAdicionar = 2;

EXEC TransferirExemplar -- 7
    @BibliotecarioID = 2, 
    @ObraID = 2,
    @OrigemNucleoID = 1,
    @DestinoNucleoID = 6,
    @QtdTransferir = 4;

EXEC RegistrarNovoLeitor -- 8
    @BibliotecarioID = 2, 
    @Nome = 'Maria Silva', 
    @DataNascimento = '1998-05-15', 
    @Email = 'maria.silva@email.com', 
    @Telefone = '931827192', 
    @Username = 'marias', 
    @PalavraPasse = 'SenhaForte321';

EXEC SuspenderAcessoLeitoresComDevolucoesAtrasadas -- 9
	@BibliotecarioID = 2;

EXEC ReativarLeitor -- 10
	@BibliotecarioID = 2,
	@LeitorID = 21;

EXEC EliminarLeitoresInativos; -- 11

--Procedures do Leitor para executar
EXEC RegistrarLeitor -- 1
    @Nome = 'Afonso Pereira',
    @DataNascimento = '2001-04-15',
    @Email = 'afonso.pereira@example.com',
    @Telefone = '912745678',
    @Username = 'afonso95',
    @PalavraPasse = 'SenhaForte123';

EXEC CancelarInscricao -- 2
	@UserID = 21;

EXEC PesquisarObrasDisponiveis -- 3
	@Autor = 'Tolkien',
	@NucleoID = 1,
	@Titulo = 'Senhor';

EXEC VerificarSituacaoAtual -- 4.1
	@UserID = 21;

EXEC HistoricoRequisicoesLeitor --4.2
    @UserID = 21, 
    @DataInicio = '2022-11-01', 
    @DataFim = '2024-12-01';

EXEC FazerRequisicaoLivro
	@UserID = 21,
	@ObraID = 2,
	@NucleoID = 1,
	@BibliotecarioID = 2;

EXEC DevolverObra
    @UserID = 2,
    @RequisicaoID = 28;

EXEC PopularDB;

EXEC EliminarConteudoTabelas;