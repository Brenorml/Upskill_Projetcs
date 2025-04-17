localStorage.setItem("db", JSON.stringify(db));
let str = localStorage.getItem("db");
let cursos = JSON.parse(str);
let paginaAtual = 1; //Inicio da paginação
const itensPorPagina = 6; // Número de cursos por página

//lista todos os cursos invocada na função init para iniciar a pagina com a lista de todos os cursos
function listarCursos() {
    paginaAtual = 1;
    paginarCursos(cursos);
}

//Função para adicionar e remover o ISBN do curso no DB Favoritos
function toggleFavorito(isbnCurso) {
    let favoritos = JSON.parse(localStorage.getItem('favoritos')) || [];

    // Certifique-se de que o ISBN está sendo tratado como string
    isbnCurso = isbnCurso.toString();

    if (favoritos.includes(isbnCurso)) {
        // Remove o curso dos favoritos
        favoritos = favoritos.filter(fav => fav !== isbnCurso);
    } else {
        // Adiciona o curso aos favoritos
        favoritos.push(isbnCurso);
    }

    // Atualiza o localStorage com os favoritos
    localStorage.setItem('favoritos', JSON.stringify(favoritos));

    // Atualiza o ícone de favorito
    atualizarIconeFavorito(isbnCurso);

    // Atualiza a lista de favoritos exibida, se estiver mostrando favoritos
    if (document.getElementById('categoria').value === 'favoritos') {
        mostrarFavoritos();
    }
}

//Verifica o favorito para mudar a classe e fazer a interação no coração favorito.
function atualizarIconeFavorito(isbnCurso) {
    const icon = document.querySelector(`.fav-icon[data-isbn="${isbnCurso}"]`);
    let favoritos = JSON.parse(localStorage.getItem('favoritos')) || [];

    // Certifique-se de comparar strings
    isbnCurso = isbnCurso.toString();

    if (favoritos.includes(isbnCurso)) {
        // Adiciona a classe de favoritado
        icon.classList.add('favoritado');
    } else {
        // Remove a classe de favoritado
        icon.classList.remove('favoritado');
    }
}

//lista todos os cursos invocada na função init para iniciar a pagina com a lista de todos os cursos
function mostrarFavoritos() {
    paginaAtual = 1;
    let favoritos = JSON.parse(localStorage.getItem('favoritos')) || [];
    let cursosFavoritos = cursos.filter(curso => favoritos.includes(curso.ISBN));
    paginarCursos(cursosFavoritos);
}

//função para listar os 5 cursos mais vendidos conforme rating.
function mostrarMaisVendidos() {
    paginaAtual = 1;

    //Filtrar e ordenar os cursos pelo rating, do maior para o menor
    let cursosOrdenados = cursos.sort((a, b) => b.rating - a.rating);

    //Selecionar os 5 primeiros cursos (ou menos, caso tenha menos de 5)
    let top5Cursos = cursosOrdenados.slice(0, 5);

    paginarCursos(top5Cursos);
}

//função para listar os cursos por categoria escolhida no select
function mostrarPorTipo(tipo) {
    paginaAtual = 1;
    let cursosExistentes = []; // Armazenar apenas cursos da categoria

    for (let i = 0; i < cursos.length; i++) {
        if (cursos[i].categoria === tipo) {
            cursosExistentes.push(cursos[i]);
        }
    }
    paginarCursos(cursosExistentes);
}


function pesquisarCursos() {
    paginaAtual = 1;
    const query = document.getElementById('buscador').value.toLowerCase();

    // Função para remover acentos e normalizar texto
    const removeDiacritics = (str) =>
        str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");

    // Normaliza a consulta de pesquisa
    const normalizedQuery = removeDiacritics(query);

    const cursosFiltrados = cursos.filter(curso => {
        return (
            removeDiacritics(curso.titulo.toLowerCase()).includes(normalizedQuery) ||
            removeDiacritics(curso.autor.toLowerCase()).includes(normalizedQuery) ||
            removeDiacritics(curso.categoria.toLowerCase()).includes(normalizedQuery)
        );
    });
    paginarCursos(cursosFiltrados); // Chama a função de paginação
}

