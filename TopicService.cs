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

    public class TopicService : ITopicService
    {
        AttendanceStatus absent = AttendanceStatus.Absent;
        AttendanceStatus present = AttendanceStatus.Present;
        AttendanceStatus late = AttendanceStatus.Late;
        private readonly CrmsContext _db;
        public TopicService(CrmsContext db)
        {
            _db = db;

        }

        //银高
        public Topic GetTopicByTopicId(long topicId)
        {
            throw new NotImplementedException();
        }

        //银高
        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            throw new NotImplementedException();
        }

        //银高
        public void DeleteTopicByTopicId(long topicId, long seminarId)
        {
            throw new NotImplementedException();
        }

        //银高
        public List<Topic> ListTopicBySeminarId(long seminarId)
        {
            throw new NotImplementedException();
        }

        //银高
        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            throw new NotImplementedException();
        }

        //昶辉
        public void DeleteTopicById(long groupId, long topicId)
        {

            if (groupId < 0) throw new System.ArgumentException("Parameter format error", "groupId");
            if (topicId < 0) throw new System.ArgumentException("Parameter format error", "topicId");

            var topic = _db.SeminarGroupTopic.Where(p => p.Topic.Id == topicId).Where(r => r.SeminarGroup.Id == groupId).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(topic);

        }

        //昶辉
        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            if (topicId < 0) throw new System.ArgumentException("Parameter format error", "topicId");

            var topic = _db.SeminarGroupTopic.Where(p => p.Topic.Id == topicId).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(topic);
        }

        //昶辉
        public SeminarGroupTopic GetSeminarGroupTopicById(long topicId, long groupId)
        {

            if (groupId < 0) throw new System.ArgumentException("Parameter format error", "groupId");
            if (topicId < 0) throw new System.ArgumentException("Parameter format error", "topicId");

            SeminarGroupTopic groupTopic = new SeminarGroupTopic();

            var topic = _db.SeminarGroupTopic.Where(p => p.Topic.Id == topicId).Where(r => r.SeminarGroup.Id == groupId).SingleOrDefault();

            groupTopic.Id = topic.Id;
            groupTopic.Topic = topic.Topic;
            groupTopic.SeminarGroup = topic.SeminarGroup;
            groupTopic.PresentationGrade = topic.PresentationGrade;

            return groupTopic;

        }

        //昶辉
        public void DeleteTopicBySeminarId(long seminarId)
        {
            if (seminarId < 0) throw new System.ArgumentException("Parameter format error", "seminarId");

            var groupTopic = _db.SeminarGroupTopic.Where(p => p.SeminarGroup.Id == seminarId).SingleOrDefault();

            long topicId = groupTopic.Id;
            var studentScore = _db.StudentScoreGroup.Where(p => p.SeminarGroupTopic.Id == topicId).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(groupTopic);
            _db.StudentScoreGroup.Remove(studentScore);
        }
    }
}