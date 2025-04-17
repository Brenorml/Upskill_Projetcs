using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Biblioteca.Dt;
using Biblioteca.Model;
using LibDB;
using Microsoft.Data.SqlClient;


namespace Biblioteca
{
    public class Biblioteca
    {

        private string _cnstring {  get; set; }

        public Biblioteca()
        {
            _cnstring = "";
        }

        public Biblioteca(string cnstring)
        {
            _cnstring = cnstring;
        }

        public List<ObraDto> MostrarTodasObras()
        {
            SqlConnection cn = null;
            DataTable dt = null;

            List<ObraDto> lst = new List<ObraDto>();

            try
            {
                cn = DB.Open(_cnstring);

                string qry = @"
    SELECT o.ObraID, o.Titulo, o.Autor, o.AnoPublicacao, o.Genero, o.Descricao, i.Capa
FROM Obras o
LEFT JOIN Imagens i ON o.ObraID = i.ID_ObraID
WHERE EXISTS (
    SELECT 1
    FROM Exemplares e
    WHERE e.ObraID = o.ObraID AND e.Disponivel = 1
);";

                dt = DB.GetSQLRead(cn, qry);

                foreach (DataRow dr in dt.Rows)
                {
                    var item = new ObraDto()
                    {
                        ObraID = Convert.ToInt32(dr["ObraID"]),
                        Titulo = dr["Titulo"].ToString(),
                        Autor = dr["Autor"].ToString(),
                        AnoPublicacao = int.TryParse(dr["AnoPublicacao"].ToString(), out int anoPublicacao) ? anoPublicacao : (int?)null,  // Usando null caso a conversão falhe
                        Genero = dr["Genero"].ToString(),
                        Descricao = dr["Descricao"].ToString()
                    };

                    if (dr["Capa"] != DBNull.Value)
                    {
                        byte[] imagemBytes = (byte[])dr["Capa"];
                        item.ImagemBase64 = Convert.ToBase64String(imagemBytes);
                    }
                    else
                    {
                        item.ImagemBase64 = null;
                    }

                    lst.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DB.Close(dt);
                DB.Close(cn);
            }

            return lst;
        }

        public int ConsultarTotalObras()
        {
            SqlConnection cn = null;
            int total = 0;

            try
            {
                string qry = "SELECT COUNT(*) FROM Obras";

                using (cn = DB.Open(_cnstring))
                {
                    SqlCommand cmd = new SqlCommand(qry, cn);
                    total = (int)cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return total;
        }

        public List<GeneroObraDTO> ConsultarObrasPorGenero()
        {
            List<GeneroObraDTO> resultado = new List<GeneroObraDTO>();

            try
            {
                using (SqlConnection cn = DB.Open(_cnstring))
                {
                    string sql = @"
                                   SELECT Genero, COUNT(*) AS NumeroObras 
                                   FROM Obras 
                                   GROUP BY Genero";
                    using (DataTable dt = DB.GetSQLRead(cn, sql))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            resultado.Add(new GeneroObraDTO
                            {
                                Genero = dr["Genero"].ToString(),
                                NumeroObras = Convert.ToInt32(dr["NumeroObras"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return resultado;
        }

        public List<Obra> ObterObras()
        {
            var obras = new List<Obra>();

            using (var connection = new SqlConnection(_cnstring))
            {
                connection.Open();

                var sql = @"
                            SELECT DISTINCT o.ObraID, o.Titulo, o.Autor, o.AnoPublicacao, o.Genero, 
                            CAST(o.Descricao AS VARCHAR(MAX)) AS Descricao
                            FROM Obras o
                            JOIN Exemplares e ON o.ObraID = e.ObraID
                            WHERE e.Disponivel = 1;";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obra = new Obra
                            {
                                ObraID = reader.GetInt32(0),
                                Titulo = reader.GetString(1),
                                Autor = reader.GetString(2),
                                AnoPublicacao = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                                Genero = reader.GetString(4),
                                Descricao = reader.GetString(5)
                            };

                            obras.Add(obra);
                        }
                    }
                }
            }

            return obras;
        }

        public List<Nucleo> ObterNucleos()
        {
            var nucleos = new List<Nucleo>();

            using (var connection = new SqlConnection(_cnstring))
            {
                connection.Open();

                var sql = @"
        SELECT NucleoID, Nome
        FROM Nucleos;";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var nucleo = new Nucleo
                            {
                                NucleoID = reader.GetInt32(0),
                                Nome = reader.GetString(1)
                            };

                            nucleos.Add(nucleo);
                        }
                    }
                }
            }

            return nucleos;
        }

        public List<ExemplarPorNucleo> ObterExemplaresPorNucleo()
        {
            var exemplares = new List<ExemplarPorNucleo>();

            using (var connection = new SqlConnection(_cnstring))
            {
                connection.Open();

                var sql = @"
        SELECT 
            O.Titulo,
            N.Nome AS NomeNucleo,
            COUNT(E.ExemplarID) AS Quantidade
        FROM Exemplares E
        INNER JOIN Obras O ON E.ObraID = O.ObraID
        INNER JOIN Nucleos N ON E.NucleoID = N.NucleoID
        WHERE E.Disponivel = 1
        GROUP BY O.Titulo, N.Nome;";

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var exemplar = new ExemplarPorNucleo
                            {
                                Titulo = reader.GetString(0),
                                NomeNucleo = reader.GetString(1),
                                Quantidade = reader.GetInt32(2)
                            };

                            exemplares.Add(exemplar);
                        }
                    }
                }
            }

            return exemplares;
        }

