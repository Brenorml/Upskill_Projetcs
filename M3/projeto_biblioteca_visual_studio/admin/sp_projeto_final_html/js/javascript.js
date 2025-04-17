const apiBaseUrl = 'http://api.test';

function criarFormularioRegisto() {
    const mainContent = document.getElementById('main-content-Registar-utilizador');
    mainContent.innerHTML = ''; // Limpar conteúdo existente

    // Criar elementos base
    const container = document.createElement('div');
    container.className = 'form-container';

    // Título
    const titulo = document.createElement('h1');
    titulo.textContent = 'Registar Novo Utilizador';

    // Formulário
    const form = document.createElement('form');
    form.id = 'formRegistoUtilizador';
    form.className = 'form-dinamico';

    // Campos do formulário
    const campos = [
        { 
            tipo: 'text', 
            id: 'nome', 
            label: 'Nome Completo', 
            obrigatorio: true
        },
        { 
            tipo: 'date', 
            id: 'dataNascimento', 
            label: 'Data de Nascimento', 
            obrigatorio: true,
            min: '1900-01-01',
            max: new Date().toISOString().split('T')[0] // Data atual
        },
        { 
            tipo: 'email', 
            id: 'email', 
            label: 'Email', 
            obrigatorio: true,
            pattern: "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"
        },
        { 
            tipo: 'tel', 
            id: 'telefone', 
            label: 'Telefone', 
            obrigatorio: true,
            min: 1
        },
        { 
            tipo: 'text', 
            id: 'username', 
            label: 'Username', 
            obrigatorio: true
        },
        { 
            tipo: 'password', 
            id: 'palavraPasse', 
            label: 'Password', 
            obrigatorio: true,
            minLength: 6
        },
        { 
            tipo: 'select', 
            id: 'tipoUser', 
            label: 'Tipo de Utilizador',
            opcoes: ['Leitor', 'Bibliotecario'] 
        }
    ];

    // Adicionar campos ao formulário
    campos.forEach(campo => {
        const grupo = document.createElement('div');
        grupo.className = 'form-group';

        const label = document.createElement('label');
        label.htmlFor = campo.id;
        label.textContent = campo.label;

        grupo.appendChild(label);

        if (campo.tipo === 'select') {
            const select = document.createElement('select');
            select.id = campo.id;
            select.name = campo.id;
            select.required = campo.obrigatorio;

            campo.opcoes.forEach(opcao => {
                const option = document.createElement('option');
                option.value = opcao.toLowerCase();
                option.textContent = opcao;
                select.appendChild(option);
            });

            grupo.appendChild(select);
        } else {
            const input = document.createElement('input');
            input.type = campo.tipo;
            input.id = campo.id;
            input.name = campo.id;
            input.required = campo.obrigatorio;

            // Adicionar validações específicas
            if (campo.min) {
                input.min = campo.min;
            }
            if (campo.max) {
                input.max = campo.max;
            }
            if (campo.pattern) {
                input.pattern = campo.pattern.replace(/\\/g, '\\'); // Corrige o escape para o HTML
            }
            if (campo.minLength) {
                input.minLength = campo.minLength;
            }

            // Adicionar evento oninput para o campo de nome
            if (campo.id === 'nome') {
                input.addEventListener('input', function () {
                    this.value = this.value.replace(/[^A-Za-zÀ-ÖØ-öø-ÿ\s]/g, '').replace(/\s{2,}/g, ' ');
                });
            }

            // Adicionar evento oninput para o campo de username
            if (campo.id === 'username') {
                input.addEventListener('input', function () {
                    this.value = this.value.replace(/\s/g, '');
                });
            }

            if (campo.id === 'telefone') {
                input.type = 'tel'; // Garante que o tipo seja 'tel'
                input.maxLength = 9; // Limita o input a 9 caracteres
            
                // Impede a entrada de letras e caracteres especiais
                input.addEventListener('keydown', function (e) {
                    // Permite apenas teclas numéricas (0-9) e algumas teclas especiais (Backspace, Tab, etc.)
                    if (!((e.key >= '0' && e.key <= '9') || ['Backspace', 'Tab', 'ArrowLeft', 'ArrowRight'].includes(e.key))) {
                        e.preventDefault(); // Impede a entrada
                    }
                });
            
                // Garante que o valor não seja negativo (não aplica para campos de telefone, mas pode ser útil)
                input.addEventListener('input', function () {
                    if (this.value < 0) {
                        this.value = '0'; // Se o valor for negativo, define como 0
                    }
                });
            }            

            grupo.appendChild(input);
        }

        form.appendChild(grupo);
    });

    // Botão de submissão
    const btnSubmit = document.createElement('button');
    btnSubmit.type = 'submit';
    btnSubmit.className = 'btn-primary';
    btnSubmit.textContent = 'Registar Utilizador';

    // Container de mensagens
    const msgContainer = document.createElement('div');
    msgContainer.id = 'mensagem-resposta';

    // Montagem da estrutura
    form.appendChild(btnSubmit);
    container.append(titulo, form, msgContainer);
    mainContent.appendChild(container);

    // Event listener para o formulário
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        
        const dados = {
            nome: document.getElementById('nome').value,
            dataNascimento: document.getElementById('dataNascimento').value,
            email: document.getElementById('email').value,
            telefone: document.getElementById('telefone').value,
            username: document.getElementById('username').value,
            palavraPasse: document.getElementById('palavraPasse').value,
            tipoUser: document.getElementById('tipoUser').value
        };

        if (!validarDadosRegisto(dados)) return;

        try {
            const resposta = await fetch(`${apiBaseUrl}/Admin_RegistrarNovoUtilizador?${new URLSearchParams(dados)}`, {
                method: 'POST'
            });

            const resultado = await processarResposta(resposta);
            mostrarFeedback(resultado);
            
            if (resposta.ok) form.reset();

        } catch (erro) {
            mostrarErro(erro.message);
        }
    });
}

