use projekt;
GO

IF OBJECT_ID('points') IS NOT NULL
	DROP TABLE points;
IF OBJECT_ID('polygon') IS NOT NULL
	DROP TABLE polygon;
	
GO

create table points (point dbo.Point);

INSERT INTO points VALUES
('-3,0'),
('-1,4'),
('1,4'),
('3,0');

GO

CREATE TABLE polygon (pol dbo.Polygon);
INSERT INTO polygon values ('');

DECLARE @pnt dbo.Point;

DECLARE curs CURSOR
	LOCAL STATIC READ_ONLY FORWARD_ONLY
FOR
SELECT point FROM points;

OPEN curs
FETCH NEXT FROM curs INTO @pnt
WHILE @@FETCH_STATUS = 0
BEGIN
UPDATE polygon set pol.Load = @pnt;
FETCH NEXT FROM curs INTO @pnt;
END
CLOSE curs;
DEALLOCATE curs;

SELECT pol.IncludedPoints FROM polygon;
SELECT pol.calculate_area() from polygon;