        public void AdicionarObra(ObraAtualizacaoDto obra, byte[] imagemBytes)
        {
            using (var cn = new SqlConnection(_cnstring))
            {
                cn.Open();
                using (var transaction = cn.BeginTransaction())
                {
                    try
                    {
                        // 1. Inserção da obra
                        string sqlInsertObra = @"
                    INSERT INTO Obras (Titulo, Autor, AnoPublicacao, Genero, Descricao)
                    VALUES (@Titulo, @Autor, @AnoPublicacao, @Genero, @Descricao);
                    SELECT SCOPE_IDENTITY();";

                        int obraId;

                        using (var command = new SqlCommand(sqlInsertObra, cn, transaction))
                        {
                            command.Parameters.AddWithValue("@Titulo", obra.Titulo);
                            command.Parameters.AddWithValue("@Autor", obra.Autor);
                            command.Parameters.AddWithValue("@AnoPublicacao", obra.AnoPublicacao ?? (object)DBNull.Value);
                            command.Parameters.AddWithValue("@Genero", obra.Genero);
                            command.Parameters.AddWithValue("@Descricao", obra.Descricao ?? (object)DBNull.Value);

                            obraId = Convert.ToInt32(command.ExecuteScalar());
                        }

                        // 2. Inserção da imagem (se existir)
                        if (imagemBytes != null && imagemBytes.Length > 0)
                        {
                            string sqlInsertImagem = @"
                        INSERT INTO Imagens (ID_ObraID, Capa)
                        VALUES (@ObraID, @Capa);";

                            using (var command = new SqlCommand(sqlInsertImagem, cn, transaction))
                            {
                                command.Parameters.AddWithValue("@ObraID", obraId);
                                command.Parameters.AddWithValue("@Capa", imagemBytes);
                                command.ExecuteNonQuery();
                            }
                        }

                        // 3. Inserção do exemplar no núcleo central de Lisboa (ID 1)
                        string sqlInsertExemplar = @"
                    INSERT INTO Exemplares (ObraID, NucleoID, Disponivel)
                    VALUES (@ObraID, @NucleoID, @Disponivel);";

                        using (var command = new SqlCommand(sqlInsertExemplar, cn, transaction))
                        {
                            command.Parameters.AddWithValue("@ObraID", obraId);
                            command.Parameters.AddWithValue("@NucleoID", 1); // ID do núcleo central de Lisboa
                            command.Parameters.AddWithValue("@Disponivel", 1); // Exemplar disponível
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Erro ao adicionar obra: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public string ApagarObra(int obraID)
        {
            SqlConnection cn = null;
            SqlTransaction transaction = null;

            try
            {
                cn = DB.Open(_cnstring);
                transaction = cn.BeginTransaction();

                // 1️⃣ Verificar se há requisições pendentes
                string sqlCheckRequisicoes = @"
            SELECT COUNT(*) 
            FROM Requisicoes r
            INNER JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
            WHERE e.ObraID = @ObraID AND r.DataDevolucao IS NULL;";

                using (SqlCommand cmd = new SqlCommand(sqlCheckRequisicoes, cn, transaction))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                    int countRequisicoesPendentes = (int)cmd.ExecuteScalar();

                    if (countRequisicoesPendentes > 0)
                    {
                        return "Não é possível apagar a obra pois há devoluções pendentes.";
                    }
                }

                // 2️⃣ Verificar se todos os exemplares já estão indisponíveis
                string sqlCheckExemplares = "SELECT COUNT(*) FROM Exemplares WHERE ObraID = @ObraID AND Disponivel = 1;";
                using (SqlCommand cmd = new SqlCommand(sqlCheckExemplares, cn, transaction))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                    int countDisponiveis = (int)cmd.ExecuteScalar();

                    if (countDisponiveis > 0)
                    {
                        // 3️⃣ Atualizar todos os exemplares para 'Indisponível' (Disponivel = 0)
                        string sqlUpdateExemplares = "UPDATE Exemplares SET Disponivel = 0 WHERE ObraID = @ObraID;";
                        using (SqlCommand updateCmd = new SqlCommand(sqlUpdateExemplares, cn, transaction))
                        {
                            updateCmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                            updateCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return "A obra foi removida com sucesso.";
                    }
                    else
                    {
                        // 4️⃣ Verificar se existem exemplares associados à obra
                        string sqlCheckExemplaresAssociados = "SELECT COUNT(*) FROM Exemplares WHERE ObraID = @ObraID;";
                        using (SqlCommand cmdCheckExemplares = new SqlCommand(sqlCheckExemplaresAssociados, cn, transaction))
                        {
                            cmdCheckExemplares.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                            int countExemplaresAssociados = (int)cmdCheckExemplares.ExecuteScalar();

                            if (countExemplaresAssociados == 0)
                            {
                                // 5️⃣ Apagar a imagem da capa associada à obra
                                string sqlDeleteImagens = "DELETE FROM Imagens WHERE ID_ObraID = @ObraID;";
                                using (SqlCommand deleteImagensCmd = new SqlCommand(sqlDeleteImagens, cn, transaction))
                                {
                                    deleteImagensCmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                                    deleteImagensCmd.ExecuteNonQuery();
                                }

                                // 6️⃣ Obter detalhes da obra antes de apagá-la
                                string sqlGetObra = "SELECT Titulo FROM Obras WHERE ObraID = @ObraID;";
                                string tituloObra = "";
                                using (SqlCommand cmdGetObra = new SqlCommand(sqlGetObra, cn, transaction))
                                {
                                    cmdGetObra.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                                    var result = cmdGetObra.ExecuteScalar();
                                    tituloObra = result != null ? result.ToString() : "Obra desconhecida";
                                }

                                // 7️⃣ Apagar a obra da base de dados
                                string sqlDeleteObra = "DELETE FROM Obras WHERE ObraID = @ObraID;";
                                using (SqlCommand deleteCmd = new SqlCommand(sqlDeleteObra, cn, transaction))
                                {
                                    deleteCmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                                    deleteCmd.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                return $"A obra '{tituloObra}' e suas imagens associadas foram apagadas da base de dados.";
                            }
                            else
                            {
                                return "Não é possível apagar a obra pois ainda existem exemplares associados.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                return "Erro ao apagar a obra: " + ex.Message;
            }
            finally
            {
                DB.Close(transaction);
                DB.Close(cn);
            }
        }

        public ObraAtualizacaoDto ObterDadosObra(int obraID)
        {
            SqlConnection cn = null;
            ObraAtualizacaoDto obraDto = null;

            try
            {
                cn = DB.Open(_cnstring);

                // Consulta para obter os dados da obra
                string sqlSelectObra = @"
            SELECT ObraID, Titulo, Autor, AnoPublicacao, Genero, Descricao
            FROM Obras
            WHERE ObraID = @ObraID;";

                using (SqlCommand cmd = new SqlCommand(sqlSelectObra, cn))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            obraDto = new ObraAtualizacaoDto
                            {
                                ObraID = reader.GetInt32(reader.GetOrdinal("ObraID")),
                                Titulo = reader.GetString(reader.GetOrdinal("Titulo")),
                                Autor = reader.GetString(reader.GetOrdinal("Autor")),
                                AnoPublicacao = reader.IsDBNull(reader.GetOrdinal("AnoPublicacao")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("AnoPublicacao")),
                                Genero = reader.GetString(reader.GetOrdinal("Genero")),
                                Descricao = reader.GetString(reader.GetOrdinal("Descricao"))
                            };
                        }
                        else
                        {
                            throw new Exception("Obra não encontrada.");
                        }
                    }
                }

                // Consulta para obter a imagem associada (se existir)
                string sqlSelectImagem = @"
            SELECT Capa
            FROM Imagens
            WHERE ID_ObraID = @ObraID;";

                using (SqlCommand cmd = new SqlCommand(sqlSelectImagem, cn))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Converte a imagem para Base64
                            byte[] imagemBytes = (byte[])reader["Capa"];
                            obraDto.ImagemBase64 = Convert.ToBase64String(imagemBytes);
                        }
                    }
                }

                return obraDto;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao obter dados da obra: " + ex.Message);
                throw;
            }
            finally
            {
                DB.Close(cn);
            }
        }

        public void AtualizarObra(ObraAtualizacaoDto obraDto)
        {
            SqlConnection cn = null;
            SqlTransaction transaction = null;

            try
            {
                cn = DB.Open(_cnstring);
                transaction = cn.BeginTransaction();

                // Atualizar os dados da obra na tabela Obras
                string sqlUpdateObra = @"
            UPDATE Obras
            SET Titulo = @Titulo,
                Autor = @Autor,
                AnoPublicacao = @AnoPublicacao,
                Genero = @Genero,
                Descricao = @Descricao
            WHERE ObraID = @ObraID;";

                using (SqlCommand cmd = new SqlCommand(sqlUpdateObra, cn, transaction))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraDto.ObraID;
                    cmd.Parameters.Add("@Titulo", SqlDbType.NVarChar, 150).Value = obraDto.Titulo;
                    cmd.Parameters.Add("@Autor", SqlDbType.NVarChar, 100).Value = obraDto.Autor;
                    cmd.Parameters.Add("@AnoPublicacao", SqlDbType.Int).Value = (object)obraDto.AnoPublicacao ?? DBNull.Value;
                    cmd.Parameters.Add("@Genero", SqlDbType.NVarChar, 50).Value = obraDto.Genero;
                    cmd.Parameters.Add("@Descricao", SqlDbType.Text).Value = obraDto.Descricao;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Erro: A obra especificada não existe.");
                    }
                }

                // Converter a string Base64 em bytes (se fornecida)
                byte[] imagemBytes = null;
                if (!string.IsNullOrEmpty(obraDto.ImagemBase64))
                {
                    imagemBytes = Convert.FromBase64String(obraDto.ImagemBase64);
                }

                // Atualizar ou inserir a imagem na tabela Imagens (se a imagem for fornecida)
                if (imagemBytes != null && imagemBytes.Length > 0)
                {
                    string sqlVerificaImagem = "SELECT COUNT(*) FROM Imagens WHERE ID_ObraID = @ObraID;";
                    int countImagens = 0;

                    using (SqlCommand cmd = new SqlCommand(sqlVerificaImagem, cn, transaction))
                    {
                        cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraDto.ObraID;
                        countImagens = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (countImagens > 0)
                    {
                        // Atualizar a imagem existente
                        string sqlUpdateImagem = "UPDATE Imagens SET Capa = @Capa WHERE ID_ObraID = @ObraID;";
                        using (SqlCommand cmd = new SqlCommand(sqlUpdateImagem, cn, transaction))
                        {
                            cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraDto.ObraID;
                            cmd.Parameters.Add("@Capa", SqlDbType.VarBinary, -1).Value = imagemBytes;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Inserir uma nova imagem
                        string sqlInsertImagem = "INSERT INTO Imagens (ID_ObraID, Capa) VALUES (@ObraID, @Capa);";
                        using (SqlCommand cmd = new SqlCommand(sqlInsertImagem, cn, transaction))
                        {
                            cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraDto.ObraID;
                            cmd.Parameters.Add("@Capa", SqlDbType.VarBinary, -1).Value = imagemBytes;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                Console.WriteLine("Obra e imagem atualizadas com sucesso!");
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine("Erro ao atualizar obra: " + ex.Message);
                throw;
            }
            finally
            {
                DB.Close(transaction);
                DB.Close(cn);
            }
        }

        public void AdicionarExemplaresAoNucleoPrincipal(int obraID, int qtdAdicionar)
        {
            if (qtdAdicionar <= 0)
            {
                throw new ArgumentException("Erro: A quantidade de exemplares a adicionar deve ser um número inteiro positivo.");
            }

            SqlConnection connection = null;
            SqlTransaction transaction = null;

            try
            {
                connection = DB.Open(_cnstring);
                transaction = connection.BeginTransaction();

                string sqlVerificarObra = "SELECT 1 FROM Obras WHERE ObraID = @ObraID;";
                using (SqlCommand cmd = new SqlCommand(sqlVerificarObra, connection, transaction))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        throw new InvalidOperationException("Erro: A obra especificada não existe no catálogo de obras.");
                    }
                }

                int nucleoPrincipalID;
                string sqlObterNucleo = "SELECT NucleoID FROM Nucleos WHERE Nome = 'Central Lisboa';";
                using (SqlCommand cmd = new SqlCommand(sqlObterNucleo, connection, transaction))
                {
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        throw new InvalidOperationException("Erro: Núcleo principal não encontrado.");
                    }
                    nucleoPrincipalID = Convert.ToInt32(result);
                }

                string sqlInserirExemplar = "INSERT INTO Exemplares (ObraID, NucleoID) VALUES (@ObraID, @NucleoID);";
                using (SqlCommand cmd = new SqlCommand(sqlInserirExemplar, connection, transaction))
                {
                    cmd.Parameters.Add("@ObraID", SqlDbType.Int).Value = obraID;
                    cmd.Parameters.Add("@NucleoID", SqlDbType.Int).Value = nucleoPrincipalID;

                    for (int i = 0; i < qtdAdicionar; i++)
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
                Console.WriteLine("Novos exemplares adicionados ao núcleo principal.");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine("Erro ao adicionar exemplares: " + ex.Message);
                throw;
            }
            finally
            {
                DB.Close(connection);
                DB.Close(transaction);
            }
        }

        public List<Usuario> EliminarLeitoresInativos()
        {
            List<Usuario> leitoresRemovidos = new List<Usuario>();
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = LibDB.DB.Open(_cnstring);
                transaction = conn.BeginTransaction();

                // Passo 1: Selecionar leitores registados há mais de 12 meses
                // e que não fizeram requisições nos últimos 12 meses
                string queryLeitores = @"
                SELECT U.UserID, U.Nome, U.Email
                FROM Usuario U
                WHERE U.TipoUser = 'Leitor'
                AND DATEDIFF(MONTH, U.DataRegisto, GETDATE()) >= 12 -- Registado há mais de 12 meses
                AND NOT EXISTS (
                    SELECT 1
                    FROM Requisicoes R
                    WHERE R.UserID = U.UserID
                    AND R.DataRequisicao >= DATEADD(YEAR, -1, GETDATE()) -- Requisição nos últimos 12 meses
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM Requisicoes R
                    WHERE R.UserID = U.UserID
                    AND (R.DataDevolucao IS NULL OR R.DataDevolucao > GETDATE()) -- Requisições ativas
                )";

                SqlCommand cmdLeitores = new SqlCommand(queryLeitores, conn, transaction);
                SqlDataReader reader = cmdLeitores.ExecuteReader();

                while (reader.Read())
                {
                    leitoresRemovidos.Add(new Usuario
                    {
                        UserID = reader.GetInt32(0),
                        Nome = reader.GetString(1),
                        Email = reader.GetString(2)
                    });
                }
                reader.Close();

                if (leitoresRemovidos.Count == 0)
                {
                    Console.WriteLine("Nenhum leitor atende aos critérios para exclusão.");
                    transaction.Commit();
                    return leitoresRemovidos;
                }

                // Passo 2: Inserir no histórico antes da exclusão
                string queryInsertHistorico = @"
            INSERT INTO HistoricoRequisicoes (RequisicaoID, UserID, NomeUser, Telefone, TituloObra, ExemplarID, Nucleo, DataRequisicao, DataDevolucao)
            SELECT R.RequisicaoID, U.UserID, U.Nome AS NomeUser, U.Telefone, O.Titulo AS TituloObra,
                   R.ExemplarID, N.Nome AS Nucleo, R.DataRequisicao, R.DataDevolucao
            FROM Requisicoes R
            INNER JOIN Usuario U ON R.UserID = U.UserID
            INNER JOIN Exemplares E ON R.ExemplarID = E.ExemplarID
            INNER JOIN Obras O ON E.ObraID = O.ObraID
            INNER JOIN Nucleos N ON E.NucleoID = N.NucleoID
            WHERE U.UserID = @UserID";

                SqlCommand cmdHistorico = new SqlCommand(queryInsertHistorico, conn, transaction);
                cmdHistorico.Parameters.Add("@UserID", SqlDbType.Int);

                foreach (Usuario leitor in leitoresRemovidos)
                {
                    cmdHistorico.Parameters["@UserID"].Value = leitor.UserID;
                    cmdHistorico.ExecuteNonQuery();
                }

                // Passo 3: Excluir os leitores que atendem a todos os critérios
                string queryDelete = "DELETE FROM Usuario WHERE UserID = @UserID";
                SqlCommand cmdDelete = new SqlCommand(queryDelete, conn, transaction);
                cmdDelete.Parameters.Add("@UserID", SqlDbType.Int);

                foreach (Usuario leitor in leitoresRemovidos)
                {
                    cmdDelete.Parameters["@UserID"].Value = leitor.UserID;
                    cmdDelete.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine("Leitores inativos foram eliminados e seu histórico foi salvo.");
            }
            catch (Exception ex)
            {
                if (transaction != null)
                    transaction.Rollback();

                Console.WriteLine("Erro ao eliminar leitores inativos: " + ex.Message);
                return new List<Usuario>(); // Retorna lista vazia em caso de erro
            }
            finally
            {
                if (transaction != null)
                    LibDB.DB.Close(transaction);

                if (conn != null)
                    LibDB.DB.Close(conn);
            }

            return leitoresRemovidos;
        }

        public List<Usuario> Admin_ListarLeitoresInativos()
        {
            List<Usuario> leitoresInativos = new List<Usuario>();
            SqlConnection cn = null;

            try
            {
                cn = DB.Open(_cnstring);

                // Query para buscar leitores inativos
                string queryLeitoresInativos = @"
            SELECT UserID, Nome, Email, Telefone, DataRegisto 
            FROM Usuario 
            WHERE TipoUser = 'Leitor' 
            AND Ativo = 0"; // Filtra apenas leitores inativos

                DataTable dt = DB.GetSQLRead(cn, queryLeitoresInativos);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        leitoresInativos.Add(new Usuario
                        {
                            UserID = Convert.ToInt32(row["UserID"]),
                            Nome = row["Nome"].ToString(),
                            Email = row["Email"].ToString(),
                            Telefone = row["Telefone"].ToString(),
                            DataRegisto = Convert.ToDateTime(row["DataRegisto"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log do erro (opcional)
                Console.WriteLine("Erro ao listar leitores inativos: " + ex.Message);
            }
            finally
            {
                DB.Close(cn); // Fecha a conexão
            }

            return leitoresInativos; // Retorna a lista de leitores inativos
        }

        public List<Usuario> Admin_reativarLeitor(int leitorId)
        {
            List<Usuario> leitorReativado = null;
            SqlConnection cn = null;
            SqlTransaction transaction = null;

            try
            {
                cn = DB.Open(_cnstring);
                transaction = cn.BeginTransaction();

                // Verificação para ver se o leitor existe
                string queryLeitorExiste = $@"
        SELECT 1 FROM Usuario 
        WHERE UserID = {leitorId} AND TipoUser = 'Leitor'";
                DataTable dt = DB.GetSQLRead(cn, queryLeitorExiste, transaction);

                if (dt.Rows.Count == 0)
                {
                    transaction.Rollback();
                    return null; // Leitor não existe
                }

                // Verificação para ver se o leitor já está ativo
                string queryLeitorAtivo = $@"
        SELECT 1 FROM Usuario 
        WHERE UserID = {leitorId} 
        AND TipoUser = 'Leitor' 
        AND Ativo = 0";
                DataTable dtAtivo = DB.GetSQLRead(cn, queryLeitorAtivo, transaction);

                if (dtAtivo.Rows.Count == 0)
                {
                    transaction.Rollback();
                    return null; // Leitor já está ativo
                }

                // Reativando o leitor
                string queryUpdate = $@"
        UPDATE Usuario 
        SET Ativo = 1 
        WHERE UserID = {leitorId}";
                int rowsAffected = DB.CmdExecute(cn, queryUpdate, transaction);

                if (rowsAffected > 0)
                {
                    // Recuperando os dados do leitor reativado
                    string queryLeitorReativado = $@"
            SELECT UserID, Nome, DataNascimento, Email, Telefone, DataRegisto, TipoUser, Ativo 
            FROM Usuario 
            WHERE UserID = {leitorId}";
                    DataTable dtLeitor = DB.GetSQLRead(cn, queryLeitorReativado, transaction);

                    if (dtLeitor.Rows.Count > 0)
                    {
                        leitorReativado = new List<Usuario>();
                        foreach (DataRow row in dtLeitor.Rows)
                        {
                            leitorReativado.Add(new Usuario
                            {
                                UserID = Convert.ToInt32(row["UserID"]),
                                Nome = row["Nome"].ToString(),
                                DataNascimento = Convert.ToDateTime(row["DataNascimento"]),
                                Email = row["Email"].ToString(),
                                Telefone = row["Telefone"].ToString(),
                                DataRegisto = Convert.ToDateTime(row["DataRegisto"]),
                                TipoUser = row["TipoUser"].ToString(),
                                Ativo = Convert.ToBoolean(row["Ativo"])
                            });
                        }
                    }

                    transaction.Commit(); // Confirma a transação
                }
                else
                {
                    transaction.Rollback(); // Reverte a transação em caso de falha
                }
            }
            catch (Exception)
            {
                transaction?.Rollback(); // Reverte a transação em caso de erro
            }
            finally
            {
                DB.Close(cn); // Fecha a conexão
            }

            return leitorReativado; // Retorna a lista de usuários ou null
        }

        public List<Usuario> Admin_RegistrarNovoUtilizador(string nome, DateTime dataNascimento, string email, string telefone, string username, string palavraPasse, string tipoUser)
        {
            List<Usuario> novoUtilizador = null;
            using (SqlConnection cn = DB.Open(_cnstring))
            {
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        // Verificar duplicidade de Email
                        string queryEmailExiste = "SELECT 1 FROM Usuario WHERE Email = @Email";
                        using (SqlCommand cmdEmail = new SqlCommand(queryEmailExiste, cn, transaction))
                        {
                            cmdEmail.Parameters.AddWithValue("@Email", email);
                            if (cmdEmail.ExecuteScalar() != null)
                            {
                                transaction.Rollback();
                                return null; // Email já existe
                            }
                        }

                        // Verificar duplicidade de Telefone
                        string queryTelefoneExiste = "SELECT 1 FROM Usuario WHERE Telefone = @Telefone";
                        using (SqlCommand cmdTelefone = new SqlCommand(queryTelefoneExiste, cn, transaction))
                        {
                            cmdTelefone.Parameters.AddWithValue("@Telefone", telefone);
                            if (cmdTelefone.ExecuteScalar() != null)
                            {
                                transaction.Rollback();
                                return null; // Telefone já existe
                            }
                        }

                        // Converter a senha para um hash usando SHA-256
                        using (SHA256 sha256Hash = SHA256.Create())
                        {
                            // Converter a senha para VARBINARY(MAX)
                            byte[] senhaEmBytes = sha256Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(palavraPasse));

                            // Inserir novo utilizador
                            string queryInsert = @"
                    INSERT INTO Usuario (Nome, DataNascimento, Email, Telefone, Username, PalavraPasse, TipoUser, Ativo)
                    VALUES (@Nome, @DataNascimento, @Email, @Telefone, @Username, @PalavraPasse, @TipoUser, 1)";

                            using (SqlCommand cmd = new SqlCommand(queryInsert, cn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@Nome", nome);
                                cmd.Parameters.AddWithValue("@DataNascimento", dataNascimento);
                                cmd.Parameters.AddWithValue("@Email", email);
                                cmd.Parameters.AddWithValue("@Telefone", telefone);
                                cmd.Parameters.AddWithValue("@Username", username);
                                cmd.Parameters.AddWithValue("@PalavraPasse", senhaEmBytes);
                                cmd.Parameters.AddWithValue("@TipoUser", tipoUser);

                                int rowsAffected = cmd.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    // Recuperar os dados do novo utilizador
                                    string queryNovoUtilizador = @"
                        SELECT UserID, Nome, DataNascimento, Email, Telefone, DataRegisto, TipoUser, Ativo 
                        FROM Usuario 
                        WHERE Email = @Email";
                                    using (SqlCommand cmdSelect = new SqlCommand(queryNovoUtilizador, cn, transaction))
                                    {
                                        cmdSelect.Parameters.AddWithValue("@Email", email);
                                        using (SqlDataReader reader = cmdSelect.ExecuteReader())
                                        {
                                            if (reader.HasRows)
                                            {
                                                novoUtilizador = new List<Usuario>();
                                                while (reader.Read())
                                                {
                                                    novoUtilizador.Add(new Usuario
                                                    {
                                                        UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                                        Nome = reader.GetString(reader.GetOrdinal("Nome")),
                                                        DataNascimento = reader.GetDateTime(reader.GetOrdinal("DataNascimento")),
                                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                                        Telefone = reader.GetString(reader.GetOrdinal("Telefone")),
                                                        DataRegisto = reader.GetDateTime(reader.GetOrdinal("DataRegisto")),
                                                        TipoUser = reader.GetString(reader.GetOrdinal("TipoUser")),
                                                        Ativo = reader.GetBoolean(reader.GetOrdinal("Ativo"))
                                                    });
                                                }
                                            }
                                        }
                                    }

                                    transaction.Commit(); // Confirma a transação
                                }
                                else
                                {
                                    transaction.Rollback(); // Reverte a transação em caso de falha
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback(); // Reverte a transação em caso de erro
                    }
                }
            }
            return novoUtilizador; // Retorna a lista de usuários ou null
        }

        public List<Usuario> Admin_suspenderAcessoLeitoresDevolucoesAtrasadas()
        {
            List<Usuario> leitoresSuspensos = new List<Usuario>();

            using (SqlConnection cn = DB.Open(_cnstring))
            {
                using (SqlTransaction transaction = cn.BeginTransaction())
                {
                    try
                    {
                        // 1. Buscar leitores com 3+ devoluções atrasadas
                        string queryLeitores = @"
                 SELECT U.UserID, U.Nome, U.Email
                 FROM Usuario U
                 JOIN (
                     SELECT R.UserID
                     FROM Requisicoes R
                     WHERE 
                         (R.DataDevolucao IS NULL AND DATEADD(DAY, 15, R.DataRequisicao) < GETDATE())
                         OR (R.DataDevolucao IS NOT NULL AND R.DataDevolucao > DATEADD(DAY, 15, R.DataRequisicao))
                     GROUP BY R.UserID
                     HAVING COUNT(*) > 3
                 ) AS Suspensos ON U.UserID = Suspensos.UserID
                 WHERE U.TipoUser = 'Leitor' AND U.Ativo = 1;";

                        DataTable dtLeitores = DB.GetSQLRead(cn, queryLeitores, transaction);

                        if (dtLeitores.Rows.Count == 0)
                        {
                            transaction.Commit(); // Apenas confirma a transação sem rollback
                            return leitoresSuspensos; // Retorna lista vazia em vez de null
                        }

                        // 2. Suspender os leitores diretamente no loop
                        foreach (DataRow row in dtLeitores.Rows)
                        {
                            int userId = (int)row["UserID"];

                            string queryUpdate = "UPDATE Usuario SET Ativo = 0 WHERE UserID = @UserID";
                            using (SqlCommand cmd = new SqlCommand(queryUpdate, cn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserID", userId);
                                cmd.ExecuteNonQuery();
                            }

                            leitoresSuspensos.Add(new Usuario
                            {
                                UserID = userId,
                                Nome = row["Nome"].ToString(),
                                Email = row["Email"].ToString()
                            });
                        }

                        transaction.Commit();
                        return leitoresSuspensos;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }

        public ResultadoTransferencia Admin_TransferirExemplar(int obraId, int origemNucleoId, int destinoNucleoId, int qtdTransferir)
        {
            SqlConnection cn = null;
            SqlTransaction transaction = null;

            try
            {
                // Abrir conexão e iniciar transação
                cn = DB.Open(_cnstring);
                transaction = cn.BeginTransaction();

                // 1️⃣ Verificar se a obra existe no núcleo de origem
                string queryVerificaObra = @"
            SELECT 1 
            FROM Exemplares 
            WHERE ObraID = @ObraID 
            AND NucleoID = @OrigemNucleoID 
            AND Disponivel = 1;"; // Adicionei o filtro por Disponivel = 1

                using (SqlCommand cmdVerificaObra = new SqlCommand(queryVerificaObra, cn, transaction))
                {
                    cmdVerificaObra.Parameters.AddWithValue("@ObraID", obraId);
                    cmdVerificaObra.Parameters.AddWithValue("@OrigemNucleoID", origemNucleoId);

                    using (SqlDataReader reader = cmdVerificaObra.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            return new ResultadoTransferencia
                            {
                                Sucesso = false,
                                Mensagem = "A obra especificada não possui exemplares disponíveis no núcleo de origem."
                            };
                        }
                    }
                }

                // 2️⃣ Verificar a quantidade de exemplares no núcleo de origem
                string queryQtdOrigem = @"
            SELECT COUNT(*) 
            FROM Exemplares 
            WHERE ObraID = @ObraID 
            AND NucleoID = @OrigemNucleoID 
            AND Disponivel = 1;"; // Adicionei o filtro por Disponivel = 1

                using (SqlCommand cmdQtdOrigem = new SqlCommand(queryQtdOrigem, cn, transaction))
                {
                    cmdQtdOrigem.Parameters.AddWithValue("@ObraID", obraId);
                    cmdQtdOrigem.Parameters.AddWithValue("@OrigemNucleoID", origemNucleoId);

                    int qtdOrigemDisponivel = Convert.ToInt32(cmdQtdOrigem.ExecuteScalar());

                    // Verificar se a quantidade de exemplares no núcleo de origem é suficiente
                    if (qtdOrigemDisponivel - qtdTransferir < 1)
                    {
                        return new ResultadoTransferencia
                        {
                            Sucesso = false,
                            Mensagem = "O núcleo de origem precisa manter pelo menos um exemplar disponível."
                        };
                    }

                    if (qtdOrigemDisponivel < qtdTransferir)
                    {
                        return new ResultadoTransferencia
                        {
                            Sucesso = false,
                            Mensagem = "A quantidade de exemplares no núcleo de origem é insuficiente para realizar a transferência."
                        };
                    }
                }

                // 3️⃣ Realizar a transferência dos exemplares
                string queryTransferencia = @"
            UPDATE Exemplares 
            SET NucleoID = @DestinoNucleoID 
            WHERE ExemplarID IN (
                SELECT TOP (@QtdTransferir) ExemplarID 
                FROM Exemplares 
                WHERE ObraID = @ObraID 
                AND NucleoID = @OrigemNucleoID 
                AND Disponivel = 1
            );"; // Adicionei TOP para limitar a quantidade de exemplares transferidos

                using (SqlCommand cmdTransferencia = new SqlCommand(queryTransferencia, cn, transaction))
                {
                    cmdTransferencia.Parameters.AddWithValue("@ObraID", obraId);
                    cmdTransferencia.Parameters.AddWithValue("@OrigemNucleoID", origemNucleoId);
                    cmdTransferencia.Parameters.AddWithValue("@DestinoNucleoID", destinoNucleoId);
                    cmdTransferencia.Parameters.AddWithValue("@QtdTransferir", qtdTransferir);

                    int rowsAffected = cmdTransferencia.ExecuteNonQuery();

                    // Verificar se o número de exemplares afetados é o esperado
                    if (rowsAffected != qtdTransferir)
                    {
                        return new ResultadoTransferencia
                        {
                            Sucesso = false,
                            Mensagem = "Erro ao transferir os exemplares. Operação abortada."
                        };
                    }
                }

                // 4️⃣ Commit da transação se tudo correr bem
                transaction.Commit();

                return new ResultadoTransferencia
                {
                    Sucesso = true,
                    Mensagem = "Transferência concluída com sucesso."
                };
            }
            catch (Exception ex)
            {
                // Rollback em caso de erro
                transaction?.Rollback();

                return new ResultadoTransferencia
                {
                    Sucesso = false,
                    Mensagem = "Erro: " + ex.Message
                };
            }
            finally
            {
                // Fechar a conexão
                DB.Close(cn);
            }
        }

        public List<(string Titulo, int TotalRequisicoes)> Admin_Top10ObrasRequisitadasUltimoAno()
        {
            List<(string, int)> obrasMaisRequisitadas = new List<(string, int)>();
            SqlConnection cn = null;

            try
            {
                cn = DB.Open(_cnstring);
                string query = @"
            SELECT TOP 10 
                o.Titulo, 
                COUNT(r.RequisicaoID) AS TotalRequisicoes
            FROM Requisicoes r
            INNER JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
            INNER JOIN Obras o ON e.ObraID = o.ObraID
            WHERE r.DataRequisicao BETWEEN DATEADD(YEAR, -1, GETDATE()) AND GETDATE()
            GROUP BY o.Titulo
            ORDER BY TotalRequisicoes DESC;";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obrasMaisRequisitadas.Add((reader["Titulo"].ToString(), Convert.ToInt32(reader["TotalRequisicoes"])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar as top 10 obras requisitadas no último ano: " + ex.Message);
            }
            finally
            {
                DB.Close(cn);
            }

            return obrasMaisRequisitadas;
        }

        public List<NucleoUltimaRequisicao> Admin_ObterUltimaRequisicaoPorNucleo()
        {
            List<NucleoUltimaRequisicao> listaNucleos = new List<NucleoUltimaRequisicao>();
            DateTime dataLimite = DateTime.Now.AddDays(-365);

            using (SqlConnection cn = DB.Open(_cnstring))
            {
                string query = @"
            SELECT N.Nome AS NomeNucleo, 
                   MAX(R.DataRequisicao) AS UltimaRequisicao
            FROM Nucleos N
            LEFT JOIN Exemplares E ON N.NucleoID = E.NucleoID
            LEFT JOIN Requisicoes R ON E.ExemplarID = R.ExemplarID
            WHERE R.DataRequisicao IS NULL OR R.DataRequisicao >= @DataLimite
            GROUP BY N.Nome";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@DataLimite", dataLimite);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime? ultimaRequisicao = reader["UltimaRequisicao"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["UltimaRequisicao"]);

                            listaNucleos.Add(new NucleoUltimaRequisicao
                            {
                                NomeNucleo = reader["NomeNucleo"].ToString(),
                                UltimaRequisicao = ultimaRequisicao.HasValue
                                    ? ultimaRequisicao.Value.ToString("yyyy-MM-dd")
                                    : "Sem requisições no último ano"
                            });
                        }
                    }
                }
            }

            return listaNucleos;
        }










        //Leitores

        public List<(string Titulo, int TotalRequisicoes)> User_Top10ObrasRequisitadas(DateTime dataInicio, DateTime dataFim)
        {
            List<(string, int)> obrasMaisRequisitadas = new List<(string, int)>();
            SqlConnection cn = null;

            try
            {
                cn = DB.Open(_cnstring);
                string query = @"
                    SELECT TOP 10 
                        o.Titulo, 
                        COUNT(r.RequisicaoID) AS TotalRequisicoes
                    FROM Requisicoes r
                    INNER JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
                    INNER JOIN Obras o ON e.ObraID = o.ObraID
                    WHERE r.DataRequisicao BETWEEN @DataInicio AND @DataFim
                    GROUP BY o.Titulo
                    ORDER BY TotalRequisicoes DESC;";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@DataInicio", dataInicio);
                    cmd.Parameters.AddWithValue("@DataFim", dataFim);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obrasMaisRequisitadas.Add((reader["Titulo"].ToString(), Convert.ToInt32(reader["TotalRequisicoes"])));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao buscar as top 10 obras requisitadas: " + ex.Message);
            }
            finally
            {
                DB.Close(cn);
            }

            return obrasMaisRequisitadas;
        }

        public List<SituacaoLeitor> User_VerificarSituacaoAtual(int userId)
        {
            List<SituacaoLeitor> situacoes = new List<SituacaoLeitor>();
            SqlConnection cn = null;

            try
            {
                cn = DB.Open(_cnstring);
                string query = "VerificarSituacaoAtual";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SituacaoLeitor situacao = new SituacaoLeitor
                            {
                                Leitor = reader["Leitor"].ToString(),
                                Obra = reader["Obra"].ToString(),
                                Nucleo = reader["Nucleo"].ToString(),
                                DataRequisicao = Convert.ToDateTime(reader["DataRequisicao"]),
                                StatusDevolucao = reader["StatusDevolucao"].ToString()
                            };
                            situacoes.Add(situacao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao verificar a situação do leitor: " + ex.Message);
            }
            finally
            {
                DB.Close(cn);
            }

            return situacoes;
        }

        public List<DevolucaoDTO> DevolverObra(string username, string requisicao)
        {
            List<DevolucaoDTO> detalhesDevolucao = new List<DevolucaoDTO>();

            if (!int.TryParse(username, out int userID) || !int.TryParse(requisicao, out int requisicaoID))
            {
                Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                return detalhesDevolucao; // Retorna lista vazia
            }

            if (userID <= 0 || requisicaoID <= 0)
            {
                Console.WriteLine("Erro: UserID e RequisicaoID devem ser maiores que zero.");
                return detalhesDevolucao;
            }

            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = LibDB.DB.Open(_cnstring);
                transaction = conn.BeginTransaction();

                // Verificar se o usuário existe
                string queryUser = "SELECT COUNT(*) FROM Usuario WHERE UserID = @UserID";
                using (SqlCommand cmdUser = new SqlCommand(queryUser, conn, transaction))
                {
                    cmdUser.Parameters.AddWithValue("@UserID", userID);
                    if ((int)cmdUser.ExecuteScalar() == 0)
                        throw new Exception("UserID inválido. Não existe no banco de dados.");
                }

                // Verificar se a requisição existe
                string queryRequisicao = "SELECT COUNT(*) FROM Requisicoes WHERE RequisicaoID = @RequisicaoID";
                using (SqlCommand cmdRequisicao = new SqlCommand(queryRequisicao, conn, transaction))
                {
                    cmdRequisicao.Parameters.AddWithValue("@RequisicaoID", requisicaoID);
                    if ((int)cmdRequisicao.ExecuteScalar() == 0)
                        throw new Exception("Erro: RequisicaoID inválido. Não existe no banco de dados.");
                }

                // Atualizar a data de devolução
                string queryUpdateRequisicao = @"
                UPDATE Requisicoes 
                SET DataDevolucao = GETDATE() 
                WHERE RequisicaoID = @RequisicaoID 
                AND DataDevolucao IS NULL";

                using (SqlCommand cmdUpdateRequisicao = new SqlCommand(queryUpdateRequisicao, conn, transaction))
                {
                    cmdUpdateRequisicao.Parameters.AddWithValue("@RequisicaoID", requisicaoID);
                    if (cmdUpdateRequisicao.ExecuteNonQuery() == 0)
                        throw new Exception("Nenhuma requisição encontrada ou já devolvida.");
                }

                // Obter o ExemplarID da requisição
                string queryExemplar = "SELECT ExemplarID FROM Requisicoes WHERE RequisicaoID = @RequisicaoID";
                int exemplarID;
                using (SqlCommand cmdExemplar = new SqlCommand(queryExemplar, conn, transaction))
                {
                    cmdExemplar.Parameters.AddWithValue("@RequisicaoID", requisicaoID);
                    object result = cmdExemplar.ExecuteScalar();
                    if (result == null) throw new Exception("Exemplar não encontrado para a requisição.");
                    exemplarID = Convert.ToInt32(result);
                }

                // Atualizar disponibilidade do exemplar
                string queryUpdateExemplar = "UPDATE Exemplares SET Disponivel = 1 WHERE ExemplarID = @ExemplarID";
                using (SqlCommand cmdUpdateExemplar = new SqlCommand(queryUpdateExemplar, conn, transaction))
                {
                    cmdUpdateExemplar.Parameters.AddWithValue("@ExemplarID", exemplarID);
                    cmdUpdateExemplar.ExecuteNonQuery();
                }

                transaction.Commit(); // Confirma todas as alterações

                // --- Consulta os detalhes APÓS o commit ---
                string queryDetalhes = @"
                SELECT 
                    o.Titulo AS TituloObra,
                    o.Autor,
                    n.Nome AS Nucleo,
                    r.DataRequisicao,
                    r.DataDevolucao,
                    r.ExemplarID
                FROM Requisicoes r
                JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
                JOIN Obras o ON e.ObraID = o.ObraID
                JOIN Nucleos n ON e.NucleoID = n.NucleoID
                WHERE r.RequisicaoID = @RequisicaoID";

                using (SqlCommand cmdDetalhes = new SqlCommand(queryDetalhes, conn))
                {
                    cmdDetalhes.Parameters.AddWithValue("@RequisicaoID", requisicaoID);

                    using (SqlDataReader reader = cmdDetalhes.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var devolucao = new DevolucaoDTO
                            {
                                TituloObra = reader["TituloObra"].ToString(),
                                Autor = reader["Autor"].ToString(),
                                Nucleo = reader["Nucleo"].ToString(),
                                DataRequisicao = Convert.ToDateTime(reader["DataRequisicao"]),
                                DataDevolucao = reader["DataDevolucao"] == DBNull.Value
                                                ? (DateTime?)null
                                                : Convert.ToDateTime(reader["DataDevolucao"]),
                                ExemplarID = Convert.ToInt32(reader["ExemplarID"])
                            };
                            detalhesDevolucao.Add(devolucao);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                Console.WriteLine("Erro ao devolver obra: " + ex.Message);
                detalhesDevolucao.Clear();
            }
            finally
            {
                if (transaction != null)
                    LibDB.DB.Close(transaction);

                if (conn != null && conn.State == ConnectionState.Open)
                    LibDB.DB.Close(conn);
            }
            return detalhesDevolucao;
        }

        public List<Obra> FazerRequisicaoLivro(string username, string obra, string nucleo)
        {
            // Validar se os parâmetros são inteiros válidos (sem espaço, caracteres especiais, vírgulas, etc)
            if (!int.TryParse(username, out int userId) || !int.TryParse(obra, out int obraId) || !int.TryParse(nucleo, out int nucleoId))
            {
                Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                return null;
            }

            // Verificar se os IDs são maiores que zero
            if (userId <= 0 || obraId <= 0 || nucleoId <= 0)
            {
                Console.WriteLine("Erro: UserID, ObraID e NucleoID devem ser maiores que zero.");
                return null;
            }

            List<Obra> obrasRequisitadas = new List<Obra>(); // Lista para armazenar as obras requisitadas
            SqlConnection conn = null;
            SqlDataAdapter daDB = null;
            DataTable dt = null;
            SqlTransaction transaction = null;

            try
            {
                // Abrir a conexão com o banco
                conn = DB.Open(_cnstring);
                transaction = conn.BeginTransaction();

                // Verificar se o usuário é um leitor ativo
                string sSQL = @"
                SELECT COUNT(*) FROM Usuario 
                WHERE UserID = @UserID 
                AND TipoUser = 'Leitor' 
                AND Ativo = 1";

                daDB = DB.GetAdapterRead(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@UserID", userId);
                dt = new DataTable();
                daDB.Fill(dt);

                if (dt.Rows[0][0] == DBNull.Value || Convert.ToInt32(dt.Rows[0][0]) == 0)
                {
                    throw new Exception("Apenas leitores ativos podem fazer requisições.");
                }

                // Verificar se o usuário já tem 4 requisições ativas
                sSQL = @"
                SELECT COUNT(*) FROM Requisicoes 
                WHERE UserID = @UserID 
                AND DataDevolucao IS NULL";

                daDB = DB.GetAdapterRead(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@UserID", userId);
                dt.Clear();
                daDB.Fill(dt);

                if (Convert.ToInt32(dt.Rows[0][0]) >= 4)
                {
                    throw new Exception("O leitor já possui o limite de 4 requisições ativas.");
                }

                // Verificar disponibilidade de exemplares no núcleo
                sSQL = @"
                SELECT COUNT(*) FROM Exemplares 
                 WHERE ObraID = @ObraID 
                 AND NucleoID = @NucleoID 
                   AND Disponivel = 1";

                daDB = DB.GetAdapterRead(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@ObraID", obraId);
                daDB.SelectCommand.Parameters.AddWithValue("@NucleoID", nucleoId);
                dt.Clear();
                daDB.Fill(dt);

                if (Convert.ToInt32(dt.Rows[0][0]) < 2)
                {
                    throw new Exception("Não há exemplares suficientes para requisição neste núcleo.");
                }

                // Obter um exemplar disponível
                sSQL = @"
                SELECT TOP 1 ExemplarID FROM Exemplares 
                WHERE ObraID = @ObraID 
                AND NucleoID = @NucleoID 
                AND Disponivel = 1";

                daDB = DB.GetAdapterRead(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@ObraID", obraId);
                daDB.SelectCommand.Parameters.AddWithValue("@NucleoID", nucleoId);
                dt.Clear();
                daDB.Fill(dt);

                // Garantir que há pelo menos uma linha antes de acessar os dados
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("Nenhum exemplar disponível para requisição.");
                }

                // Garantir que o valor não seja DBNull
                if (dt.Rows[0]["ExemplarID"] == DBNull.Value)
                {
                    throw new Exception("Erro inesperado: ExemplarID é NULL.");
                }

                int exemplarId = Convert.ToInt32(dt.Rows[0]["ExemplarID"]);

                // Marcar exemplar como indisponível
                sSQL = @"
                UPDATE Exemplares SET Disponivel = 0 
                WHERE ExemplarID = @ExemplarID";

                daDB = DB.GetAdapterWrite(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@ExemplarID", exemplarId);
                daDB.SelectCommand.ExecuteNonQuery();

                // Registrar a requisição
                sSQL = @"
                INSERT INTO Requisicoes (UserID, ExemplarID, DataRequisicao) 
                VALUES (@UserID, @ExemplarID, GETDATE())";

                daDB = DB.GetAdapterWrite(conn, sSQL, transaction);
                daDB.SelectCommand.Parameters.AddWithValue("@UserID", userId);
                daDB.SelectCommand.Parameters.AddWithValue("@ExemplarID", exemplarId);

                // Executa a inserção e verifica se foi bem-sucedida
                int rowsAffected = daDB.SelectCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    // Agora vamos buscar as obras requisitadas
                    sSQL = @"
                    SELECT O.ObraID, O.Titulo, O.Autor, O.AnoPublicacao, O.Genero, O.Descricao
                    FROM Obras O
                    INNER JOIN Exemplares E ON O.ObraID = E.ObraID
                    WHERE E.ExemplarID = @ExemplarID";

                    daDB = DB.GetAdapterRead(conn, sSQL, transaction);
                    daDB.SelectCommand.Parameters.AddWithValue("@ExemplarID", exemplarId);
                    dt.Clear();
                    daDB.Fill(dt);

                    // Montar a lista de obras requisitadas
                    foreach (DataRow row in dt.Rows)
                    {
                        Obra obraRequisitada = new Obra
                        {
                            ObraID = Convert.ToInt32(row["ObraID"]),
                            Titulo = row["Titulo"].ToString(),
                            Autor = row["Autor"].ToString(),
                            AnoPublicacao = Convert.ToInt32(row["AnoPublicacao"]),
                            Genero = row["Genero"].ToString(),
                            Descricao = row["Descricao"].ToString()
                        };

                        obrasRequisitadas.Add(obraRequisitada);
                    }
                }
                else
                {
                    throw new Exception("Falha ao registrar a requisição.");
                }

                // Confirmar transação
                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine("Erro ao fazer requisição: " + ex.Message);
                return null;
            }
            finally
            {
                DB.Close(conn);
                DB.Close(daDB);
                DB.Close(dt);
            }

            return obrasRequisitadas; // Retorna a lista de obras requisitadas
        }

        public List<HistoricoRequisicaoDTO> ObterHistoricoRequisicoes(string username, string? nucleo = null, string? dataInicioInput = null, string? dataFimInput = null)
        {
            // Validações de parametros
            if (!int.TryParse(username, out int userId) || userId <= 0)
            {
                Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                return null;
            }

            int? nucleoId = null;
            if (!string.IsNullOrEmpty(nucleo))
            {
                if (!int.TryParse(nucleo, out int parseNucleoId) || parseNucleoId <= 0)
                {
                    Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                    return null;
                }
                nucleoId = parseNucleoId;
            }

            DateTime? dataInicio = null; // Tornando dataInicio como null por padrão
            if (!string.IsNullOrEmpty(dataInicioInput))
            {
                if (!DateTime.TryParse(dataInicioInput, out DateTime dataInicioParse) || dataInicioParse.Date < new DateTime(2000, 1, 1))
                {
                    Console.WriteLine("Entrada inválida! Digite uma data em formato válido (AAAA/mm/dd).");
                    return null;
                }
                dataInicio = dataInicioParse; // Atribuindo dataInicioParse se for válido
            }

            DateTime? dataFim = null; // Tornando dataFim como null por padrão
            if (!string.IsNullOrEmpty(dataFimInput))
            {
                if (!DateTime.TryParse(dataFimInput, out DateTime dataFimParse) ||
                    (dataInicio.HasValue && dataFimParse.Date < dataInicio.Value.Date))
                {
                    Console.WriteLine("Entrada inválida! Digite uma data em formato válido (AAAA/mm/dd) ou uma data maior que a data inicial.");
                    return null;
                }
                dataFim = dataFimParse; // Atribuindo dataFimParse se for válido
            }

            List<HistoricoRequisicaoDTO> historico = new List<HistoricoRequisicaoDTO>();
            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                // Abrir a conexão usando a LibDB
                conn = DB.Open(_cnstring);  // Supondo que _cnString seja sua string de conexão
                transaction = conn.BeginTransaction();  // Iniciar a transação

                string query = @"
                SELECT 
                    u.Nome AS Leitor,
                    o.Titulo AS Obra,
                    n.Nome AS Nucleo,
                    r.DataRequisicao,
                    CASE 
                        WHEN r.DataDevolucao IS NULL THEN 'Em Aberto'
                        ELSE 'Devolvido'
                    END AS StatusRequisicao
                FROM Requisicoes r
                JOIN Usuario u ON r.UserID = u.UserID
                JOIN Exemplares e ON r.ExemplarID = e.ExemplarID
                JOIN Obras o ON e.ObraID = o.ObraID
                JOIN Nucleos n ON e.NucleoID = n.NucleoID
                WHERE r.UserID = @UserID";

                // Adicionar parâmetros
                List<SqlParameter> parametros = new List<SqlParameter>
        {
            new SqlParameter("@UserID", userId)
        };

                if (nucleoId.HasValue)
                {
                    query += " AND n.NucleoID = @NucleoID";
                    parametros.Add(new SqlParameter("@NucleoID", nucleoId.Value));
                }

                if (dataInicio.HasValue)
                {
                    query += " AND r.DataRequisicao >= @DataInicio";
                    parametros.Add(new SqlParameter("@DataInicio", dataInicio.Value.Date));
                }

                if (dataFim.HasValue)
                {
                    query += " AND r.DataRequisicao <= @DataFim";
                    parametros.Add(new SqlParameter("@DataFim", dataFim.Value.Date));
                }

                query += " ORDER BY r.DataRequisicao DESC";

                // Usar o DB.GetAdapterRead para criar o comando
                SqlDataAdapter daDB = DB.GetAdapterRead(conn, query, transaction);
                daDB.SelectCommand.Parameters.AddRange(parametros.ToArray());

                // Criar um DataTable para armazenar os dados
                DataTable dt = new DataTable();
                daDB.Fill(dt);

                // Verificar se retornou algum registro
                if (dt.Rows.Count == 0)
                {
                    throw new Exception($"Nenhuma requisição encontrada para o usuário com ID {userId}.");
                }

                // Converter os dados para o DTO
                foreach (DataRow row in dt.Rows)
                {
                    var registro = new HistoricoRequisicaoDTO
                    {
                        Leitor = row["Leitor"].ToString(),
                        Obra = row["Obra"].ToString(),
                        Nucleo = row["Nucleo"].ToString(),
                        DataRequisicao = Convert.ToDateTime(row["DataRequisicao"]),
                        StatusRequisicao = row["StatusRequisicao"].ToString()
                    };
                    historico.Add(registro);
                }

                // Commit da transação
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Se ocorrer um erro, reverter a transação e lançar a exceção
                if (transaction != null)
                {
                    transaction.Rollback();
                }
                Console.WriteLine("Erro ao fazer requisição: " + ex.Message);
                return null;
            }
            finally
            {
                // Garantir que a conexão seja fechada corretamente
                DB.Close(conn);
            }

            return historico;
        }

        public List<ObraDisponivelDto> ObterObrasDisponiveis(string? nucleo = null, string? tema = null, string? titulo = null, string? autor = null)
        {
            int? nucleoId = null;
            if (!string.IsNullOrEmpty(nucleo))
            {
                if (!int.TryParse(nucleo, out int parseNucleoId) || parseNucleoId <= 0)
                {
                    Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                    return null;
                }
                nucleoId = parseNucleoId;
            }

            List<ObraDisponivelDto> obrasDisponiveis = new List<ObraDisponivelDto>();

            SqlConnection conn = null;
            SqlTransaction transaction = null;

            try
            {
                conn = DB.Open(_cnstring);
                transaction = conn.BeginTransaction();

                // Query base ajustada
                string query = @"
        SELECT 
            o.Titulo AS Obra,
            o.Autor,
            o.AnoPublicacao,
            o.Genero AS Tema,
            COUNT(CASE WHEN e.Disponivel = 1 THEN 1 END) AS QuantidadeDisponivel"; // Usar COUNT para simplificar

                if (nucleoId.HasValue)
                {
                    query += ", n.Nome AS Nucleo";
                }

                query += @" FROM Obras o
        JOIN Exemplares e ON o.ObraID = e.ObraID";

                if (nucleoId.HasValue)
                {
                    query += " JOIN Nucleos n ON e.NucleoID = n.NucleoID";
                }

                // Ajuste nas condições para usar COALESCE e tratar strings vazias
                query += @" WHERE e.Disponivel = 1
        AND (@Tema IS NULL OR o.Genero LIKE '%' + COALESCE(@Tema, '') + '%')
        AND (@Titulo IS NULL OR o.Titulo LIKE '%' + COALESCE(@Titulo, '') + '%')
        AND (@Autor IS NULL OR o.Autor LIKE '%' + COALESCE(@Autor, '') + '%')";

                if (nucleoId.HasValue)
                {
                    query += " AND n.NucleoID = @NucleoID";
                }

                // GROUP BY simplificado
                query += " GROUP BY o.Titulo, o.Autor, o.AnoPublicacao, o.Genero";
                if (nucleoId.HasValue)
                {
                    query += ", n.Nome";
                }

                query += " HAVING COUNT(CASE WHEN e.Disponivel = 1 THEN 1 END) > 0 ORDER BY o.Titulo";

                using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                {
                    // Usar DBNull.Value apenas se o parâmetro for nulo ou vazio
                    cmd.Parameters.AddWithValue("@Tema", string.IsNullOrEmpty(tema) ? DBNull.Value : (object)tema);
                    cmd.Parameters.AddWithValue("@Titulo", string.IsNullOrEmpty(titulo) ? DBNull.Value : (object)titulo);
                    cmd.Parameters.AddWithValue("@Autor", string.IsNullOrEmpty(autor) ? DBNull.Value : (object)autor);

                    if (nucleoId.HasValue)
                        cmd.Parameters.AddWithValue("@NucleoID", nucleoId.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var obra = new ObraDisponivelDto
                            {
                                Obra = reader["Obra"].ToString(),
                                Autor = reader["Autor"].ToString(),
                                AnoPublicacao = Convert.ToInt32(reader["AnoPublicacao"]),
                                Tema = reader["Tema"].ToString(),
                                QuantidadeDisponivel = Convert.ToInt32(reader["QuantidadeDisponivel"])
                            };

                            if (nucleoId.HasValue)
                            {
                                obra.Nucleo = reader["Nucleo"].ToString();
                            }

                            obrasDisponiveis.Add(obra);
                        }
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                throw new Exception("Erro ao obter obras disponíveis: " + ex.Message);
            }
            finally
            {
                DB.Close(conn);
            }

            return obrasDisponiveis;
        }

    }


}