// Função para validar os dados do formulário
function validarDadosRegisto(dados) {
    const erros = [];

    // Validação do Nome Completo
    if (!/^[A-Za-zÀ-ÖØ-öø-ÿ\s]+$/.test(dados.nome)) {
        erros.push('O nome deve conter apenas letras e espaços.');
    }

    // Validação da Data de Nascimento
    const dataNascimento = new Date(dados.dataNascimento);
    const dataMinima = new Date('1900-01-01');
    const dataAtual = new Date();
    if (dataNascimento < dataMinima || dataNascimento > dataAtual) {
        erros.push('A data de nascimento deve estar entre 1900-01-01 e a data atual.');
    }

    // Validação do Email
    if (!/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/.test(dados.email)) {
        erros.push('O email deve seguir o formato nome@provedor.dominio.');
    }

    // Validação do Telefone
    if (dados.telefone <= 0) {
        erros.push('O telefone deve ser um número positivo.');
    }

    // Validação do Nome de Utilizador
    if (/\s/.test(dados.username)) {
        erros.push('O nome de utilizador não pode conter espaços.');
    }

    // Validação da Password
    if (dados.palavraPasse.length < 6) {
        erros.push('A password deve ter pelo menos 6 caracteres.');
    }

    if (erros.length > 0) {
        mostrarErro(erros.join('\n'));
        return false;
    }

    return true;
}

// Função para exibir mensagens de erro
function mostrarErro(mensagem) {
    const msgContainer = document.getElementById('mensagem-resposta');
    msgContainer.className = 'erro'; // Adiciona a classe "erro"
    msgContainer.textContent = mensagem;

    // Limpar a mensagem após 5 segundos
    setTimeout(() => {
        msgContainer.textContent = '';
        msgContainer.className = '';
    }, 5000);
}

async function processarResposta(resposta) {
    const tipoConteudo = resposta.headers.get('content-type');
    
    if (!resposta.ok) {
        const texto = await resposta.text();
        return { sucesso: false, mensagem: texto };
    }
    
    return tipoConteudo?.includes('json') 
        ? await resposta.json() 
        : { sucesso: true, mensagem: 'Registo efetuado com sucesso!' };
}

function mostrarFeedback(resultado) {
    const container = document.getElementById('mensagem-resposta');
    container.className = resultado.sucesso ? 'sucesso' : 'erro';
    container.textContent = resultado.mensagem || 'Registo efetuado com sucesso!';
    
    setTimeout(() => {
        container.textContent = '';
        container.className = '';
    }, 5000);
}

document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('carregarRegistarUtilizador').addEventListener('click', (e) => {
        e.preventDefault();
        
        document.querySelectorAll('[id^="main-content"]').forEach(el => {
            el.style.display = 'none';
        });
        
        const secao = document.getElementById('main-content-Registar-utilizador');
        secao.style.display = 'block';
        
        if (!secao.querySelector('.form-container')) {
            criarFormularioRegisto();
        }
    });
});



// Suspender leitores
// Função para carregar leitores suspensos
async function carregarLeitoresSuspensos() {
    try {
        const response = await fetch(`${apiBaseUrl}/Admin_ListarLeitoresInativos`);
        if (!response.ok) throw new Error('Erro ao carregar leitores suspensos');
        
        const leitores = await response.json();
        const selectLeitor = document.getElementById('selectLeitor');
        
        if (selectLeitor) {
            selectLeitor.innerHTML = '<option value="">Selecione um leitor</option>';
            leitores.forEach(leitor => {
                const option = document.createElement('option');
                option.value = leitor.userID;
                option.textContent = `${leitor.nome}`;
                selectLeitor.appendChild(option);
            });
        }
    } catch (error) {
        alert(error.message);
    }
}

// Função para suspender os leitores
function criarInterfaceSuspensaoLeitores() {
    const mainContent = document.getElementById('main-content-Suspender-leitores');
    
    while (mainContent.firstChild) {
        mainContent.removeChild(mainContent.firstChild);
    }

    const container = document.createElement('div');
    container.className = 'form-container';

    const titulo = document.createElement('h1');
    titulo.textContent = 'Suspender Leitores com Devoluções Atrasadas';
    
    const infoContainer = document.createElement('div');
    infoContainer.className = 'alert-info';
    infoContainer.innerHTML = `
        <p>Esta ação suspenderá automaticamente leitores que:</p>
        <ul>
            <li>Possuem exemplares com devolução atrasada</li>
            <li>Estão com mais de 3 dias de atraso</li>
        </ul>
    `;

    const btnAcao = document.createElement('button');
    btnAcao.id = 'btn-suspender-leitores';
    btnAcao.className = 'btn-primary';
    btnAcao.textContent = 'Executar Verificação e Suspensão';

    const resultadoContainer = document.createElement('div');
    resultadoContainer.className = 'resultado-container';
    
    const tabela = document.createElement('table');
    tabela.innerHTML = `
        <thead>
            <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Email</th>
            </tr>
        </thead>
        <tbody></tbody>
    `;

    resultadoContainer.appendChild(tabela);
    container.append(titulo, infoContainer, btnAcao, resultadoContainer);
    mainContent.appendChild(container);

    btnAcao.addEventListener('click', executarSuspensaoLeitores);
}

