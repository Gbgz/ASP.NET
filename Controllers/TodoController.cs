using AspNetCoreTodo.Models;
using AspNetCoreTodo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreTodo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoItemService _todoItemService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoItemService todoItemService,
        UserManager<ApplicationUser> userManager)
        {
            _todoItemService = todoItemService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var items = await _todoItemService
                .GetIncompleteItemsAsync(currentUser);

            var model = new TodoViewModel()
            {
                Items = items
            };

            return View(model);
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(TodoItem newItem)
        {
            // 添加这行来确认方法被调用
            Console.WriteLine("=================================================");
            Console.WriteLine($"AddItem方法被调用了！Title: {newItem?.Title}");

            var currentUser = await _userManager.GetUserAsync(User);
            Console.WriteLine("0000000000000000000000000000000000000000");
            Console.WriteLine("currentUserId = " + currentUser.Id);
            if (currentUser == null)
            {
                Console.WriteLine("0000000000000000000000000000000000000000");
                newItem.UserId = currentUser.Id;
                Console.WriteLine("currentUserId = "+currentUser.Id);
                //return Challenge();
            } 

            if (!ModelState.IsValid)
            {
                Console.WriteLine("/////////////////////////////////////////////");
                Console.WriteLine("ModelState验证失败");

                // 检查ModelState中的错误
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"错误: {error.ErrorMessage}");
                    }
                }

                return RedirectToAction("Index");
            }
                        

            var successful = await _todoItemService
                .AddItemAsync(newItem, currentUser);

            if (!successful)
            {
                return BadRequest("Could not add item.");
            }

            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(Guid id)
        {
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var successful = await _todoItemService
                .MarkDoneAsync(id, currentUser);

            if (!successful)
            {
                return BadRequest("Could not mark item as done.");
            }

            return RedirectToAction("Index");
        }
    }
}