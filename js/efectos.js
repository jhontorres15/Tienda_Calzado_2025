const elementos = document.querySelectorAll('.contenedor, .foto, article');

const mostrarElemento = () => {
  elementos.forEach(el => {
    const pos = el.getBoundingClientRect().top;
    if (pos < window.innerHeight - 100) {
      el.style.opacity = 1;
      el.style.transform = "translateY(0)";
    }
  });
};

window.addEventListener('scroll', mostrarElemento);


const btnArriba = document.getElementById('btn-arriba');

window.addEventListener('scroll', () => {
  if (window.scrollY > 200) {
    btnArriba.style.display = "block";
  } else {
    btnArriba.style.display = "none";
  }
});

btnArriba.addEventListener('click', () => {
  window.scrollTo({ top: 0, behavior: 'smooth' });
});



// Selecciona el elemento
const typing = document.getElementById("typing");

// Texto que se escribirá
const texto = "Bienvenido a GYG SPORTS";
let i = 0;

// Función que escribe letra por letra
function escribir() {
  if (i < texto.length) {
    typing.innerHTML += texto.charAt(i);
    i++;
    setTimeout(escribir, 100); // Velocidad en ms (100 = rápido)
  }
}

// Inicia el efecto cuando cargue la página
document.addEventListener("DOMContentLoaded", escribir);