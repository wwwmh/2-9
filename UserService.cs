using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xmu.Crms.Shared.Exceptions;
using Xmu.Crms.Shared.Models;
using Xmu.Crms.Shared.Service;
using Microsoft.EntityFrameworkCore.Internal;

namespace Xmu.Crms.Services.Group1
{

    public class UserService : IUserService
    {
        AttendanceStatus absent = AttendanceStatus.Absent;
        AttendanceStatus present = AttendanceStatus.Present;
        AttendanceStatus late = AttendanceStatus.Late;
        private readonly CrmsContext _db;
        public UserService(CrmsContext db)
        {
            _db = db;

        }

        //昶辉
        public void InsertAttendanceById(long classId, long seminarId, long userId, double longitude, double latitude)
        {

            Attendance att = new Attendance();
            var attendanceLocation = _db.Location.Where(p => p.Seminar.Id == seminarId).SingleOrDefault();
            var user = _db.UserInfo.Where(p => p.Id == userId).SingleOrDefault();
            var class1 = _db.ClassInfo.Where(p => p.Id == classId).SingleOrDefault();
            var seminar = _db.Seminar.Where(p => p.Id == seminarId).SingleOrDefault();

            if (classId < 0 || seminarId < 0 || userId < 0)
            {
                if (classId < 0) throw new System.ArgumentException("Parameter format error", "classId");
                else if (seminarId < 0) throw new System.ArgumentException("Parameter format error", "seminarId");
                else throw new System.ArgumentException("Parameter format error", "userId");
            }

            if (attendanceLocation.Longitude == longitude && attendanceLocation.Latitude == latitude)
            {
                if (class1 == null)
                    throw new ClassNotFoundException();
                if (seminar == null)
                    throw new SeminarNotFoundException();
                att.Student = user;
                att.ClassInfo = class1;
                att.Seminar = seminar;
            }

            _db.Attendences.Add(att);

        }

        //昶辉
        public List<Attendance> ListAttendanceById(long classId, long seminarId)
        {
            if (classId < 0 || seminarId < 0)
                throw new ArgumentException();
            else
            {
                var a = from s in _db.Attendences where s.ClassInfo.Id == classId && s.Seminar.Id == seminarId select s;
                if (a == null)
                    throw new ClassNotFoundException();
                else
                    return a.ToList<Attendance>();
            }
        }

        //昶辉
        public UserInfo GetUserByUserId(long userId)
        {
            UserInfo us = new UserInfo();
            if (userId < 0)
            {
                throw new System.ArgumentException("Parameter format error", "userId");
            }

            var userinformation = _db.UserInfo.Where(p => p.Id == userId).SingleOrDefault();


            if (userinformation == null)
            {
                throw new UserNotFoundException();
            }

            else
            {
                us.Id = userinformation.Id;
                us.Phone = userinformation.Phone;
                us.Avatar = userinformation.Avatar;
                us.Password = userinformation.Password;
                us.Name = userinformation.Name;
                us.School = userinformation.School;
                us.Gender = userinformation.Gender;
                us.Type = userinformation.Type;
                us.Number = userinformation.Number;
                us.Education = userinformation.Education;
                us.Title = userinformation.Title;
                us.Email = userinformation.Email;
            }
            return us;
        }

        //吴帅
        //根据用户名获取用户ID
        public List<long> ListUserIdByUserName(string userName)
        {
            List<long> idList = new List<long>();
            var userlist = _db.UserInfo.Where(p => p.Name == userName).ToList();
            foreach (var i in userlist)
                idList.Add(i.Id);
            return idList;
        }

        //吴帅
        //根据用户ID修改用户信息
        public void UpdateUserByUserId(long userId, UserInfo user)
        {

            if (userId < 0)
            {
                throw new System.ArgumentException("Parameter format error", "userId");
            }
            var userget = _db.UserInfo.Where(p => p.Id == userId).SingleOrDefault();

            if (userget == null)
            {
                throw new UserNotFoundException();
            }

            else
            {

                userget.Id = user.Id;
                userget.Phone = user.Phone;
                userget.Avatar = user.Avatar;
                userget.Password = user.Password;
                userget.Name = user.Name;
                userget.School = user.School;
                userget.Gender = user.Gender;
                userget.Type = user.Type;
                userget.Number = user.Number;
                userget.Education = user.Education;
                userget.Title = user.Title;
                userget.Email = user.Email;
            }
            _db.SaveChanges();
        }