async function executarSuspensaoLeitores() {
    try {
        const resposta = await fetch(`${apiBaseUrl}/Admin_suspenderAcessoLeitoresDevolucoesAtrasadas`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        const dados = await processarResposta(resposta);
        atualizarInterfaceResultados(dados);

        await carregarLeitoresSuspensos();
        
    } catch (erro) {
        gerenciarErros(erro);
    }
}

async function processarResposta(resposta) {
    const tipoConteudo = resposta.headers.get('content-type');
    
    if (!resposta.ok) {
        const textoErro = await resposta.text();
        throw new Error(tipoConteudo?.includes('json') 
            ? JSON.parse(textoErro).detail 
            : textoErro);
    }

    return tipoConteudo?.includes('json') 
        ? resposta.json() 
        : [];
}

function atualizarInterfaceResultados(dados) {
    const tbody = document.querySelector('#main-content-Suspender-leitores tbody');
    tbody.innerHTML = '';

    if (dados.length === 0) {
        tbody.innerHTML = `<tr><td colspan="4">Nenhum leitor necessita de suspensão</td></tr>`;
        return;
    }

    dados.forEach(leitor => {
        const linha = document.createElement('tr');
        linha.innerHTML = `
            <td>${leitor.userID}</td>
            <td>${leitor.nome}</td>
            <td>${leitor.email}</td>
        `;
        tbody.appendChild(linha);
    });
}

function gerenciarErros(erro) {
    
    const containerErro = document.createElement('div');
    containerErro.className = 'erro-container';
    containerErro.innerHTML = `
        <p class="erro-mensagem">⚠️ ${erro.message}</p>
    `;
    
    document.getElementById('main-content-Suspender-leitores')
            .appendChild(containerErro);
}

//Reativar Leitores
// Função para Reativar leitores
function criarInterfaceReativarLeitores() {
    const mainContent = document.getElementById('main-content-reativar-leitores-inativos');
    
    mainContent.innerHTML = `
        <div class="form-container">
            <h1>Reativar Leitores Suspensos</h1>
            <div class="form-group">
                <label for="selectLeitor">Selecione o Leitor:</label>
                <select id="selectLeitor" class="form-control"></select>
            </div>
            <button id="btnReativarLeitor" class="btn-primary">Reativar Leitor</button>
            <div id="resultado-reativacao" class="alert" style="display: none; margin-top: 15px;"></div>
        </div>
    `;

    const selectLeitor = document.getElementById('selectLeitor');
    const btnReativar = document.getElementById('btnReativarLeitor');
    const resultadoDiv = document.getElementById('resultado-reativacao');

    btnReativar.addEventListener('click', async () => {
        const leitorId = selectLeitor.value;
        
        if (!leitorId) {
            mostrarResultado('Selecione um leitor primeiro!', 'warning');
            return;
        }
    
        try {
            const response = await fetch(`${apiBaseUrl}/Admin_reativarLeitor`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(parseInt(leitorId))
            });
    
            const contentType = response.headers.get('content-type');
            if (!contentType || !contentType.includes('application/json')) {
                const text = await response.text();
                throw new Error(`Resposta inválida: ${text.substring(0, 100)}`);
            }
    
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data.error || 'Erro na reativação');
            }
    
            mostrarResultado(`Leitor reativado com sucesso!`, 'success');
            await carregarLeitoresSuspensos();
        } catch (error) {
            mostrarResultado(error.message, 'error');
        }
    });

    function mostrarResultado(mensagem, tipo) {
        resultadoDiv.style.display = 'block';
        resultadoDiv.textContent = mensagem;
        resultadoDiv.className = `alert ${tipo}`;
        
        setTimeout(() => {
            resultadoDiv.style.display = 'none';
        }, 5000);
    }

    carregarLeitoresSuspensos();
}

