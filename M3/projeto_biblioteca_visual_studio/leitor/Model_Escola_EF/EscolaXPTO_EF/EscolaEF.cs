using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LibDB_EF;
using LibDB_EF.Models;
using static EscolaXPTO_EF.Dto.Dtos;
using Microsoft.Data.SqlClient;
using System.Collections;
using System.Security.Cryptography;


namespace EscolaXPTO_EF
{
    public class EscolaEF
    {
        private readonly string _cnString;

        public EscolaEF(string cnString)
        {
            _cnString = cnString;
            BibliotecaXptoContext obj = new BibliotecaXptoContext(_cnString);
        }

        // Método para devolver obra
        public List<DevolucaoDTO> DevolverObra(string userId, string requisicao)
        {
            if (!int.TryParse(userId, out int userID) || !int.TryParse(requisicao, out int requisicaoID))
            {
                Console.WriteLine("Entrada inválida! Digite um número inteiro válido.");
                return new List<DevolucaoDTO>();
            }

            if (userID <= 0 || requisicaoID <= 0)
            {
                Console.WriteLine("Erro: UserID e RequisicaoID devem ser maiores que zero.");
                return new List<DevolucaoDTO>();
            }

            using (var context = new BibliotecaXptoContext(_cnString))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Verificar se o usuário existe
                        var usuario = context.Usuarios.FirstOrDefault(u => u.UserId == userID);
                        if (usuario == null)
                            throw new Exception("UserID inválido. Não existe no banco de dados.");

                        // Verificar se a requisição existe
                        var requisicaoAtual = context.Requisicoes.FirstOrDefault(r => r.RequisicaoId == requisicaoID);
                        if (requisicaoAtual == null)
                            throw new Exception("Erro: RequisicaoID inválido. Não existe no banco de dados.");

                        // Atualizar a data de devolução
                        if (requisicaoAtual.DataDevolucao != null)
                            throw new Exception("Nenhuma requisição encontrada ou já devolvida.");

                        requisicaoAtual.DataDevolucao = DateOnly.FromDateTime(DateTime.Now);

                        // Atualizar disponibilidade do exemplar
                        var exemplar = context.Exemplares.FirstOrDefault(e => e.ExemplarId == requisicaoAtual.ExemplarId);
                        if (exemplar == null)
                            throw new Exception("Exemplar não encontrado para a requisição.");

                        exemplar.Disponivel = true;

                        context.SaveChanges();
                        transaction.Commit();

                        // Consultar detalhes da devolução
                        var detalhesDevolucao = context.Requisicoes
                            .Where(r => r.RequisicaoId == requisicaoID)
                            .Join(context.Exemplares, r => r.ExemplarId, e => e.ExemplarId, (r, e) => new { r, e })
                            .Join(context.Obras, re => re.e.ObraId, o => o.ObraId, (re, o) => new { re.r, re.e, o })
                            .Join(context.Nucleos, reo => reo.e.NucleoId, n => n.NucleoId, (reo, n) => new DevolucaoDTO
                            {
                                RequisicaoId = reo.r.RequisicaoId, // Adicionando o RequisicaoId
                                TituloObra = reo.o.Titulo,
                                Autor = reo.o.Autor,
                                Nucleo = n.Nome,
                                DataRequisicao = reo.r.DataRequisicao,
                                DataDevolucao = reo.r.DataDevolucao,
                                ExemplarID = reo.e.ExemplarId
                            })
                            .ToList();

                        return detalhesDevolucao;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Erro ao devolver obra: " + ex.Message);
                        return new List<DevolucaoDTO>();
                    }
                }
            }
        }

        public DateOnly? FazerRequisicaoLivro(int userId, int obraId, int nucleoId)
        {
            // Validação dos parâmetros
            if (userId <= 0 || obraId <= 0 || nucleoId <= 0)
            {
                Console.WriteLine("Erro: IDs devem ser maiores que zero.");
                return null;
            }

            using (var context = new BibliotecaXptoContext(_cnString))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Verificar usuário leitor ativo
                        var usuario = context.Usuarios.FirstOrDefault(u =>
                            u.UserId == userId &&
                            u.TipoUser == "Leitor" &&
                            u.Ativo == true);

                        if (usuario == null)
                            throw new Exception("Apenas leitores ativos podem fazer requisições.");

                        // Verificar limite de requisições
                        var requisicoesAtivas = context.Requisicoes
                            .Count(r => r.UserId == userId && r.DataDevolucao == null);

                        if (requisicoesAtivas >= 4)
                            throw new Exception("Limite de 4 requisições ativas atingido.");

                        // Verificar disponibilidade no núcleo (mínimo 2 exemplares disponíveis)
                        var exemplaresDisponiveis = context.Exemplares
                            .Count(e =>
                                e.ObraId == obraId &&
                                e.NucleoId == nucleoId &&
                                e.Disponivel == true);

                        if (exemplaresDisponiveis < 2)
                            throw new Exception("Exemplares insuficientes neste núcleo.");

                        // Selecionar exemplar disponível
                        var exemplar = context.Exemplares
                            .FirstOrDefault(e =>
                                e.ObraId == obraId &&
                                e.NucleoId == nucleoId &&
                                e.Disponivel == true);

                        if (exemplar == null)
                            throw new Exception("Nenhum exemplar disponível.");

                        // Atualizar disponibilidade
                        exemplar.Disponivel = false;
                        context.SaveChanges();

                        // Criar nova requisição
                        var dataRequisicao = DateOnly.FromDateTime(DateTime.Now);
                        var novaRequisicao = new Requisico
                        {
                            UserId = userId,
                            ExemplarId = exemplar.ExemplarId,
                            DataRequisicao = dataRequisicao
                        };

                        context.Requisicoes.Add(novaRequisicao);
                        context.SaveChanges();

                        transaction.Commit();

                        // Calcula a data de devolução (15 dias após a requisição)
                        DateOnly dataDevolucao = dataRequisicao.AddDays(15);
                        return dataDevolucao;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Erro: {ex.Message}");
                        return null;
                    }
                }
            }
        }

        public List<HistoricoRequisicaoDTO> ObterHistoricoRequisicoes(string username, string? nucleo = null, string? dataInicioInput = null, string? dataFimInput = null)
        {
            if (!int.TryParse(username, out int userId) || userId <= 0)
                throw new ArgumentException("UserID inválido!");

            DateOnly? dataInicio = null;
            if (!string.IsNullOrEmpty(dataInicioInput) && DateOnly.TryParse(dataInicioInput, out DateOnly parsedDataInicio))
            {
                dataInicio = parsedDataInicio;
            }

            DateOnly? dataFim = null;
            if (!string.IsNullOrEmpty(dataFimInput) && DateOnly.TryParse(dataFimInput, out DateOnly parsedDataFim))
            {
                if (dataInicio.HasValue && parsedDataFim < dataInicio.Value)
                    throw new ArgumentException("A data final deve ser maior ou igual à data de início.");
                dataFim = parsedDataFim;
            }

            List<HistoricoRequisicaoDTO> historico = new List<HistoricoRequisicaoDTO>();

            using (var context = new BibliotecaXptoContext(_cnString))
            {
                var query = context.Requisicoes
                    .Include(r => r.User)
                    .Include(r => r.Exemplar)
                        .ThenInclude(e => e.Obra)
                    .Include(r => r.Exemplar)
                        .ThenInclude(e => e.Nucleo)
                    .Where(r => r.UserId == userId);

                // Filtro por nome do núcleo (string)
                if (!string.IsNullOrEmpty(nucleo))
                {
                    query = query.Where(r => r.Exemplar.Nucleo.Nome == nucleo);
                }

                // Filtro por data de início
                if (dataInicio.HasValue)
                {
                    query = query.Where(r => r.DataRequisicao >= dataInicio.Value);
                }

                // Filtro por data de fim
                if (dataFim.HasValue)
                {
                    query = query.Where(r => r.DataRequisicao <= dataFim.Value);
                }

                historico = query
                    .OrderByDescending(r => r.DataRequisicao)
                    .Select(r => new HistoricoRequisicaoDTO
                    {
                        Leitor = r.User.Nome,
                        Obra = r.Exemplar.Obra.Titulo,
                        Nucleo = r.Exemplar.Nucleo.Nome,
                        DataRequisicao = r.DataRequisicao,
                        StatusRequisicao = r.DataDevolucao == null ? "Em Aberto" : "Devolvido"
                    })
                    .ToList();
            }

            return historico;
        }
        public List<SituacaoLeitorDTO> User_VerificarSituacaoAtual(int userId)
        {
            using (var context = new BibliotecaXptoContext(_cnString))
            {
                using (var transaction = context.Database.BeginTransaction()) // 🔥 Gerencia a transação
                {
                    try
                    {
                        var hoje = DateOnly.FromDateTime(DateTime.Now);

                        var situacoes = context.Requisicoes
                            .Where(r => r.UserId == userId && r.DataDevolucao == null) // Apenas requisições não devolvidas
                            .Select(r => new SituacaoLeitorDTO
                            {
                                RequisicaoId = r.RequisicaoId,
                                Leitor = r.User.Nome,
                                Obra = r.Exemplar.Obra.Titulo,
                                Nucleo = r.Exemplar.Nucleo.Nome,
                                DataRequisicao = r.DataRequisicao,
                                StatusDevolucao =
                                    DateOnly.FromDateTime(DateTime.Now) > r.DataRequisicao.AddDays(15) ? "ATRASO" :
                                    (r.DataRequisicao.AddDays(15).DayNumber - hoje.DayNumber) <= 3 ? "Devolução URGENTE" :
                                    (r.DataRequisicao.AddDays(15).DayNumber - hoje.DayNumber) <= 5 ? "Devolver em breve" :
                                    "No Prazo"
                            })
                            .ToList();

                        transaction.Commit(); // 🔥 Confirmar a transação

                        return situacoes;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); // 🔥 Reverter a transação se houver erro
                        throw new Exception("Erro ao verificar a situação do leitor: " + ex.Message);
                    }
                }
            }
        }

        public List<(string Titulo, int TotalRequisicoes)> User_Top10ObrasRequisitadasUltimoAno()
        {
            using (var context = new BibliotecaXptoContext(_cnString))
            {
                try
                {
                    var dataInicio = DateOnly.FromDateTime(DateTime.Now.AddYears(-1)); // Data de 1 ano atrás
                    var dataFim = DateOnly.FromDateTime(DateTime.Now);// Último momento de hoje

                    var topObras = context.Requisicoes
                        .Where(r => r.DataRequisicao >= dataInicio && r.DataRequisicao <= dataFim)
                        .GroupBy(r => r.Exemplar.Obra.Titulo)
                        .Select(g => new
                        {
                            Titulo = g.Key,
                            TotalRequisicoes = g.Count()
                        })
                        .OrderByDescending(o => o.TotalRequisicoes)
                        .Take(10)
                        .ToList();

                    // Converte o resultado para uma lista de tuplas
                    var obrasMaisRequisitadas = topObras
                        .Select(o => (o.Titulo, o.TotalRequisicoes))
                        .ToList();

                    return obrasMaisRequisitadas;
                }
                catch (Exception ex)
                {
                    throw new Exception("Erro ao buscar as top 10 obras requisitadas no último ano: " + ex.Message);
                }
            }
        }

        public bool User_RegistrarNovoLeitor(string nome, DateOnly dataNascimento, string email, string telefone, string username, string palavraPasse, out string mensagemRetorno)
        {
            bool sucesso = false;
            mensagemRetorno = "";

            using (var context = new BibliotecaXptoContext(_cnString))
            {
                using (var transaction = context.Database.BeginTransaction()) // 🔥 Iniciando transação no EF Core
                {
                    try
                    {
                        // 🔹 Verificar se o Email já existe
                        if (context.Usuarios.Any(u => u.Email == email))
                        {
                            mensagemRetorno = "Erro: Já existe um usuário com este email.";
                            transaction.Rollback();
                            return false;
                        }

                        // 🔹 Verificar se o Telefone já existe
                        if (context.Usuarios.Any(u => u.Telefone == telefone))
                        {
                            mensagemRetorno = "Erro: Já existe um usuário com este telefone.";
                            transaction.Rollback();
                            return false;
                        }

                        // 🔥 Converter senha para Hash (SHA-256)
                        byte[] senhaHash = ConvertToSHA256(palavraPasse);

                        // 🔹 Criar novo usuário
                        var novoUsuario = new Usuario
                        {
                            Nome = nome,
                            DataNascimento = dataNascimento,
                            Email = email,
                            Telefone = telefone,
                            Username = username,
                            PalavraPasse = senhaHash, // 🔥 Agora armazena a senha como Hash
                            TipoUser = "Leitor",
                            Ativo = true
                        };

                        context.Usuarios.Add(novoUsuario);
                        int registrosAfetados = context.SaveChanges(); // 🔥 Persistindo no banco

                        if (registrosAfetados > 0)
                        {
                            mensagemRetorno = "Registo efetuado. <a href='/User/Login'>Faça o login aqui</a>.";
                            transaction.Commit(); // 🔥 Confirmando transação
                            sucesso = true;
                        }
                        else
                        {
                            mensagemRetorno = "Erro ao inserir usuário.";
                            transaction.Rollback(); // 🔥 Revertendo transação
                        }
                    }
                    catch (Exception ex)
                    {
                        mensagemRetorno = "Erro inesperado: " + ex.Message;
                        transaction.Rollback(); // 🔥 Revertendo transação em caso de erro
                    }
                }
            }
            return sucesso;
        }

        public bool AutenticarLeitor(string username, string password, out string mensagemRetorno)
        {
            bool autenticado = false;
            mensagemRetorno = ""; 

            try
            {
                using (var context = new BibliotecaXptoContext(_cnString))
                {
                    var usuario = context.Usuarios
                        .FirstOrDefault(u => u.Username == username && u.TipoUser == "Leitor" && u.Ativo == true);

                    // Se o usuário não existe:
                    if (usuario == null)
                    {
                        
                        var bibliotecario = context.Usuarios
                            .FirstOrDefault(u => u.Username == username && u.TipoUser == "Bibliotecario");

                        if (bibliotecario != null)
                        {
                            mensagemRetorno = "Deve se logar como Leitor.";
                            return false;
                        }

                        // Verifica se a conta está inativa
                        var inativo = context.Usuarios
                            .FirstOrDefault(u => u.Username == username && u.Ativo == false);

                        if (inativo != null)
                        {
                            mensagemRetorno = "Conta inativa. Compareça à biblioteca para reativar.";
                            return false;
                        }

                        // Usuário não encontrado
                        mensagemRetorno = "Usuário ou senha incorretos.";
                        return false;
                    }

                    
                    if (!VerificarSenha(password, usuario.PalavraPasse))
                    {
                        mensagemRetorno = "Usuário ou senha incorretos."; 
                        return false;
                    }

                    mensagemRetorno = "Autenticado com sucesso!";
                    autenticado = true;
                     
                }
            }
            catch (Exception ex)
            {
                mensagemRetorno = "Erro interno. Tente novamente mais tarde.";
                Console.WriteLine($"Erro na autenticação: {ex.Message}");
            }

            return autenticado;
        }

        public PerfilDTO ObterPerfilUsuario(int userId)
        {
            try
            {
                using (var context = new BibliotecaXptoContext(_cnString))
                {
                    return context.Usuarios
                        .Where(u => u.UserId == userId)
                        .Select(u => new PerfilDTO
                        {
                            Nome = u.Nome,
                            DataNascimento = u.DataNascimento,
                            Email = u.Email,
                            Telefone = u.Telefone,
                            Username = u.Username
                        }).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter perfil: {ex.Message}");
                return null;
            }
        }

        public bool AtualizarPerfilUsuario(int userId, PerfilDTO perfilAtualizado)
        {
            try
            {
                using (var context = new BibliotecaXptoContext(_cnString))
                {
                    var usuario = context.Usuarios.Find(userId);
                    if (usuario == null) return false;

                    // Atualiza campos apenas se forem fornecidos novos valores
                    usuario.Nome = perfilAtualizado.Nome ?? usuario.Nome;
                    usuario.Email = perfilAtualizado.Email ?? usuario.Email;
                    usuario.Telefone = perfilAtualizado.Telefone ?? usuario.Telefone;
                    usuario.Username = perfilAtualizado.Username ?? usuario.Username;

                    if (perfilAtualizado.DataNascimento != DateOnly.MinValue)
                        usuario.DataNascimento = perfilAtualizado.DataNascimento;

                    context.SaveChanges(); // ✅ Persiste as alterações
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar: {ex.Message}");
                return false;
            }
        }

        public List<ObraDto> MostrarTodasObras(string? pesquisa = null)
        {
            using (var context = new BibliotecaXptoContext(_cnString))
            {
                // 1) Monta a query base
                var query =
                    from o in context.Obras
                    join img in context.Imagens on o.ObraId equals img.IdObraId into obraImg
                    from i in obraImg.DefaultIfEmpty() // LEFT JOIN
                                                       // WHERE NOT EXISTS (Exemplar com Disponivel=1)
                    where context.Exemplares.Any(e => e.ObraId == o.ObraId && e.Disponivel == true)
                    select new ObraDto
                    {
                        ObraID = o.ObraId,
                        Titulo = o.Titulo,
                        Autor = o.Autor,
                        AnoPublicacao = o.AnoPublicacao,
                        Genero = o.Genero,
                        Descricao = o.Descricao,
                        ImagemBase64 = i.Capa != null ? Convert.ToBase64String(i.Capa) : null
                    };

                // 2) Filtros de pesquisa
                if (!string.IsNullOrEmpty(pesquisa))
                {
                    query = query.Where(x =>
                        x.Titulo.Contains(pesquisa) ||
                        x.Autor.Contains(pesquisa)
                    );
                }

                // 3) Executa e retorna
                return query.ToList();
            }
        }

        public List<CancelarInscricaoDTO> CancelarInscricao(int userId)
        {
            Console.WriteLine($"Iniciando CancelarInscricao para UserID: {userId}");
            List<CancelarInscricaoDTO> dadosUtilizador = new List<CancelarInscricaoDTO>();

            if (userId <= 0)
            {
                Console.WriteLine("UserID inválido recebido.");
                return null; // Retorna nulo se o userID não for válido
            }

            using (var context = new BibliotecaXptoContext(_cnString))
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Busca as requisições em aberto do usuário
                        var requisicoesEmAberto = context.Requisicoes
                            .Where(r => r.UserId == userId && r.DataDevolucao == null)
                            .Select(r => r.RequisicaoId)
                            .ToList();

                        Console.WriteLine($"Requisições em aberto encontradas: {requisicoesEmAberto.Count}");

                        // Busca os dados do usuário
                        var usuario = context.Usuarios.FirstOrDefault(u => u.UserId == userId);
                        if (usuario == null)
                        {
                            Console.WriteLine("Usuário não encontrado.");
                            return null;
                        }

                        foreach (var requisicaoId in requisicoesEmAberto)
                        {
                            Console.WriteLine($"Devolvendo obra para RequisicaoID: {requisicaoId}");

                            // Realiza a devolução e obtém os detalhes
                            var dadosRequisicao = DevolverObra(userId.ToString(), requisicaoId.ToString());

                            if (dadosRequisicao == null || !dadosRequisicao.Any())
                            {
                                Console.WriteLine($"Erro: Devolução falhou para RequisicaoID: {requisicaoId}");
                            }
                            else
                            {
                                dadosUtilizador.Add(new CancelarInscricaoDTO
                                {
                                    Id = usuario.UserId.ToString(),
                                    Nome = usuario.Nome,
                                    FimInscrincao = usuario.DataRegisto ?? DateOnly.MinValue,
                                    ObraDevolvida = dadosRequisicao
                                });
                            }
                        }

                        if (!dadosUtilizador.Any())
                        {
                            Console.WriteLine("Nenhuma devolução realizada.");
                        }

                        // Atualiza o campo Ativo para false (0)
                        usuario.Ativo = false;
                        context.SaveChanges();
                        Console.WriteLine("Usuário desativado.");

                        // Commit da transação
                        transaction.Commit();
                        Console.WriteLine("Transação concluída com sucesso.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Erro ao cancelar inscrição: {ex.Message}");
                        return null;
                    }
                }
            }

            return dadosUtilizador;
        }

        public string AutenticarBibliotecario(string username, string password)
        {
            string mensagem = "";
            try
            {
                using (var context = new BibliotecaXptoContext(_cnString))
                {
                    var usuario = context.Usuarios
                        .FirstOrDefault(u => u.Username == username && u.TipoUser == "Bibliotecario" && u.Ativo == true);

                    // Se o usuário não existe:
                    if (usuario == null)
                    {

                        var leitor = context.Usuarios
                            .FirstOrDefault(u => u.Username == username && u.TipoUser == "Leitor");

                        if (leitor != null)
                        {
                            
                            return mensagem = "Leitores não tem permissão pra aceder.";
                            
                        }

                        // Verifica se a conta está inativa
                        var inativo = context.Usuarios
                            .FirstOrDefault(u => u.Username == username && u.Ativo == false);

                        if (inativo != null)
                        {
                            return mensagem = "Conta inativa. Compareça à biblioteca para reativar.";
                        }

                        // Usuário não encontrado
                        return mensagem = "Usuário ou senha incorretos.";                       
                    }


                    if (!VerificarSenha(password, usuario.PalavraPasse))
                    {
                        return mensagem = "Usuário ou senha incorretos.";
                    }

                    return mensagem = "";

                }
            }
            catch (Exception ex)
            {              
                Console.WriteLine($"Erro na autenticação: {ex.Message}");
            }

            return mensagem;
        }

        // ---------------------🔥 Métodos auxiliares-------------------
        private bool VerificarSenha(string senhaDigitada, byte[] senhaArmazenada)
        {
            // Converte a senha digitada em hash para comparar
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] senhaDigitadaHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaDigitada));
                return StructuralComparisons.StructuralEqualityComparer.Equals(senhaDigitadaHash, senhaArmazenada);
            }
        }

        public int ObterUserIdPorUsername(string username)
        {
            using (var context = new BibliotecaXptoContext(_cnString))
            {
                var usuario = context.Usuarios
                    .FirstOrDefault(u => u.Username == username); // Supondo que 'Nome' seja o campo do username
                return usuario?.UserId ?? 0;
            }
        }

        // 🔥 Método para converter a senha em Hash SHA-256
        private byte[] ConvertToSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
        }

        public List<NucleoDto> ObterNucleosDisponiveisPorObra(int obraId)
        {
            using (var context = new BibliotecaXptoContext(_cnString))
            {
                var query = from e in context.Exemplares
                            join n in context.Nucleos on e.NucleoId equals n.NucleoId
                            where e.ObraId == obraId && e.Disponivel == true
                            group e by new { n.NucleoId, n.Nome } into g
                            where g.Count() >= 2
                            select new NucleoDto
                            {
                                NucleoID = g.Key.NucleoId,
                                Nome = g.Key.Nome
                            };
                return query.ToList();
            }
        }

        
    }
}
