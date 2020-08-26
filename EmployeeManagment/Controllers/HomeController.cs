using EmployeeManagment.Models;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IEmployeeRepository employeeRepository,
                              IWebHostEnvironment hostingEnvironment)
        {
            _employeeRepository = employeeRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {
            var employees = _employeeRepository.GetAllEmployees();

            return View(employees);
        }

        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            Employee employee = _employeeRepository.GetEmployee(id.Value);

            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    ImagePath = uniqueFileName
                };

                _employeeRepository.AddEmployee(newEmployee);

                return RedirectToAction("details", new { newEmployee.Id });
            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);

            EmployeeEditViewModel editEmployee = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingImagePath = employee.ImagePath
            };

            return View(editEmployee);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if(model.Image != null)
                {
                    if(model.ExistingImagePath != null)
                    {
                        string filePath = Path.Combine(_hostingEnvironment.WebRootPath, "img", model.ExistingImagePath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.ImagePath = ProcessUploadedFile(model);
                }

                _employeeRepository.UpdateEmployee(employee);

                return RedirectToAction("index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fileStream = new FileStream(filePath, FileMode.Create);
                model.Image.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