//Eliminar leitores inativos:
function criarInterfaceEliminarLeitores() {
    const mainContent = document.getElementById('main-content-eliminar-leitores-inativos');
    
    mainContent.innerHTML = '';
    
    const container = document.createElement('div');
    container.className = 'form-container';
    
    const titulo = document.createElement('h1');
    titulo.textContent = 'Eliminar Leitores Inativos';
    
    const alerta = document.createElement('div');
    alerta.className = 'alert-info';
    alerta.innerHTML = `
        <p>Serão eliminados automaticamente os leitores que:</p>
        <ul>
            <li>Não fizeram requisições nos últimos 12 meses</li>
            <li>Não têm requisições ativas ou pendentes</li>
        </ul>
    `;
    
    const botao = document.createElement('button');
    botao.id = 'btnEliminarLeitores';
    botao.className = 'btn-primary';
    botao.textContent = 'Verificar e Eliminar Leitores Inativos';
    
    const resultadoContainer = document.createElement('div');
    resultadoContainer.id = 'resultado-eliminacao';
    resultadoContainer.style.marginTop = '20px';
    
    const tabela = document.createElement('table');
    tabela.id = 'tabela-leitores-eliminados';
    tabela.innerHTML = `
        <thead>
            <tr>
                <th>ID</th>
                <th>Nome</th>
                <th>Email</th>
            </tr>
        </thead>
        <tbody></tbody>
    `;
    
    resultadoContainer.appendChild(tabela);
    container.append(titulo, alerta, botao, resultadoContainer);
    mainContent.appendChild(container);
    
    botao.addEventListener('click', async () => {
        try {
            const response = await fetch(`${apiBaseUrl}/Admin_EliminarLeitoresInativos`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
            
            const data = await response.json();
            
            if (!response.ok) throw new Error(data.detail || 'Erro na operação');
            
            const tbody = tabela.querySelector('tbody');
            tbody.innerHTML = '';
            
            if (data.length === 0) {
                tbody.innerHTML = '<tr><td colspan="3">Nenhum leitor foi eliminado</td></tr>';
                return;
            }
            
            data.forEach(leitor => {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${leitor.userID}</td>
                    <td>${leitor.nome}</td>
                    <td>${leitor.email}</td>
                `;
                tbody.appendChild(row);
            });
            
        } catch (error) {
            alert(error.message);
        }
    });
}

// Mostrar Obras
document.getElementById('carregarObras').addEventListener('click', async () => {
    try {
        const response = await fetch(`${apiBaseUrl}/MostrarTodasObras`);
        if (!response.ok) throw new Error('Erro ao carregar as obras');
        const obras = await response.json();
        mostrarobrasmenu()

        const listaObras = document.getElementById('main-content');
        listaObras.innerHTML = '<div class="obra2"><h1>Obras Disponíveis</h1></br></br><p>Consulte, altere ou remova as suas obras.</div>';

        obras.forEach(obra => {
            const obraDiv = document.createElement('div');
            obraDiv.className = 'obra';

            const titulo = document.createElement('h2');
            titulo.textContent = obra.titulo;
            obraDiv.appendChild(titulo);

            const autor = document.createElement('p');
            autor.textContent = `Autor: ${obra.autor}`;
            obraDiv.appendChild(autor);

            const anoPublicacao = document.createElement('p');
            anoPublicacao.textContent = `Ano: ${obra.anoPublicacao || 'N/A'}`;
            obraDiv.appendChild(anoPublicacao);

            const genero = document.createElement('p');
            genero.textContent = `Gênero: ${obra.genero}`;
            obraDiv.appendChild(genero);

            const desc = document.createElement('p');
            desc.textContent = `${obra.descricao}`;
            obraDiv.appendChild(desc);

            if (obra.imagemBase64) {
                const imagem = document.createElement('img');
                imagem.src = `data:image/jpeg;base64,${obra.imagemBase64}`;
                obraDiv.appendChild(imagem);
            }

            const botoesDiv = document.createElement('div');
            botoesDiv.className = 'botoesDiv';

            const botaoEditar = document.createElement('button');
            botaoEditar.textContent = 'Editar';
            botaoEditar.className = 'editar';
            botaoEditar.onclick = () => mostrarFormularioEditar(obra);
            botoesDiv.appendChild(botaoEditar);

            const botaoRemover = document.createElement('button');
            botaoRemover.textContent = 'Remover';
            botaoRemover.className = 'remover';
            botaoRemover.onclick = () => {
                if (obra.obraID) {
                    removerObra(obra.obraID);
                } else {
                    alert('Erro: ID da obra não está definido.');
                }
            };
            botoesDiv.appendChild(botaoRemover);

            obraDiv.appendChild(botoesDiv);
            listaObras.appendChild(obraDiv);
        });
    } catch (error) {
        alert('Erro ao carregar as obras');
    }
});

// Remover obras 
// Função para remover uma obra
async function removerObra(obraID) {
    const modalOverlay = document.createElement('div');
    modalOverlay.className = 'modal-overlay';

    const modalContent = document.createElement('div');
    modalContent.className = 'modal-content';

    const message = document.createElement('p');
    message.textContent = 'Tem a certeza que quer eliminar esta obra?';
    modalContent.appendChild(message);

    const modalButtons = document.createElement('div');
    modalButtons.className = 'modal-buttons';

    const confirmButton = document.createElement('button');
    confirmButton.className = 'confirm-button';
    confirmButton.textContent = 'Confirmar';

    const cancelButton = document.createElement('button');
    cancelButton.className = 'cancel-button';
    cancelButton.textContent = 'Cancelar';

    modalButtons.appendChild(confirmButton);
    modalButtons.appendChild(cancelButton);
    modalContent.appendChild(modalButtons);
    modalOverlay.appendChild(modalContent);

    document.body.appendChild(modalOverlay);

    confirmButton.addEventListener('click', async () => {
        try {
            const response = await fetch(`${apiBaseUrl}/ApagarObra?obraID=${obraID}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
            });

            const mensagem = await response.text();
            const mensagemSemAspas = mensagem.replace(/['"]+/g, '');

        if (response.ok) {
            alert(mensagemSemAspas);
            carregarObras();
        } else {
            alert(mensagemSemAspas);
        }
    } catch (error) {
        alert("Erro ao tentar remover a obra: " + error.message);
    } finally {
        document.body.removeChild(modalOverlay);
    }
});
    cancelButton.addEventListener('click', () => {
        document.body.removeChild(modalOverlay);
    });
}

// Editar Obras
async function mostrarFormularioEditar(obra) {
    try {
        const modal = document.getElementById('modalEditar');
        const response = await fetch(`${apiBaseUrl}/ObterDadosObra?obraID=${obra.obraID}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        });
        
        if (!response.ok) throw new Error('Erro ao obter dados da obra');
        const obraData = await response.json();

        // Preencher os campos do formulário
        document.getElementById('obraID').value = obraData.obraID;
        document.getElementById('editarTitulo').value = obraData.titulo;
        document.getElementById('editarAutor').value = obraData.autor;
        document.getElementById('editarAno').value = obraData.anoPublicacao || '';
        document.getElementById('editarGenero').value = obraData.genero;
        document.getElementById('editarDescricao').value = obraData.descricao;

        // Mostrar imagem atual se existir
        const previewImagem = document.getElementById('previewImagem');
        if (obraData.imagemBase64) {
            previewImagem.style.display = 'block';
            previewImagem.src = `data:image/jpeg;base64,${obraData.imagemBase64}`;
        } else {
            previewImagem.style.display = 'none';
        }

        modal.style.display = 'block';
    } catch (error) {
        console.error('Erro:', error);
        alert('Erro ao carregar dados para edição');
    }
}

// Fechar modal ao clicar no × ou fora
document.querySelector('.fechar').onclick = fecharModal;
window.onclick = function(event) {
    const modal = document.getElementById('modalEditar');
    if (event.target === modal) {
        fecharModal();
    }
}

function fecharModal() {
    document.getElementById('modalEditar').style.display = 'none';
}

document.getElementById('formEditarObra').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    try {
        const obraID = document.getElementById('obraID').value;
        const imagemInput = document.getElementById('editarImagem');
        let imagemBase64 = '';

        // Função para limpar espaços duplos e trim nos campos de texto
        function limparEspacos(texto) {
            return texto.replace(/\s+/g, ' ').trim();
        }

        // Captura e limpa os dados do formulário
        const obraDto = {
            obraID: parseInt(obraID),
            titulo: limparEspacos(document.getElementById('editarTitulo').value),
            autor: limparEspacos(document.getElementById('editarAutor').value),
            anoPublicacao: document.getElementById('editarAno').value || null,
            genero: limparEspacos(document.getElementById('editarGenero').value),
            descricao: limparEspacos(document.getElementById('editarDescricao').value),
            imagemBase64: ''
        };

        if (obraDto.anoPublicacao !== null && (isNaN(obraDto.anoPublicacao) || obraDto.anoPublicacao <= 0)) {
            alert('O Ano de Publicação deve ser um número maior que 0.');
            return;
        }

        // Converter nova imagem para Base64 se for carregada
        if (imagemInput && imagemInput.files[0]) {
            imagemBase64 = await new Promise((resolve) => {
                const reader = new FileReader();
                reader.onload = (e) => resolve(e.target.result.split(',')[1]);
                reader.readAsDataURL(imagemInput.files[0]);
            });
        } else {
            // Manter imagem existente se não for carregada nova
            const previewImagem = document.getElementById('previewImagem');
            if (previewImagem && previewImagem.src.startsWith('data:')) {
                imagemBase64 = previewImagem.src.split(',')[1];
            }
        }

        obraDto.imagemBase64 = imagemBase64;

        const response = await fetch(`${apiBaseUrl}/AtualizarObra`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(obraDto)
        });

        if (!response.ok) throw new Error('Erro ao atualizar obra');

        fecharModal();
        carregarObras();
        alert('Obra atualizada com sucesso!');
    } catch (error) {
        console.error('Erro:', error);
        alert(error.message);
    }
});

// Adiciona eventos para garantir que os campos sejam corrigidos enquanto o usuário digita
document.getElementById('editarTitulo').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

// Impede caracteres especiais e números no campo Autor
document.getElementById('editarAutor').addEventListener('input', function () {
    this.value = this.value
        .replace(/[^A-Za-zÀ-ÖØ-öø-ÿ.\s]/g, '') // Permite letras, acentos, ponto e espaços
        .replace(/\s{2,}/g, ' ')
        .replace(/\.{2,}/g, '.')
        .trimStart(); // Impede espaços no início
});

document.getElementById('editarGenero').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

document.getElementById('editarDescricao').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

// Garante que o ano de publicação nunca seja negativo ou zero
document.getElementById('editarAno').addEventListener('input', function () {
    if (this.value < 1) {
        this.value = '';
    }
    const anoAtual = new Date().getFullYear(); // Obtém o ano atual
    // Garante que o input tenha no máximo 4 caracteres
    this.value = this.value.slice(0, 4);

    // Converte para número
    const ano = parseInt(this.value, 10);

    // Se for menor ou igual a 0 ou maior que o ano atual, exibe um aviso
    if (isNaN(ano) || ano <= 0 || ano > anoAtual) {
        this.setCustomValidity('Ano inválido! Deve estar entre 1 e ' + anoAtual + '.');
    } else {
        this.setCustomValidity('');
    }    
});

//Adicionar obra
document.getElementById('formAdicionarObra').addEventListener('submit', async (e) => {
    e.preventDefault();

    function limparEspacos(texto) {
        return texto.replace(/\s+/g, ' ').trim();
    }

    // Captura os dados do formulário
    const formData = {
        Titulo: document.getElementById('titulo').value,
        Autor: document.getElementById('autor').value,
        AnoPublicacao: parseInt(document.getElementById('anoPublicacao').value),
        Genero: document.getElementById('genero').value,
        Descricao: document.getElementById('descricao').value,
        ImagemBase64: null 
    };

    if (isNaN(formData.AnoPublicacao) || formData.AnoPublicacao <= 0) {
        alert('O Ano de Publicação deve ser um número maior que 0.');
        return;
    }

    const fileInput = document.getElementById('imagem');
    const file = fileInput.files[0];

    if (file) {
        const reader = new FileReader();
        reader.onload = function (e) {
            formData.ImagemBase64 = e.target.result;

            enviarDados(formData);
        };
        reader.readAsDataURL(file);
    } else {
        // Envia os dados sem imagem
        enviarDados(formData);
    }
});

// Adiciona eventos para garantir que os campos sejam corrigidos enquanto o usuário digita
document.getElementById('titulo').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

document.getElementById('autor').addEventListener('input', function () {
    this.value = this.value
        .replace(/[^A-Za-zÀ-ÖØ-öø-ÿ.\s]/g, '') // Permite letras, acentos, ponto e espaços
        .replace(/\s{2,}/g, ' ')
        .replace(/\.{2,}/g, '.')
        .trimStart(); // Impede espaços no início
});

document.getElementById('genero').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

document.getElementById('descricao').addEventListener('input', function () {
    this.value = this.value.replace(/\s+/g, ' ').trimStart();
});

// Garante que o ano de publicação nunca seja negativo ou zero
document.getElementById('anoPublicacao').addEventListener('input', function () {
    const anoAtual = new Date().getFullYear();
    const mensagemErro = document.getElementById('erroAnoPublicacao');
    if (!mensagemErro) {
        const spanErro = document.createElement('span');
        spanErro.id = 'erroAnoPublicacao';
        spanErro.style.color = 'red';
        spanErro.style.fontSize = '0.9em';
        spanErro.style.marginLeft = '10px';
        this.parentNode.appendChild(spanErro);
    }

    this.value = this.value.slice(0, 4);

    const ano = parseInt(this.value, 10);
    const erroElemento = document.getElementById('erroAnoPublicacao');

    if (isNaN(ano) || ano <= 0 || ano > anoAtual) {
        erroElemento.textContent = 'Ano inválido! Deve estar entre 1 e ' + anoAtual + '.';
    } else {
        erroElemento.textContent = '';
    }
});

async function enviarDados(formData) {
    try {
        const response = await fetch(`${apiBaseUrl}/AdicionarObra`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        });

        if (!response.ok) throw new Error('Erro ao adicionar obra');

        alert('Obra adicionada com sucesso!');
        cancelarEdicao();
        document.getElementById('carregarObras').click();
    } catch (error) {
        alert('Erro ao adicionar obra: ' + error.message);
    }
}

function cancelarEdicao() {
    document.getElementById('formAdicionarObra').reset();
    document.getElementById('main-content-adicionarobra').style.display = 'none';
}

// Transferir exemplares
async function carregarDadosTransferencia() {
    try {
        const responseObras = await fetch(`${apiBaseUrl}/ObterObras`);
        if (!responseObras.ok) throw new Error('Erro ao carregar obras');
        const obras = await responseObras.json();

        const responseNucleos = await fetch(`${apiBaseUrl}/ObterNucleos`);
        if (!responseNucleos.ok) throw new Error('Erro ao carregar núcleos');
        const nucleos = await responseNucleos.json();

        const responseExemplares = await fetch(`${apiBaseUrl}/ObterExemplaresPorNucleo`);
        if (!responseExemplares.ok) throw new Error('Erro ao carregar exemplares');
        const exemplares = await responseExemplares.json();

        const selectObra = document.getElementById('obra');
        selectObra.innerHTML = '<option value="">Selecione uma obra</option>';
        obras.forEach(obra => {
            const option = document.createElement('option');
            option.value = obra.obraID;
            option.textContent = obra.titulo;
            selectObra.appendChild(option);
        });

        const preencherNucleos = (selectElement) => {
            selectElement.innerHTML = '<option value="">Selecione um núcleo</option>';
            nucleos.forEach(nucleo => {
                const option = document.createElement('option');
                option.value = nucleo.nucleoID;
                option.textContent = nucleo.nome;
                selectElement.appendChild(option);
            });
        };

        preencherNucleos(document.getElementById('origemNucleo'));
        preencherNucleos(document.getElementById('destinoNucleo'));

        const tbody = document.querySelector('#tabelaObras tbody');
        tbody.innerHTML = '';
        exemplares.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.titulo}</td>
                <td>${item.nomeNucleo}</td>
                <td>${item.quantidade}</td>
            `;
            tbody.appendChild(row);
        });

    } catch (error) {
        alert(error.message);
    }
}

document.getElementById('formTransferirExemplares').addEventListener('submit', async (e) => {
    e.preventDefault();

    const obraId = document.getElementById('obra').value;
    const origemNucleoId = document.getElementById('origemNucleo').value;
    const destinoNucleoId = document.getElementById('destinoNucleo').value;
    const quantidade = document.getElementById('quantidade').value;

    if (!obraId || !origemNucleoId || !destinoNucleoId || !quantidade) {
        alert("Todos os campos são obrigatórios!");
        return;
    }
    try {
        const url = `${apiBaseUrl}/Admin_TransferirExemplar?` + 
                    `obraId=${encodeURIComponent(obraId)}&` +
                    `origemNucleoId=${encodeURIComponent(origemNucleoId)}&` +
                    `destinoNucleoId=${encodeURIComponent(destinoNucleoId)}&` +
                    `qtdTransferir=${encodeURIComponent(quantidade)}`;

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'accept': '*/*'
            },
        });
        
        if (!response.ok) {
            throw new Error(`Erro na requisição: ${response.statusText}`);
        }

        const result = await response.json();

        if (!result.sucesso) {
            throw new Error(result.mensagem || 'Erro na transferência');
        }

        alert(result.mensagem);

        await carregarDadosTransferencia();

        document.getElementById('formTransferirExemplares').reset();

    } catch (error) {
        alert(error.message);
    }
});

