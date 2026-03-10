-- DELETE from Answers
-- DBCC CHECKIDENT ('[Answers]', RESEED, 0);
-- GO
-- DELETE from Questions
-- DBCC CHECKIDENT ('[Questions]', RESEED, 0);
-- Go
-- DELETE from Polls
-- DBCC CHECKIDENT ('[Polls]', RESEED, 0);
-- Go
-- DELETE from Votes
-- DBCC CHECKIDENT ('[Votes]', RESEED, 0);
-- Go

select *
from VoteAnswers
select *
from Votes
