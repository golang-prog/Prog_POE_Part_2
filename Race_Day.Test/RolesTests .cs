using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Race_Day.Controllers;
using Race_Day.Data;
using Race_Day.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Race_Day.Test
{
    public class RolesTests : IDisposable
    {
        private readonly ApplicationDbcontext _context;
        private readonly RolesController _controller;

        public RolesControllerTests()
        {
            // Use InMemory database - unique name per test class instance
            var options = new DbContextOptionsBuilder<ApplicationDbcontext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbcontext(options);
            _controller = new RolesController(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        // Helper to seed roles
        private void SeedRoles(params Role[] roles)
        {
            _context.roles.AddRange(roles);
            _context.SaveChanges();
        }

        #region GetAllRoles Tests

        [Fact]
        public void GetAllRoles_ReturnsOkResult_WithEmptyList_WhenNoRolesExist()
        {
            // Act
            var result = _controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<List<Role>>(okResult.Value);
            Assert.Empty(roles);
        }

        [Fact]
        public void GetAllRoles_ReturnsOkResult_WithAllRoles()
        {
            // Arrange
            SeedRoles(
                new Role { Id = 1, Name = "Admin", Description = "Administrator" },
                new Role { Id = 2, Name = "User", Description = "Regular user" }
            );

            // Act
            var result = _controller.GetAllRoles();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var roles = Assert.IsAssignableFrom<List<Role>>(okResult.Value);
            Assert.Equal(2, roles.Count);
        }

        #endregion

        #region GetRoleById Tests

        [Fact]
        public void GetRoleById_ReturnsOkResult_WithRole_WhenRoleExists()
        {
            // Arrange
            SeedRoles(new Role { Id = 1, Name = "Admin", Description = "Administrator" });

            // Act
            var result = _controller.GetRoleById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var role = Assert.IsType<Role>(okResult.Value);
            Assert.Equal("Admin", role.Name);
        }

        [Fact]
        public void GetRoleById_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Act
            var result = _controller.GetRoleById(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region AddNewRole Tests

        [Fact]
        public void AddNewRole_ReturnsCreatedAtAction_WithNewRole()
        {
            // Arrange
            var dto = new NewRoleDTO
            {
                Name = "Manager",
                Description = "Manager role"
            };

            // Act
            var result = _controller.AddNewRole(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdRole = Assert.IsType<Role>(createdResult.Value);
            Assert.Equal("Manager", createdRole.Name);
            Assert.Equal("Manager role", createdRole.Description);
            Assert.True(createdRole.IsActive);
            Assert.Single(_context.roles);
        }

        [Fact]
        public void AddNewRole_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = _controller.AddNewRole(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddNewRole_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Name is required");
            var dto = new NewRoleDTO { Name = "", Description = "Test" };

            // Act
            var result = _controller.AddNewRole(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region UpdateRole Tests

        [Fact]
        public void UpdateRole_ReturnsOkResult_WithUpdatedRole()
        {
            // Arrange
            SeedRoles(new Role { Id = 1, Name = "Admin", Description = "Old description" });
            var dto = new NewRoleDTO { Name = "SuperAdmin", Description = "New description" };

            // Act
            var result = _controller.UpdateRole(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = Assert.IsType<Role>(okResult.Value);
            Assert.Equal("SuperAdmin", updated.Name);
            Assert.Equal("New description", updated.Description);
            Assert.NotNull(updated.UpdatedAt);
        }

        [Fact]
        public void UpdateRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var dto = new NewRoleDTO { Name = "Test", Description = "Test" };

            // Act
            var result = _controller.UpdateRole(999, dto);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void UpdateRole_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = _controller.UpdateRole(1, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        #endregion

        #region DeleteRole Tests

        [Fact]
        public void DeleteRole_ReturnsOkResult_WhenRoleExists()
        {
            // Arrange
            SeedRoles(new Role { Id = 1, Name = "Admin", Description = "Admin" });

            // Act
            var result = _controller.DeleteRole(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(_context.roles);
        }

        [Fact]
        public void DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Act
            var result = _controller.DeleteRole(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion

        #region SoftDeleteRole Tests

        [Fact]
        public void SoftDeleteRole_ReturnsOkResult_AndMarksRoleInactive()
        {
            // Arrange
            SeedRoles(new Role { Id = 1, Name = "Admin", Description = "Admin", IsActive = true });

            // Act
            var result = _controller.SoftDeleteRole(1);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var role = _context.roles.First(r => r.Id == 1);
            Assert.False(role.IsActive);
            Assert.NotNull(role.UpdatedAt);
        }

        [Fact]
        public void SoftDeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Act
            var result = _controller.SoftDeleteRole(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        #endregion
    }
}