function cancelarTransferencia() {
    document.getElementById('formTransferirExemplares').reset();
    document.getElementById('main-content-transferirexemplares').style.display = 'none';
}

document.getElementById('carregarTransferirExemplares').addEventListener('click', () => {
    document.querySelectorAll('[id^="main-content"]').forEach(el => {
        el.style.display = 'none';
    });
    
    const transferSection = document.getElementById('main-content-transferirexemplares');
    transferSection.style.display = 'block';
    
    carregarDadosTransferencia();
});

//Adicionar Exemplares
async function carregarObrasParaAdicionarExemplares() {
    try {
        const response = await fetch(`${apiBaseUrl}/MostrarTodasObras`);
        if (!response.ok) throw new Error('Erro ao carregar as obras');
        const obras = await response.json();

        const selectObra = document.getElementById('obra1');
        selectObra.innerHTML = '<option value="">Selecione uma obra</option>';

        obras.forEach(obra => {
            const option = document.createElement('option');
            option.value = obra.obraID;
            option.textContent = obra.titulo;
            selectObra.appendChild(option);
        });
    } catch (error) {
        alert('Erro ao carregar as obras');
    }
}

document.getElementById('formAdicionarExemplares').addEventListener('submit', async (e) => {
    e.preventDefault();

    const obraId = document.getElementById('obra1').value;
    const quantidade = document.getElementById('quantidade1').value;

    if (!obraId || !quantidade) {
        alert("Todos os campos são obrigatórios!");
        return;
    }

    try {
        const response = await fetch(`${apiBaseUrl}/AdicionarExemplaresAoNucleoPrincipal?obraID=${obraId}&qtdAdicionar=${quantidade}`, {
            method: 'POST',
            headers: {
                'accept': '*/*',
            },
        });

        if (!response.ok) {
            throw new Error(`Erro na requisição: ${response.statusText}`);
        }

        alert('Exemplares adicionados com sucesso!');
        document.getElementById('formAdicionarExemplares').reset(); // Limpa o formulário
    } catch (error) {
        alert(error.message);
    }
});

