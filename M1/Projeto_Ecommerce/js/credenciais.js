let credenciais = [
    { "id": "1", "username": "cliente1", "password": "teste123", "tipo": "cliente" },
    { "id": "2", "username": "cliente2", "password": "teste123", "tipo": "cliente" },
    { "id": "3", "username": "cliente3", "password": "teste123", "tipo": "cliente" }
];

localStorage.setItem("credenciais", JSON.stringify(credenciais));
let strPass = localStorage.getItem("credenciais");
let credList = JSON.parse(strPass) || []; // Se não houver, inicializa como array vazio

async function fazerLogin() {
    // Chama a função que cria o popup e aguarda a resposta
    const credenciais = await criarPopupLogin();

    // Se o usuário clicar em "Cancelar", credenciais será null
    if (!credenciais) {
        alert("Login cancelado.");
        return; // Sair da função se o usuário cancelar
    }

    const [username, password] = credenciais;        
    let flag = false;
    
    for (let i = 0; i < credList.length; i++){
        if (credList[i].username === username && password === credList[i].password)
            flag = true;
    }
    // Verificar credenciais
    if (flag) {
        // Se as credenciais estão corretas
        alert("Login bem-sucedido! Sua compra foi confirmada.");
        limparcarrinho();
    } else {
        // Se as credenciais estão incorretas
        alert("Nome de usuário ou senha incorretos. Tente novamente.");
    }
}

function criarPopupLogin() {
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

        const usernameInput = document.createElement('input');
        usernameInput.placeholder = 'Nome de usuário';
        usernameInput.type = 'text';

        const passwordInput = document.createElement('input');
        passwordInput.placeholder = 'Senha';
        passwordInput.type = 'password';

        const loginButton = document.createElement('button');
        loginButton.innerText = 'Entrar';
        const cancelButton = document.createElement('button');
        cancelButton.innerText = 'Cancelar';

        // Lógica para o botão "Entrar"
        loginButton.addEventListener('click', () => {
            const username = usernameInput.value;
            const password = passwordInput.value;

            // Fecha o modal e resolve a promessa com as credenciais
            modal.remove();
            resolve([username, password]);
        });

        // Lógica para o botão "Cancelar"
        cancelButton.addEventListener('click', () => {
            modal.remove();
            resolve(null); // Retorna null se o usuário cancelar
        });

        // Monta o modal
        container.appendChild(usernameInput);
        container.appendChild(document.createElement('br')); // Linha em branco
        container.appendChild(passwordInput);
        container.appendChild(document.createElement('br'));
        container.appendChild(loginButton);
        container.appendChild(cancelButton);
        modal.appendChild(container);
        document.body.appendChild(modal);
    });
}