function paginarCursos(cursosParaPaginar) {
    const totalPaginas = Math.ceil(cursosParaPaginar.length / itensPorPagina);
    const inicio = (paginaAtual - 1) * itensPorPagina;
    const fim = inicio + itensPorPagina;
    let contador = 0;
    // Limpa a lista atual
    document.getElementById('lista-cursos').innerHTML = '';

    // Adiciona cursos da página atual
    let linhaAtual = '<div class="row">';
    for (let i = inicio; i < fim && i < cursosParaPaginar.length; i++) {
        let classPreco = "";
        //alert(cursosParaPaginar[i].promocao + " " + typeof cursosParaPaginar[i].promocao)
        //condição para se o valor for 0, null ou undefined usar a classe preco-promo que não coloca a linha sobre o valor e não mostra valor na promocao
        if (cursosParaPaginar[i].promocao > 0 && cursosParaPaginar[i].promocao != null && cursosParaPaginar[i].promocao != undefined) {
            classPreco = "preco";
        }
        else {
            classPreco = "preco-promo"
            cursosParaPaginar[i].promocao = "";

        }
        linhaAtual += `
            <div class="four columns">
                <div class="curso" data-isbn="${cursosParaPaginar[i].ISBN}">
                    <div class="card">
                        <img src="img/${cursosParaPaginar[i].imagem}" onclick="javascript:goToDetail(${cursosParaPaginar[i].ISBN})" class="imagen-curso u-full-width">
                        <div class="info-card">
                            <h4>${cursosParaPaginar[i].titulo}</h4>
                            <p>${cursosParaPaginar[i].autor}</p>
                            <img src="img/estrelas.png">
                            <p class="${classPreco}">${cursosParaPaginar[i].preco} <span class="u-pull-right">${cursosParaPaginar[i].promocao}</span></p>
                            <a href="#" class="u-full-width button-primary button input adicionar-carrinho" data-isbn="${cursosParaPaginar[i].ISBN}">Adicionar ao Carrinho</a>
                            <span class="fav-icon" data-isbn="${cursosParaPaginar[i].ISBN}" onclick="toggleFavorito(${cursosParaPaginar[i].ISBN})">
                            &#9829;
                            </span>
                        </div>
                    </div>
                </div>
            </div>`;

        contador++;

        if (contador % 3 === 0) {
            linhaAtual += '</div>';
            if (i < cursos.length - 1) {
                linhaAtual += '<div class="row">';
            }
        }
    }
    if (linhaAtual !== '<div class="row">') {
        linhaAtual += '</div>';
    }
    // Atualiza a visualização
    document.getElementById('lista-cursos').innerHTML = linhaAtual;

    // Atualiza ícones de favoritos
    for (let i = inicio; i < fim && i < cursosParaPaginar.length; i++) {
        atualizarIconeFavorito(cursosParaPaginar[i].ISBN);
    }
    // Cria botões de navegação
    criarBotoesNavegacao(totalPaginas, cursosParaPaginar);
}

function criarBotoesNavegacao(totalPaginas, arrPaginada) {
    const navContainer = document.getElementById('nav-paginacao');
    navContainer.innerHTML = ''; // Limpa navegação anterior

    for (let i = 1; i <= totalPaginas; i++) {
        const button = document.createElement('button');
        button.innerText = i;
        button.classList.add('pagina-btn');
        button.addEventListener('click', () => {
            paginaAtual = i;
            paginarCursos(arrPaginada); // Chama a função de paginação com a lista completa
        });
        navContainer.appendChild(button);
    }
}

function goToDetail(isbn) {
    isbn = isbn.toString().trim();

    for (let i = 0; i < cursos.length; i++) {
        if (isbn === cursos[i].ISBN) {
            criarPopupDetails(cursos[i]);
        }
    }

}