function cancelarAdicaoExemplares() {
    document.getElementById('formAdicionarExemplares').reset();
}

function mostrarAdicionarExemplares() {
    document.querySelectorAll('[id^="main-content"]').forEach(el => el.style.display = 'none');
    document.getElementById('main-content-adicionar-exemplares').style.display = 'block';
    carregarObrasParaAdicionarExemplares();
}

//Relatorio ultimas requisições
document.getElementById('carregarUltimasRequisicoes').addEventListener('click', async () => {
    try {

        document.getElementById('main-content').style.display = 'none';
        document.getElementById('main-content-adicionarobra').style.display = 'none';
        document.getElementById('main-content-transferirexemplares').style.display = 'none';
        document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
        document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
        document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
        document.getElementById('main-content-Registar-utilizador').style.display = 'none';
        document.getElementById('main-content-Suspender-leitores').style.display = 'none';
        document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
        document.getElementById('main-content-relatorio-top10').style.display = 'none';
        document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'block';

        const response = await fetch(`${apiBaseUrl}/Admin_ObterUltimaRequisicaoPorNucleo`);
        if (!response.ok) throw new Error('Erro ao carregar o relatório');

        const data = await response.json();

        const relatorioDiv = document.getElementById('main-content-relatorio-requisicoes-nucleos');
        relatorioDiv.innerHTML = '<h1>Últimas Requisições por Núcleo</h1>';

        const tabela = document.createElement('table');
        tabela.innerHTML = `
            <thead>
                <tr>
                    <th>Núcleo</th>
                    <th>Última Requisição</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        `;

        const tbody = tabela.querySelector('tbody');
        data.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.nomeNucleo}</td>
                <td>${item.ultimaRequisicao}</td>
            `;
            tbody.appendChild(row);
        });
        relatorioDiv.appendChild(tabela);
    } catch (error) {
        alert(error.message);
    }
});

//Relatorio top10 obras mais requisitadas ultimo ano
document.getElementById('carregarTop10Obras').addEventListener('click', async () => {
    try {
        document.getElementById('main-content').style.display = 'none';
        document.getElementById('main-content-adicionarobra').style.display = 'none';
        document.getElementById('main-content-transferirexemplares').style.display = 'none';
        document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
        document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
        document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
        document.getElementById('main-content-Registar-utilizador').style.display = 'none';
        document.getElementById('main-content-Suspender-leitores').style.display = 'none';
        document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
        document.getElementById('main-content-relatorio-top10').style.display = 'block';

        const response = await fetch(`${apiBaseUrl}/Admin_Top10ObrasRequisitadasUltimoAno`);
        if (!response.ok) throw new Error('Erro ao carregar o relatório');

        const data = await response.json();

        if (!data.sucesso) {
            throw new Error(data.mensagem || 'Erro ao carregar o relatório');
        }

        const relatorioDiv = document.getElementById('main-content-relatorio-top10');
        relatorioDiv.innerHTML = '<h1>Top 10 Obras Mais Requisitadas no Último Ano</h1>';

        const tabela = document.createElement('table');
        tabela.innerHTML = `
            <thead>
                <tr>
                    <th>Título</th>
                    <th>Total de Requisições</th>
                </tr>
            </thead>
            <tbody>
            </tbody>
        `;

        const tbody = tabela.querySelector('tbody');
        data.obras.forEach(obra => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${obra.titulo}</td>
                <td>${obra.totalRequisicoes}</td>
            `;
            tbody.appendChild(row);
        });

        relatorioDiv.appendChild(tabela);
    } catch (error) {
        alert(error.message);
    }
});


