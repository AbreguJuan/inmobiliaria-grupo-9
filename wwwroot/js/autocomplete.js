function initAutocomplete(inputId, hiddenId, listId, url, valorInicialTexto) {
    const input = document.getElementById(inputId);
    const hidden = document.getElementById(hiddenId);
    const lista = document.getElementById(listId);

    if (valorInicialTexto) {
        input.value = valorInicialTexto;
    }

    let timeoutId;

    input.addEventListener('input', () => {
        hidden.value = '';
        clearTimeout(timeoutId);
        const termino = input.value.trim();

        if (termino.length < 2) {
            lista.innerHTML = '';
            lista.style.display = 'none';
            return;
        }

        timeoutId = setTimeout(() => {
            fetch(`${url}?term=${encodeURIComponent(termino)}`)
                .then(r => r.json())
                .then(datos => {
                    lista.innerHTML = '';
                    if (datos.length === 0) {
                        lista.style.display = 'none';
                        return;
                    }
                    datos.forEach(item => {
                        const li = document.createElement('li');
                        li.textContent = item.texto;
                        li.className = 'list-group-item list-group-item-action';
                        li.style.cursor = 'pointer';
                        li.addEventListener('click', () => {
                            input.value = item.texto;
                            hidden.value = item.id;
                            lista.innerHTML = '';
                            lista.style.display = 'none';
                        });
                        lista.appendChild(li);
                    });
                    lista.style.display = 'block';
                });
        }, 300); // espera 300ms después de que el usuario deja de tipear
    });

    document.addEventListener('click', (e) => {
        if (!lista.contains(e.target) && e.target !== input) {
            lista.style.display = 'none';
        }
    });
}