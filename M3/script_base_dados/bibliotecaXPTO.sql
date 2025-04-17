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
('Diego Souza', '1980-04-18', 'diego.souza@example.com', '934567123', '2019-04-10', 'diegos', HASHBYTES('SHA2_256', 'password4'), 'Leitor', 1),
('Elena Gomes', '1992-09-30', 'elena.gomes@example.com', '956789012', '2019-05-05', 'elenag', HASHBYTES('SHA2_256', 'password5'), 'Leitor', 1),
('Fernando Costa', '1983-06-07', 'fernando.costa@example.com', '923456789', '2018-06-15', 'fernandoc', HASHBYTES('SHA2_256', 'password6'), 'Bibliotecario', 1),
('Gabriel Menezes', '1985-12-25', 'gabriel.menezes@example.com', '987123456', '2019-07-20', 'gabrielm', HASHBYTES('SHA2_256', 'password7'), 'Leitor', 1),
('Helena Ramos', '1998-08-14', 'helena.ramos@example.com', '932156789', '2019-08-10', 'helenar', HASHBYTES('SHA2_256', 'password8'), 'Leitor', 1),
('Igor Nunes', '1991-11-11', 'igor.nunes@example.com', '921234567', '2020-09-12', 'igorn', HASHBYTES('SHA2_256', 'password9'), 'Leitor', 1),
('Julia Ferreira', '1993-02-03', 'julia.ferreira@example.com', '954321678', '2020-10-05', 'juliaf', HASHBYTES('SHA2_256', 'password10'), 'Leitor', 1),
('Lucas Vieira', '1984-11-15', 'lucas.vieira@example.com', '912345987', '2020-11-01', 'lucasv', HASHBYTES('SHA2_256', 'password11'), 'Leitor', 1),
('Maria Silva', '1979-12-25', 'maria.silva@example.com', '912543789', '2020-11-25', 'marias', HASHBYTES('SHA2_256', 'password12'), 'Leitor', 1),
('Nina Oliveira', '1987-07-04', 'nina.oliveira@example.com', '915432987', '2020-01-10', 'ninao', HASHBYTES('SHA2_256', 'password13'), 'Leitor', 1),
('Otávio Campos', '1996-03-18', 'otavio.campos@example.com', '918654321', '2020-02-20', 'otavioc', HASHBYTES('SHA2_256', 'password14'), 'Leitor', 0),
('Patricia Souza', '1999-06-05', 'patricia.souza@example.com', '913245678', '2020-03-15', 'pats', HASHBYTES('SHA2_256', 'password15'), 'Leitor', 0),
('Quintino Santos', '1990-10-22', 'quintino.santos@example.com', '911234567', '2020-04-02', 'quints', HASHBYTES('SHA2_256', 'password16'), 'Leitor', 1),
('Renata Lopes', '1991-08-10', 'renata.lopes@example.com', '911987654', '2020-05-10', 'renatal', HASHBYTES('SHA2_256', 'password17'), 'Leitor', 1),
('Samuel Mendes', '1986-01-19', 'samuel.mendes@example.com', '910234567', '2020-06-22', 'samm', HASHBYTES('SHA2_256', 'password18'), 'Leitor', 1),
('Tatiana Lima', '1989-11-30', 'tatiana.lima@example.com', '914567890', '2020-07-18', 'tatil', HASHBYTES('SHA2_256', 'password19'), 'Leitor', 1),
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
(1, 24, '2022-01-05', '2022-01-30'),
(7, 31, '2024-06-07', '2024-06-21'),
(8, 32, '2024-03-08', '2024-03-22'),
(9, 41, '2024-02-09', '2024-02-23'),
(10, 42, '2024-01-10', '2024-01-24'),
(11, 51, '2024-11-11', Null),
(12, 52, '2024-09-12', '2024-09-26'),
(13, 63, '2024-10-13', '2024-10-27'),
(12, 64, '2024-11-14', Null),
(16, 75, '2024-08-15', '2024-08-29'),
(16, 76, '2024-10-16', '2024-10-30'),
(12, 62, '2024-09-12', '2024-11-26'),
(12, 70, '2024-08-12', '2024-11-26'),
(12, 70, '2024-09-12', '2024-11-26'),
(17, 88, '2024-08-17', Null),
(18, 89, '2024-11-14', Null),
(19, 93, '2024-11-14', Null),
(20, 101, '2024-11-05', Null);