// Menu lateral
document.addEventListener("DOMContentLoaded", function() {
    var dropdownButtons = document.querySelectorAll(".dropdown-btn");
    dropdownButtons.forEach(function(button) {
        button.addEventListener("click", function() {
            var dropdownContent = this.nextElementSibling;
            if (dropdownContent.style.display === "block") {
                dropdownContent.style.display = "none";
            } else {
                dropdownContent.style.display = "block";
            }
        });
    });

    var dropdownItems = document.querySelectorAll(".dropdown-container a");
    dropdownItems.forEach(function(item) {
        item.addEventListener("click", function(event) {
            event.preventDefault();
        });
    });
});

//Mostrar página inicial
document.getElementById("logo").addEventListener("click", function () {
    let mainContent = document.getElementById("main-content2");
    mainContent.style.display = "block";
    document.getElementById('main-content').style.display = 'none';
    document.getElementById('main-content-adicionarobra').style.display = 'none';
    document.getElementById('main-content-transferirexemplares').style.display = 'none';
    document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
    document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
    document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
    document.getElementById('main-content-Registar-utilizador').style.display = 'none';
    document.getElementById('main-content-Suspender-leitores').style.display = 'none';
    document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
    document.getElementById('main-content-relatorio-top10').style.display = 'none';
});