function criarPopupDetails(dadosCurso) {
    return new Promise((resolve) => {
        // Cria os elementos do modal
        const modal = document.createElement('div');
        modal.style.position = 'fixed';
        modal.style.top = '0';
        modal.style.left = '0';
        modal.style.width = '100%';
        modal.style.height = '100%';
        modal.style.backgroundColor = 'rgba(0, 0, 0, 0.7)';
        modal.style.display = 'flex';
        modal.style.justifyContent = 'center';
        modal.style.alignItems = 'center';
        modal.style.zIndex = '1000';

        const container = document.createElement('div');
        container.style.background = 'white';
        container.style.padding = '20px';
        container.style.borderRadius = '5px';
        container.style.textAlign = 'center';

        const isbnDetail = document.createElement('h4');
        isbnDetail.className = "detalhes-curso";
        isbnDetail.textContent = "ISBN: " + dadosCurso.ISBN;

        const tituloDetail = document.createElement('div');
        isbnDetail.className = "detalhes-curso";
        tituloDetail.textContent = "Título: " + dadosCurso.titulo;

        const autorDetail = document.createElement('div');
        autorDetail.className = "detalhes-curso";
        autorDetail.textContent = "Autor: " + dadosCurso.autor;

        const categDetail = document.createElement('div');
        categDetail.className = "detalhes-curso";
        categDetail.textContent = "Categoria: " + dadosCurso.categoria;

        const precoDetail = document.createElement('div');
        precoDetail.className = "detalhes-curso";
        precoDetail.textContent = "Preço: " + dadosCurso.preco;


        const promoDetail = document.createElement('div');
        promoDetail.className = "detalhes-curso";
        promoDetail.textContent = "Promoção: " + dadosCurso.promocao;


        const ratingDetail = document.createElement('div');
        ratingDetail.className = "detalhes-curso";
        ratingDetail.textContent = "Rating: " + dadosCurso.rating;

        const okButton = document.createElement('button');
        okButton.innerText = 'Ok';

        // Lógica para o botão "Ok"
        okButton.addEventListener('click', () => {
            modal.remove();
            resolve(null); // Retorna null se o usuário cancelar
        });

        // Monta o modal        
        container.appendChild(isbnDetail);
        container.appendChild(document.createElement('br')); // Linha em branco
        container.appendChild(tituloDetail);
        container.appendChild(document.createElement('br'));
        container.appendChild(categDetail);
        container.appendChild(document.createElement('br'));
        container.appendChild(precoDetail);
        container.appendChild(document.createElement('br'));
        if (dadosCurso.promocao > 0 && dadosCurso.promocao != null && dadosCurso.promocao != undefined) {
            container.appendChild(promoDetail);
            container.appendChild(document.createElement('br'));
        }
        container.appendChild(ratingDetail);
        container.appendChild(document.createElement('br'));
        container.appendChild(autorDetail);
        container.appendChild(document.createElement('br'));
        container.appendChild(okButton);
        modal.appendChild(container);
        document.body.appendChild(modal);
    });
}

function init() {
    listarCursos();

    const categoriaSelect = document.getElementById('categoria');
    categoriaSelect.addEventListener('change', verificarCategoria);

    document.getElementById('pesquisa').addEventListener('submit', function (event) {
        event.preventDefault(); // Impede o envio do formulário
        pesquisarCursos(); // Chama a função de pesquisa
    });
}

function verificarCategoria() {
    const select = document.getElementById('categoria');
    const valorSelecionado = select.value;

    if (valorSelecionado === 'maisVendidos') {
        mostrarMaisVendidos(); // Chama a função para mostrar mais vendidos
    } else if (valorSelecionado === 'favoritos') {
        mostrarFavoritos(); // Chama a função para mostrar favoritos
    } else if (valorSelecionado === '') {
        listarCursos(); // Chama a função para mostrar favoritos
    } else {
        mostrarPorTipo(valorSelecionado);
    }
}