SELECT *
FROM Grades g
LEFT JOIN Questions q
ON g.QuestionId = q.Id