let menuLinks = document.querySelectorAll('.dropdown-container a');

menuLinks.forEach(function(link) {
    link.addEventListener("click", function () {
        let mainContent = document.getElementById("main-content2");
        mainContent.style.display = "none";
    });
});

// Mostrar todas as obras Menu
function mostrarobrasmenu() {
    document.querySelectorAll('[id^="main-content"]').forEach(el => el.style.display = 'grid');
    document.getElementById('main-content-relatorio-top10').style.display = 'none';
    document.getElementById('main-content-adicionarobra').style.display = 'none';
    document.getElementById('main-content-transferirexemplares').style.display = 'none';
    document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
    document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
    document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
    document.getElementById('main-content-Registar-utilizador').style.display = 'none';
    document.getElementById('main-content-Suspender-leitores').style.display = 'none';
    document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
    document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
    document.getElementById('main-content2').style.display = 'none';
}

// Adicionar obra Menu
function mostrarFormularioAdicionar() {
    document.querySelectorAll('[id^="main-content"]').forEach(el => el.style.display = 'none');
    document.getElementById('main-content-relatorio-top10').style.display = 'none';
    document.getElementById('main-content-transferirexemplares').style.display = 'none';
    document.getElementById('main-content-adicionarobra').style.display = 'block';
    document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
    document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
    document.getElementById('main-content-Registar-utilizador').style.display = 'none';
    document.getElementById('main-content-Suspender-leitores').style.display = 'none';
    document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
    document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
}

// Suspender Leitores Menu
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('carregarSuspenderLeitores').addEventListener('click', (e) => {
        e.preventDefault();
        
        document.querySelectorAll('[id^="main-content"]').forEach(el => {
            el.style.display = 'none';
            document.getElementById('main-content-relatorio-top10').style.display = 'none';
            document.getElementById('main-content-transferirexemplares').style.display = 'none';
            document.getElementById('main-content-adicionarobra').style.display = 'none';
            document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
            document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
            document.getElementById('main-content-Registar-utilizador').style.display = 'none';
            document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
            document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
        });
        
        const secao = document.getElementById('main-content-Suspender-leitores');
        secao.style.display = 'block';
        
        if (!secao.querySelector('.form-container')) {
            criarInterfaceSuspensaoLeitores();
        }
    });
});

// Reativar Leitor Menu
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('carregarReativarLeitores').addEventListener('click', async (e) => {
        e.preventDefault();
        document.querySelectorAll('[id^="main-content"]').forEach(el => el.style.display = 'none');
        const section = document.getElementById('main-content-reativar-leitores-inativos');
        section.style.display = 'block';
        document.getElementById('main-content-relatorio-top10').style.display = 'none';
        document.getElementById('main-content-transferirexemplares').style.display = 'none';
        document.getElementById('main-content-adicionarobra').style.display = 'none';
        document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
        document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
        document.getElementById('main-content-Registar-utilizador').style.display = 'none';
        document.getElementById('main-content-Suspender-leitores').style.display = 'none';
        document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'none';
        
        if (section.querySelector('.form-container')) {
            await carregarLeitoresSuspensos();
        } else {
            criarInterfaceReativarLeitores();
        }
    });
});

// Eliminar leitores inativos Menu
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('carregarEliminarLeitoresInativos').addEventListener('click', (e) => {
        e.preventDefault();
        document.querySelectorAll('[id^="main-content"]').forEach(el => el.style.display = 'none');
        document.getElementById('main-content-eliminar-leitores-inativos').style.display = 'block';
        document.getElementById('main-content-relatorio-top10').style.display = 'none';
        document.getElementById('main-content-transferirexemplares').style.display = 'none';
        document.getElementById('main-content-adicionarobra').style.display = 'none';
        document.getElementById('main-content-relatorio-requisicoes-nucleos').style.display = 'none';
        document.getElementById('main-content-adicionar-exemplares').style.display = 'none';
        document.getElementById('main-content-Registar-utilizador').style.display = 'none';
        document.getElementById('main-content-Suspender-leitores').style.display = 'none';
        document.getElementById('main-content-reativar-leitores-inativos').style.display = 'none';
        
        if (!document.getElementById('tabela-leitores-eliminados')) {
            criarInterfaceEliminarLeitores();
        }
    });
});

//Botão de sair Menu
document.addEventListener("DOMContentLoaded", function() {
    document.getElementById("btnSair").addEventListener("click", function() {
        window.location.href = "http://biblioteca.test";
    });
});

// Função para recarregar a lista de obras
function carregarObras() {
    document.getElementById('carregarObras').click();
}