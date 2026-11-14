SELECT g.SectionId, g.StudentId, g.Score, q.ExamType
FROM Grades g
JOIN Questions q 
ON g.QuestionId = q.Id