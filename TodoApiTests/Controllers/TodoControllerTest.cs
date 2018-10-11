using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace ControllerTests{
    [TestFixture]
    public class TodoControllerTest{

        private TodoController ControllerUnderTest;
        private TodoContext TodoContext;

        [SetUp]
        public void Setup(){
            var testContextOptions = new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

            TodoContext = new TodoContext(testContextOptions);
            ControllerUnderTest = new TodoController(TodoContext);
        }

        [Test, Order(1)]
        public async Task Get_All_Items(){
            var result = await ControllerUnderTest.GetAll() as OkObjectResult;
            var items = result.Value as List<TodoItem>;
            
            Assert.AreEqual(items.Count, 1);
            Assert.AreEqual(items[0].Id, 1);
            Assert.AreEqual(items[0].Name, "None");
        }

        [Test, Order(2)]
        public async Task Get_Item_By_Id(){
            var id = 1;
            var result = await ControllerUnderTest.GetById(id) as OkObjectResult;
            var item = result.Value as TodoItem;
            
            Assert.AreEqual(item.Id, 1);
            Assert.AreEqual(item.Name, "None");
        }

        [Test, Order(3)]
        public async Task Create_Item(){
            var newItem = new TodoItem{
                Name = "AName",
                IsComplete = false
            };

            var result = await ControllerUnderTest.CreateTodo(newItem) as CreatedAtRouteResult;

            var createdItem = result.Value as TodoItem;

            Assert.AreEqual(createdItem.Id, 2);
        }

        [Test, Order(4)]
        public async Task Edit_Item(){
            var itemDetails = new TodoItem{
                Name = "AName",
                IsComplete = false
            };

            var result = await ControllerUnderTest.EditTodo(2, itemDetails) as OkObjectResult;
            var item = result.Value as TodoItem;

            Assert.AreEqual(item.Name, itemDetails.Name);
            Assert.AreEqual(item.IsComplete, itemDetails.IsComplete);

            var editedItem = TodoContext.TodoItems.FirstOrDefault(i => i.Id == 2); 
            Assert.IsNotNull(editedItem);

            Assert.AreEqual(item.Name, editedItem.Name);
            Assert.AreEqual(item.IsComplete, itemDetails.IsComplete);

        }

        [Test, Order(5)]
        public async Task DeleteItem(){
            var result = await ControllerUnderTest.DeleteTodo(2) as NoContentResult;

            Assert.IsNotNull(result);

            var shouldBeMissingItem = TodoContext.TodoItems.FirstOrDefault(i => i.Id == 2);

            Assert.IsNull(shouldBeMissingItem);
        }
    }
}