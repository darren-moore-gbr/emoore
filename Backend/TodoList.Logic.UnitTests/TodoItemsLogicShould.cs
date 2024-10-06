using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using TodoList.Data;
using TodoList.Data.Models;
using TodoList.MoqExtended;
using Xunit;

namespace TodoList.Logic.UnitTests
{
    public class TodoItemsLogicShould
    {
        [Fact]
        public async Task Add_Should_BeOk()
        {
            var todoItem = new TodoItem
            {
                Description = "description",
            };

            var todoItemsProvider = new Mock<ITodoItemsProvider>();
            todoItemsProvider.Setup(x => x.AreAnyNotCompleted(todoItem.Description)).ReturnsAsync(false);
            todoItemsProvider.Setup(x => x.Add(todoItem)).Returns(Task.CompletedTask);
            
            var logger = new MockILogger<TodoItemsLogic>();

            var todoItemsRules = new TodoItemsLogic(todoItemsProvider.Object, logger.Object);
            await todoItemsRules.Add(todoItem);

            todoItemsProvider.Verify(x => x.AreAnyNotCompleted(todoItem.Description), Times.Once());
            todoItemsProvider.Verify(x => x.Add(todoItem), Times.Once());
            logger.VerifyLogger(Times.Never);
        }

        [Fact]
        public async Task Add_Should_Throw_ValidationException()
        {
            var todoItem = new TodoItem();

            var todoItemsProvider = new Mock<ITodoItemsProvider>();
            todoItemsProvider.Setup(x => x.AreAnyNotCompleted(todoItem.Description)).ReturnsAsync(true);
            todoItemsProvider.Setup(x => x.Add(todoItem)).Returns(Task.CompletedTask);

            var logger = new MockILogger<TodoItemsLogic>();

            var todoItemsRules = new TodoItemsLogic(todoItemsProvider.Object, logger.Object);
            await Assert.ThrowsAsync<ValidationException>(() => todoItemsRules.Add(todoItem));

            todoItemsProvider.Verify(x => x.AreAnyNotCompleted(todoItem.Description), Times.Never());
            todoItemsProvider.Verify(x => x.Add(todoItem), Times.Never());
            logger.VerifyLogger(LogLevel.Information, Times.Never);
        }

        [Fact]
        public async Task Add_Should_Throw_AlreadyExistsException()
        {
            var todoItem = new TodoItem
            {
                Description = "description",
            };

            var todoItemsProvider = new Mock<ITodoItemsProvider>();
            todoItemsProvider.Setup(x => x.AreAnyNotCompleted(todoItem.Description)).ReturnsAsync(true);
            todoItemsProvider.Setup(x => x.Add(todoItem)).Returns(Task.CompletedTask);

            var logger = new MockILogger<TodoItemsLogic>();

            var todoItemsRules = new TodoItemsLogic(todoItemsProvider.Object, logger.Object);
            await Assert.ThrowsAsync<ExistsException>(() => todoItemsRules.Add(todoItem));

            todoItemsProvider.Verify(x => x.AreAnyNotCompleted(todoItem.Description), Times.Once());
            todoItemsProvider.Verify(x => x.Add(todoItem), Times.Never());
            logger.VerifyLogger(LogLevel.Information, Times.Once);
        }

        [Fact]
        public async Task Add_Should_Handle_DbUpdateException()
        {
            var todoItem = new TodoItem
            {
                Description = "description",
            };

            var todoItemsProvider = new Mock<ITodoItemsProvider>();
            todoItemsProvider.Setup(x => x.AreAnyNotCompleted(todoItem.Description)).ReturnsAsync(false);
            todoItemsProvider.Setup(x => x.Add(todoItem)).Throws(new DbUpdateException());

            var logger = new MockILogger<TodoItemsLogic>();

            var todoItemsRules = new TodoItemsLogic(todoItemsProvider.Object, logger.Object);
            await Assert.ThrowsAsync<DbUpdateException>(() => todoItemsRules.Add(todoItem));

            todoItemsProvider.Verify(x => x.AreAnyNotCompleted(todoItem.Description), Times.Once());
            todoItemsProvider.Verify(x => x.Add(todoItem), Times.Once());
            logger.VerifyLogger(LogLevel.Error, Times.Once);
        }

        /* Most complicated example done, cut and paste away as appropriate ;) */
    }
}
