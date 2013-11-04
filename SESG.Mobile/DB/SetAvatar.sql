update Users set ProfilePictureId = 8
where ABS(UserID) % 3 = 1
AND UserID > 2