ALTER PROCEDURE PopularDB
AS
BEGIN
    BEGIN TRANSACTION;
    BEGIN TRY
		-- Inserir dados na tabela usuario
		INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, DataRegisto, Username, PalavraPasse, TipoUser, Ativo) VALUES 
		('Alice Martins', '1988-03-15', 'alice.martins@example.com', '987654321', '2019-01-15', 'alicem', HASHBYTES('SHA2_256', 'password1'), 'Leitor', 1),
		('Bruno Silva', '1990-07-22', 'bruno.silva@example.com', '912345678', '2018-02-20', 'brunos', HASHBYTES('SHA2_256', 'password2'), 'Bibliotecario', 1),
		('Carla Andrade', '1995-01-10', 'carla.andrade@example.com', '965432198', '2019-03-25', 'carlaa', HASHBYTES('SHA2_256', 'password3'), 'Leitor', 1),
		('Diego Souza', '1980-04-18', 'diego.souza@example.com', '934567123', '2019-04-10', 'diegos', HASHBYTES('SHA2_256', 'password4'), 'Leitor', 1),
		('Elena Gomes', '1992-09-30', 'elena.gomes@example.com', '956789012', '2019-05-05', 'elenag', HASHBYTES('SHA2_256', 'password5'), 'Leitor', 1),
		('Fernando Costa', '1983-06-07', 'fernando.costa@example.com', '923456789', '2018-06-15', 'fernandoc', HASHBYTES('SHA2_256', 'password6'), 'Bibliotecario', 1),
		('Gabriel Menezes', '1985-12-25', 'gabriel.menezes@example.com', '987123456', '2019-07-20', 'gabrielm', HASHBYTES('SHA2_256', 'password7'), 'Leitor', 1),
		('Helena Ramos', '1998-08-14', 'helena.ramos@example.com', '932156789', '2019-08-10', 'helenar', HASHBYTES('SHA2_256', 'password8'), 'Leitor', 1),
		('Igor Nunes', '1991-11-11', 'igor.nunes@example.com', '921234567', '2020-09-12', 'igorn', HASHBYTES('SHA2_256', 'password9'), 'Leitor', 1),
		('Julia Ferreira', '1993-02-03', 'julia.ferreira@example.com', '954321678', '2020-10-05', 'juliaf', HASHBYTES('SHA2_256', 'password10'), 'Leitor', 1),
		('Lucas Vieira', '1984-11-15', 'lucas.vieira@example.com', '912345987', '2020-11-01', 'lucasv', HASHBYTES('SHA2_256', 'password11'), 'Leitor', 1),
		('Maria Silva', '1979-12-25', 'maria.silva@example.com', '912543789', '2020-11-25', 'marias', HASHBYTES('SHA2_256', 'password12'), 'Leitor', 1),
		('Nina Oliveira', '1987-07-04', 'nina.oliveira@example.com', '915432987', '2020-01-10', 'ninao', HASHBYTES('SHA2_256', 'password13'), 'Leitor', 1),
		('Otávio Campos', '1996-03-18', 'otavio.campos@example.com', '918654321', '2020-02-20', 'otavioc', HASHBYTES('SHA2_256', 'password14'), 'Leitor', 0),
		('Patricia Souza', '1999-06-05', 'patricia.souza@example.com', '913245678', '2020-03-15', 'pats', HASHBYTES('SHA2_256', 'password15'), 'Leitor', 0),
		('Quintino Santos', '1990-10-22', 'quintino.santos@example.com', '911234567', '2020-04-02', 'quints', HASHBYTES('SHA2_256', 'password16'), 'Leitor', 1),
		('Renata Lopes', '1991-08-10', 'renata.lopes@example.com', '911987654', '2020-05-10', 'renatal', HASHBYTES('SHA2_256', 'password17'), 'Leitor', 1),
		('Samuel Mendes', '1986-01-19', 'samuel.mendes@example.com', '910234567', '2020-06-22', 'samm', HASHBYTES('SHA2_256', 'password18'), 'Leitor', 1),
		('Tatiana Lima', '1989-11-30', 'tatiana.lima@example.com', '914567890', '2020-07-18', 'tatil', HASHBYTES('SHA2_256', 'password19'), 'Leitor', 1),
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
		(1, 24, '2022-01-05', '2022-01-30'),
		(7, 31, '2024-06-07', '2024-06-21'),
		(8, 32, '2024-03-08', '2024-03-22'),
		(9, 41, '2024-02-09', '2024-02-23'),
		(10, 42, '2024-01-10', '2024-01-24'),
		(11, 51, '2024-11-11', Null),
		(12, 52, '2024-09-12', '2024-09-26'),
		(13, 63, '2024-10-13', '2024-10-27'),
		(12, 64, '2024-11-14', Null),
		(16, 75, '2024-08-15', '2024-08-29'),
		(16, 76, '2024-10-16', '2024-10-30'),
		(12, 62, '2024-09-12', '2024-11-26'),
		(12, 70, '2024-08-12', '2024-11-26'),
		(12, 70, '2024-09-12', '2024-11-26'),
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

EXEC PopularDB;

EXEC EliminarConteudoTabelas;