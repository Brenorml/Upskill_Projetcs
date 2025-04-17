const carrinho = document.querySelector('#carrinho');
const listaCursos = document.querySelector('#lista-cursos');
const divcarrinho = document.querySelector('#lista-carrinho tbody');
const limparcarrinhoBtn = document.querySelector('#limpar-carrinho');
const pagCarrinho = document.querySelector('#pagamento');

let artigoscarrinho = [];

initEventListeners();

function initEventListeners() {
    listaCursos.addEventListener('click', adicionarCurso);
    carrinho.addEventListener('click', eliminarCurso);
    limparcarrinhoBtn.addEventListener('click', limparcarrinho);
    pagCarrinho.addEventListener('click', pagarCarrinho);
}

function adicionarCurso(e) {
    e.preventDefault();
    if (e.target.classList.contains('adicionar-carrinho')) {
        const curso = e.target.parentElement.parentElement;
        lerDadosCurso(curso);
    }
}

function lerDadosCurso(curso) {
     // Seleciona o elemento de preço com base nas classes
     const elementoPreco = curso.querySelector('.preco') || curso.querySelector('.preco-promo');

     // Verifica se o elemento possui a classe `.preco` (indica que há promoção)
    const temPromocao = elementoPreco.classList.contains('preco');
    
    // Define o preço final com base na existência de promoção
    let precoFinal;
    if (temPromocao) {
        // Se há promoção, capturamos o preço normal e o promocional
        const precoNormal = elementoPreco.childNodes[0].textContent.trim();
        const precoPromocional = elementoPreco.querySelector('span').textContent.trim();
        precoFinal = (precoPromocional && precoPromocional !== "" && precoPromocional > 0)?precoPromocional:precoNormal;
    } else {
        // Se não há promoção, usamos o preço direto de `.preco-promo`
        precoFinal = elementoPreco.textContent.trim();
    }

    let infoCurso = {
        imagem: curso.querySelector('img').src,
        titulo: curso.querySelector('h4').textContent,
        preco: precoFinal,
        ISBN: curso.querySelector('a').getAttribute('data-isbn'), 
        qtd: 1
    }

     if (artigoscarrinho.some(curso => curso.ISBN === infoCurso.ISBN)) {
          const cursos = artigoscarrinho.map(curso => {
               if (curso.ISBN === infoCurso.ISBN) {
                    curso.qtd++;
                    return curso;
               } else {
                    return curso;
               }
          })
          artigoscarrinho = [...cursos];
     } else {
          artigoscarrinho = [...artigoscarrinho, infoCurso];
     }

    console.log(artigoscarrinho)

    carrinhoHTML();
}

function eliminarCurso(e) {
    e.preventDefault();
    if (e.target.classList.contains('apagar-curso')) {
         e.target.parentElement.parentElement.remove();
        const cursoId = e.target.getAttribute('data-id')
       
        artigoscarrinho = artigoscarrinho.filter(curso => curso.ISBN !== cursoId);
        
        carrinhoHTML();
    }
}

function carrinhoHTML() {
    removeDoCarrinho();

    artigoscarrinho.forEach(curso => {
        const row = document.createElement('tr');
        row.innerHTML = `
               <td>  
                    <img src="${curso.imagem}" width=100>
               </td>
               <td>${curso.titulo}</td>
               <td>${curso.preco}</td>
               <td>${curso.qtd} </td>
               <td>
                    <a href="#" class="apagar-curso" data-id="${curso.ISBN}">X</a>
               </td>
          `;
        divcarrinho.appendChild(row);
    });
}

function removeDoCarrinho() {
    while (divcarrinho.firstChild) {
        divcarrinho.removeChild(divcarrinho.firstChild);
    }
}

function limparcarrinho() {
    divcarrinho.innerHTML = '';
    artigoscarrinho = [];    
}

function pagarCarrinho(e) {
    e.preventDefault();
    if (e.target.classList.contains('pagar-carrinho')) {
        fazerLogin();
    }
}
