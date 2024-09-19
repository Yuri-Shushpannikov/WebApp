using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.Backend.Data;
using WebApp.Backend.Models;

namespace WebApp.Backend.Controllers
{
    public class WorkersController : Controller
    {
        //Контекст базы данных
        private readonly WebAppContext _context;

        public WorkersController(WebAppContext context)
        {
            //Поле контекста
            _context = context;
        }
        

        // Отображение работников
        public async Task<IActionResult> Index(bool showFired = false)
        {
            //Запрос из базы данных вместе с подразделениями
            var workers = _context.Worker.Include(w => w.Subdivision).AsQueryable();
            
            //Фильтр увольнения 
            if (!showFired)
            {
                //Исключение уволенных
                workers = workers.Where(w => !w.IsFired);
            }

            ViewData["ShowFired"] = showFired;
            return View(await workers.ToListAsync());
        }

        // Информация о работниках
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Subdivision)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // Принятие работника
        public IActionResult Create()
        {
            //Список с существующими подразделениями
            ViewData["SubdivisionId"] = new SelectList(_context.Subdivision, "Id", "Fullname");
            return View();
        }

        // Post Запрос принятия работника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Code,Workername,Sex,BirthDate,FireDate,MoveDate,SubdivisionId,Role,PhoneNumber,Email,HireDate,IsFired")] Worker worker, IFormFile? picture)
        {
            //Путь для загрузки изображений
            var uploadDirectory = Path.Combine(Environment.CurrentDirectory, "wwwroot", "images");

            if (ModelState.IsValid)
            {
                //Загрузка изображения
                if (picture != null && picture.Length > 0)
                {
                    //Получение имени файла и пути к нему для PicturePath
                    var fileName = Path.GetFileName(picture.FileName);
                    var filePath = Path.Combine(uploadDirectory, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        //Сохранение изображения в images
                        await picture.CopyToAsync(stream);
                    }
                    //Запись полного пути к изображению в PicturePath
                    worker.PicturePath = Path.Combine("images", fileName);
                }
                _context.Add(worker);
                //Сохранение изменений в базе данных
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //Выбор подразделения
            ViewData["SubdivisionId"] = new SelectList(_context.Subdivision, "Id", "Fullname", worker.SubdivisionId);
            return View(worker);
        }

        // Проверка существования работника
        private bool WorkerExists(int id)
        {
            return _context.Worker.Any(e => e.Id == id);
        }

        // Редактирование информации о работнике
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            ViewData["SubdivisionId"] = new SelectList(_context.Subdivision, "Id", "Fullname", worker.SubdivisionId);
            return View(worker);
        }

        // Post Запрос редактирования информации о работнике
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Workername,Sex,BirthDate,FireDate,MoveDate,SubdivisionId,Role,PhoneNumber,Email,HireDate,IsFired")] Worker worker, IFormFile? picture, bool removePicture = false)
        {
            if (id != worker.Id)
            {
                return NotFound();
            }

            var uploadDirectory = Path.Combine(Environment.CurrentDirectory, "wwwroot", "images");

            if (ModelState.IsValid)
            {
                try
                {
                    //Проверка существующего работника
                    var existingWorker = await _context.Worker.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
                    if (existingWorker == null)
                    {
                        return NotFound();
                    }
                    //Удаление изображения
                    if (removePicture)
                    {
                        worker.PicturePath = null;
                    }
                    //Загрузка нового изображения
                    else if (picture != null)
                    {
                        var fileName = Path.GetFileName(picture.FileName);
                        var filePath = Path.Combine(uploadDirectory, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await picture.CopyToAsync(stream);
                        }
                        worker.PicturePath = Path.Combine("images", fileName);
                    }
                    //Сохранение предыдущего пути если изображение не изменено
                    else
                    {
                        worker.PicturePath = existingWorker.PicturePath;
                    }
                    _context.Update(worker);
                    await _context.SaveChangesAsync();
                }
                // Обработка исключения при параллельном обновлении
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkerExists(worker.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = worker.Code });
            }
            return View(worker);
        }

        // Увольнение работника
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Subdivision)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }

            return View(worker);
        }

        // Post Запрос увольнения работника
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var worker = await _context.Worker.FindAsync(id);
            if (worker != null)
            {
                //Установка статуса "уволен"
                worker.IsFired = true;
                //Фиксация даты увольнения
                worker.FireDate = DateOnly.FromDateTime(DateTime.Today);
                _context.Update(worker);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        //Перевод работника
        public async Task<IActionResult> Move(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worker = await _context.Worker
                .Include(w => w.Subdivision)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (worker == null)
            {
                return NotFound();
            }
            ViewData["SubdivisionId"] = new SelectList(_context.Subdivision, "Id", "Fullname", worker.SubdivisionId);
            return View(worker);
        }

        //Post Запрос перевода работника
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Move(int id, int SubdivisionId)
        {
            var worker = await _context.Worker.FindAsync(id);
            if (worker == null)
            {
                return NotFound();
            }
            //Изменение подразделения
            worker.SubdivisionId = SubdivisionId;
            //Фиксация даты перевода
            worker.MoveDate = DateOnly.FromDateTime(DateTime.Today);
            _context.Update(worker);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = worker.Id });
        }

        // Создание отчета
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            var workers = _context.Worker.Include(w => w.Subdivision).AsQueryable();
            // Фильтр работников по выбранным датам
            if (startDate.HasValue && endDate.HasValue)
            {
                workers = workers.Where(w =>
                    (w.HireDate >= DateOnly.FromDateTime(startDate.Value) && w.HireDate <= DateOnly.FromDateTime(endDate.Value)) ||
                    (w.FireDate >= DateOnly.FromDateTime(startDate.Value) && w.FireDate <= DateOnly.FromDateTime(endDate.Value)) ||
                    (w.MoveDate >= DateOnly.FromDateTime(startDate.Value) && w.MoveDate <= DateOnly.FromDateTime(endDate.Value)));
            }
            ViewBag.IsSubmitted = startDate.HasValue && endDate.HasValue;
            return View(await workers.ToListAsync());
        }
    }
}
