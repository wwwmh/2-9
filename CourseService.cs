using System;
using System.Collections.Generic;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;

namespace Xmu.Crms.Services.Group1
{
    public class CourseService : ICourseService
    {
        private readonly CrmsContext _db;
        public CourseService(CrmsContext db)
        {
            _db = db;
        }

        public long InsertClassById(long courseId, ClassInfo classInfo)
        {
            var c = _db.Course.Find(courseId) ?? throw new CourseNotFoundException();
            classInfo.Course = c;
            var ent = _db.ClassInfo.Add(classInfo);
            _db.SaveChanges();
            return ent.Entity.Id;
        }

        public IList<Course> ListCourseByUserId(long userId)
        {
            if (userId < 0)  //ID格式错误
                throw new ArgumentException("Parameter format error", "userId");

            else   //ID格式正确
            {
                var type = from s in _db.UserInfo where s.Id == userId select s.Type;

                foreach (var tt in type)
                {
                    if (tt == Xmu.Crms.Shared.Models.Type.Student)
                    {
                        var classid = from s in _db.CourseSelection where s.Student.Id == userId select s.ClassInfo.Id; //从CourseSelection根据userid选出课堂

                        if (classid == null)   //未找到
                            throw new CourseNotFoundException();

                        else   //找到userid的选的课堂
                        {
                            foreach (long obj in classid)
                            {
                                var courseid = from s in _db.ClassInfo where s.Id == obj select s.Course.Id;  //从ClassInfo找出对应classid的courseid

                                if (courseid == null)  //未根据classid从ClassInfo表找到课程
                                    throw new CourseNotFoundException();

                                else       //根据classid找到课程
                                {
                                    foreach (long obj2 in courseid)
                                    {
                                        var course = from s in _db.Course where s.Id == obj2 select s;  //从Course表根据courseid找Course
                                        if (course == null)
                                            throw new CourseNotFoundException();
                                        else
                                        {
                                            return course.ToList<Course>();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (tt == Xmu.Crms.Shared.Models.Type.Teacher)
                    {
                        var c = from s in _db.Course where s.Teacher.Id == userId select s;
                        if (c == null)
                            throw new CourseNotFoundException();
                        else
                            return c.ToList<Course>();
                    }
                    else if (tt == Xmu.Crms.Shared.Models.Type.Unbinded)
                        throw new CourseNotFoundException();
                }


            }
            throw new CourseNotFoundException();
        }

        public long InsertCourseByUserId(long userId, Course course)
        {
            if (userId < 0)   //ID格式错误
                throw new ArgumentException("Parameter format error", "userId");
            else
            {
                var u = _db.UserInfo.Find(userId) ?? throw new UserNotFoundException();
                course.Teacher = u;
                var ent = _db.Course.Add(course);
                _db.SaveChanges();
                return ent.Entity.Id;
            }
        }

        /// <exception cref="T:Xmu.Crms.Shared.Exceptions.CourseNotFoundException">未找到课程</exception>
        public Course GetCourseByCourseId(long courseId)
        {
            if (courseId < 0)   //ID格式错误
                throw new ArgumentException("Parameter format error", "courseId");
            else
            {
                var course = _db.Course.Find(courseId);

                if (course == null)   //未找到课程
                    throw new CourseNotFoundException();
                else
                    return course;
            }
        }

        public void UpdateCourseByCourseId(long courseId, Course course)
        {
            if (courseId < 0)
                throw new ArgumentException("Parameter format error", "courseId");
            else
            {
                var c = _db.Course.Where(p => p.Id == courseId).SingleOrDefault();
                if (c == null)
                    throw new CourseNotFoundException();
                else
                {
                    c.Id = course.Id;
                    c.Name = course.Name;
                    c.StartDate = course.StartDate;
                    c.EndDate = course.EndDate;
                    c.Teacher = course.Teacher;
                    c.Description = course.Description;
                    c.ReportPercentage = course.ReportPercentage;
                    c.PresentationPercentage = course.PresentationPercentage;
                    c.FivePointPercentage = course.FivePointPercentage;
                    c.FourPointPercentage = course.FourPointPercentage;
                    c.ThreePointPercentage = course.ThreePointPercentage;
                }
            }
            _db.SaveChanges();
        }

        public void DeleteCourseByCourseId(long courseId)
        {
            if (courseId < 0)   //ID格式错误
                throw new ArgumentException("Parameter format error", "courseId");
            else
            {
                var c = _db.Course.Find(courseId);

                if (c == null)    //未找到课程
                    throw new CourseNotFoundException();
                else
                {
                    _db.Course.Remove(c);
                    _db.SaveChanges();
                }
            }
        }

        public IList<Course> ListCourseByCourseName(string courseName)
        {
            var course = from s in _db.Course where s.Name == courseName select s;

            if (course == null)     //未找到课程
                throw new CourseNotFoundException();
            else
                return course.ToList<Course>();
        }

        public IList<ClassInfo> ListClassByCourseName(string courseName)
        {
            var c = from s in _db.ClassInfo where s.Name == courseName select s;
            if (c == null)
                throw new ClassNotFoundException();
            else
                return c.ToList<ClassInfo>();
        }

        public IList<ClassInfo> ListClassByTeacherName(string teacherName)
        {
            var teacherid = from s in _db.UserInfo where s.Name == teacherName select s.Id;

            if (teacherid == null)    //未找到老师
                throw new ClassNotFoundException();
            else
            {
                foreach (long obj in teacherid)
                {
                    var courseid = from s in _db.Course where s.Teacher.Id == obj select s.Id;

                    if (courseid == null)
                        throw new ClassNotFoundException();
                    else
                    {
                        foreach (long obj2 in courseid)
                        {
                            var c = from s in _db.ClassInfo where s.Course.Id == obj2 select s;

                            if (c == null)
                                throw new ClassNotFoundException();
                            else
                                return c.ToList<ClassInfo>();
                        }
                    }
                }
            }
            throw new ClassNotFoundException();
        }

        public IList<ClassInfo> ListClassByName(string courseName, string teacherName)
        {
            var teacher = from s in _db.UserInfo where s.Name == teacherName select s.Id;
            foreach(long obj in teacher)
            {
                var course = from s in _db.Course where s.Name == courseName && s.Teacher.Id == obj select s.Id;
                foreach(long obj2 in course)
                {
                    var c = from s in _db.ClassInfo where s.Course.Id == obj2 select s;
                    return c.ToList<ClassInfo>();
                }
            }
            throw new ClassNotFoundException();
        }
    }
}
