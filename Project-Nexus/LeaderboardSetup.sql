------ CREATE THE DATABASE ------
--USE master
--GO

--CREATE DATABASE ProjectNexus
--GO

--use ProjectNexus

--CREATE TABLE LeaderBoard (
--	LeaderBoardId	INT				IDENTITY	PRIMARY KEY,
--	PlayerNickName	NVARCHAR(25)	NOT NULL,
--	LobbyName		NVARCHAR(25)	NOT NULL,
--	PlayerKills		INT				NOT NULL,
--	GamePlayedDate	NVARCHAR(22)	NOT NULL,
--	GameDuration	NVARCHAR(13)	NOT NULL,
--)

--use ProjectNexus

--CREATE PROCEDURE CreateLeaderboardPost

--	@PlayerNickName		NVARCHAR(25),
--	@LobbyName			NVARCHAR(25),
--	@PlayerKills		INT,
--	@GamePlayedDate		NVARCHAR(22),
--	@GameDuration		NVARCHAR(13)

--AS

--BEGIN
--	SET NOCOUNT ON;

--	INSERT INTO LeaderBoard(PlayerNickName, LobbyName, PlayerKills, GamePlayedDate, GameDuration)
--	VALUES(@PlayerNickName, @LobbyName, @PlayerKills, @GamePlayedDate, @GameDuration)

--END