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

        private readonly CrmsContext _db;
        public TopicService(CrmsContext db)
        {
            _db = db;

        }

        public Topic GetTopicByTopicId(long topicId)//按topicId获取topic.
        {
            if (topicId < 0)
                throw new System.ArgumentException("Parameter format error", "topicId");
            else
            {
                Topic topic = new Topic();
                topic = _db.Topic.Find(topicId);
                if (topic == null)
                    throw new TopicNotFoundException();
                else
                    return topic;
            }
        }

        public void UpdateTopicByTopicId(long topicId, Topic topic)
        {
            if (topicId < 0)
                throw new System.ArgumentException("Parameter format error", "topicId");
            else
            {
                var t = _db.Topic.Where(p => p.Id == topicId).SingleOrDefault();
                if (t == null)
                    throw new TopicNotFoundException();
                else
                {
                    t.Id = topic.Id;
                    t.Name = topic.Name;
                    t.Description = topic.Description;
                    t.GroupNumberLimit = topic.GroupNumberLimit;
                    t.GroupStudentLimit = topic.GroupStudentLimit;
                    t.Seminar = topic.Seminar;
                }
            }
            _db.SaveChanges();
        }

        public void DeleteTopicByTopicId(long topicId)
        {
            if (topicId < 0)
                throw new ArgumentException("Parameter format error", "topicId");
            else
            {
                _db.Topic.Remove(_db.Topic.Find(topicId));
                _db.SaveChanges();
            }
        }

        public IList<Topic> ListTopicBySeminarId(long seminarId)
        {
            if (seminarId < 0)
                throw new System.ArgumentException("Parameter format error", "seminarId");
            else
            {
                List<Topic> topiclist = new List<Topic>();
                //List<Topic> topiclist = _db.Topic.Where(p=>p.);
                var _topiclist = _db.Topic.Where(p => p.Seminar.Id == seminarId).ToList();
                return _topiclist.ToList<Topic>();
            }
        }

        public long InsertTopicBySeminarId(long seminarId, Topic topic)
        {
            if (seminarId < 0)
                throw new ArgumentException("Parameter format error", "seminarId");
            else
            {
                _db.Topic.Add(topic);
                _db.SaveChanges();
                return topic.Id;
            }
        }

        public void DeleteSeminarGroupTopicById(long groupId, long topicId)
        {
            if (topicId < 0) throw new System.ArgumentException("Parameter format error", "topicId");
            if (groupId < 0) throw new System.ArgumentException("Parameter format error", "groupId");

            var topic = _db.SeminarGroupTopic.Where(p => (p.Topic.Id == topicId && p.SeminarGroup.Id == groupId)).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(topic);
            _db.SaveChanges();
        }

        public void DeleteSeminarGroupTopicByTopicId(long topicId)
        {
            if (topicId < 0) throw new System.ArgumentException("Parameter format error", "topicId");

            var topic = _db.SeminarGroupTopic.Where(p => p.Topic.Id == topicId).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(topic);
            _db.SaveChanges();
        }

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

        public IList<SeminarGroupTopic> ListSeminarGroupTopicByGroupId(long groupId)
        {
            if (groupId < 0)
                throw new ArgumentException("Parameter format error", "groupId");
            else
            {
                var sgt = from s in _db.SeminarGroupTopic where s.SeminarGroup.Id == groupId select s;
                return sgt.ToList<SeminarGroupTopic>();
            }
        }

        public void DeleteTopicBySeminarId(long seminarId)
        {
            if (seminarId < 0) throw new System.ArgumentException("Parameter format error", "seminarId");

            var groupTopic = _db.SeminarGroupTopic.Where(p => p.SeminarGroup.Id == seminarId).SingleOrDefault();

            long topicId = groupTopic.Id;
            var studentScore = _db.StudentScoreGroup.Where(p => p.SeminarGroupTopic.Id == topicId).SingleOrDefault();

            _db.SeminarGroupTopic.Remove(groupTopic);
            _db.StudentScoreGroup.Remove(studentScore);
            _db.SaveChanges();
        }
    }
}
