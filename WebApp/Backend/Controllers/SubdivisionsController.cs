using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Backend.Data;
using WebApp.Backend.Models;

namespace WebApp.Backend.Controllers
{
    public class SubdivisionsController : Controller
    {
        //Контекст базы данных
        private readonly WebAppContext _context;

        public SubdivisionsController(WebAppContext context)
        {
            //Поле контекста
            _context = context;
        }

        // Отображение подразделений
        public async Task<IActionResult> Index(bool showLiquidated = false)
        {
            var subdivisions = _context.Subdivision.AsQueryable();
            //Фильтр ликвидации
            if (!showLiquidated)
            {
                //Исключение ликвидированных
                subdivisions = subdivisions.Where(s => !s.IsLiquidated);
            }

            ViewData["ShowLiquidated"] = showLiquidated;
            return View(await subdivisions.ToListAsync());
        }

        // Информация о подразделении
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivision = await _context.Subdivision
                // Поиск подразделения по id
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subdivision == null)
            {
                return NotFound();
            }

            return View(subdivision);
        }

        // Создание подразделения
        public IActionResult Create()
        {
            return View();
        }

        // Post Запрос создания подразделения
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Fullname,Shortname,StartDate,EndDate,IsLiquidated")] Subdivision subdivision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subdivision);
                // Сохранение изменений в базе данных
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subdivision);
        }

        //Проверка существования подразделения
        private bool SubdivisionExists(int id)
        {
            return _context.Subdivision.Any(e => e.Id == id);
        }

        // Редактирование информации о подразделении
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivision = await _context.Subdivision.FindAsync(id);
            if (subdivision == null)
            {
                return NotFound();
            }
            return View(subdivision);
        }

        // Post Запрос редактирования информации о подразделении
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Fullname,Shortname,StartDate,EndDate,IsLiquidated")] Subdivision subdivision)
        {
            if (id != subdivision.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Обновление данных в контексте
                    _context.Update(subdivision);
                    await _context.SaveChangesAsync();
                }
                // Обработка исключения при параллельном обновлении
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubdivisionExists(subdivision.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subdivision);
        }

        // Ликвидация подразделения
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subdivision = await _context.Subdivision
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subdivision == null)
            {
                return NotFound();
            }

            return View(subdivision);
        }

        // Post Запрос ликвидации подразделения
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subdivision = await _context.Subdivision.FindAsync(id);
            if (subdivision != null)
            {
                subdivision.IsLiquidated = true;
                //Установка даты ликвидации
                subdivision.EndDate = DateOnly.FromDateTime(DateTime.Today);
                _context.Update(subdivision);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
