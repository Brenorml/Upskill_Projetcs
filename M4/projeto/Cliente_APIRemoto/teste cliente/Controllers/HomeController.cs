using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;
using teste_cliente.Models;

namespace teste_cliente.Controllers
{
    public class HomeController : Controller
    {        
        //Adicionar ultimas noticias a Home
        private readonly NoticiasController _noticiasController;

        public HomeController(NoticiasController noticiasController)
        {
            _noticiasController = noticiasController;
        }

        public async Task<IActionResult> Index()
        {
            // Obter até 3 notícias mais recentes para exibir na home
            var noticiasResult = await _noticiasController.GetNoticiasAsync(3);

            // Garante que sempre tenhamos exatamente 3 notícias
            var noticiasFinais = new List<Noticia>(noticiasResult);
            if (noticiasFinais.Count < 3)
            {
                var noticiasDemo = _noticiasController.CriarNoticiasDemonstracao();
                int noticiasFaltando = 3 - noticiasFinais.Count;
                noticiasFinais.AddRange(noticiasDemo.Take(noticiasFaltando));
            }

            ViewBag.Noticias = noticiasFinais.Take(3).ToList(); // Garante exatamente 3 notícias
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

    }
}
