using FireSharp;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace TestingProject.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "gibqI9uwUwtrToLPcY8VvXai70XvBeE7mZK1TXin",
            BasePath = "https://syncdatabase-73aa0-default-rtdb.firebaseio.com"
        };
        FirebaseClient client;
        // GET: Student
        public ActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students/");//nên có dấu "/" để set data không bị lỗi
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list=new List<Student>();
            foreach(var item in data)
            {
                list.Add(JsonConvert.DeserializeObject<Student>(((JProperty)item).Value.ToString()));
            }
            return View(list);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                AddStudentToFireBase(student);
                ModelState.AddModelError(string.Empty,"added Successfully");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty,ex.Message);
            }
            
            return View();
        }

        public void AddStudentToFireBase(Student student)
        {
            client=new FireSharp.FirebaseClient(config);
            var data = student;
            PushResponse response = client.Push("Students/",data);
            data.student_id = response.Result.name;
            SetResponse setResponse = client.Set("Students/"+data.student_id,data);
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students/" + id);

            if (response.Body != "null")
            {
                Student data = response.ResultAs<Student>();
                return View(data);
            }
            else
            {
                // Handle the case where no data was found
                return View("NotFoundView");
            }
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Students/" + id);

            if (response.Body != "null")
            {
                Student data = response.ResultAs<Student>();
                return View(data);
            }
            else
            {
                // Handle the case where no data was found
                return View("NotFoundView");
            }
        }

        [HttpPost]
        public ActionResult Edit(Student student)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Students/" + student.student_id,student);//nên có dấu "/" để set data không bị lỗi
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            client = new FirebaseClient(config);
            FirebaseResponse response = client.Delete("Students/" + id);
            return RedirectToAction("Index");
        }
    }
}