        //吴帅
        //按班级ID、学号开头、姓名开头获取学生列表.
        public List<UserInfo> ListUserByClassId(long classId, string numBeginWith, string nameBeginWith)
        {
            List<UserInfo> studentlist = new List<UserInfo>();
            if (classId < 0)
            {
                throw new System.ArgumentException("Parameter format error", "classId");
            }
            else
            {
                var clas = _db.Attendences.Where(p => p.ClassInfo.Id == classId).SingleOrDefault();
                if (clas == null)
                {

                    throw new ClassNotFoundException();

                }
                else
                {
                    var list = _db.Attendences.Where(p => p.ClassInfo.Id == classId).Where(p => p.Student.Number.StartsWith(numBeginWith)).Where(p => p.Student.Name.StartsWith(nameBeginWith)).ToList();
                    foreach (var i in list)
                        studentlist.Add(i.Student);
                    return studentlist;
                }
            }
        }

        //吴帅
        //根据用户名获取用户列表.
        public List<UserInfo> ListUserByUserName(string userName)
        {
            var userlist = _db.UserInfo.Where(p => p.Name == userName).ToList();
            return userlist;
        }

        //吴帅
        //获取讨论课所在的班级的出勤学生名单
        public List<UserInfo> ListPresentStudent(long seminarId, long classId)
        {

            if (seminarId < 0 || classId < 0)
            {
                if (seminarId < 0)
                    throw new System.ArgumentException("Parameter format error", "seminarId");
                else
                    throw new System.ArgumentException("Parameter format error", "classId");
            }
            else
            {
                var testseminar = _db.Attendences.Where(p => p.Seminar.Id == seminarId).ToList();
                if (testseminar == null)
                {
                    throw new SeminarNotFoundException();
                }
                else
                {
                    var testclass = _db.Attendences.Where(p => p.ClassInfo.Id == classId).ToList();
                    if (testclass == null)
                    {
                        throw new ClassNotFoundException();
                    }
                    else
                    {
                        List<UserInfo> studentlist = new List<UserInfo>();
                        var attendancelist = _db.Attendences.Where(p => p.Seminar.Id == seminarId).Where(q => q.ClassInfo.Id == classId).Where(r => r.AttendanceStatus == present).ToList();
                        foreach (var i in attendancelist)
                            studentlist.Add(i.Student);
                        return studentlist;
                    }
                }
            }


        }


        //吴帅
        // 获取讨论课所在的班级的迟到学生名单.
        public List<UserInfo> ListLateStudent(long seminarId, long classId)
        {
            if (seminarId < 0 || classId < 0)
            {
                if (seminarId < 0)
                    throw new System.ArgumentException("Parameter format error", "seminarId");
                else
                    throw new System.ArgumentException("Parameter format error", "classId");
            }
            else
            {
                var testseminar = _db.Attendences.Where(p => p.Seminar.Id == seminarId).ToList();
                if (testseminar == null)
                {
                    throw new SeminarNotFoundException();
                }
                else
                {
                    var testclass = _db.Attendences.Where(p => p.ClassInfo.Id == classId).ToList();
                    if (testclass == null)
                    {
                        throw new ClassNotFoundException();
                    }
                    else
                    {
                        List<UserInfo> studentlist = new List<UserInfo>();
                        var attendancelist = _db.Attendences.Where(p => p.Seminar.Id == seminarId).Where(q => q.ClassInfo.Id == classId).Where(r => r.AttendanceStatus == late).ToList();
                        foreach (var i in attendancelist)
                            studentlist.Add(i.Student);
                        return studentlist;
                    }
                }
            }

        }

        //吴帅
        // 获取讨论课所在班级缺勤学生名单.
        public List<UserInfo> ListAbsenceStudent(long seminarId, long classId)
        {
            if (seminarId < 0 || classId < 0)
            {
                if (seminarId < 0)
                    throw new System.ArgumentException("Parameter format error", "seminarId");
                else
                    throw new System.ArgumentException("Parameter format error", "classId");
            }
            else
            {
                var testseminar = _db.Attendences.Where(p => p.Seminar.Id == seminarId).ToList();
                if (testseminar == null)
                {
                    throw new SeminarNotFoundException();
                }
                else
                {
                    var testclass = _db.Attendences.Where(p => p.ClassInfo.Id == classId).ToList();
                    if (testclass == null)
                    {
                        throw new ClassNotFoundException();
                    }
                    else
                    {
                        List<UserInfo> studentlist = new List<UserInfo>();
                        var attendancelist = _db.Attendences.Where(p => p.Seminar.Id == seminarId).Where(q => q.ClassInfo.Id == classId).Where(r => r.AttendanceStatus == absent).ToList();
                        foreach (var i in attendancelist)
                            studentlist.Add(i.Student);
                        return studentlist;
                    }
                }
            }

        }

        //吴帅
        //根据教师名称列出课程名称
        public List<Course> ListCourseByTeacherName(string teacherName)
        {

            var courselist = _db.Course.Where(p => p.Teacher.Name == teacherName).ToList();
            return courselist;
        }
    }
}